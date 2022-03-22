using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    private void FixedUpdate()
    {
        transform.position = _transform.position;
    }
}
