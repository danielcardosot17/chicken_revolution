using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        
        Entities.ForEach((ref Translation translation, in MoveData movement) => {
            translation.Value += movement.targetDirection * movement.moveSpeed * deltaTime;
        }).Schedule();
    }
}
