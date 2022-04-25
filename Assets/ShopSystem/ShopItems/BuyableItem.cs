using UnityEngine;

[RequireComponent(typeof(SelectableObject))]
public class BuyableItem : MonoBehaviour, IInteractable
{
    private ShopKeeper _shopKeeper;
    private SpriteRenderer _spriteRenderer;
    private ShopItem _shopItem;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();        
    }
    public void Interact()
    {
        _shopKeeper.BuyItem(_shopItem);
    }
    public void SetupShopitem(ShopKeeper shopKeeper, ShopItem shopItem, Sprite sprite, Color color)
    {
        _shopKeeper = shopKeeper;
        _shopItem = shopItem;
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;
    }
}
