using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepComponent : ActorComponent
{
    [SerializeField]
    protected AudioCueScriptableObject footstep;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected MovementComponent movement;
    [SerializeField]
    protected float timeToStep;
    protected float nextStep;
    protected bool lastMove = false;

    protected override void OnStart()
    {
        nextStep = Time.time + timeToStep;
    }

    protected override void OnLateUpdate()
    {
        if (movement.IsMove)
        {
            if (!lastMove)
                nextStep = Time.time + timeToStep;
            if (Time.time >= nextStep)
            {
                source.clip = footstep;
                source.time = 0;
                source.Play();
                nextStep = Time.time + timeToStep;
            }
        }
        lastMove = movement.IsMove;
    }

    private void Start()
    {
        OnStart();
    }
    private void LateUpdate()
    {
        OnLateUpdate();
    }
}
