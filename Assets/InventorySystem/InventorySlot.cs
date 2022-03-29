using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private Inventory _inventory;
    public int _invSlotNumber;
    void Start()
    {
        _inventory = GameAssets.instance.playerCharacter.GetComponent<Inventory>();
    }
    void Update()
    {
        // checks whether the inventory has an item (child)
        if(transform.childCount <= 0)
        {
            _inventory._isSlotTaken[_invSlotNumber] = false;
        }
    }
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<UI2GO>().DropItem();
            GameObject.Destroy(child.gameObject);
        }
    }
}
