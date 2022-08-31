using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
    {
        return ((layerMask & 1 << gameObject.layer) != 0);
    }
    public static bool IsNotInLayerMask(GameObject gameObject, LayerMask layerMask)
    {
        return ((layerMask & 1 << gameObject.layer) == 0);
    }
}
