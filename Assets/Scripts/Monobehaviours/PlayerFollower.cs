using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform target;
    public float radius;
    public float followSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        if(direction.magnitude > radius)
        {
            transform.position += direction.normalized * followSpeed * Time.deltaTime;
        }        
    }
    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
