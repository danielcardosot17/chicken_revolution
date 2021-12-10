using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class ProcessInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // temp variables because strings are not blittable types
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        
        Entities.ForEach((ref RawInputData input, ref MoveData movement) => {
            // set input data
            input.inputH = inputH;
            input.inputV = inputV;

            // set direction data
            movement.targetDirection = new Unity.Mathematics.float3(input.inputH,0,input.inputV); 

        }).Schedule();
    }
}
