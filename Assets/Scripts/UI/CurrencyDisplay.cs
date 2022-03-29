using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyDisplay : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        UpdateCurrencyUI();
    }
    private void FixedUpdate()
    {
        UpdateCurrencyUI();
    }
    private void UpdateCurrencyUI()
    {
        _textMeshPro.SetText(GameManager.instance._noCoins.ToString());
    }
}
