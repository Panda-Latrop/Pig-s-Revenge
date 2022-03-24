using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFarmerController : AIDogController
{
    protected bool inStun;
    [SerializeField]
    protected float timeStun;
    protected float nextStun;
    [SerializeField]
    protected AudioCueScriptableObject deathSound;
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnHurt(ds, raycastHit);
        if (!inStun)
        {
            inStun = true;
            character.PlayVoice(deathSound);
            character.Movement.Stop();
            nextStun = Time.time + timeStun;
            inRage = false;
            hasTarget = false;
            character.enabled = false;
        }
    }
    protected override void OnUpdate()
    {
        if (!inStun)
        {
            base.OnUpdate();
        }
        else
        {
            if (Time.time >= nextStun)
            {
                character.Animation.Play("Sprite");
                character.enabled = true;
                inStun = false;
            }
                
        }
    }
}
