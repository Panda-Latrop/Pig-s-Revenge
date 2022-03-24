using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathImpactComponent : DeathComponent
{
    [SerializeField]
    protected DynamicActor death;
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        GameInstance.Instance.PoolManager.Pop(death).SetPosition(transform.position);
        GameInstance.Instance.PoolManager.Push(owner);
    }
}
