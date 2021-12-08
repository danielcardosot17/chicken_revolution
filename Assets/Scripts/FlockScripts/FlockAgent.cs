using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock AgentFlock { get => agentFlock; private set => agentFlock = value; }

    Collider agentCollider;
    public Collider AgentCollider { get => agentCollider; private set => agentCollider = value; }

    // Start is called before the first frame update
    void Start()
    {
        AgentCollider =  GetComponent<Collider>();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }
    public void Move(Vector3 velocity)
    {
        transform.forward = velocity.normalized;
        transform.position += velocity * Time.deltaTime;
    }
}
