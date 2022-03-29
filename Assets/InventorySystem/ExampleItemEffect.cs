using UnityEngine;

public class ExampleItemEffect : MonoBehaviour
{
    public GameObject _itemUsedFX;
    public void ItemEffect()
    {
        Instantiate(_itemUsedFX, GameAssets.instance.playerCharacter.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
