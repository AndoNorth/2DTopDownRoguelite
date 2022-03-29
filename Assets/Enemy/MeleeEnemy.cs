using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private float _attackRange;
    [SerializeField] private float _retreatRange;
    [SerializeField] [Range(0f, 1f)] private float _retreatPercentage;
    [SerializeField] private Color _color;

    private BasicCharacterController _characterController;
    private CharacterWeaponSystem _weaponSystem;
    private HealthSystem _healthSystem;
    private SpriteRenderer _spriteRenderer;
    private GameObject _target;
    private Vector3 _directionVector;
    private State _currentState;

    private enum State
    {
        Chasing,
        Attacking,
        Retreating,
        Dead
    }
    private void Awake()
    {
        _characterController = GetComponent<BasicCharacterController>();
        _weaponSystem = GetComponent<CharacterWeaponSystem>();
        _healthSystem = GetComponent<HealthSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        FindTarget();
        _healthSystem.OnHealthChanged += TakeDamageVisuals;
    }
    private void Update()
    {
        _directionVector = _target.transform.position - transform.position;
        if (_healthSystem.IsDead)
        {
            _currentState = State.Dead;
        }
        else if (InRetreatRange() || _healthSystem.HealthPercent() <= _retreatPercentage)
        {
            _currentState = State.Retreating;
        }
        else if (InAttackRange())
        {
            _currentState = State.Attacking;
        }
        else
        {
            _currentState = State.Chasing;
        }
    }
    private void FixedUpdate()
    {
        _characterController.FaceTarget(_target.transform.position);
        switch (_currentState)
        {
            case State.Chasing:
                _characterController.SetMoveVector(_directionVector.normalized);
                _characterController.RollThisFrame();
                break;
            case State.Attacking:
                _weaponSystem.Fire();
                break;
            case State.Retreating:
                _characterController.SetMoveVector((-_directionVector).normalized);
                break;
            case State.Dead:
                gameObject.SetActive(false);
                break;
        }
    }
    private void OnEnable()
    {
        GameManager.instance._noEnemies++;
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged += TakeDamageVisuals;
        }
    }
    private void OnDisable()
    {
        GameManager.instance._noEnemies--;
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= TakeDamageVisuals;
        }
        GameManager.instance._noCoins++;
    }
    private bool InAttackRange()
    {
        return Vector3.Distance(_target.transform.position, transform.position) <= _attackRange;
    }
    private bool InRetreatRange()
    {
        return Vector3.Distance(_target.transform.position, transform.position) <= _retreatRange;
    }
    private void FindTarget()
    {
        _target = GameAssets.instance.playerCharacter;
    }
    private void TakeDamageVisuals()
    {
        SpriteChanger.instance.ChangeSpriteColorForTime(_spriteRenderer, Color.white, _color, 0.3f);
    }
}
