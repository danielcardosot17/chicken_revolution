using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform target;
    public float radius;
    public float followSpeed;
    
    [Range(0f,5f)]
    public float followDelay = 2;

    public AudioEventSO warcryLong;
    public AudioEventSO warcryShort;
    public AudioEventSO flockStandby;
    public GameEventSO stopCurrentAudio;

    private float timer = 0;
    private float followTimer = 0;
    private bool isShortScream = false;
    private bool isScreaming = false;

    private void Start() {
        flockStandby.Raise();
        timer = followDelay;
        followTimer = timer;
        isShortScream = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        if(direction.magnitude > radius)
        {
            if(timer <= 0)
            {
                if(!isScreaming)
                {
                    if(((direction.magnitude - radius) / followSpeed) < followDelay)
                    {
                        warcryShort.Raise();
                        isShortScream = true;
                    }
                    else
                    {
                        isShortScream = false;
                        warcryLong.Raise();
                    }
                    isScreaming = true;
                    followTimer = followDelay;
                }
                followTimer -= Time.deltaTime;
                transform.position += direction.normalized * followSpeed * Time.deltaTime;
                
                if(followTimer <= 0)
                {
                    if(isShortScream)
                    {
                        stopCurrentAudio.Raise();
                        isShortScream = false;
                        warcryLong.Raise();
                    }
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }        
        else
        {
            if(isScreaming)
            {
                StartCoroutine(DoAfterTimeCoroutine(followDelay, () => 
                {
                    isScreaming = false;
                    isShortScream = false;
                    stopCurrentAudio.Raise();
                    flockStandby.Raise();
                }));
            }
            timer = followDelay;
        }
    }
    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,radius);
        Gizmos.DrawWireSphere(transform.position,radius + followDelay * followSpeed);
    }

    
    public static IEnumerator DoAfterTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
