using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPigController : AIController
{
    protected Pawn commander;
    protected bool hasCommander;
    [SerializeField]
    protected float distanceToDirectMove = 3.0f,distanceToClose = 1.0f;

    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        hasCommander = false;
        commander = null;
        base.OnDeath(ds, raycastHit);
    }
    public override void Unpossess()
    {
        hasCommander = false;
        commander = null;
        base.Unpossess();
    }
    protected void MovementTo(Pawn target)
    {
        Vector3 distance = (target.Center - character.Center);
        Vector3 direction = distance;
        direction.Normalize();
        float xym = distance.x * distance.x + distance.y * distance.y;
        if (xym < distanceToDirectMove* distanceToDirectMove)
        {
            if(xym > distanceToClose* distanceToClose)
            Move(direction);
        }
        else
        {
            if (pathState.Equals(PathState.has))
            {
                MoveByPath();
                if (CheckConditionToRebuildPath())
                    CreatePath(target.transform.position, false, true);

            }
            else if (pathState.Equals(PathState.none))
            {
                CreatePath(target.transform.position,false);
            }
        }
    }
    protected virtual void Update()
    {
        if (hasCommander)
            MovementTo(commander);
    }
    protected void LateUpdate()
    {
        if(hasCommander)
        hasCommander = commander.Health.IsAlive;
    }
    protected override void AddTarget(Pawn target)
    {
        base.AddTarget(target);
        if (!hasCommander && target.Health.Team.Equals(character.Health.Team) && target.name.Equals(GameInstance.Instance.PlayerController.ControlledPawn.name))
        {
            hasCommander = true;
            commander = target;
        }
    }
}
