using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvoidanceBehaviorSO", menuName = "chicken_revolution/Flock/Behaviors/AvoidanceBehaviorSO", order = 0)]
public class AvoidanceBehaviorSO : FilteredFlockBehaviorSO 
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbors, return no adjustment
        if(context.Count == 0)
            return Vector3.zero;
        // Add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent,context);

        foreach(Transform item in filteredContext)
        {
            Vector3 closestPoint = item.gameObject.GetComponent<Collider>().ClosestPoint(agent.transform.position);
            if(Vector3.SqrMagnitude(closestPoint - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += (agent.transform.position -  closestPoint);
            }
        }

        if(nAvoid > 0)
            avoidanceMove /= nAvoid;

        return Vector3.ProjectOnPlane(avoidanceMove,Vector3.up);
        // return avoidanceMove;
    }
}