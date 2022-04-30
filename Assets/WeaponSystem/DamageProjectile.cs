using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageProjectile : MonoBehaviour
{
    private Action<DamageProjectile> _killAction;

    private Rigidbody2D _rb;
    private PolygonCollider2D _collider;
    private MeshRenderer _meshRenderer;
    private ExtrudeSprite _extrudeSprite;
    [SerializeField] private Material _material;

    private int _damage = 1;
    private LayerMask _damageLayerMask;
    private float _lifetime = 0f;
    private int _pierce = 0;
    private bool _shootThroughWall;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _extrudeSprite = GetComponent<ExtrudeSprite>();
        _meshRenderer.material = new Material(_material);
    }
    private void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0)
        {
            _killAction(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGO = collision.gameObject;
        if (collisionGO.IsInLayerMask(_damageLayerMask))
        {
            IDamagable damagable = collisionGO.GetComponent<IDamagable>();
            damagable?.TakeDamage(_damage);
            if (_pierce <= 0 || IfCantShootThroughWalls())
            {
                _killAction(this);
                return;
            }
            _pierce--;
        }
        else if (IfCantShootThroughWalls())
            _killAction(this);

        bool IfCantShootThroughWalls() => _shootThroughWall ? false : collisionGO.IsInLayerMask(LayerMask.GetMask("Walls"));
    }
    public void SetupProjectile(Vector3 pos, Quaternion rot, float offset, float speed, PolygonCollider2D collider, Color color, int damage, int pierce, bool shootThroughWall, float lifetime, LayerMask damageLayerMask)
    {
        this.transform.SetPositionAndRotation(pos, rot);
        float rotz = Mathf.Deg2Rad * (rot.eulerAngles.z + offset);
        Vector2 directionVector = new Vector2(MathF.Cos(rotz), MathF.Sin(rotz));
        _rb.velocity = directionVector * speed;
        _collider.points = collider.points;
        _extrudeSprite.ExtrudeSpriteFromCollider(color);
        _damage = damage;
        _pierce = pierce;
        _shootThroughWall = shootThroughWall;
        _lifetime = lifetime;
        _damageLayerMask = damageLayerMask;
    }
    public void InitKillAction(Action<DamageProjectile> killAction)
    {
        _killAction = killAction;
    }
}
