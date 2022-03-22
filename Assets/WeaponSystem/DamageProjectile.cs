using System;
using UnityEngine;

public class DamageProjectile : MonoBehaviour
{
    private Action<DamageProjectile> _killAction;

    private Rigidbody2D _rb;
    private PolygonCollider2D _collider;
    private MeshRenderer _meshRenderer;
    private ExtrudeSprite _extrudeSprite;

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
            if (_pierce <= 0 || (_shootThroughWall ? false : collisionGO.layer == LayerMask.GetMask("Walls")))
            {
                _killAction(this);
                return;
            }
            _pierce--;
        }
    }
    public void SetupProjectile(Vector3 pos, Quaternion rot, float offset, float speed, PolygonCollider2D collider, Color color, int damage, int pierce, bool shootThroughWall, float lifetime, LayerMask damageLayerMask)
    {
        this.transform.SetPositionAndRotation(pos, rot);
        float rotz = Mathf.Deg2Rad * (rot.eulerAngles.z + offset);
        Vector2 directionVector = new Vector2(MathF.Cos(rotz), MathF.Sin(rotz));
        _rb.velocity = directionVector * speed;
        _collider.points = collider.points;
        _extrudeSprite.ExtrudeSpriteFromCollider();
        _meshRenderer.material.color = color;
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
