using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    protected GameState gameState;
    [SerializeField]
    protected PlayerController playerController;
    [SerializeField]
    protected Pawn playerPawn;
    [SerializeField]
    protected new CameraActor camera;
    [SerializeField]
    protected  Navigation navigation;
    [SerializeField]
    protected MusicHolder musicHolder;
    public GameState GameState => gameState;
    public PlayerController PlayerController => playerController;
    public Pawn PlayerPawn => playerPawn;
    public CameraActor Camera => camera;
    public Navigation Navigation => navigation;
    public MusicHolder MusicHolder => musicHolder;
}
