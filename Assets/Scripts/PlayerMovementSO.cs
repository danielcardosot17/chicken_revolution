using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementSO", menuName = "chicken_revolution/PlayerMovementSO", order = 0)]
public class PlayerMovementSO : ScriptableObject {
    
    [Header("PARAMETROS DO PULO")]
    public float groundedBufferTime;
    public float planeSpeed;
    public float turboSpeed;
    public float turboBufferTime;
    public float rotationSpeed;
    public float jumpHeight;
    public float gravity;

    [Range(0.1f,0.9f)]
    public float glideFactor;
}
