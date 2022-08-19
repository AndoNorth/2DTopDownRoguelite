using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    public int _noEnemies = 0;
    public int _noCoins = 0;
    private GameState _gameState;
    private enum GameState
    {
        Setup,
        Stage,
        End
    }
    /* 
     * start
     * setup
     * end current stage
     * 
     */
    private void Start()
    {
        _gameState = GameState.Setup;
        _noCoins += 30;
    }
    // this scripts responsibility is to manage the game state
    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Setup:
                if(_noEnemies > 0)
                {
                    _gameState = GameState.Stage;
                }
                break;
            case GameState.Stage:
                bool IsCharacterDead = GameAssets.instance.playerCharacter.GetComponent<HealthSystem>().CurrentHealth <= 0;
                if (_noEnemies <= 0 || IsCharacterDead)
                {
                    _gameState = GameState.End;
                    PauseMenu.GameOver();
                }
                break;
            case GameState.End:
                break;
        }
    }
}
