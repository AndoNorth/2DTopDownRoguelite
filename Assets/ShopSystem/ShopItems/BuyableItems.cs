using UnityEngine;
using System;

[CreateAssetMenu(menuName = "GameAssets/Buyable Items", fileName = "New List of Buyable")]
public class BuyableItems : ScriptableObject
{
    [SerializeField] WeaponData[] _weapons;
    [SerializeField] WeaponUpgrade[] _weaponUpgrades;
    [SerializeField] ItemCost[] _typeAndCost;
    public ShopItem GetRandomWeapon() => GetRandomItem(_weapons);
    public ShopItem GetRandomWeaponUpgrade() => GetRandomItem(_weaponUpgrades);
    private ShopItem GetRandomItem(Array array)
    {
        int randomIdx = UnityEngine.Random.Range(0, array.Length);
        var randomItem = array.GetValue(randomIdx);
        string type = randomItem.GetType().ToString();
        IBuyableItem buyableItem = (IBuyableItem) randomItem;
        ShopItem shopItem = new ShopItem { item = buyableItem.Name(), type = type };
        return shopItem;
    }
    public int GetItemCost(ShopItem shopItem)
    {
        foreach (ItemCost itemCost in _typeAndCost)
        {
            if(itemCost.type == shopItem.type)
            {
                return itemCost.cost;
            }
        }
        return 999;
    }
    public WeaponData GetWeaponData(ShopItem shopItem)
    {
        foreach (WeaponData weaponData in _weapons)
        {
            if(weaponData.Name() == shopItem.item)
            {
                return weaponData;
            }
        }
        return null;
    }
    public WeaponUpgrade GetWeaponUpgrade(ShopItem shopItem)
    {
        foreach (WeaponUpgrade weaponUpgrade in _weaponUpgrades)
        {
            if (weaponUpgrade.Name() == shopItem.item)
            {
                return weaponUpgrade;
            }
        }
        return null;
    }
}
[System.Serializable]
public class ItemCost
{
    public string type;
    public int cost;
    public bool TypeExists => (null != Type.GetType(type));
}
[System.Serializable]
public class ShopItem
{
    public string item;
    public string type;
}
public interface IBuyableItem
{
    string Name();
    Sprite Sprite();
    Color Color();
}
