using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    public Canvas gameover, gamewin;
    public Text lives, score;
    protected GameBattleState state;
    public void Start()
    {
        state = GameInstance.Instance.GameState as GameBattleState;
    }
    private void LateUpdate()
    {
        lives.text = "x" + state.PlayerLives;
        score.text = state.SaveCount + ":" + state.MaxSaveCount;
        if (state.IsGameOver)
            gameover.enabled = true;
        if (state.IsGameWin)
            gamewin.enabled = true;
    }
}
