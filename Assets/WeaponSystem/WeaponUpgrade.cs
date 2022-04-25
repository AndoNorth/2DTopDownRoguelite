using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Weapon Upgrade", fileName = "New Weapon Upgrade")]
public class WeaponUpgrade : ScriptableObject, IBuyableItem
{
    public string _upgradeName;
    public string Name() => _upgradeName;
    public UpgradeType _upgradeType;
    public UpgradableWeaponStats _upgradeStat;
    public float _upgradeAmount;
    [Header("Visuals")]
    public Sprite _sprite;
    public Sprite Sprite() => _sprite;
    public Color _color;
    public Color Color() => _color;
    public WeaponStats UpgradedWeaponStats(WeaponStats weaponStats)
    {
        switch (_upgradeStat)
        {
            case UpgradableWeaponStats.MagazineSize:
                weaponStats.MagazineSize = (int) UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.BulletSpeed:
                weaponStats.BulletSpeed = UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.ReloadTime:
                weaponStats.ReloadTime = UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.BulletsPerSec:
                weaponStats.BulletsPerSec = UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.Damage:
                weaponStats.Damage = (int) UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.Pierce:
                weaponStats.Pierce = (int) UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            case UpgradableWeaponStats.ShootThroughWalls:
                weaponStats.ShootThroughWalls = 0f != UpgradeStat(weaponStats.MagazineSize, _upgradeAmount, _upgradeType);
                break;
            default:
                break;
        }
        return weaponStats;
    }
    public float UpgradeStat(float stat, float amount, UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Additive:
                stat += amount;
                break;
            case UpgradeType.Multiplicative:
                stat *= amount;
                break;
            default:
                break;
        }
        return stat;
    }
    public void DropWeaponUpgrade(Vector3 dropPosition)
    {
        PickUpWeaponUpgrade weaponUpgradeGO = GameObject.Instantiate(
            GameAssets.instance.templateWeaponUpgradeGO,
            dropPosition,
            Quaternion.identity);
        weaponUpgradeGO.SetWeaponUpgrade(this);
    }
}
public enum UpgradeType
{
    Unique,
    Additive,
    Multiplicative,
}
public enum UpgradableWeaponStats
{
    Unique,
    MagazineSize,
    BulletSpeed,
    ReloadTime,
    BulletsPerSec,
    Damage,
    Pierce,
    ShootThroughWalls,
}
public class WeaponStats
{
    public int MagazineSize;
    public float BulletSpeed;
    public float ReloadTime;
    public float BulletsPerSec;
    public int Damage;
    public int Pierce;
    public bool ShootThroughWalls;
}
