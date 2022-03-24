using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDogController : AIController
{
    [SerializeField]
    protected float distanceToDirectMove = 3.0f, distanceToClose = 1.0f;
    protected bool closeToTarget;
    [SerializeField]
    protected float timeToGetClosest = 5.0f;
    protected float nextClosest;
    [SerializeField]
    protected float timeToRandomAttack = 1.0f;
    protected float nextRandomAttack;
    protected bool inRage;
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        inRage = false;
        base.OnDeath(ds, raycastHit);
    }
    protected void MovementTo(Pawn target)
    {
        Vector3 distance = (target.Center - character.Center);
        Vector3 direction = distance;
        direction.Normalize();
        float xym = distance.x * distance.x + distance.y * distance.y;
        if (xym < distanceToDirectMove * distanceToDirectMove)
        {
            if (xym > distanceToClose * distanceToClose)
            {
                Move(direction);
                closeToTarget = false;
            }
            else
                closeToTarget = true;
            pathState = PathState.none;
        }
        else
        {
            closeToTarget = false;
            if (pathState.Equals(PathState.has))
            {
                MoveByPath();
                if (CheckConditionToRebuildPath())
                    CreatePath(target.transform.position, true,true);

            }
            else if (pathState.Equals(PathState.none))
            {
                CreatePath(target.transform.position, true);
            }
        }
    }
    protected void RandomMove()
    {
        if (pathState.Equals(PathState.none))
        {
            NavigationGrid grid = GameInstance.Instance.Navigation.grid;
            CreatePath(grid.GetOpenNode(Random.Range(0, grid.OpenLenth)).position, true, true);
        }
        else
        {
            MoveByPath();
        }
    }
    protected override void OnUpdate()
    {
        CheckConditionToChangeTarget();
        if (!inRage && hasTarget && CheckTargetVisibility(target))
        {
            nextRebuild = 0.0f;
            inRage = true;
        }
        if (inRage && hasTarget)
        {
            MovementTo(target);
            if (closeToTarget)
            {
                character.Shoot(true, character.Center, (target.Center - character.Center).normalized);
                return;
            }
            if (Time.time >= nextRandomAttack)
            {
                character.Shoot(true, character.Center,( target.Center - character.Center).normalized);
                nextRandomAttack = Time.time + timeToRandomAttack;
                return;
            }
            character.Movement.SpeedMultiply = 2.0f;
        }
        else
        {
            if (!hasTarget)
                inRage = false;
            character.Movement.SpeedMultiply = 1.0f;
            RandomMove();
        }

    }
    protected override void OnLateUpdate()
    {
        if (hasTarget && !target.Health.IsAlive)
        {
            target = null;
            hasTarget = false;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].Health.IsAlive)
            {
                targets.RemoveAt(i);
                i--;
            }
            if (needMoreTarget)
                needMoreTarget = controlledPawn.Health.Team.Equals(targets[i].Health.Team);

        }
    }

    private void Update()
    {
        OnUpdate();
    }
    private void LateUpdate()
    {
        OnLateUpdate();
    }
}
