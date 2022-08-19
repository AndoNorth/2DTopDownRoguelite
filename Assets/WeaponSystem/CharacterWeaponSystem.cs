using UnityEngine;

public class CharacterWeaponSystem : MonoBehaviour
{
    [SerializeField]
    private WeaponData _currentWeapon;
    [SerializeField]
    private WeaponData[] _weapons = new WeaponData[2];
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private SpriteRenderer _currentWeaponSprite;
    [SerializeField]
    [Range(-180, 180)] private float _offset;
    private bool _currentSlot = false;
    private float _fireCooldown;
    public bool ReadyToFire => _fireCooldown <= 0f;
    private void Awake()
    {
        _currentWeaponSprite = _firePoint.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        SelectWeapon(_currentSlot ? 1 : 0);
    }
    private void Update()
    {
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            _fireCooldown = 0;
        }
    }
    private void SelectWeapon(int slot)
    {
        _currentWeapon = _weapons[slot];
        RefreshWeaponVisuals();
    }
    public void ToggleWeapon()
    {
        _currentSlot = !_currentSlot;
        SelectWeapon(_currentSlot ? 1 : 0);
    }
    public void Fire()
    {
        if (ReadyToFire)
        {
            DamageProjectile bullet = CharacterBulletPool.instance.Spawn();
            bullet.SetupProjectile(_firePoint.position, transform.rotation, _offset, _currentWeapon._bulletSpeed,
                _currentWeapon._bullet._projCollider, _currentWeapon._bullet._projColor,
                _currentWeapon._damage, _currentWeapon._pierce, _currentWeapon._shootThroughWalls,
                _currentWeapon._lifetime, LayerMask.GetMask(GameAssets.instance.enemyShootableLayers));
            _fireCooldown = 1.0f/_currentWeapon._bulletsPerSec;
        }
    }

    private void RefreshWeaponVisuals()
    {
        _currentWeaponSprite.sprite = _currentWeapon._sprite;
        _currentWeaponSprite.color = _currentWeapon._color;
    }
}
