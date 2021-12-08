using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsLayerFilterSO", menuName = "chicken_revolution/Flock/Filter/PhysicsLayerFilterSO", order = 0)]
public class PhysicsLayerFilterSO : ContextFilterSO
{
    public LayerMask mask;

    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        foreach(Transform item in original)
        {
            if(mask == (mask | 1 << item.gameObject.layer))
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}