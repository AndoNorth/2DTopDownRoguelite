using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Weapon Data", fileName = "New Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string _weaponName;
    public ProjectileData _bullet;
    [Header("Visuals")]
    public Sprite _sprite;
    public Color _color;
    [Header("Stats")]
    public int _magazineSize;
    public float _bulletSpeed;
    public float _reloadTime;
    public float _bulletsPerSec;
    public int _damage;
    public int _pierce;
    public bool _shootThroughWalls;
    public float _lifetime;
    public enum AmmoType
    {
        Light,
        Heavy,
        Energy,
        Explosive,
    }
    public AmmoType _ammoType;
    private int _ammoInMagazine;
    public int AmmoInMagazine { get { return _ammoInMagazine; } }
    public bool HasAmmo => AmmoInMagazine > 0;
    public void ConsumeAmmo(int ammoConsumed)
    {
        _ammoInMagazine -= ammoConsumed;
    }
    public int AddAmmoToMagazine(int ammoAmount)
    {
        int ammoChange = 0;
        if (ammoAmount <= 0)
        {
            return ammoChange;
        }
        if (ammoAmount + _ammoInMagazine > _magazineSize)
        {
            ammoChange = _magazineSize - _ammoInMagazine;
            _ammoInMagazine += ammoChange;
        }
        else
        {
            ammoChange = _ammoInMagazine + ammoAmount;
            _ammoInMagazine = ammoChange;
        }
        return ammoAmount - ammoChange;
    }
}
