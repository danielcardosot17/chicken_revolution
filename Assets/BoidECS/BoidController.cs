using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
// como transformar isso em um ScriptableObject????

public class BoidController : MonoBehaviour
{
    public static BoidController Instance;

    [SerializeField] private int boidAmount;
    [SerializeField] private Mesh sharedMesh;
    [SerializeField] private Material sharedMaterial;

    public bool planeMovementOnly = false;

    public float boidSpeed;
    public float boidPerceptionRadius;
    // public float cageSize;
    public float cageX;
    public float cageY;
    public float cageZ;
    public Transform cageCenterObject;

    public float separationWeight;
    public float cohesionWeight;
    public float alignmentWeight;

    public float avoidWallsWeight;
    public float avoidWallsTurnDist;

    private void Awake() {

        Instance = this;
        
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype boidArchetype = entityManager.CreateArchetype(
            typeof(BoidSharedData),
            typeof(BoidIdData),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
        );
        
        NativeArray<Entity> boidArray = new NativeArray<Entity>(boidAmount, Allocator.Temp);
        entityManager.CreateEntity(boidArchetype, boidArray);

        for (int i = 0; i < boidArray.Length; i++) {
            Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)i + 1);
            entityManager.SetComponentData(boidArray[i], new LocalToWorld {
                Value = float4x4.TRS(
                    RandomPosition(),
                    RandomRotation(),
                    new float3(1f))
            });
            entityManager.SetSharedComponentData(boidArray[i], new RenderMesh {
                mesh = sharedMesh,
                material = sharedMaterial,
            });
            
            entityManager.SetComponentData(boidArray[i], new RenderBounds {
                Value = sharedMesh.bounds.ToAABB()
            });
            entityManager.SetComponentData(boidArray[i], new BoidIdData {
                boidIndex = i,
                boidsNearby = 0,
            });
        }

        boidArray.Dispose();
    }

    // private float3 RandomPosition() {
    //     return new float3(
    //         UnityEngine.Random.Range(cageCenterObject.position.x - cageSize / 2f, cageCenterObject.position.x + cageSize / 2f),
    //         UnityEngine.Random.Range(cageCenterObject.position.y - cageSize / 2f, cageCenterObject.position.y + cageSize / 2f),
    //         UnityEngine.Random.Range(cageCenterObject.position.z - cageSize / 2f, cageCenterObject.position.z + cageSize / 2f)
    //     );
    // }
    private float3 RandomPosition() {
        if(planeMovementOnly)
        {
            return new float3(
                UnityEngine.Random.Range(cageCenterObject.position.x - cageX / 2f, cageCenterObject.position.x + cageX / 2f),
                0,
                UnityEngine.Random.Range(cageCenterObject.position.z - cageZ / 2f, cageCenterObject.position.z + cageZ / 2f)
            );
        }
        return new float3(
            UnityEngine.Random.Range(cageCenterObject.position.x - cageX / 2f, cageCenterObject.position.x + cageX / 2f),
            UnityEngine.Random.Range(cageCenterObject.position.y - cageY / 2f, cageCenterObject.position.y + cageY / 2f),
            UnityEngine.Random.Range(cageCenterObject.position.z - cageZ / 2f, cageCenterObject.position.z + cageZ / 2f)
        );
    }
    
    private quaternion RandomRotation() {
        if(planeMovementOnly)
        {
            return quaternion.Euler(
                0,
                UnityEngine.Random.Range(-360f, 360f),
                0
            );
        }
        return quaternion.Euler(
            UnityEngine.Random.Range(-360f, 360f),
            UnityEngine.Random.Range(-360f, 360f),
            UnityEngine.Random.Range(-360f, 360f)
        );
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            cageCenterObject.position,
            new Vector3(
                cageX,
                cageY,
                cageZ
            )
        );
    }
}
