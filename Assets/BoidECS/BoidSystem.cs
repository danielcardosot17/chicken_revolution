using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BoidSystem : SystemBase
{
    private BoidController controller;

    protected override void OnUpdate()
    {
        if(!controller)
        {
            controller = BoidController.Instance;
        }
        if(controller)
        {
            float boidPerceptionRadius = controller.boidPerceptionRadius;
            float separationWeight = controller.separationWeight;
            float cohesionWeight = controller.cohesionWeight;
            float alignmentWeight = controller.alignmentWeight;
            float cageSize = controller.cageSize;
            float avoidWallsTurnDist = controller.avoidWallsTurnDist;
            float avoidWallsWeight = controller.avoidWallsWeight;
            float boidSpeed = controller.boidSpeed;
            float deltaTime = Time.DeltaTime;
            

            EntityQuery boidQuery = GetEntityQuery(ComponentType.ReadOnly<BoidIdData>(),ComponentType.ReadOnly<BoidSharedData>(),ComponentType.ReadOnly<LocalToWorld>());
            
            NativeArray<BoidIdData> boidIdDataArray = boidQuery.ToComponentDataArray<BoidIdData>(Allocator.Temp);

            NativeArray<LocalToWorld> boidLocalToWorldArray = boidQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);

            NativeArray<float4x4> newBoidPositions = new NativeArray<float4x4>(boidQuery.CalculateEntityCount(), Allocator.Temp);

            Entities.WithAll<BoidSharedData>().ForEach((ref LocalToWorld localToWorld, ref BoidIdData boidData) => {
                int boidIndex = boidData.boidIndex;
                float3 boidPosition = localToWorld.Position;
                
                float3 seperationSum = float3.zero;
                float3 positionSum = float3.zero;
                float3 headingSum = float3.zero;

                for(int i = 0; i < boidIdDataArray.Length; i++)
                {
                    if (boidIndex != boidIdDataArray[i].boidIndex) {
                        
                        float distToOtherBoid = math.length(boidPosition - boidLocalToWorldArray[i].Position);
                        if (distToOtherBoid < boidPerceptionRadius) {

                            seperationSum += -(boidLocalToWorldArray[i].Position - boidPosition) * (1f / math.max(distToOtherBoid, .0001f));
                            positionSum += boidLocalToWorldArray[i].Position;
                            headingSum += boidLocalToWorldArray[i].Forward;
                            boidData.boidsNearby++;
                        }
                    }
                }

                float3 force = float3.zero;

                if (boidData.boidsNearby > 0) {
                    force += (seperationSum / boidData.boidsNearby)                * separationWeight;
                    force += ((positionSum / boidData.boidsNearby) - boidPosition) * cohesionWeight;
                    force += (headingSum / boidData.boidsNearby)                   * alignmentWeight;
                }
                if (math.min(math.min(
                    (cageSize / 2f) - math.abs(boidPosition.x),
                    (cageSize / 2f) - math.abs(boidPosition.y)),
                    (cageSize / 2f) - math.abs(boidPosition.z))
                        < avoidWallsTurnDist) {
                    force += -math.normalize(boidPosition) * avoidWallsWeight;
                }

                float3 velocity = localToWorld.Forward * boidSpeed;
                velocity += force * deltaTime;
                velocity = math.normalize(velocity) * boidSpeed;

                newBoidPositions[boidIndex] = float4x4.TRS(
                    localToWorld.Position + velocity * deltaTime,
                    quaternion.LookRotationSafe(velocity, localToWorld.Up),
                    new float3(1f)
                );
            }).Run();
            
            Entities.WithAll<BoidSharedData>().ForEach((ref LocalToWorld localToWorld, ref BoidIdData boidData) => {
                localToWorld.Value = newBoidPositions[boidData.boidIndex];
            }).Run();

        }
    }

    
}
