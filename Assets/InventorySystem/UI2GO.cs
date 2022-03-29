using UnityEngine;

public class UI2GO : MonoBehaviour
{
    public GameObject _item;
    [SerializeField] private Vector3 _dropVector = new Vector3(0f, 1f, 0f);

    public void DropItem()
    {
        Instantiate(_item, GameAssets.instance.playerCharacter.transform.position + _dropVector, Quaternion.identity);
    }
}
