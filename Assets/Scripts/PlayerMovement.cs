using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterController)) ]
public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementSO playerMovementSO;
    private Animator playerAnimator;
    private bool isGrounded = true;
    private float groundedTimer;
    private CharacterController characterController;
    private float groundedBufferTime;
    private float planeSpeed;
    private float turboSpeed;
    private float turboBufferTime;
    private float rotationSpeed;
    private float jumpHeight;
    private float gravity;
    private float amplitude;
    private float frequency;
    private Color killColor;
    private float glideFactor;
    private Camera playerCamera;
    private float verticalSpeed = 0.0f;
    private float turboTimer = 0.0f;
    private Vector3 originalScale;
    private bool isShaking = false;
    private bool isGliding = false;

    // Start is called before the first frame update
    void Start()
    {
        groundedBufferTime = playerMovementSO.groundedBufferTime;
        planeSpeed = playerMovementSO.planeSpeed;
        turboSpeed = playerMovementSO.turboSpeed;
        turboBufferTime = playerMovementSO.turboBufferTime;
        rotationSpeed = playerMovementSO.rotationSpeed;
        jumpHeight = playerMovementSO.jumpHeight;
        gravity = playerMovementSO.gravity;
        glideFactor = playerMovementSO.glideFactor;

        if(!Application.isEditor){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        originalScale = transform.localScale;
        playerAnimator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        if(playerCamera==null){
            playerCamera = Camera.main;
        }
    }
    void FixedUpdate()
    {
        transform.localScale = originalScale;
        if(characterController.isGrounded){
            verticalSpeed=0;
            groundedTimer=0;
            isGrounded=true;
        }
        else{
            groundedTimer += Time.deltaTime;
            if(groundedTimer>=groundedBufferTime){
                isGrounded=false;
            }
        }

        var direction =
                Vector3.ProjectOnPlane(playerCamera.transform.forward,Vector3.up).normalized * Input.GetAxis("Vertical") + playerCamera.transform.right * Input.GetAxis("Horizontal");
        var planeMove = direction * planeSpeed * Time.deltaTime;

        if(Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) >= 1){
            if(turboTimer > 0){
                turboTimer -= Time.deltaTime;
            }
            else{
                planeMove = direction * turboSpeed * Time.deltaTime;
            }
        }
        else{
            turboTimer = turboBufferTime;
        }

        if(isGrounded){
            playerAnimator.SetBool("isWalking",direction.magnitude!=0);
            playerAnimator.SetBool("isFlying",false);
            // playerAnimator.SetBool("isEating",false);
            isGliding = false;
            if(Input.GetMouseButtonDown(0)){
                playerAnimator.SetBool("isWalking",false);
                playerAnimator.SetBool("isFlying",false);
                verticalSpeed=0;
                planeMove = Vector3.zero;
                // playerAnimator.SetBool("isEating",true);
                playerAnimator.SetTrigger("eat");
            }
            if(Input.GetKeyDown(KeyCode.Space)){
                verticalSpeed=Mathf.Sqrt(2*gravity*jumpHeight);
                isGrounded=false;
            }
        }
        else{
            if(Input.GetKey(KeyCode.Space)){
                isGliding = true;
            }
            else{
                isGliding = false;
            }
            // playerAnimator.SetBool("isEating",false);
            playerAnimator.SetBool("isWalking",false);
            playerAnimator.SetBool("isFlying",true);
        }

        if(isGliding){
            verticalSpeed-=gravity * glideFactor * Time.deltaTime;
        }
        else{
            verticalSpeed-=gravity * Time.deltaTime;
        }
        var verticalMove = Vector3.up * verticalSpeed * Time.deltaTime;

        if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Eating")){
            if( direction.magnitude != 0){
                this.transform.forward = Vector3.Slerp(this.transform.forward, direction.normalized, rotationSpeed);
            }
            characterController.Move(planeMove + verticalMove);
        }
    }
    public void ResetTurboTimer()
    {
        turboTimer = turboBufferTime;
    }
}
