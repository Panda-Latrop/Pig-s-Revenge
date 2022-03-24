using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathComponent : ActorComponent
{
    [SerializeField]
    protected Pawn owner;
    protected override void OnAwake()
    {
        owner.Health.BindOnDeath(OnDeath);
    }
    protected override void OnDestroyLate()
    {
        owner.Health.UnbindOnDeath(OnDeath);
    }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return;
    }
    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void OnDestroy()
    {
        OnDestroyLate();
    }
}
