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


            bool planeMovementOnly = controller.planeMovementOnly;
            float boidPerceptionRadius = controller.boidPerceptionRadius;
            float separationWeight = controller.separationWeight;
            float cohesionWeight = controller.cohesionWeight;
            float alignmentWeight = controller.alignmentWeight;
            // float cageSize = controller.cageSize;
            float cageX = controller.cageX;
            float cageY = controller.cageY;
            float cageZ = controller.cageZ;
            float cageCenterPositionX = controller.cageCenterObject.position.x;
            float cageCenterPositionY = controller.cageCenterObject.position.y;
            float cageCenterPositionZ = controller.cageCenterObject.position.z;
            float avoidWallsTurnDist = controller.avoidWallsTurnDist;
            float avoidWallsWeight = controller.avoidWallsWeight;
            float boidSpeed = controller.boidSpeed;
            float deltaTime = Time.DeltaTime;
            
            // avoidance adjust
            float squareNeigborRadius = boidPerceptionRadius * boidPerceptionRadius;
            float squareAvoidanceRadius = squareNeigborRadius * controller.avoidanceRadiusMultiplier * controller.avoidanceRadiusMultiplier;


            Entities
                .WithAll<BoidSharedData>()
                .WithDisposeOnCompletion(boidArray)
                .ForEach((Entity boid, int entityInQueryIndex, in LocalToWorld localToWorld ) => {
                    float3 boidPosition = localToWorld.Position;
                    float3 seperationSum = float3.zero;
                    float3 positionSum = float3.zero;
                    float3 headingSum = float3.zero;
                    float3 cageCenterPosition = new float3(cageCenterPositionX,cageCenterPositionY,cageCenterPositionZ);

                    int boidsNearby = 0;
                    // Avodance adjust
                    int nAvoid = 0;

                    for (int otherBoidIndex = 0; otherBoidIndex < boidArray.Length; otherBoidIndex++) {
                        if (boid != boidArray[otherBoidIndex].entity) {
                            
                            float3 otherPosition = boidArray[otherBoidIndex].localToWorld.Position;
                            float distToOtherBoid = math.length(boidPosition - otherPosition);

                            if (distToOtherBoid < boidPerceptionRadius) {
                                // AvoidanceBehavior
                                if( distToOtherBoid * distToOtherBoid < squareAvoidanceRadius)
                                {
                                    nAvoid++;
                                    // seperationSum += (boidPosition - otherPosition) * (1f / math.max(distToOtherBoid, .0001f));
                                    seperationSum += (boidPosition - otherPosition) * (squareAvoidanceRadius / math.max((distToOtherBoid * distToOtherBoid), .0001f));
                                }
                                //  CohesionBehavior
                                positionSum += otherPosition;
                                // AligmentBehavior
                                headingSum += boidArray[otherBoidIndex].localToWorld.Forward;

                                boidsNearby++;
                            }
                        }
                    }

                    float3 force = float3.zero;
                    // partialMove from Behavior GameObject algorithm approach ->
                    float3 partialMove = float3.zero;
                    float partialMoveSqrMagnitude = 0;

                    if (boidsNearby > 0) {
                        // same checks of Behavior GameObject algorithm approach: CompositeBehavior
                        // first for Avoidance (separation)
                        if(nAvoid > 0)
                        {
                            partialMove = (seperationSum / nAvoid) * separationWeight;
                            partialMoveSqrMagnitude = 
                                partialMove.x * partialMove.x +
                                partialMove.y * partialMove.y +
                                partialMove.z * partialMove.z;
                            
                            if(partialMoveSqrMagnitude > 0){
                                if(partialMoveSqrMagnitude > separationWeight * separationWeight)
                                {
                                    partialMove = math.normalize(partialMove) * separationWeight;
                                }
                                force += partialMove;
                            }
                            partialMove = float3.zero;
                            partialMoveSqrMagnitude = 0;
                        }

                        // Second for Cohesion (positionSum)
                        partialMove = ((positionSum / boidsNearby) - boidPosition) * cohesionWeight;
                        partialMoveSqrMagnitude = 
                            partialMove.x * partialMove.x +
                            partialMove.y * partialMove.y +
                            partialMove.z * partialMove.z;
                        
                        if(partialMoveSqrMagnitude > 0){
                            if(partialMoveSqrMagnitude > cohesionWeight * cohesionWeight)
                            {
                                partialMove = math.normalize(partialMove) * cohesionWeight;
                            }
                            force += partialMove;
                        }
                        partialMove = float3.zero;
                        partialMoveSqrMagnitude = 0;

                        // Third for Aligment (headingSum)
                        partialMove = (headingSum / boidsNearby) * alignmentWeight;
                        partialMoveSqrMagnitude = 
                            partialMove.x * partialMove.x +
                            partialMove.y * partialMove.y +
                            partialMove.z * partialMove.z;
                        
                        if(partialMoveSqrMagnitude > 0){
                            if(partialMoveSqrMagnitude > alignmentWeight * alignmentWeight)
                            {
                                partialMove = math.normalize(partialMove) * alignmentWeight;
                            }
                            force += partialMove;
                        }
                        partialMove = float3.zero;
                        partialMoveSqrMagnitude = 0;

                        
                        // force += (seperationSum / boidsNearby) * separationWeight;
                        // force += ((positionSum / boidsNearby) - boidPosition) * cohesionWeight;
                        // force += (headingSum / boidsNearby) * alignmentWeight;
                    }

                    // Here is "Stay in cage. I will try to change to StayInRadiusBehavior
                    // maybe just a cage offset float3
                    // if (math.min(math.min(
                    //     (cageSize / 2f) - math.abs(boidPosition.x),
                    //     (cageSize / 2f) - math.abs(boidPosition.y)),
                    //     (cageSize / 2f) - math.abs(boidPosition.z))
                    //         < avoidWallsTurnDist) {
                    //     force += -math.normalize(boidPosition) * avoidWallsWeight;
                    // }
                    if (math.min(math.min(
                        (cageX / 2f) - math.abs(boidPosition.x - cageCenterPositionX),
                        (cageY / 2f) - math.abs(boidPosition.y - cageCenterPositionY)),
                        (cageZ / 2f) - math.abs(boidPosition.z - cageCenterPositionZ))
                            < avoidWallsTurnDist) {
                        force += -math.normalize(boidPosition - cageCenterPosition) * avoidWallsWeight;
                    }

                    // here will try to adapt to "Flock" code: maxSpeed limit
                    // float forceSqrMagnitude = 
                    //     force.x * force.x +
                    //     force.y * force.y +
                    //     force.z * force.z;
                    // if( forceSqrMagnitude > boidSpeed * boidSpeed)
                    // {
                    //     force = math.normalize(force) * boidSpeed;
                    // }

                    // This below is ok!
                    float3 velocity = localToWorld.Forward * boidSpeed;
                    velocity += force * deltaTime;
                    velocity = math.normalize(velocity) * boidSpeed;
                    if(planeMovementOnly)
                    {
                        velocity = math.normalize(new float3(velocity.x,0,velocity.z)) * boidSpeed;
                    }

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
