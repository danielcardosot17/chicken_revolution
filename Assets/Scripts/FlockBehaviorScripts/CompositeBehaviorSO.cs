using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompositeBehaviorSO", menuName = "chicken_revolution/Flock/Behaviors/CompositeBehaviorSO", order = 0)]
public class CompositeBehaviorSO : FlockBehaviorSO
{
    public FlockBehaviorSO[] behaviors;
    public float[] weights;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //  handle data mismatch
        if(weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        // setup moves

        Vector3 move = Vector3.zero;

        // iterate through behaviors
        for(int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock) * weights[i];

            if(partialMove != Vector3.zero)
            {
                if(partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove = partialMove.normalized * weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}