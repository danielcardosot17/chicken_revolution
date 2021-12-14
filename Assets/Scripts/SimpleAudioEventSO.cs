using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SimpleAudioEventSO", menuName = "chicken_revolution/Audio/SimpleAudioEventSO", order = 0)]
public class SimpleAudioEventSO : AudioEventSO
{
    public Sound[] sounds;
    public override void Play(AudioSource source)
    {
        if(sounds.Length == 0) return;

        // if(source == null)
        // {
        //     var obj = new GameObject(name: "Sound", typeof(AudioSource));
        //     source = obj.GetComponent<AudioSource>();
        // }

        var randomNumber = Random.Range(0,sounds.Length);
        source.clip = sounds[randomNumber].clip;
        source.priority = sounds[randomNumber].priority;
        source.volume = sounds[randomNumber].volume;
        source.pitch = sounds[randomNumber].pitch;
        source.loop = sounds[randomNumber].loop;
        source.Play();
        // if(!source.loop)
        // {
        //     Destroy(source.gameObject, source.clip.length/source.pitch);
        // }
    }
}