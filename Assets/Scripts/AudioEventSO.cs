using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEventSO : GameEventSO 
{
    public abstract void Play(AudioSource source = null);
}