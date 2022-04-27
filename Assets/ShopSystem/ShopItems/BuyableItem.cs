using UnityEngine;

[RequireComponent(typeof(SelectableObject))]
public class BuyableItem : MonoBehaviour, IInteractable
{
    private ShopKeeper _shopKeeper;
    private SpriteRenderer _spriteRenderer;
    private ShopItem _shopItem;
    public ShopItem ShopItem { get { return _shopItem; } }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Interact()
    {
        _shopKeeper.BuyItem(_shopItem);
    }
    public void SetShopItem(ShopKeeper shopKeeper, ShopItem shopItem)
    {
        _shopKeeper = shopKeeper;
        _shopItem = shopItem;
    }
    public void SetShopitemVisuals(Sprite sprite, Color color)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;
    }
}
