using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : Pawn
{
    [SerializeField]
    protected new Collider2D collider;
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected AIPerceptionComponent perception;
    [SerializeField]
    protected MovementComponent movement;
    [SerializeField]
    protected new AnimationCharacterComponent animation;
    [SerializeField]
    protected WeaponHolderComponent weaponHolder;
    [SerializeField]
    protected AudioSource voice;

    public Collider2D Collider { get => collider; }
    public Rigidbody2D Rigidbody { get => rigidbody; }
    public AIPerceptionComponent Perception { get => perception; }
    public MovementComponent Movement { get => movement; }
    public AnimationCharacterComponent Animation { get => animation; }
    public WeaponHolderComponent WeaponHolder { get => weaponHolder; }
    public override Vector3 Center => collider.bounds.center;
    public override Bounds Bounds => collider.bounds;
    public AudioSource Voice => voice;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        movement.SetRigidbody(rigidbody);
        animation.SetRigidbody(rigidbody);
        animation.SetMovement(movement);
        weaponHolder.SetOwner(this);
    }
    public override void OnPush()
    {
        base.OnPush();
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = 0.0f;
    }

    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        collider.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = false;
        movement.enabled = false;
        movement.Direction = Vector2.right;
    }

    protected override void OnResurrect()
    {
        collider.enabled = true;
        rigidbody.simulated = true;
        movement.enabled = true;    
    }
    public override void SetRotation(Quaternion rotation) { transform.rotation = rotation; movement.Direction = rotation * Vector2.right; }
    public override void SetTransform(Vector3 position, Quaternion rotation) { transform.position = position; transform.rotation = rotation; movement.Direction = rotation * Vector2.right; }
    public virtual void Move(Vector3 direction)
    {
        movement.Move(direction);
    }
    public virtual void SetFire(bool fire)
    {
        weaponHolder.SetFire(fire);
    }
    public virtual ShootState Shoot(bool fire, Vector3 position, Vector3 direction)
    {
        SetFire(fire);
        return Shoot(position, direction);
    }
    public virtual ShootState Shoot(Vector3 position, Vector3 direction)
    {
        ShootState state = weaponHolder.Shoot(position, direction);
        switch (state)
        {
            case ShootState.initiated:
                animation.PlayShoot();
                break;
            case ShootState.process:
                break;
            case ShootState.ended:
                break;
            default:
                break;
        }
        return state;
    }
    public void PlayVoice(AudioClip clip)
    {
        voice.Stop();
        voice.time = 0;
        voice.clip = clip;
        voice.Play();
    }
    public bool HasWeapon => weaponHolder.HasWeapon;

}