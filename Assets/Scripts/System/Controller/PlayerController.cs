using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    public bool gameInput = true;
    protected GameState gameState;
    protected new CameraActor camera;

    public void SetCamera(CameraActor camera)
    {
        this.camera = camera;
        this.camera.Teleport(controlledPawn.Center);
    }
    public void SetGameState(GameState gameState) => this.gameState = gameState;

    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        gameState.PawnDeath(controlledPawn, ds, raycastHit);
    }
    public virtual void GameOver()
    {

    }
}
