using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvoidanceBehaviorSO", menuName = "chicken_revolution/Flock/AvoidanceBehaviorSO", order = 0)]
public class AvoidanceBehaviorSO : FlockBehaviorSO 
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbors, return no adjustment
        if(context.Count == 0)
            return Vector3.zero;
        // Add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;

        foreach(Transform item in context)
        {
            if(Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += (agent.transform.position -  item.position);
            }
        }

        if(nAvoid > 0)
            avoidanceMove /= nAvoid;

        return Vector3.ProjectOnPlane(avoidanceMove,Vector3.up);
        // return avoidanceMove;
    }
}