using UnityEngine;
using System;
using System.Collections;

public class ReloadSystem
{
    public Action OnReloaded = delegate { };

    private MonoBehaviour _mono;
    private WeaponInventory _weaponInventory;
    private Coroutine _reloadRoutine;
    private bool _isReloading;
    public bool IsReloading { get { return _isReloading; } }
    public bool HasAmmoLeft { get { return _weaponInventory.CurrentReserveAmmo > 0; } }
    public bool MagazineIsFull { get { return _weaponInventory.CurrentWeapon.AmmoInMagazine > _weaponInventory.CurrentWeapon.WeaponData._magazineSize; } }
    public ReloadSystem(MonoBehaviour mono, WeaponInventory weaponInventory)
    {
        this._mono = mono;
        this._weaponInventory = weaponInventory;
    }
    public void Reload(Weapon weapon)
    {
        _reloadRoutine = _mono.StartCoroutine(ReloadRoutine(weapon));
    }
    IEnumerator ReloadRoutine(Weapon weapon)
    {
        _isReloading = true;
        yield return new WaitForSeconds(weapon.WeaponData._reloadTime);
        LoadAmmo(weapon);
        _isReloading = false;
    }
    public void InterruptReload()
    {
        if (_reloadRoutine != null)
        {
            _mono.StopCoroutine(_reloadRoutine);
            _isReloading = false;
        }
    }
    private void LoadAmmo(Weapon weapon)
    {
        WeaponData weaponData = weapon.WeaponData;
        int ammoInMagazine = weapon.AmmoInMagazine;
        int reserveAmmo = _weaponInventory.CurrentReserveAmmo;
        // calculate change in ammo
        int ammoChange = 0;
        if (reserveAmmo + ammoInMagazine > weaponData._magazineSize)
            ammoChange = weaponData._magazineSize - ammoInMagazine;
        else
            ammoChange = ammoInMagazine + reserveAmmo;
        // resolve change in ammo
        _weaponInventory.RemoveReserveAmmo(_weaponInventory.CurrentAmmoType, ammoChange);
        _weaponInventory.CurrentWeapon.AddAmmoInMagazine(ammoChange);

        OnReloaded();
    }
}
