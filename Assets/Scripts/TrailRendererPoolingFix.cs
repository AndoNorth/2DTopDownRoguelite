using UnityEngine;
// script used to fix trailing trail renderer when using object pooling
public class TrailRendererPoolingFix : MonoBehaviour
{
    TrailRenderer _trail;
    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
    }
    private void OnDisable()
    {
        _trail.Clear();
    }
}
