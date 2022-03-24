using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicActor : Actor
{
    [SerializeField]
    protected float timeToPush = 5.0f;
    protected float nextPush;
    public override void OnPop()
    {
        base.OnPop();
        nextPush = Time.time + timeToPush;
        enabled = true;
    }
    public override void OnPush()
    {
        base.OnPush();
        enabled = false;
    }
    protected override void OnStart()
    {
        nextPush = Time.time + timeToPush;
    }
    protected override void OnLateUpdate()
    {
        if (Time.time >= nextPush)
        {
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
    protected void Start()
    {
        OnStart();
    }
    protected void LateUpdate()
    {
        OnLateUpdate();
    }
}
