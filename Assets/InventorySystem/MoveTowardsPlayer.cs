using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float _initSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;
    private float _speed;
    [SerializeField] private float _magnetRange;
    private bool _targetInMagnetRange = false;
    [Header("UpdateTargetPositionSettings")]
    [SerializeField] private float _delayStartMovingSec;
    [SerializeField] private float _repeatEveryNSec;
    private Vector3 _targetPos;
    private bool _targetSet = false;
    private void Start()
    {
        InvokeRepeating("UpdateTargetPosition", _delayStartMovingSec, _repeatEveryNSec);
        _speed = _initSpeed;
    }
    private void Update()
    {
        UpdateSpeed();
        if (_targetSet)
        {
            if (_targetInMagnetRange)
                transform.position = _targetPos;
            else
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);
        }
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    private void UpdateSpeed()
    {
        _speed += _acceleration * Time.deltaTime;
        if (_speed >= _maxSpeed)
            _speed = _maxSpeed;
    }
    private void UpdateTargetPosition()
    {
        if (!_targetSet)
            _targetSet = true;
        if (Vector3.Distance(transform.position, GameAssets.instance.playerCharacter.transform.position) <= _magnetRange)
            _targetInMagnetRange = true;
        _targetPos = GameAssets.instance.playerCharacter.transform.position;
    }
}
