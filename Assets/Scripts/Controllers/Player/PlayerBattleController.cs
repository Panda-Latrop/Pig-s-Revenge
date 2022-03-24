using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : PlayerController
{
    protected Character character;
    [SerializeField]
    protected float timeDeathPause = 1.0f;
    protected float nextDeathPause;
    protected bool gameOver;
    protected float timeImortal = 1.0f;
    protected float nextImortal;

    protected SimpleUI ui;
    protected void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent< SimpleUI>();  
    }
    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as Character;
        gameInput = true;
    }
    public override void Unpossess()
    {
        base.Unpossess();
        character = null;
        gameInput = false;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnDeath(ds, raycastHit);
        GameOver();
        character.Health.IsImmortal = true;
        nextImortal = Time.time + timeImortal + timeDeathPause;
    }
    public override void GameOver()
    {
        if (!gameOver)
        {
            gameInput = false;
            gameOver = true;
            nextDeathPause = Time.time + timeDeathPause;
        }
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {

    }

    protected override void OnUpdate()
    {
        if (!gameOver)
        {
            if (gameInput)
            {
                //character.Move(ui.left.Direction);
                //character.Shoot(ui.right.Pressed, controlledPawn.transform.position, character.Movement.Direction);
                if (Application.platform.Equals(RuntimePlatform.WindowsEditor))
                {
                    Vector3 move = Vector3.zero;
                    move.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
                    move.Normalize();
                    character.Move(move);

                    if (Input.GetMouseButtonDown(0))
                        character.SetFire(true);
                    else
                    {
                        if (Input.GetMouseButton(0))
                            character.Shoot(true, controlledPawn.transform.position, character.Movement.Direction);
                        else
                        {
                            if (Input.GetMouseButtonUp(0))
                            {
                                character.Shoot(false, Vector3.zero, Vector3.zero);
                            }
                        }
                    }
                }
                else
                {
                    if (Application.platform.Equals(RuntimePlatform.Android))
                    {
                        character.Move(ui.left.Direction);
                        character.Shoot(ui.right.Pressed, controlledPawn.transform.position, character.Movement.Direction);
                    }
                }

            }
        }
    }
    protected override void OnLateUpdate()
    {
        if (gameOver)
        {
            if (Time.time >= nextDeathPause)
            {
                gameInput = true;
                gameOver = false;
                GameInstance.Instance.RespawnPlayer();
                
                //if (Input.GetMouseButtonDown(0) || (Time.time - nextDeathPause) >= 2.0f)
                //{
                //    //GameInstance.Instance.LoadScene("level1",0, 0);
                //}
            }
        }
        else
        {
            if(controlledPawn.Health.IsImmortal && Time.time >= nextImortal)
            {
                controlledPawn.Health.IsImmortal = false;
            }
        }
        //else
        //{
        //    camera.Lerp(controlledPawn.Center);
        //}
    }
    protected void Update()
    {
        OnUpdate();
    }
    protected void LateUpdate()
    {
        OnLateUpdate();
    }
}
