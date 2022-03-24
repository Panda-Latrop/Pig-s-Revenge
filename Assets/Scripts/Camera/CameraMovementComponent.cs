using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementComponent : ActorComponent
{
    [SerializeField]
    protected float maxSpeed = 10.0f;
    [SerializeField]
    [Range(0.06f,60.0f)]
    protected float lerpOffset = 6.0f;
    [SerializeField]
    protected float speedMultiply = 1.0f;
    [SerializeField]
    protected Vector3 offset;
    protected bool hasMove;
    protected bool hasLerp;
    protected Vector3 direction;

    public float SpeedMultiply { get => speedMultiply; set => speedMultiply = value; }

    public virtual void Teleport(Vector3 point)
    {
        point.z = 0;
        point += offset;
        transform.position = point;
    }
    public virtual void Lerp(Vector3 point)
    {
        hasLerp = true;
        hasMove = false;
        direction = point;
        direction.z = 0;
        direction += offset;
    }
    public virtual void Move(Vector3 direction)
    {
        direction.z = 0;
        if (direction.sqrMagnitude > 0)
        {
            
            this.direction = direction;
            hasMove = true;
            hasLerp = false;
        }
        else
        {
            hasMove = false;
        }
    }

    protected override void OnStart()
    {
        offset.z = transform.position.z;
    }
    protected override void OnLateUpdate()
    {
        if (hasLerp)
        {
            transform.position = Vector3.Lerp(transform.position, direction, lerpOffset * speedMultiply * Time.deltaTime);
            hasLerp = false;
        }
        else
        {
            if (hasMove)
            {
                transform.Translate(direction * maxSpeed * speedMultiply * Time.deltaTime);
                hasMove = false;
            }
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