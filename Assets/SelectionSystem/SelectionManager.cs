using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    [SerializeField] private Collider2D _selector;
    private List<SelectableObject> _selectableObjects;
    [SerializeField] private SelectableObject _currentlySelectedObject;
    public SelectableObject CurrentlySelectedItem { get { return _currentlySelectedObject; } }
    private void Start()
    {
        _selectableObjects = new List<SelectableObject>(FindObjectsOfType<SelectableObject>());
    }
    private void Update()
    {
        if(_currentlySelectedObject != null)
        {
            _currentlySelectedObject.SetIsSelected(false);
            _currentlySelectedObject = null;
        }

        RaycastHit2D hit = Physics2D.Raycast(GeneralUtility.GetMouseWorldPosition(),-Vector2.up);
        if(hit.collider != null)
        {
            Transform selection = hit.transform;
            SelectableObject selectableObj = selection.GetComponent<SelectableObject>();
            if (selectableObj != null)
            {
                _currentlySelectedObject = selectableObj;
                _currentlySelectedObject.SetIsSelected(true);
            }
        }
    }

    public void AddToObjectsList(SelectableObject selectableObject)
    {
        _selectableObjects.Add(selectableObject);
    }
    public void RemoveFromObjectsList(SelectableObject selectableObject)
    {
        _selectableObjects.Remove(selectableObject);
    }
}
