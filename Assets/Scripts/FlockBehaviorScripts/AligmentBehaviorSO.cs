using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AligmentBehaviorSO", menuName = "chicken_revolution/Flock/Behaviors/AligmentBehaviorSO", order = 0)]
public class AligmentBehaviorSO : FilteredFlockBehaviorSO
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbors, mantain current alignment
        if(context.Count == 0)
            return agent.transform.forward;
        // Add all points together and average
        Vector3 aligmentMove = Vector3.zero;
        
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent,context);

        foreach(Transform item in filteredContext)
        {
            aligmentMove += item.transform.forward;
        }

        aligmentMove /= context.Count;
        
        return Vector3.ProjectOnPlane(aligmentMove,Vector3.up);
        // return aligmentMove;
    }
}
