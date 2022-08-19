using UnityEngine;
using UnityEngine.UI;

public class GameStateDisplay : MonoBehaviour
{
    private Text _gameStateText;
    private int _lastNoEnemies;
    void Start()
    {
        _gameStateText = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if(GameManager.instance._noEnemies != _lastNoEnemies)
        {
            _lastNoEnemies = GameManager.instance._noEnemies;
            string text = "Enemies Left: " + _lastNoEnemies;
            if (_lastNoEnemies <= 0)
                text = "All Enemies Killed";
            _gameStateText.text = text;
        }
    }
}
