using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        SetupScene,
        SetupUI,
        SetupStage,
        Stage,
        End
    }
    public static GameManager instance;
    private Transform _environment;
    private MapGenerator _mapGenerator;
    private HealthDisplay _healthUI;
    private WeaponInventoryDisplay _weaponInventoryUI;
    private Cinemachine.CinemachineVirtualCamera _VirtualCamera;
    private HealthSystem _playerHealthSystem;
    private bool _stageStarted = false;
    [SerializeField] private int _stageCompletionCoins = 30;
    public int _noEnemies = 0;
    public int _noCoins = 0;
    private GameState _gameState = GameState.SetupScene;
    private void Awake()
    {
        _environment = GameObject.Find("----- Environment -----").transform;
        _healthUI = GameObject.FindObjectOfType<HealthDisplay>();
        _weaponInventoryUI = GameObject.FindObjectOfType<WeaponInventoryDisplay>();
        _VirtualCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
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
    // this scripts responsibility is to manage the game state
    private void Update()
    {
        // Debug.Log("current state:" + _gameState.ToString());
        switch (_gameState)
        {
            case GameState.SetupScene:
                InitilisePlayerCharacter();
                InitialiseMapGenerator();
                _VirtualCamera.Follow = GameAssets.instance.playerCharacter.transform;
                _gameState = GameState.SetupUI;
                break;
            case GameState.SetupUI:
                _healthUI.InitialiseHealthBarUI();
                _weaponInventoryUI.InitialiseWeaponInventoryDisplayUI();
                _gameState = GameState.SetupStage;
                break;
            case GameState.SetupStage:
                _noCoins += _stageCompletionCoins;
                _mapGenerator.ResetGrid();
                _mapGenerator.GenerateLevel();
                _mapGenerator.SpawnLevel();
                GameAssets.instance.playerCharacter.transform.position = _mapGenerator.FirstWalkerSpawnWorldPosition;
                _gameState = GameState.Stage;
                break;
            case GameState.Stage:
                if (_playerHealthSystem.IsDead)
                {
                    _gameState = GameState.End;
                    PauseMenu.GameOver();
                }
                if(_noEnemies <= 0 && _stageStarted)
                {
                    GameObject nextStageGO = Instantiate(GameAssets.instance.pfNextStage);
                    nextStageGO.transform.position = _mapGenerator.FirstWalkerSpawnWorldPosition;
                }
                else if (_noEnemies > 0)
                {
                    _stageStarted = true;
                }
                break;
            case GameState.End:
                break;
        }
        /* helper functions */
        // setup scene
        void InitilisePlayerCharacter()
        {
            GameObject playerCharacterGO = Instantiate(GameAssets.instance.pfPlayerCharacter);
            playerCharacterGO.transform.SetParent(_environment);
            GameAssets.instance.playerCharacter = playerCharacterGO;
            _playerHealthSystem = GameAssets.instance.playerCharacter.GetComponent<HealthSystem>();
        }

        void InitialiseMapGenerator()
        {
            GameObject mapGeneratorGO = Instantiate(GameAssets.instance.pfMapGenerator);
            mapGeneratorGO.transform.SetParent(_environment);
            _mapGenerator = mapGeneratorGO.GetComponent<MapGenerator>();
        }
    }
    public void ResetStage()
    {
        _stageStarted = false;
        _gameState = GameState.SetupStage;
    }
}
