using Unity.Entities;

public struct BoidIdData : IComponentData
{
    public int boidIndex;
    public int boidsNearby;
}
