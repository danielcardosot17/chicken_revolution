using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SameFlockFIlterSO", menuName = "chicken_revolution/Flock/Filter/SameFlockFIlterSO", order = 0)]
public class SameFlockFIlterSO : ContextFilterSO
{
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        foreach(Transform item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();

            if(itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}