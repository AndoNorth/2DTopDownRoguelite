using System;
using UnityEngine;

public class WeaponInventory
{
    public Action OnWeaponChanged = delegate { };
    private MonoBehaviour _mono;
    private AmmoReserve _ammoReserve;
    public AmmoReserve AmmoReserve { get { return _ammoReserve; } }
    private Weapon[] _weapons = new Weapon[3];
    private int _weaponIdx, _lastWeaponIdx;
    public int WeaponIdx { get { return _weaponIdx; } }
    public WeaponInventory(WeaponData templateWeapon)
    {
        _ammoReserve = new AmmoReserve(60, 40, 120, 20);
        _weapons[0] = null;
        _weapons[1] = null;
        _weapons[2] = new Weapon(templateWeapon, templateWeapon._magazineSize);
        _weaponIdx = 2;
        _lastWeaponIdx = 2;
    }
    // ammo
    public AmmoType CurrentAmmoType { get { return CurrentWeapon.WeaponData._ammoType; } }
    public bool HasAmmo => CurrentWeapon.AmmoInMagazine > 0;
    public int CurrentReserveAmmo { get { return _ammoReserve.ReserveAmmo(CurrentAmmoType); } }
    public int GetReserveAmmo(AmmoType ammoType) => _ammoReserve.ReserveAmmo(ammoType);
    public bool HasReserveAmmo => CurrentReserveAmmo > 0;
    public void AddReserveAmmo(AmmoType ammoType, int amount) => _ammoReserve.AddReserveAmmo(ammoType, amount);
    public void RemoveReserveAmmo(AmmoType ammoType, int amount) => _ammoReserve.AddReserveAmmo(ammoType, -amount);
    private void UnloadWeapon(Weapon weapon)
    {
        AddReserveAmmo(weapon.WeaponData._ammoType, weapon.AmmoInMagazine);
        weapon.RemoveAmmoInMagazine(weapon.AmmoInMagazine);
    }
    // weapon
    public Weapon GetWeapon(int weaponIdx) => _weapons[weaponIdx];
    public Weapon CurrentWeapon { get { return _weapons[_weaponIdx]; } }
    private Weapon LastWeapon { get { return _weapons[_lastWeaponIdx]; } }
    private bool UsingTemplateWeapon { get { return _weaponIdx == 2; } }
    public void ChangeWeapon(int idx)
    {
        if (_weaponIdx == idx)
            return;
        if (_weapons[idx] == null)
            return;
        _lastWeaponIdx = _weaponIdx;
        _weaponIdx = idx;
        OnWeaponChanged();
    }
    public void ChangeToLastWeapon() => ChangeWeapon(_lastWeaponIdx);
    public void DropCurrentWeapon()
    {
        if(UsingTemplateWeapon)
            return;
        UnloadWeapon(CurrentWeapon);
        CurrentWeapon.DropWeapon(GameAssets.instance.playerCharacter.transform.position);
        _weapons[_weaponIdx] = null;
    }
    public void PickUpWeapon(Weapon newWeapon)
    {
        // check for empty slots
        if(_weapons[0] == null)
        {
            _weapons[0] = newWeapon;
            return;
        }
        if (_weapons[1] == null)
        {
            _weapons[1] = newWeapon;
            return;
        }
        if(UsingTemplateWeapon)
        {
            UnloadWeapon(LastWeapon);
            Weapon lastWeapon = _weapons[_lastWeaponIdx];
            lastWeapon.DropWeapon(GameAssets.instance.playerCharacter.transform.position);
            _weapons[_lastWeaponIdx] = newWeapon;
            return;
        }
        UnloadWeapon(CurrentWeapon);
        Weapon oldWeapon = CurrentWeapon;
        oldWeapon.DropWeapon(GameAssets.instance.playerCharacter.transform.position);
        _weapons[_weaponIdx] = newWeapon;
    }
    // weapon upgrades
    public void PickUpWeaponUpgrade(WeaponUpgrade weaponUpgrade)
    {
        if (CurrentWeapon.IsUpgraded)
        {
            CurrentWeapon.WeaponUpgrade.DropWeaponUpgrade(GameAssets.instance.playerCharacter.transform.position);
        }
        CurrentWeapon.SetWeaponUpgrade(weaponUpgrade);
    }
}