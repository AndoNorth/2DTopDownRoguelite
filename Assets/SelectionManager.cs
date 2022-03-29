using System.Collections.Generic;
using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    public List<SelectableObject> _selectableObjects;
    SelectableObject _currentlySelectedObject;
    private void Start()
    {
        _selectableObjects = new List<SelectableObject>(FindObjectsOfType<SelectableObject>());
    }
    private void Update()
    {
        foreach (SelectableObject selectableObject in _selectableObjects)
        {
            selectableObject.SetIsSelected(false);
        }
        RaycastHit2D hit = Physics2D.CircleCast(GeneralUtility.GetMouseWorldPosition(), 0.1f, Vector2.zero);
        if(hit.collider != null)
        {
            _currentlySelectedObject = hit.collider.GetComponent<SelectableObject>();
            _currentlySelectedObject?.SetIsSelected(true);
        }
    }
}
