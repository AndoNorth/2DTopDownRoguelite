using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float _initSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;
    private float _speed;
    [Header("UpdateTargetPositionSettings")]
    [SerializeField] private float _delayStartMovingSec;
    [SerializeField] private float _repeatEveryNSec;
    private Vector3 _targetPos;
    private bool _targetSet;
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
        _targetPos = GameAssets.instance.playerCharacter.transform.position;
    }
}
