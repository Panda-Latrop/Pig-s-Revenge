using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStaticBombActor : ProjectileActor
{
    [SerializeField]
    protected float radius;
    [SerializeField]
    protected AudioClip sound;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected DynamicActor explosion;
    public override void OnPop()
    {
        base.OnPop();
        source.clip = sound;
        source.time = 0;
        source.Play();
    }
    public override void OnPush()
    {
        base.OnPush();
        source.Stop();
    }
    protected override void CheckCollision()
    {
        List<Collider2D> targets = new List<Collider2D>();
        targets.AddRange(Physics2D.OverlapBoxAll(transform.position, new Vector2(radius*2.0f, 0.25f), 0.0f, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10)));
        targets.AddRange(Physics2D.OverlapBoxAll(transform.position, new Vector2(0.25f, radius * 2.0f), 0.0f, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10)));
        // targets = Physics2D.OverlapCircleAll(transform.position, radius, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10));
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Collider2D target = targets[i];
                IHealth health = target.GetComponent<IHealth>();
                if (health != null)
                {
                    RaycastHit2D hit = new RaycastHit2D();
                    health.Hurt(ds, hit);
                }
            }
        }
    }
    protected override void OnLateUpdate()
    {
        if (Time.time >= nextPush)
        {
            CheckCollision();
            GameInstance.Instance.PoolManager.Pop(explosion).SetPosition(transform.position);
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
