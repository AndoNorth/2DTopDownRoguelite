using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    private HealthSystem _healthSystem;
    private void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _healthSystem.OnHealthChanged += UpdateVisuals;
    }
    private void OnEnable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged += UpdateVisuals;
        }
    }
    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= UpdateVisuals;
        }
    }
    private void UpdateVisuals()
    {
        if(_healthSystem.CurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
