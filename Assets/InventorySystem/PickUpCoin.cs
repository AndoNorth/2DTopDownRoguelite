using UnityEngine;

public class PickUpCoin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            GameManager.instance._noCoins++;
            GameObject.Destroy(gameObject);
        }
    }
}
