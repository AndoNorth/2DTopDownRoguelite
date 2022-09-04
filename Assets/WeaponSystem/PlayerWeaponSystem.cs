using UnityEngine;
using System;

public class PlayerWeaponSystem : MonoBehaviour
{
    public Action OnAmmoChanged = delegate { };
    public void TriggerOnAmmoChanged()
    {
        OnAmmoChanged();
    }
    public Action OnWeaponChanged = delegate { };
    public void TriggerOnWeaponChanged()
    {
        OnWeaponChanged();
    }
    public Weapon CurrentWeapon => _weaponInventory.CurrentWeapon;
    public Weapon GetWeapon(int weaponIdx) => _weaponInventory.GetWeapon(weaponIdx);
    public int WeaponIdx => _weaponInventory.WeaponIdx;
    public AmmoReserve AmmoReserve => _weaponInventory.AmmoReserve;
    [SerializeField] private WeaponData _defaultWeapon;
    [SerializeField] private Transform _firePoint;
    [SerializeField][Range(-180, 180)] private float _offset;
    private SpriteRenderer _currentWeaponSprite;
    // systems to manage
    private WeaponInventory _weaponInventory;
    public int ReserveAmmo { get { return _weaponInventory.CurrentReserveAmmo; } }
    private float _currentChangeWeaponCooldown;
    private float _changeWeaponCooldown = 0.2f;
    private bool CanChangeWeapon => _currentChangeWeaponCooldown <= 0f;
    private ShootingSystem _shootingSystem;
    private ReloadSystem _reloadSystem;
    public bool IsReloading { get { return _reloadSystem.IsReloading; } }
    private void Awake()
    {
        _currentWeaponSprite = _firePoint.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        OnWeaponChanged += RefreshWeaponVisuals;
        _weaponInventory = new WeaponInventory(_defaultWeapon);
        _weaponInventory.OnWeaponChanged += TriggerOnWeaponChanged;
        _shootingSystem = new ShootingSystem();
        _reloadSystem = new ReloadSystem(this, _weaponInventory);
        _reloadSystem.OnReloaded += TriggerOnAmmoChanged;
        _weaponInventory.OnWeaponChanged += InterruptReload;
        OnWeaponChanged();
    }
    private void OnEnable()
    {
        OnWeaponChanged += RefreshWeaponVisuals;
        if (_weaponInventory != null)
        {
            _weaponInventory.OnWeaponChanged += TriggerOnWeaponChanged;
            if (_reloadSystem != null)
            {
                _weaponInventory.OnWeaponChanged += InterruptReload;
            }
        }
        if (_reloadSystem != null)
        {
            _reloadSystem.OnReloaded += TriggerOnAmmoChanged;
        }
    }
    private void OnDisable()
    {
        OnWeaponChanged -= RefreshWeaponVisuals;
        if (_weaponInventory != null)
        {
            _weaponInventory.OnWeaponChanged -= TriggerOnWeaponChanged;
            if (_reloadSystem != null)
            {
                _weaponInventory.OnWeaponChanged -= InterruptReload;
            }
        }
        if (_reloadSystem != null)
        {
            _reloadSystem.OnReloaded -= TriggerOnAmmoChanged;
        }
    }
    private void Update()
    {
        TickChangeWeaponCooldown();
        _shootingSystem.TickFireCooldown();
    }
    private void RefreshWeaponVisuals()
    {
        _currentWeaponSprite.sprite = CurrentWeapon.WeaponData.Sprite();
        _currentWeaponSprite.color = CurrentWeapon.WeaponData.Color();
    }
    public void ToggleWeaponSlot()
    {
        if (CanChangeWeapon)
        {
            InterruptReload();
            _weaponInventory.ChangeToLastWeapon();
            OnAmmoChanged();
            SetChangeWeaponCooldown();
        }
    }
    public void ChangeWeapon(int weaponIdx)
    {
        if (CanChangeWeapon)
        {
            InterruptReload();
            _weaponInventory.ChangeWeapon(weaponIdx);
            OnAmmoChanged();
            SetChangeWeaponCooldown();
        }
    }
    private void TickChangeWeaponCooldown()
    {
        _currentChangeWeaponCooldown -= Time.deltaTime;
        if (_currentChangeWeaponCooldown <= 0f)
        {
            _currentChangeWeaponCooldown = 0f;
        }
    }
    private void SetChangeWeaponCooldown() => _currentChangeWeaponCooldown = _changeWeaponCooldown;
    public void Reload()
    {
        if (!_weaponInventory.HasReserveAmmo || _reloadSystem.MagazineIsFull || IsReloading)
        {
            return;
        }
        _reloadSystem.Reload(_weaponInventory.CurrentWeapon);
    }
    private void InterruptReload() => _reloadSystem.InterruptReload();
    public void Fire()
    {
        if (_weaponInventory.HasAmmo)
        {
            if (_shootingSystem.ReadyToFire)
            {
                Weapon weapon = _weaponInventory.CurrentWeapon;
                _reloadSystem.InterruptReload();
                _shootingSystem.Fire(weapon, _firePoint.position, this.transform.rotation, _offset);
                weapon.RemoveAmmoInMagazine(1);
                OnAmmoChanged();
            }
        }
        else
            Reload();
    }
    public void DropCurrentWeapon()
    {
        if (CanChangeWeapon)
        {
            InterruptReload();
            _weaponInventory.DropCurrentWeapon();
            OnWeaponChanged();
            OnAmmoChanged();
            SetChangeWeaponCooldown();
        }
    }
    public void PickUpWeapon(PickUpWeapon weapon)
    {
        if (CanChangeWeapon)
        {
            InterruptReload();
            _weaponInventory.PickUpWeapon(weapon.Weapon);
            OnWeaponChanged();
            OnAmmoChanged();
            SetChangeWeaponCooldown();
        }
    }
    public void PickUpWeaponUpgrade(PickUpWeaponUpgrade weaponUpgrade)
    {
        if (CanChangeWeapon)
        {
            _weaponInventory.PickUpWeaponUpgrade(weaponUpgrade.WeaponUpgrade);
            OnWeaponChanged();
            SetChangeWeaponCooldown();
        }
    }
    public void AddToAmmoReserve(AmmoType ammoType, int ammoAmount)
    {
        _weaponInventory.AddReserveAmmo(ammoType, ammoAmount);
        OnAmmoChanged();
    }
}
