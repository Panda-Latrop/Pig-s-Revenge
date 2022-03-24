using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeClimbComponent : ActorComponent
{
    protected new Collider2D collider;
    protected Queue<Collider2D> ignore = new Queue<Collider2D>();
    [SerializeField]
    protected Transform mesh;
    [SerializeField]
    protected SpriteRenderer sprite;
    [SerializeField]
    protected Vector3 position;
    protected Vector3 defaultPosition;
    protected bool climb;
    protected float nextStop,nextRemove;

    public void Restart()
    {
        mesh.localPosition = defaultPosition;
        sprite.sortingOrder = 0;
    }

    public void SetCollider(Collider2D collider)
    {
        this.collider = collider;
    }

    protected override void OnStart()
    {
        defaultPosition = mesh.localPosition;
    }
    protected override void OnLateUpdate()
    {
        if (climb)
        {
            sprite.sortingOrder = 1;
            mesh.localPosition = Vector3.Lerp(mesh.localPosition, position, 10 * Time.deltaTime);
            if (Time.time >= nextStop)
                climb = false;
        }
        else
        {
            sprite.sortingOrder = 0;
            mesh.localPosition = Vector3.Lerp(mesh.localPosition, defaultPosition, 10 * Time.deltaTime);
        }
        if(ignore.Count > 0)
        {
            if (Time.time >= nextRemove)
            {          
                Physics2D.IgnoreCollision(this.collider, ignore.Dequeue(), false);
                nextRemove = Time.time + 1.0f;
            }
        }
    }
    private void Start()
    {
        OnStart();
    }
    private void LateUpdate()
    {
        OnLateUpdate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Enter " +collision.collider.gameObject.name);

        Collider2D collider = collision.collider;
        if ((1 << collider.gameObject.layer) == (1 << 7))
        {
            ignore.Enqueue(collider);
            climb = true;
            nextStop = Time.time + 0.7f;
            nextRemove = Time.time + 1.0f;
            Physics2D.IgnoreCollision(this.collider, collider, true);
        }
    }
}
