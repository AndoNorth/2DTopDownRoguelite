using System;
using UnityEngine;

[RequireComponent(typeof(BasicCharacterController))]
[RequireComponent(typeof(CharacterWeaponSystem))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Astar2DPathfinding))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private bool _debugStateMachine = true;
    [Header("Behaviour")]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _aggroRange;
    [SerializeField] private float _retreatRange;
    [SerializeField][Range(0f, 1f)] private float _retreatPercentage;
    [SerializeField] private float _patrolRadius = 4.0f;
    [SerializeField] private bool _attackWhileRetreating;
    [SerializeField] private bool _canRoll;
    [Header("Visuals")]
    [SerializeField] private Color _color;

    // systems
    private StateMachine _stateMachine;
    private BasicCharacterController _characterController;
    private CharacterWeaponSystem _weaponSystem;
    private HealthSystem _healthSystem;
    private SpriteRenderer _spriteRenderer;
    private Astar2DPathfinding _pathfinding;

    // internal variables
    private GameObject _target;
    private Vector3 _directionVector;
    private Vector3 _patrolAnchorPos;
    private bool _isSetup;
    public bool SetupBool { get { return _isSetup; } }
    bool _reset;
    public bool ResetBool { get { return _reset; } }

    private void Awake()
    {
        _characterController = GetComponent<BasicCharacterController>();
        _weaponSystem = GetComponent<CharacterWeaponSystem>();
        _healthSystem = GetComponent<HealthSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _pathfinding = GetComponent<Astar2DPathfinding>();
    }
    private void Start()
    {
        _healthSystem.OnHealthChanged += TakeDamageVisuals;
        Reset();
    }
    private void FixedUpdate()
    {
        Tick();
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
    }
    private void FindTarget() => _target = GameAssets.instance.playerCharacter;
    private void Reset()
    {
        SetReset(false);
        SetSetup(false);

        ResetHealthSystem();

        FindTarget();
        ResetPathfindingSystem();

        SetupStateMachine();

        void ResetHealthSystem()
        {
            _healthSystem.SetToMaxHealth();
        }
        void ResetPathfindingSystem()
        {
            _directionVector = Vector3.zero;
            _patrolAnchorPos = this.transform.position;
            _pathfinding.SetTarget(this.transform.position);
            _pathfinding.SetTarget(_target);
            _pathfinding.CancelFindPath();
        }
    }
    public void Spawn(Vector3 spawnPos)
    {
    }

    // interfaces for state machine
    // setup
    private void SetSetup(bool setup) => _isSetup = setup;
    private void SetReset(bool reset) => _reset = reset;
    // patrol
    private void PatrolMovement()
    {
        if (_pathfinding.EndNodeReached)
        {
            _directionVector = Vector3.zero;
            SetRandomPatrolPoint();
            return;
        }
        _directionVector = (_pathfinding.NextPosition - this.transform.position).normalized;
    }
    private void SetPatrolAnchorPosition(Vector3 pos) => _patrolAnchorPos = pos;
    private void SetRandomPatrolPoint()
    {
        Vector3 targetPos = UnityEngine.Random.insideUnitCircle * _patrolRadius;
        targetPos += _patrolAnchorPos;
        _pathfinding.FindPath(this.transform.position, targetPos);
    }
    // aggro
    private bool CanAttack => InRange(_attackRange) && _weaponSystem.ReadyToFire;
    private bool CanRoll => _canRoll;
    private void Attack()
    {
        _characterController.FaceTarget(_target.transform.position);
        _weaponSystem.Fire();
    }
    private void ChaseMovement()
    {
        if (_pathfinding.EndNodeReached)
        {
            _directionVector = Vector3.zero;
            return;
        }
        _directionVector = (_pathfinding.NextPosition - this.transform.position).normalized;
        if (CanRoll)
        {
            _characterController.RollThisFrame();
        }
    }
    // flee
    private bool CanAttackWhileRetreating => _attackWhileRetreating;
    private void FleeMovement()
    {
        _directionVector = -(_target.transform.position - this.transform.position).normalized;
    }
    // dead
    private void DeadAction()
    {
        gameObject.SetActive(false);
    }
    // generic actions
    private void HandleMovement()
    {
        if (InRange(_aggroRange))
        {
            _characterController.FaceTarget(_target.transform.position);
        }
        else
        {
            float rotZ = Mathf.Atan2(_directionVector.y, _directionVector.x) * Mathf.Rad2Deg;
            _characterController.FaceTarget(rotZ);
        }
        _characterController.SetMoveVector(_directionVector);
    }
    private void InvokeFindPath() => _pathfinding.InvokeFindPath();
    private void CancelFindPath() => _pathfinding.CancelFindPath();
    // visuals
    private void TakeDamageVisuals() => SpriteChanger.instance.ChangeSpriteColorForTime(_spriteRenderer, Color.white, _color, 0.3f);
    private void StatePopupText() => TemplateProject.TextPopup.Create(Vector3.zero, _stateMachine.CurrentState, 16, Vector3.up, TemplateProject.TextPopup.TextPopupEffect.FLOAT, 1f, 1f);
    // AI
    private void SetupStateMachine()
    {
        // initialise states
        TemplateState template = new TemplateState(_debugStateMachine, this);
        ResetState reset = new ResetState(_debugStateMachine, this);
        SetupState setup = new SetupState(_debugStateMachine, this);
        PatrolState patrol = new PatrolState(_debugStateMachine, this);
        AggroState aggro = new AggroState(_debugStateMachine, this);
        FleeState flee = new FleeState(_debugStateMachine, this);
        DeadState dead = new DeadState(_debugStateMachine, this);
        // initialise state machine
        _stateMachine = new StateMachine();
        // state transitions
        At(setup, patrol, IsSetup());
        At(patrol, aggro, AggroBool());
        At(aggro, patrol, LoseAggroBool());
        At(flee, aggro, SafeDistance());
        At(flee, patrol, LoseAggroBool());
        _stateMachine.addAnyTransition(reset, IsResetSet());
        _stateMachine.addAnyTransition(setup, IsNotSetup());
        _stateMachine.addAnyTransition(dead, IsDead());
        _stateMachine.addAnyTransition(flee, RetreatBool());
        // func bool methods
        Func<bool> IsSetup() => () => SetupBool;
        Func<bool> IsNotSetup() => () => !SetupBool;
        Func<bool> IsResetSet() => () => ResetBool;
        Func<bool> AggroBool() => () => InRange(_aggroRange);
        Func<bool> RetreatBool() => () => InRange(_retreatRange) || HPBelowRetreatThreshold();
        Func<bool> SafeDistance() => () => OutOfRange(_retreatRange);
        Func<bool> LoseAggroBool() => () => OutOfRange(_aggroRange);
        Func<bool> IsDead() => () => _healthSystem.IsDead;
        // shortcut to add transition
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.addTransition(from, to, condition);
    }
    // booleans
    private bool InRange(float range) => Vector3.Distance(_target.transform.position, transform.position) <= range;
    private bool OutOfRange(float range) => Vector3.Distance(_target.transform.position, transform.position) > range;
    private bool HPBelowRetreatThreshold() => _healthSystem.HealthPercent() <= _retreatPercentage;
    private void Tick() => _stateMachine.Tick();
    // states
    #region States
    class SetupState : IState
    {
        string stateName = "Setup State";
        public string StateName() => stateName;
        private bool debugLogs = false;
        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public SetupState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            if (!_enemy._isSetup)
            {
                _enemy.SetSetup(true);
            }
        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    class PatrolState : IState
    {
        string stateName = "Patrol State";
        public string StateName() => stateName;
        private bool debugLogs = false;
        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public PatrolState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            _enemy.PatrolMovement();
            _enemy.HandleMovement();
        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
            _enemy.SetPatrolAnchorPosition(_enemy.transform.position);
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    class DeadState : IState
    {
        string stateName = "Dead State";
        public string StateName() => stateName;
        private bool debugLogs = false;
        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public DeadState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            _enemy.DeadAction();
        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    class FleeState : IState
    {
        string stateName = "Flee State";
        public string StateName() => stateName;
        private bool debugLogs = false;
        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public FleeState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            _enemy.FleeMovement();
            _enemy.HandleMovement();
            if(_enemy.CanAttack && _enemy.CanAttackWhileRetreating)
            {
                _enemy.Attack();
            }
        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    class AggroState : IState
    {
        string stateName = "Aggro State";
        public string StateName() => stateName;
        private bool debugLogs = false;
        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public AggroState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            _enemy.ChaseMovement();
            _enemy.HandleMovement();
            if (_enemy.CanAttack)
            {
                _enemy.Attack();
            }
        }

        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
            _enemy.InvokeFindPath();
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
            _enemy.CancelFindPath();
        }
    }
    class ResetState : IState
    {
        string stateName = "Reset State";
        public string StateName() => stateName;
        private bool debugLogs = false;

        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public ResetState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {
            _enemy.Reset();
        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    class TemplateState : IState
    {
        string stateName = "Template State";
        public string StateName() => stateName;
        private bool debugLogs = false;

        Vector3 textPosition => _enemy.transform.position + Vector3.up * 0.5f;

        private Enemy _enemy;
        public TemplateState(bool debug, Enemy enemy)
        {
            debugLogs = debug;
            _enemy = enemy;
        }
        public void Tick()
        {

        }
        public void OnEnter()
        {
            if (debugLogs)
            {
                Debug.Log("Entered " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        public void OnExit()
        {
            if (debugLogs)
            {
                Debug.Log("Exiting " + stateName);
                TemplateProject.TextPopup.Create(textPosition, "Entered State: " + stateName, 12, Vector3.zero, TemplateProject.TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
    }
    #endregion
}
