using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileActor : DynamicActor, IPoolProjectile
{
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    //[SerializeField]
    //protected float rayLength = 2.0f;
    [SerializeField]
    protected MovementComponent movement;
    protected Vector3 direction;
    protected RaycastHit2D hit;
    [SerializeField]
    protected DamageStruct ds;
    [SerializeField]

    public virtual void SetDamage(DamageStruct ds, float speed)
    {
        this.ds = ds;
        movement.MaxSpeed = speed;
    }
    public virtual void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public override void OnPop()
    {
        base.OnPop();
        movement.enabled = true;
    }
    public override void OnPush()
    {
        base.OnPush();
        movement.Stop();
        movement.enabled = false;
    }
    protected virtual void HitProcessing(RaycastHit2D hit, Vector3 direction, float power, HurtResult result)
    {

        if (!result.Equals(HurtResult.friend) && !result.Equals(HurtResult.none) && !result.Equals(HurtResult.projectile))
        {
            if (!result.Equals(HurtResult.kill))
            {
                if (result.Equals(HurtResult.miss))
                {
                    int layer = 1 << hit.collider.gameObject.layer;
                    if (layer == (1 << 7) || layer == (1 << 10))
                    {
                        hit.rigidbody.AddForceAtPosition(direction * power, hit.point, ForceMode2D.Impulse);
                    }
                }
            }
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
    protected virtual void CheckCollision()
    { }
    //protected virtual void CheckCollision()
    //{
    //    IHealth health;
    //    HurtResult result;
    //    Vector3 origin = transform.position;
    //    Vector3 direction = transform.forward;
    //    if (Physics.Raycast(origin - direction * rayLength*0.5f,  direction , out hit, rayLength * 2, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide))
    //    {
    //        bool isPawnLayer = (1 << hit.collider.gameObject.layer) == (1 << 8);
    //        if (isPawnLayer)
    //        {
    //            health = hit.collider.GetComponent<IHealth>();
    //            ds.direction = direction;
    //            result = health.Hurt(ds, hit);
    //        }
    //        else
    //        {
    //            result = HurtResult.miss;
    //        }
    //        HitProcessing(hit, direction, ds.power, result);
    //        return;
    //    }
    //}
    protected override void OnAwake()
    {
        movement.SetRigidbody(rigidbody);
    }
    //protected override void OnUpdate()
    //{
    //    movement.Move(direction);
    //}
    //protected override void OnFixedUpdate()
    //{
    //    CheckCollision();
    //}
    protected void Awake()
    {
        OnAwake();
    }
    //protected void FixedUpdate()
    //{
    //    OnFixedUpdate();
    //}
//#if UNITY_EDITOR
//    protected void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawLine(transform.position- transform.forward * rayLength, transform.position + transform.forward * rayLength);
//    }
//#endif
}
