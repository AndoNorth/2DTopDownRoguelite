using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class SelectableObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro _pfText;
    [SerializeField] private float _outlineScale = 1.1f;
    [Header("Selectable")]
    [SerializeField] private Color _selectedOutlineColor = Color.HSVToRGB(182, 100, 100);
    [SerializeField] private bool _showSelectedText;
    [SerializeField] private string _selectedMessage = "Move Closer To Interact";
    [Header("Interactable")]
    [SerializeField] private Color _interactableOutlineColor = Color.HSVToRGB(55, 100, 100);
    [SerializeField][Range(0.1f, 5f)] private float _interactableDistance;
    [SerializeField] private bool _showInteractableText;
    [SerializeField] private string _interactableMessage = "Press 'E' to Interact";
    SpriteRenderer _outlineSpriteRenderer;
    GameObject _outlineGO;
    private TextMeshPro _text;
    private bool _isSelected = false;
    public bool IsSelected => _isSelected;
    public bool IsInteractable => _interactableDistance > Vector3.Distance(this.transform.position, GameAssets.instance.playerCharacter.transform.position);
    private bool _isSetup = false;
    public bool IsSetup => _isSetup;
    private void OnEnable()
    {
        if (_isSetup)
        {
            SelectionManager.instance.AddToObjectsList(this);
        }
    }
    private void OnDisable()
    {
        SelectionManager.instance.RemoveFromObjectsList(this);
    }
    private void Start()
    {
        SelectionManager.instance.AddToObjectsList(this);

        SetupOutline();
        if (_showInteractableText || _showSelectedText)
        {
            _text = Instantiate(_pfText, transform);
            _text.text = "";
        }
        SetIsSelected(false);
        _isSetup = true;
    }
    private void SetupOutline()
    {
        GameObject outlineGO = new GameObject();
        outlineGO.transform.SetParent(transform);
        outlineGO.transform.localScale = transform.localScale * _outlineScale;
        outlineGO.name = "Outline";
        outlineGO.transform.localPosition = Vector3.zero;
        _outlineGO = outlineGO;
        _outlineSpriteRenderer = _outlineGO.AddComponent<SpriteRenderer>();
        UpdateOutline();
    }
    public void UpdateOutline()
    {
        SpriteRenderer selectableObjectSpriteRenderer = GetComponent<SpriteRenderer>();
        _outlineSpriteRenderer.sprite = selectableObjectSpriteRenderer.sprite;
        _outlineSpriteRenderer.sortingLayerID = selectableObjectSpriteRenderer.sortingLayerID;
        _outlineSpriteRenderer.color = _selectedOutlineColor;
        _outlineSpriteRenderer.sortingOrder = 1;
    }
    public void SetIsSelected(bool boolean)
    {
        _isSelected = boolean;
        _outlineGO.SetActive(boolean);
        _outlineSpriteRenderer.color = IsInteractable ? _interactableOutlineColor : _selectedOutlineColor;
        if (_text != null)
        {
            string text = IsInteractable ? _interactableMessage : _selectedMessage;
            _text.text = boolean ? text : "";
        }
    }
    public void SetIsInteractableText(string text)
    {
        _interactableMessage = text;
    }
}
