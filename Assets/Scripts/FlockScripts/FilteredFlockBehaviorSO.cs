using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FilteredFlockBehaviorSO", menuName = "chicken_revolution/Flock/Behavior/FilteredFlockBehaviorSO", order = 0)]
public abstract class FilteredFlockBehaviorSO : FlockBehaviorSO 
{
    public ContextFilterSO filter;
}