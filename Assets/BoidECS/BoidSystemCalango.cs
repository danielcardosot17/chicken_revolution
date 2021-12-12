using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BoidSystemCalango : SystemBase
{
    private BoidController controller;
    
    private struct EntityWithLocalToWorld {
        public Entity entity;
        public LocalToWorld localToWorld;
    }
    protected override void OnUpdate()
    {
        if (!controller) {
            controller = BoidController.Instance;
        }
        else
        {
            EntityQuery boidQuery = GetEntityQuery(ComponentType.ReadOnly<BoidSharedData>(), ComponentType.ReadOnly<LocalToWorld>());

            NativeArray<Entity> entityArray = boidQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<LocalToWorld> localToWorldArray = boidQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            // These arrays get deallocated after job completion
            NativeArray<EntityWithLocalToWorld> boidArray = new NativeArray<EntityWithLocalToWorld>(entityArray.Length, Allocator.TempJob);
            NativeArray<float4x4> newBoidTransforms = new NativeArray<float4x4>(entityArray.Length, Allocator.TempJob);

            for (int i = 0; i < entityArray.Length; i++) {
                boidArray[i] = new EntityWithLocalToWorld {
                    entity = entityArray[i],
                    localToWorld = localToWorldArray[i]
                };
            }

            entityArray.Dispose();
            localToWorldArray.Dispose();
            
            float boidPerceptionRadius = controller.boidPerceptionRadius;
            float separationWeight = controller.separationWeight;
            float cohesionWeight = controller.cohesionWeight;
            float alignmentWeight = controller.alignmentWeight;
            float cageSize = controller.cageSize;
            float avoidWallsTurnDist = controller.avoidWallsTurnDist;
            float avoidWallsWeight = controller.avoidWallsWeight;
            float boidSpeed = controller.boidSpeed;
            float deltaTime = Time.DeltaTime;

            Entities
                .WithAll<BoidSharedData>()
                .WithDisposeOnCompletion(boidArray)
                .ForEach((Entity boid, int entityInQueryIndex, in LocalToWorld localToWorld ) => {
                float3 boidPosition = localToWorld.Position;
                float3 seperationSum = float3.zero;
                float3 positionSum = float3.zero;
                float3 headingSum = float3.zero;

                int boidsNearby = 0;

                for (int otherBoidIndex = 0; otherBoidIndex < boidArray.Length; otherBoidIndex++) {
                    if (boid != boidArray[otherBoidIndex].entity) {
                        
                        float3 otherPosition = boidArray[otherBoidIndex].localToWorld.Position;
                        float distToOtherBoid = math.length(boidPosition - otherPosition);

                        if (distToOtherBoid < boidPerceptionRadius) {

                            seperationSum += -(otherPosition - boidPosition) * (1f / math.max(distToOtherBoid, .0001f));
                            positionSum += otherPosition;
                            headingSum += boidArray[otherBoidIndex].localToWorld.Forward;

                            boidsNearby++;
                        }
                    }
                }

                float3 force = float3.zero;

                if (boidsNearby > 0) {
                    force += (seperationSum / boidsNearby)                * separationWeight;
                    force += ((positionSum / boidsNearby) - boidPosition) * cohesionWeight;
                    force += (headingSum / boidsNearby)                   * alignmentWeight;
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

                newBoidTransforms[entityInQueryIndex] = float4x4.TRS(
                    localToWorld.Position + velocity * deltaTime,
                    quaternion.LookRotationSafe(velocity, localToWorld.Up),
                    new float3(1f)
                );
            }).Schedule();
            
            Entities
                .WithDisposeOnCompletion(newBoidTransforms)
                .WithAll<BoidSharedData>()
                .ForEach((int entityInQueryIndex, ref LocalToWorld localToWorld ) => {
                localToWorld.Value = newBoidTransforms[entityInQueryIndex];
            }).Schedule();
        }
    }

}
