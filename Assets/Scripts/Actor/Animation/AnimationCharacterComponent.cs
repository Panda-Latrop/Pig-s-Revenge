using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacterComponent : AnimationComponent
{
    protected new Rigidbody2D rigidbody;
    protected MovementComponent movement;
    protected int
        shootHash = Animator.StringToHash("Shoot"),
        hurtHash = Animator.StringToHash("Hurt"),
        moveHash = Animator.StringToHash("Move"),
        runHash = Animator.StringToHash("Run"),
        directionXHash = Animator.StringToHash("DirectionX"),
        directionYHash = Animator.StringToHash("DirectionY");
    [SerializeField]
    protected int locomotionLayer, spriteLayer;

    public void SetRigidbody(Rigidbody2D rigidbody)
    {
        this.rigidbody = rigidbody;
    }
    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    protected override void OnLateUpdate()
    {
        bool isMove = movement.IsMove;
        animator.SetBool(moveHash, isMove);
        Vector2 dir = movement.Direction;
        animator.SetFloat(directionXHash, dir.x);
        animator.SetFloat(directionYHash, dir.y);

    }
    public void PlayShoot()
    {
        animator.Play(shootHash);
    }
    public void PlayHurt()
    {
        animator.Play(hurtHash);
    }
    public override void AnimatorMessage(string message, int value = 0)
    {
       
    }
    private void LateUpdate()
    {
        OnLateUpdate();
    }
}
