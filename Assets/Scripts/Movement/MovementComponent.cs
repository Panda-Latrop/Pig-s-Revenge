using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PathState
{
    none,
    request,
    has,
}
public class MovementComponent : ActorComponent, ISaveableComponent
{
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected float maxSpeed = 10.0f, speedMultiply = 1.0f;

    protected bool hasMove,hasJump;

    protected Vector2 direction,velocity;

    public bool IsMove => hasMove;
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public float SpeedMultiply { get => speedMultiply; set => speedMultiply = value; }
    public Vector2 Direction { get => direction; set => direction = value; }

    public void SetRigidbody(Rigidbody2D rigidbody)
    {
        this.rigidbody = rigidbody;
    }
    public void Teleport(Vector2 position)
    {
        rigidbody.MovePosition(rigidbody.position+position);
    }
    public void Move(Vector3 direction)
    {
        
        if (direction.sqrMagnitude > 0.1f)
        {
            this.direction = direction;   
            hasMove = true;
        }
        else
        {
            hasMove = false;
        }
    }
   public void Stop()
    {
        hasMove = false;
        velocity = Vector2.zero;
        rigidbody.velocity = velocity;
    }
    protected override void OnFixedUpdate()
    {
        velocity = Vector2.zero;
        if (hasMove)
        {
            velocity = direction * maxSpeed * speedMultiply;
            hasMove = false;
        }
        rigidbody.velocity = velocity;
    }
    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
}
