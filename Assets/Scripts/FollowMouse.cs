using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public bool replaceCursorWithCrosshair;
    public GameObject crosshair;
    void Update()
    {
        if (replaceCursorWithCrosshair)
        {
            Cursor.visible = false;
            crosshair.SetActive(true);
            crosshair.transform.position = GeneralUtility.GetMouseWorldPosition();
        }
        else
        {
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
    }
}
