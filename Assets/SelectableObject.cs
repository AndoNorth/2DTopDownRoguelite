using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] Color _selectedOutlineColor;
    [SerializeField] float _outlineScale;
    SpriteRenderer _outlineSpriteRenderer;
    GameObject _outlineGO;
    bool _isSelected;
    public bool IsSelected => _isSelected;
    private void Start()
    {
        GameObject outlineGO = new GameObject();
        outlineGO.transform.SetParent(transform);
        outlineGO.transform.localScale = transform.localScale*_outlineScale;
        outlineGO.name = "SelectableOutline";
        outlineGO.transform.localPosition = Vector3.zero;
        _outlineGO = outlineGO;
        _outlineSpriteRenderer = outlineGO.AddComponent<SpriteRenderer>();
        SpriteRenderer selectableObjectSpriteRenderer = GetComponent<SpriteRenderer>();
        _outlineSpriteRenderer.sprite = selectableObjectSpriteRenderer.sprite;
        _outlineSpriteRenderer.sortingLayerID = selectableObjectSpriteRenderer.sortingLayerID;
        _outlineSpriteRenderer.color = _selectedOutlineColor;
        _outlineSpriteRenderer.sortingOrder = 1;
    }
    private void OnEnable()
    {
        FindObjectOfType<SelectionManager>()._selectableObjects.Add(this);
    }
    private void OnDisable()
    {
        FindObjectOfType<SelectionManager>()._selectableObjects.Remove(this);
    }
    private void Update()
    {
        if (_isSelected)
        {
            _outlineGO.SetActive(true);
        }
        else
        {
            _outlineGO.SetActive(false);
        }
    }
    public void SetIsSelected(bool boolean)
    {
        _isSelected = boolean;
    }
}
