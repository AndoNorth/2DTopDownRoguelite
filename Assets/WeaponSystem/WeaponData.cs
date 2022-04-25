using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Weapon Data", fileName = "New Weapon Data")]
public class WeaponData : ScriptableObject, IBuyableItem
{
    public string _weaponName;
    public string Name() => _weaponName;
    public ProjectileData _bullet;
    [Header("Visuals")]
    public Sprite _sprite;
    public Sprite Sprite() => _sprite;
    public Color _color;
    public Color Color() => _color;
    [Header("Stats")]
    public int _magazineSize;
    public float _bulletSpeed;
    public float _reloadTime;
    public float _bulletsPerSec;
    public int _damage;
    public int _pierce;
    public bool _shootThroughWalls;
    public float _lifetime;
    public AmmoType _ammoType;
    public WeaponStats GetWeaponStats()
    {
        return new WeaponStats()
        {
            MagazineSize = _magazineSize,
            BulletSpeed = _bulletSpeed,
            BulletsPerSec = _bulletsPerSec,
            ReloadTime = _reloadTime,
            Damage = _damage,
            Pierce = _pierce,
            ShootThroughWalls = _shootThroughWalls,
        };
    }
}
