using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Actor, IPerceptionTarget
{
    [SerializeField]
    protected ControllerBase autoController;
    protected bool hasAutoController,hasController;
    protected ControllerBase controller;
    [SerializeField]
    protected HealthComponent health;

    public Pawn Self => this;
    public bool HasController { get => hasController; }
    public ControllerBase Controller { get => controller; }
    public HealthComponent Health { get => health; }
    public virtual void OnPossess(ControllerBase controller)
    {
        hasController = true;
        this.controller = controller;
        return;
    }
    public virtual void OnUnpossess()
    {
        if (hasController)
        {
            hasController = false;
            controller = null;
        }
        return;
    }
    public override void OnPop()
    {
        base.OnPop();
        health.Resurrect();
        if (!hasController && hasAutoController)
        {
            (GameInstance.Instance.PoolManager.Pop(autoController) as ControllerBase).Possess(this);
        }
        return;
    }
    public override void OnPush()
    {
        base.OnPush();
    }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return;
    }
    protected virtual void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return;
    }
    protected virtual void OnResurrect()
    {
        return;
    }

    protected override void OnAwake()
    {
        health.BindOnHurt(OnHurt);
        health.BindOnDeath(OnDeath);
        health.BindOnResurrect(OnResurrect);
    }
    protected override void OnStart()
    {
        if (!hasController && (hasAutoController = autoController != null) && health.IsAlive)
        {
            (GameInstance.Instance.PoolManager.Pop(autoController) as ControllerBase).Possess(this);
        }
    }
    protected override void OnDestroyLate()
    {
        health.UnbindOnHurt(OnHurt);
        health.UnbindOnDeath(OnDeath);
        health.UnbindOnResurrect(OnResurrect);
        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasController)
        {
            controller.Unpossess();
        }
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
