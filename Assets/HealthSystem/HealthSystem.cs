using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour, IDamagable
{
    [SerializeField] private string _characterName;
    [SerializeField] private int _maxHealth;
    public int MaxHealth { get { return _maxHealth; } }
    [SerializeField] private bool _hasHealthBar;
    [SerializeField] private bool _damageTextVisuals;
    [SerializeField] private int _damageFont;
    [SerializeField] private bool _takeOneDamage;
    private int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } }
    public bool IsDead { get { return _currentHealth <= 0; } }
    private HealthBar _healthBar = null;
    public Action OnHealthChanged = delegate { };
    [SerializeField] private GameObject _pfBloodParticles;
    [SerializeField] private bool _bloodParticles;
    private void Start()
    {
        if (_hasHealthBar)
        {
            _healthBar = new HealthBar(transform.position, _characterName, transform);
        }
        SetToMaxHealth();
    }
    private void Update()
    {
        _healthBar?.SetInverseRotation(Quaternion.Euler(0.0f,0.0f, transform.rotation.z));
    }
    public HealthSystem(int maxHealth)
    {
        _maxHealth = maxHealth;
        SetToMaxHealth();
    }
    public HealthSystem(int maxHealth, HealthBar healthBar)
    {
        SetHealthBar(healthBar);
        _maxHealth = maxHealth;
        SetToMaxHealth();
    }
    public void SetToMaxHealth()
    {
        SetHealth(_maxHealth);
    }
    private void SetHealth(int health)
    {
        _currentHealth = health;
        UpdateHealthBar();
        if (OnHealthChanged != null)
        {
            OnHealthChanged();
        }
    }
    private void SetHealthBar(HealthBar healthBar)
    {
        _healthBar = healthBar;
    }
    private void UpdateHealthBar()
    {
        _healthBar?.SetHealthBar(HealthPercent());
    }
    public float HealthPercent()
    {
        return (float)_currentHealth / (float)_maxHealth;
    }
    public void TakeDamage(int damage)
    {
        int damageToHealth = damage;
        if (_takeOneDamage)
        {
            damageToHealth = 1;
        }
        _currentHealth -= damageToHealth;
        if (_damageTextVisuals)
        {
            TemplateProject.TextPopup.Create(transform.position, damage, _damageFont, UnityEngine.Random.insideUnitCircle, TemplateProject.TextPopup.TextPopupEffect.POP, 20, 0.5f);
        }
        if (_bloodParticles)
        {
            Instantiate(_pfBloodParticles, transform);
        }
        if (IsDead)
        {
            SetHealth(0);
        }
        UpdateHealthBar();
        if(OnHealthChanged != null)
        {
            OnHealthChanged();
        }
    }
}
