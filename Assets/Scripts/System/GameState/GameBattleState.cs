using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleState : GameState
{
    [SerializeField]
    protected int playerLives, saveCount;
    protected int currentSaveCount;
    [SerializeField]
    protected Character pig, farmer, dog;
    [SerializeField]
    protected float timeToSpawnPig ,timeToSpawnDog, timeToSpawnFarmer;
    [SerializeField]
    protected int pigCount, dogCount, farmerCount;
    protected int currentPigCount, currentDogCount, currentFarmerCount;
    [SerializeField]
    protected Transform enemyPoint, outGridPoint;
    protected float nextSpawnPig, nextSpawnDog, nextSpawnFarmer;
    protected int savePig;
    [SerializeField]
    protected DynamicActor pigSaveEffect,spawnEffect;
    protected bool gameOver, gameWin;
    protected float timeRestart = 3.0f;
    protected float nextRestart;

    public int SaveCount => currentSaveCount;
    public int MaxSaveCount => saveCount;
    public int PlayerLives => playerLives;
    public bool IsGameOver => gameOver;
    public bool IsGameWin => gameWin;
    public void SaveZone(Pawn pawn)
    {
        if (pawn.Specifier.Equals(pig.Specifier))
        {
            GameInstance.Instance.PoolManager.Pop(pigSaveEffect).SetPosition(pawn.transform.position);
            pawn.Controller.Unpossess();
            GameInstance.Instance.PoolManager.Push(pawn);
            currentPigCount--;
            currentSaveCount++;
            if(currentSaveCount >= saveCount)
            {
                gameWin = true;
                nextRestart = Time.time + timeRestart;
            }
        }
    }
    protected override void OnStart()
    {
        base.OnStart();       
        GameInstance.Instance.Navigation.grid.CreateGrid();
        nextSpawnPig = Time.time + timeToSpawnPig;
        nextSpawnDog = Time.time + timeToSpawnDog;
        nextSpawnFarmer = Time.time + timeToSpawnFarmer;
    }
    public override void PawnDeath(Pawn actor, DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (actor.name.Equals(GameInstance.Instance.PlayerController.ControlledPawn.name))
        {
            playerLives--;
            if(playerLives <= 0)
            {
                gameOver = true;
                nextRestart = Time.time + timeRestart;
            }
        }
        else
        {
            if (actor.Specifier.Equals(pig.Specifier))
            {
                currentPigCount--;
                nextSpawnPig = Time.time + timeToSpawnPig;
            }
            else
            {
                if (actor.Specifier.Equals(dog.Specifier))
                {
                    currentDogCount--;
                    nextSpawnDog = Time.time + timeToSpawnDog;
                }
                else
                {
                    if (actor.Specifier.Equals(farmer.Specifier))
                    {
                        currentFarmerCount--;
                        nextSpawnFarmer = Time.time + timeToSpawnFarmer;
                    }
                }
            }
        }
    }
    protected override void OnUpdate()
    {
        if(currentPigCount < pigCount && Time.time >= nextSpawnPig)
        {
            NavigationGrid grid = GameInstance.Instance.Navigation.grid;
            Pawn pawn = GameInstance.Instance.PoolManager.Pop(pig) as Pawn;
            pawn.SetPosition(grid.GetOpenNode(Random.Range(0, grid.OpenLenth)).position);
            GameInstance.Instance.PoolManager.Pop(spawnEffect).SetPosition(pawn.transform.position);
            currentPigCount++;
            nextSpawnPig = Time.time + timeToSpawnPig;

        }
        if (currentDogCount < dogCount && Time.time >= nextSpawnDog)
        {
            Pawn pawn = GameInstance.Instance.PoolManager.Pop(dog) as Pawn;
            pawn.SetPosition(enemyPoint.position);
            GameInstance.Instance.PoolManager.Pop(spawnEffect).SetPosition(pawn.transform.position);
            currentDogCount++;
            nextSpawnDog = Time.time + timeToSpawnDog;

        }
        if (currentFarmerCount < farmerCount && Time.time >= nextSpawnFarmer)
        {
            Pawn pawn = GameInstance.Instance.PoolManager.Pop(farmer) as Pawn;
            pawn.SetPosition(enemyPoint.position);
            GameInstance.Instance.PoolManager.Pop(spawnEffect).SetPosition(pawn.transform.position);
            currentFarmerCount++;
            nextSpawnFarmer = Time.time + timeToSpawnFarmer;

        }
        if((gameOver || gameWin) && Time.time >= nextRestart)
        {
            GameInstance.Instance.LoadScene(0, 0);
        }
    }
    private void Update()
    {
        OnUpdate();
    }
    private void Start()
    {
        OnStart();
    }
}
