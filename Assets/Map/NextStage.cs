using UnityEngine;

[RequireComponent(typeof(SelectableObject))]
public class NextStage : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GameManager.instance.ResetStage();
        GameObject.Destroy(gameObject);
    }
}
