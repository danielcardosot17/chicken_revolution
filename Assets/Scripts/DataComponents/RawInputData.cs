using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RawInputData : IComponentData
{
    [HideInInspector] public float inputH;
    [HideInInspector] public float inputV;
}
