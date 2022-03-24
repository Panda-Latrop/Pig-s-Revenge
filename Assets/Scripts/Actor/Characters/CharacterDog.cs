using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDog : Character
{
    [SerializeField]
    protected FakeClimbComponent fakeClimb;

    protected override void OnAwake()
    {
        base.OnAwake();
        fakeClimb.SetCollider(collider);
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnDeath(ds, raycastHit);
        fakeClimb.Restart();
    }
}
