using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleActor : Pawn
{
    protected LevelGenerator levelGenerator;
    protected Node node;
    [SerializeField]
    protected DynamicActor death;
    public void SetLevelGenerator(LevelGenerator levelGenerator, Node node)
    {
        this.levelGenerator = levelGenerator;
        this.node = node;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        levelGenerator.OpenPoint(node);
        GameInstance.Instance.PoolManager.Pop(death).SetPosition(transform.position);
        GameInstance.Instance.PoolManager.Push(this);
    }
}
