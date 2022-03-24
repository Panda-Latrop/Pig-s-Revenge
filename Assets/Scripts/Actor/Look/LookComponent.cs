using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookComponent : ActorComponent
{
    [SerializeField]
    protected Transform look;
    [SerializeField]
    protected float speedMultiply,angular;
    protected Vector3 target;
    protected Vector3 axis;
    protected bool hasRotate;
    public float SpeedMultiply { get => speedMultiply; set => speedMultiply = value; }
    public void Rotate(Vector3 to, Vector3 axis)
    {
        Vector3 from = look.position, to2 = to, axis2 = Vector3.one - axis;
        from.Scale(axis2);
        to.Scale(axis2);
        if (axis.sqrMagnitude > 0.1f && !from.Equals(to2))
        {
            this.target = to;
            this.axis = axis;
            hasRotate = true;
        }
        else
        {
            hasRotate = false;
        }
    }
    protected virtual void ApplyRotation()
    {
        Vector3 from = look.position, to = target,axis = Vector3.one - this.axis;
        from.Scale(axis);
        to.Scale(axis);
        Quaternion rotation = Quaternion.LookRotation((to - from).normalized, this.axis);
        look.rotation = Quaternion.RotateTowards(look.rotation, rotation, angular * Time.fixedDeltaTime);
    }
    protected override void OnFixedUpdate()
    {
        if (hasRotate)
        {
            ApplyRotation();
            hasRotate = false;
        }
    }
    protected void FixedUpdate()
    {
        OnFixedUpdate();
    }
}
