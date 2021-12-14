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
    private bool isScreaming = false;

    private void Start() {
        flockStandby.Raise();
        timer = followDelay;
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
                    }
                    else
                    {
                        warcryLong.Raise();
                    }
                    isScreaming = true;
                }
                transform.position += direction.normalized * followSpeed * Time.deltaTime;
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
    }

    
    public static IEnumerator DoAfterTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
