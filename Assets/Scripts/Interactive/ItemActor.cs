using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Pickup(Character by);
}
public abstract class ItemActor : Actor, IItem
{
    [SerializeField]
    protected new Collider2D collider;
    public override void OnPop()
    {
        base.OnPop();
    }
    public override void OnPush()
    {
        base.OnPush();
    }
    public abstract void Pickup(Character by);

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals(GameInstance.Instance.PlayerController.ControlledPawn.name))
        {

        }
    }
}
