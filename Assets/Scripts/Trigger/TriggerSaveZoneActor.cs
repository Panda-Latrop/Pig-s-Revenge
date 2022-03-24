using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSaveZoneActor : TriggerActor
{
    [SerializeField]
    protected Team requiredTeam;
    protected override void CheckCollision(Collider2D other)
    {
        if (!other.name.Equals(GameInstance.Instance.PlayerController.ControlledPawn.name))
        {
            if((1<<other.gameObject.layer) == (1 << 8))
            {
                Pawn pawn = other.GetComponent<Pawn>();
                if (pawn.Health.Team.Equals(requiredTeam))
                {
                    (GameInstance.Instance.GameState as GameBattleState).SaveZone(pawn);
                    Execute(other);
                    if (closeOnEnter)
                        collider.enabled = false;
                }
            }          
        }
    }
}
