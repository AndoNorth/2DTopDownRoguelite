using UnityEngine;

public class BasicCharacterController : MonoBehaviour
{
    [SerializeField] private float _offset;
    private Rigidbody2D _rb;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private bool _moveWithTransform;
    [SerializeField] private LayerMask _moveCollisionMask;
    private Vector3 _moveVector;
    private Vector3 _lastMoveVector;
    [Header("Roll")]
    [SerializeField] private float _maxRollSpeed = 150f;
    [SerializeField] private float _minRollSpeed = 25f;
    [SerializeField] private bool _smoothDecelleration;
    [SerializeField] private float _rollDecelleration = 25f;
    [SerializeField] private float _rollSpeedDropMultiplier = 5f;
    [SerializeField] private float _rollDistance = 50f;
    [SerializeField] private float _rollCooldown = 3f;
    [SerializeField] private LayerMask _rollCollisionMask;
    [SerializeField] private float _rollThisFrameBufferTime = 0.1f;
    private bool _rollThisFrame;
    private Vector3 _rollVector;
    private float _rollSpeed;
    private float _rollCooldownTimer;
    private float _rollThisFrameBufferTimer;
    private State _state;
    private enum State
    {
        Normal,
        Rolling,
    }
    private void Awake()
    {
        _rb = transform.GetComponent<Rigidbody2D>();
        _state = State.Normal;
    }
    private void Update()
    {
        switch (_state)
        {
            case State.Normal:
                if(_moveVector.x != 0 || _moveVector.y != 0)
                {
                    _lastMoveVector = _moveVector;
                }
                break;
            case State.Rolling:
                if (_moveWithTransform)
                {
                    break;
                }
                ResolveRoll();
                break;
        }
        RollBuffer();
    }
    private void FixedUpdate()
    {
        switch (_state)
        {
            case State.Normal:
                if (_rollThisFrame && _rollCooldownTimer == 0)
                {
                    if (_moveWithTransform)
                    {
                        Dash();
                    }
                    else
                    {
                        StartRoll();
                    }
                }
                else
                {
                    CooldownRoll();
                    Move();
                }
                break;
            case State.Rolling:
                Rolling();
                break;
        }
    }
    // movement
    public void SetMoveVector(Vector3 moveVector)
    {
        _moveVector = moveVector;

    }
    private void Move()
    {
        if (_moveWithTransform)
        {
            MoveWithTransform();
        }
        else
        {
            MoveWithVelocity();
        }
    }
    private void MoveWithVelocity()
    {
        _rb.velocity = _moveVector * _moveSpeed;

    }
    private void MoveWithTransform()
    {
        transform.position += _moveVector * _moveSpeed * Time.deltaTime;
    }
    // rolling
    public void RollThisFrame()
    {
        _rollThisFrame = true;
        _rollThisFrameBufferTimer = _rollThisFrameBufferTime;
    }
    private void StartRoll()
    {
        _rollVector = _lastMoveVector;
        _rollSpeed = _maxRollSpeed;
        _rollCooldownTimer = _rollCooldown;
        _state = State.Rolling;
        _rollThisFrame = false;
    }
    private void Rolling()
    {
        _rb.velocity = _rollVector * _rollSpeed;
    }
    private void ResolveRoll()
    {
        float speedReduction = _rollSpeed * _rollSpeedDropMultiplier;
        if (_smoothDecelleration)
        {
            speedReduction = _rollDecelleration;
        }
        _rollSpeed -= speedReduction * Time.deltaTime;
        if (_rollSpeed < _minRollSpeed)
        {
            _state = State.Normal;
        }
    }
    private void CooldownRoll()
    {
        _rollCooldownTimer -= Time.deltaTime;
        if (_rollCooldownTimer <= 0)
        {
            _rollCooldownTimer = 0;
        }
    }
    private void RollBuffer()
    {
        _rollThisFrameBufferTimer -= Time.deltaTime;
        if (_rollThisFrameBufferTimer <= 0)
        {
            _rollThisFrame = false;
        }
    }
    private void Dash()
    {
        Vector3 dashPos = transform.position + _lastMoveVector * _rollDistance;
        RaycastHit2D raycastHit2d = Physics2D.Raycast(transform.position, _lastMoveVector, _rollDistance, _rollCollisionMask);
        if (raycastHit2d.collider != null)
        {
            dashPos = raycastHit2d.point;
        }
        _rb.MovePosition(dashPos);
        _rollCooldownTimer = _rollCooldown;
        _rollThisFrame = false;
    }
    // other
    public void SetPosition(Vector3 position)
    {
        _rb.MovePosition(position);
    }
    public void FaceTarget(Vector3 position)
    {
        Vector3 difference = position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + _offset);
    }
}
