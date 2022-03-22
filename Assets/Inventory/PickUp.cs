using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject itemButton;

    private Inventory _inventory;
    void Start()
    {
        _inventory = GameAssets.instance.playerCharacter.GetComponent<Inventory>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            for (int i = 0; i < _inventory._inventorySlots.Length; i++)
            {
                if(_inventory._isSlotTaken[i] == false)
                {
                    _inventory._isSlotTaken[i] = true;
                    Instantiate(itemButton, _inventory._inventorySlots[i].transform);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
