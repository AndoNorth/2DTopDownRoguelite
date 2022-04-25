using UnityEngine;
using System;

[RequireComponent(typeof(SelectableObject))]
public class ShopKeeper : MonoBehaviour, IInteractable
{
    [SerializeField] private BuyableItems _buyableItems;
    [SerializeField] private GameObject _shopItems;
    [SerializeField] private SelectableObject _itemSlot1;
    [SerializeField] private SelectableObject _itemSlot2;
    [SerializeField] private SelectableObject _itemSlot3;
    private BuyableItem _item1;
    private BuyableItem _item2;
    private BuyableItem _item3;
    private bool _shopIsOpen = false;
    private bool _canSell = false;
    private void Start()
    {
        _item1 = _itemSlot1.GetComponent<BuyableItem>();
        _item2 = _itemSlot2.GetComponent<BuyableItem>();
        _item3 = _itemSlot3.GetComponent<BuyableItem>();
        SetupShop();
        _shopItems.SetActive(false);
    }
    private void ToggleShop()
    {
        bool isShopActive = _shopItems.activeInHierarchy;
        _shopItems.SetActive(!isShopActive);
        _shopIsOpen = !isShopActive;
    }
    public void Interact()
    {
        if (_canSell)
            ToggleShop();
    }
    // generate inventory of items
    private void SetupShop()
    {
        _canSell = true;
        // get 3 random items
        ShopItem item1 = _buyableItems.GetRandomWeaponUpgrade();
        ShopItem item2 = _buyableItems.GetRandomWeapon();
        ShopItem item3 = _buyableItems.GetRandomWeaponUpgrade();
        IBuyableItem buyableItem1 = (IBuyableItem)_buyableItems.GetWeaponUpgrade(item1);
        IBuyableItem buyableItem2 = (IBuyableItem)_buyableItems.GetWeaponData(item2);
        IBuyableItem buyableItem3 = (IBuyableItem)_buyableItems.GetWeaponUpgrade(item3);
        _item1.SetupShopitem(this, item1, buyableItem1.Sprite(), buyableItem1.Color());
        _item2.SetupShopitem(this, item2, buyableItem2.Sprite(), buyableItem2.Color());
        _item3.SetupShopitem(this, item3, buyableItem3.Sprite(), buyableItem3.Color());
        _itemSlot1.UpdateOutline();
        _itemSlot2.UpdateOutline();
        _itemSlot3.UpdateOutline();
        _itemSlot1.SetIsInteractableText(_buyableItems.GetItemCost(item1).ToString());
        _itemSlot2.SetIsInteractableText(_buyableItems.GetItemCost(item2).ToString());
        _itemSlot3.SetIsInteractableText(_buyableItems.GetItemCost(item3).ToString());
    }
    public void BuyItem(ShopItem shopItem)
    {
        int cost = _buyableItems.GetItemCost(shopItem);
        if (GameManager.instance._noCoins < cost)
        {
            return;
        }
        GameManager.instance._noCoins -= cost;
        DropItem(shopItem);
        _canSell = false;
        this.gameObject.SetActive(false);
    }
    private void DropItem(ShopItem shopItem)
    {
        // float distanceFromShopkeeper = 1.0f;
        // Vector3 dropPosition = Vector3.MoveTowards(this.transform.position, GameAssets.instance.playerCharacter.transform.position, distanceFromShopkeeper);
        Vector3 dropPosition = this.transform.position;
        if (Type.GetType(shopItem.type) == typeof(WeaponData))
        {
            WeaponData weaponData = _buyableItems.GetWeaponData(shopItem);
            Weapon weapon = new Weapon(weaponData, weaponData._magazineSize);
            weapon.DropWeapon(dropPosition);
        }
        if (Type.GetType(shopItem.type) == typeof(WeaponUpgrade))
        {
            _buyableItems.GetWeaponUpgrade(shopItem).DropWeaponUpgrade(dropPosition);
        }
    }
}
