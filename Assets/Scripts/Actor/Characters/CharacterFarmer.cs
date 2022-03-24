using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFarmer : Character
{
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnHurt(ds, raycastHit);
        animation.PlayHurt();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if((1<<collision.collider.gameObject.layer) == (1<<7))
        {
            Shoot(true, Center, (collision.collider.transform.position - Center).normalized);
        }
    }
}
