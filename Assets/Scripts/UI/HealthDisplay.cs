using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    private Text _healthText;
    private HealthSystem _healthSystem;
    private GeneralUtility.UI_Bar _healthBarUI;
    [SerializeField] Vector2 barSize;
    [SerializeField] Vector2 anchorPosition;

    public void InitialiseHealthBarUI()
    {
        GeneralUtility.UI_Bar.Outline outline = new GeneralUtility.UI_Bar.Outline(2f, Color.black);
        _healthBarUI = new GeneralUtility.UI_Bar(transform, anchorPosition, barSize, Color.white, Color.red, 1f, outline);
        _healthText = GetComponentInChildren<Text>();
        _healthSystem = GameObject.FindGameObjectWithTag("Character").GetComponent<HealthSystem>();
        _healthSystem.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        if(_healthSystem != null)
        {
            _healthSystem.OnHealthChanged += UpdateHealthUI;
        }
    }
    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= UpdateHealthUI;
        }
    }
    private void UpdateHealthUI()
    {
        if(_healthSystem != null)
        {
            int playerMaxHealth = _healthSystem.MaxHealth;
            int playerCurrentHealth = _healthSystem.CurrentHealth;
            _healthText.text = playerCurrentHealth.ToString() + '/' + playerMaxHealth.ToString();
            _healthBarUI.SetSize(_healthSystem.HealthPercent());
        }
    }
}