using UnityEngine;
using System;
using System.Collections;

public class PlayerWeaponSystem : MonoBehaviour
{
    struct PlayerAmmo
    {
        public int lightReserveAmmo;
        public int heavyReserveAmmo;
        public int energyReserveAmmo;
        public int explosiveReserveAmmo;
    }
    public Action OnWeaponChanged = delegate { };
    public Action OnAmmoChanged = delegate { };
    // Weapon
    WeaponData _currentWeapon;
    public WeaponData CurrentWeapon => _currentWeapon;
    [SerializeField] WeaponData[] _weapons = new WeaponData[2];
    [SerializeField] Transform _firePoint;
    [SerializeField] [Range(-180,180)] float _offset;
    SpriteRenderer _currentWeaponSprite;
    private bool _currentSlot = false;
    // Shooting
    private float _fireCooldown;
    private bool ReadyToFire => _fireCooldown <= 0f;
    // Reload
    private int _reserveAmmo = 100;
    private Coroutine _reloadRoutine;
    private bool _isReloading;
    public bool IsReloading { get { return _isReloading; } }
    public int ReserveAmmo { get { return _reserveAmmo; } }
    private void Awake()
    {
        _currentWeaponSprite = _firePoint.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        ResetAllWeaponAmmo();
        SelectWeapon(_currentSlot ? 1 : 0);

        void ResetAllWeaponAmmo()
        {
            foreach (WeaponData weapon in _weapons)
            {
                ResetWeaponAmmo(weapon);
            }
        }
    }
    private void Update()
    {
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            _fireCooldown = 0;
        }
    }
    private void ResetWeaponAmmo(WeaponData weapon)
    {
        int resetAmmo = weapon._magazineSize - weapon.AmmoInMagazine;
        weapon.ConsumeAmmo(-resetAmmo);
    }
    private void SelectWeapon(int slot)
    {
        InterruptReload();
        _currentWeapon = _weapons[slot];
        RefreshWeaponVisuals();
        OnWeaponChanged();
        OnAmmoChanged();
    }
    public void ToggleWeapon()
    {
        _currentSlot = !_currentSlot;
        SelectWeapon(_currentSlot ? 1 : 0);
    }
    public void Fire()
    {
        if (_currentWeapon.HasAmmo)
        {
            InterruptReload();
            if (ReadyToFire)
            {
                DamageProjectile bullet = PlayerBulletPool.instance.Spawn();
                bullet.SetupProjectile(_firePoint.position, transform.rotation, _offset, _currentWeapon._bulletSpeed,
                    _currentWeapon._bullet._projCollider, _currentWeapon._bullet._projColor,
                    _currentWeapon._damage, _currentWeapon._pierce, _currentWeapon._shootThroughWalls,
                    _currentWeapon._lifetime, LayerMask.GetMask(GameAssets.instance.playerShootableLayers));
                _fireCooldown = 1.0f / _currentWeapon._bulletsPerSec;
                _currentWeapon.ConsumeAmmo(1);
                OnAmmoChanged();
            }
        }
        else
        {
            Reload();
        }
    }
    private void InterruptReload()
    {
        if (_reloadRoutine != null)
        {
            StopCoroutine(_reloadRoutine);
            _isReloading = false;
        }
    }
    public void Reload()
    {
        if (!IsReloading)
        {
            _reloadRoutine = StartCoroutine(ReloadRoutine(_currentWeapon._reloadTime));
        }
    }
    IEnumerator ReloadRoutine(float reloadTime)
    {
        _isReloading = true;
        if(_reserveAmmo <= 0)
        {
            _isReloading = false;
            yield return null;
        }
        yield return new WaitForSeconds(reloadTime);
        _reserveAmmo = _currentWeapon.AddAmmoToMagazine(_reserveAmmo);
        OnAmmoChanged();
        _isReloading = false;
    }
    private void RefreshWeaponVisuals()
    {
        _currentWeaponSprite.sprite = _currentWeapon._sprite;
        _currentWeaponSprite.color = _currentWeapon._color;
    }
}
