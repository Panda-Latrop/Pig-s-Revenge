using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : ControllerBase
{
    protected GameState gameState;
    protected Character character;

    protected bool hasTarget, needMoreTarget;
    protected Pawn target;
    protected List<Pawn> targets = new List<Pawn>();
    protected float timeToGetClosestTarget = 5.0f;
    protected float nextClosestTarget;

    protected PathState pathState;
    //protected NavigationRequest navigationRequest;
    protected List<Vector3> waypoints = new List<Vector3>();
    protected int pathLength, currentPoint;
    [SerializeField]
    protected float pathArrival—orrection = 0.25f;
    [SerializeField]
    protected float maxTargetOffsetToRebuildPath = 5.0f;
    protected float lastDistanceToPoint = 0;

    [SerializeField]
    protected float timeToRebuildPath = 10.0f;
    protected float cooldownToCreatePathOnFailureTime = 1.0f;
    protected float nextRebuild, next—ooldown;

    [SerializeField]
    protected float distanceToLostTargetVisibility = 100.0f;
    [SerializeField]
    protected float timeToCheckTargetVisibility = 1.0f;
    protected float nextCheckVisibility;
    protected bool targetIsVisible;

   

    protected void Awake()
    {
        gameState = GameInstance.Instance.GameState;
    }

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as Character;
        character.Perception.BindOnPerceptionDetection(OnPerceptionDetection);
    }
    public override void Unpossess()
    {
        character.Perception.UnbindOnPerceptionDetection(OnPerceptionDetection);
        nextRebuild = 0.0f;
        currentPoint = 0;
        pathLength = 0;
        pathState = PathState.none;
        targets.Clear();
        hasTarget = false;
        target = null;
        character = null;
        base.Unpossess();
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        gameState.PawnDeath(controlledPawn, ds, raycastHit);
        Unpossess();
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        gameState.PawnHurt(controlledPawn, ds, raycastHit);
    }
    protected virtual void Move(Vector3 direction)
    {
       character.Move(direction);
    }
    protected bool CreatePath(Vector3 point, bool skipDynamic = false, bool forceRebuild = false)
    {
        if (Time.time >= next—ooldown)
        {
            if (pathState.Equals(PathState.none) || (forceRebuild && pathState.Equals(PathState.has)))
            {
                bool find = GameInstance.Instance.Navigation.FindPath(controlledPawn.Center, point, ref waypoints, skipDynamic);
                if (find)
                {
                    pathLength = waypoints.Count;
                    currentPoint = 0;
                    pathState = PathState.has;
                    nextRebuild = Time.time + timeToRebuildPath;
                    Vector3 distance = (waypoints[currentPoint] - character.transform.position);
                    lastDistanceToPoint = distance.sqrMagnitude;

                }
                else
                {
                    next—ooldown = Time.time + cooldownToCreatePathOnFailureTime;
                    pathState = PathState.none;
                }
            }
            return !pathState.Equals(PathState.none);
        }
        return false;
    }
 

    protected void MoveByPath()
    {
        if (pathState.Equals(PathState.has))
        {
            Vector3 distance = (waypoints[currentPoint] - character.transform.position);
            if (distance.sqrMagnitude < pathArrival—orrection* pathArrival—orrection)
            {
                currentPoint++;
                if (currentPoint >= pathLength)
                {
                    pathState = PathState.none;
                    currentPoint = 0;
                    pathLength = 0;
                    return;
                }
                else
                {
                    distance = (waypoints[currentPoint] - character.transform.position);
                    lastDistanceToPoint = distance.sqrMagnitude;
                }
            }
            
            {
                float xym = distance.x * distance.x + distance.y * distance.y;
                if (xym >= lastDistanceToPoint * 1.25f)
                {
                    pathState = PathState.none;
                    currentPoint = 0;
                    pathLength = 0;
                }
                else
                {
                    Move(distance.normalized);
                }
                if (lastDistanceToPoint > xym)
                    lastDistanceToPoint = xym;
            }
           
        }
    }
    protected bool CheckConditionToRebuildPath()
    {
        if (hasTarget && pathState.Equals(PathState.has))
        {
            float offsetDistance = (target.transform.position - waypoints[pathLength - 1]).sqrMagnitude;
            return (Time.time >= nextRebuild || offsetDistance >= maxTargetOffsetToRebuildPath * maxTargetOffsetToRebuildPath);
        }
        return false;
    }
   
    protected bool CheckConditionToChangeTarget()
    {
        if ((!needMoreTarget && !hasTarget) ||
            (hasTarget &&
            (!target.Health.IsAlive || target.Health.Team.Equals(controlledPawn.Health.Team) || Time.time >= nextClosestTarget)))
        {
            ChangeTarget();
            nextClosestTarget = Time.time + timeToGetClosestTarget;
            return true;
        }
        return false;
    }
    protected virtual bool CheckTargetVisibility(Pawn target, bool ignoreTime = false)
    {

        if (Time.time >= nextCheckVisibility || ignoreTime)
        {
            RaycastHit2D hit = Physics2D.Linecast(controlledPawn.Center, target.Center, (1 << 6) | (1 << 7) | (1<<8));
            if (!hit.normal.Equals(Vector2.zero) && (1<<hit.collider.gameObject.layer) == (1 << 8))
            {
                return targetIsVisible = true;
            }
            nextCheckVisibility = Time.time + timeToCheckTargetVisibility;
        }
        return targetIsVisible = false;
    }
    protected void RefreshTargets()
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
    protected void ChangeTarget()
    {

        if (!hasTarget)
        {
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;
                return;
            }
        }
        else
        {
            if (target.Health.Team.Equals(controlledPawn.Health.Team))
            {
                if (!ClosestTarget(ref target))
                {
                    hasTarget = false;
                    needMoreTarget = true;
                    return;
                }
            }
        }
        while (!target.Health.IsAlive)
        {
            targets.Remove(target);
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;
                return;
            }
        }
        hasTarget = true;
        return;
    }
    protected bool ClosestTarget(ref Pawn pawn)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasTarget = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (!controlledPawn.Health.Team.Equals(targets[i].Health.Team) && targets[i].Health.Team != Team.world)
            {
                minTmp = (targets[i].transform.position - controlledPawn.transform.position).sqrMagnitude;
                if (minM > minTmp)
                {
                    if (CheckTargetVisibility(targets[i]))
                    {
                        hasTarget = true;
                        minM = minTmp;
                        pawn = targets[i];
                    }
                }
            }
        }
        return hasTarget;
    }
    protected bool ClosestFriend(ref Pawn pawn)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasTarget = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (controlledPawn.Health.Team.Equals(targets[i].Health.Team)   )
            {
                minTmp = (targets[i].transform.position - controlledPawn.transform.position).sqrMagnitude;
                if (minM > minTmp)
                {
                    hasTarget = true;
                    minM = minTmp;
                    pawn = targets[i];
                }
            }
        }
        return hasTarget;
    }

    protected virtual void AddTarget(Pawn target)
    {
        targets.Add(target);
    }
    protected virtual void RemoveTarget(Pawn target)
    {
        targets.Remove(target);
    }
    protected virtual void OnPerceptionDetection(StimulusStruct stimulus)
    {
        if (stimulus.enter)
        {
            if (!targets.Contains(stimulus.target))
            {
                AddTarget(stimulus.target);
                needMoreTarget = false;
            }
        }
        else
        {
            if (hasTarget && target.Equals(stimulus.target))
            {
                RemoveTarget(target);
                hasTarget = false;
                target = null;
            }
            else
            {
                if (targets.Contains(stimulus.target))
                {
                    targets.Remove(stimulus.target);
                }
            }
        }
    }
    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (pathState.Equals(PathState.has))
            {
                for (int i = 0; i < pathLength; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(waypoints[i], Vector3.one * 0.5f);
                }
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(waypoints[currentPoint], Vector3.one * 0.25f);
                Gizmos.DrawLine(controlledPawn.Center, waypoints[currentPoint]);
            }
            if (hasTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(target.Center, Vector3.one * 0.25f);
                Gizmos.DrawLine(controlledPawn.Center, target.Center);
            }
        }
    }
}
