using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicImpactActor : DynamicActor
{
    [SerializeField]
    protected AudioCueScriptableObject sound;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected ParticleSystem particle;

    public override void OnPop()
    {
        base.OnPop();
        source.clip = sound;
        source.time = 0;
        source.Play();
        particle.Play();
    }

    public override void OnPush()
    {
        base.OnPush();
        source.Stop();
        particle.Stop();
    }
}
