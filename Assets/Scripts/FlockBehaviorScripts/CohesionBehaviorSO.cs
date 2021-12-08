using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CohesionBehaviorSO", menuName = "chicken_revolution/Flock/Behaviors/CohesionBehaviorSO", order = 0)]
public class CohesionBehaviorSO : FilteredFlockBehaviorSO
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbors, return no adjustment
        if(context.Count == 0)
            return Vector3.zero;
        // Add all points together and average
        Vector3 cohesionMove = Vector3.zero;
        
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent,context);

        foreach(Transform item in filteredContext)
        {
            cohesionMove += item.position;
        }
        cohesionMove /= context.Count;
        //  create offset from agent position
        cohesionMove -= agent.transform.position;
        
        return Vector3.ProjectOnPlane(cohesionMove,Vector3.up);
        // return cohesionMove;
    }
}
