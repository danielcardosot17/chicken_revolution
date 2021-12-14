using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioEventSO musicEvent;
    // Start is called before the first frame update
    void Start()
    {
        musicEvent.Raise();
    }
}
