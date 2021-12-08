using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StayInRadiusBehaviorSO", menuName = "chicken_revolution/Flock/Behaviors/StayInRadiusBehaviorSO", order = 0)]
public class StayInRadiusBehaviorSO : FlockBehaviorSO
{
    public Vector3 center;
    
    public float radius = 15f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 centerOffset = Vector3.ProjectOnPlane(center - agent.transform.position,Vector3.up);
        float t = centerOffset.magnitude / radius;

        if( t < 0.9f)
        {
            return Vector3.zero;
        }
        
        return centerOffset * t * t;
    }
}
