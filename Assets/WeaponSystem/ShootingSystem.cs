using UnityEngine;
public class ShootingSystem
{
    private float _fireCooldown;
    public bool ReadyToFire => _fireCooldown <= 0f;
    public void TickFireCooldown()
    {
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            _fireCooldown = 0f;
        }
    }
    public void ResetFireCooldown()
    {
        _fireCooldown = 0.1f;
    }
    public void Fire(Weapon weapon, Vector3 firePoint, Quaternion rotation, float offset)
    {
        WeaponData weaponData = weapon.WeaponData;
        DamageProjectile bullet = PlayerBulletPool.instance.Spawn();
        WeaponStats weaponStats = weaponData.GetWeaponStats();
        if (weapon.IsUpgraded)
             weaponStats = weapon.WeaponUpgrade.UpgradedWeaponStats(weaponStats);

        bullet.SetupProjectile(firePoint, rotation, offset, weaponStats.BulletSpeed,
            weaponData._bullet._projCollider, weaponData._bullet._projColor,
            weaponStats.Damage, weaponStats.Pierce, weaponStats.ShootThroughWalls,
            weaponData._lifetime, LayerMask.GetMask(GameAssets.instance.playerShootableLayers));
        _fireCooldown = 1.0f / weaponData._bulletsPerSec;
    }
}
