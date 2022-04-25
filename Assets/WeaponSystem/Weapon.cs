using UnityEngine;

public class Weapon
{
    public Weapon(WeaponData weapon, int ammo = 0)
    {
        this.weapon = weapon;
        this.ammoInMagazine = ammo;
    }
    private WeaponData weapon;
    public WeaponData WeaponData { get { return weapon; } }
    private int ammoInMagazine;
    public int AmmoInMagazine { get { return ammoInMagazine; } }
    public void AddAmmoInMagazine(int amount)
    {
        SetAmmoInMagazine(ammoInMagazine + amount);
    }
    public void RemoveAmmoInMagazine(int amount)
    {
        SetAmmoInMagazine(ammoInMagazine - amount);
    }
    public void SetAmmoInMagazine(int amount)
    {
        this.ammoInMagazine = amount;
    }
    private WeaponUpgrade _weaponUpgrade;
    public WeaponUpgrade WeaponUpgrade { get { return _weaponUpgrade; } }
    public bool IsUpgraded => _weaponUpgrade != null;
    public void SetWeaponUpgrade(WeaponUpgrade weaponUpgrade)
    {
        _weaponUpgrade = weaponUpgrade;
    }
    public void DropWeapon(Vector3 dropPosition)
    {
        PickUpWeapon weaponGO = GameObject.Instantiate(
            GameAssets.instance.templateWeaponGO,
            dropPosition,
            Quaternion.identity);
        weaponGO.SetWeapon(this);
    }
}
