using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : Actor
{
    protected bool hasPawn;
    protected Pawn controlledPawn;
    public bool HasPawn { get => hasPawn; }
    public Pawn ControlledPawn { get => controlledPawn; }
    public virtual void Possess(Pawn pawn)
    {
        gameObject.SetActive(true);
        enabled = true;
        controlledPawn = pawn;
        pawn.OnPossess(this);
        hasPawn = true;
        controlledPawn.Health.BindOnHurt(OnHurt);
        controlledPawn.Health.BindOnDeath(OnDeath);
    }
    public virtual void Unpossess()
    {
        gameObject.SetActive(false);
        enabled = false;
        if (hasPawn)
        {
            controlledPawn.Health.UnbindOnHurt(OnHurt);
            controlledPawn.Health.UnbindOnDeath(OnDeath);
            controlledPawn.OnUnpossess();
            controlledPawn = null;
            hasPawn = false;
        }
        GameInstance.Instance.PoolManager.Push(this);
    }
    protected virtual void OnHurt(DamageStruct ds, RaycastHit2D raycastHit) { }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit) { }

    protected override void OnDestroyLate()
    {
        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasPawn)
        {
            Unpossess();
        }
    }

    protected virtual void OnDestroy()
    {
        OnDestroyLate();
    }
}
