using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryDisplay : MonoBehaviour
{
    [SerializeField] private Image _firstWeaponImage;
    private Image _firstWeaponOutline;
    [SerializeField] private Image _secondWeaponImage;
    private Image _secondtWeaponOutline;
    [SerializeField] private Image _templateWeaponImage;
    private Image _templateWeaponOutline;
    private Sprite _emptyWeaponSprite;
    private PlayerWeaponSystem _playerWeaponSystem;
    private TextMeshProUGUI _textMeshPro;
    private Weapon _currentWeapon;
    private float _timeSpentReloading = 0f;
    private GeneralUtility.UI_Bar _reloadBar;
    [SerializeField] private Vector2 barSize;
    [SerializeField] private Vector2 anchorPosition;
    private void Start()
    {
        _emptyWeaponSprite = GameAssets.instance.templateWeaponGO.GetComponent<SpriteRenderer>().sprite;
        _firstWeaponOutline = GetOutlineImage(_firstWeaponImage.GetComponentsInParent<Image>());
        _secondtWeaponOutline = GetOutlineImage(_secondWeaponImage.GetComponentsInParent<Image>());
        _templateWeaponOutline = GetOutlineImage(_templateWeaponImage.GetComponentsInParent<Image>());
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        _playerWeaponSystem = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerWeaponSystem>();
        _playerWeaponSystem.OnWeaponChanged += UpdateWeaponUI;
        UpdateWeaponUI();
        _playerWeaponSystem.OnAmmoChanged += UpdateAmmoUI;
        UpdateAmmoUI();
        GeneralUtility.UI_Bar.Outline outline = new GeneralUtility.UI_Bar.Outline(1f, Color.black);
        _reloadBar = new GeneralUtility.UI_Bar(transform, anchorPosition, barSize, Color.black, Color.white, 1f, outline);
        _reloadBar.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        Image GetOutlineImage(Image[] imageArray)
        {
            foreach (Image image in imageArray)
            {
                if(image.gameObject.name.Contains("Outline"))
                    return image;
            }
            return null;
        }
    }
    private void Update()
    {
        if(_playerWeaponSystem.IsReloading)
            _timeSpentReloading += Time.deltaTime;
        else
            _timeSpentReloading = 0;
    }
    private void FixedUpdate()
    {
        UpdateReloadTimerUI();
    }
    private void OnEnable()
    {
        if (_playerWeaponSystem != null)
        {
            _playerWeaponSystem.OnWeaponChanged += UpdateWeaponUI;
            _playerWeaponSystem.OnAmmoChanged += UpdateAmmoUI;
        }
    }
    private void OnDisable()
    {
        if (_playerWeaponSystem != null)
        {
            _playerWeaponSystem.OnWeaponChanged -= UpdateWeaponUI;
            _playerWeaponSystem.OnAmmoChanged -= UpdateAmmoUI;
        }
    }
    private void UpdateWeaponUI()
    {
        _currentWeapon = _playerWeaponSystem.CurrentWeapon;
        ResetWeaponSprites();
        Weapon _firstWeapon = _playerWeaponSystem.GetWeapon(0);
        if(_firstWeapon != null)
        {
            _firstWeaponImage.sprite = _firstWeapon.WeaponData.Sprite();
            _firstWeaponImage.color = _firstWeapon.WeaponData.Color();
        }
        Weapon _secondWeapon = _playerWeaponSystem.GetWeapon(1);
        if (_secondWeapon != null)
        {
            _secondWeaponImage.sprite = _secondWeapon.WeaponData.Sprite();
            _secondWeaponImage.color = _secondWeapon.WeaponData.Color();
        }
        Weapon _templateWeapon = _playerWeaponSystem.GetWeapon(2);
        if (_templateWeapon != null)
        {
            _templateWeaponImage.sprite = _templateWeapon.WeaponData.Sprite();
            _templateWeaponImage.color = _templateWeapon.WeaponData.Color();
        }
        UpdateWeaponOutline();
    }
    private void ResetWeaponSprites()
    {
        _firstWeaponImage.sprite = _emptyWeaponSprite;
        _firstWeaponImage.color = Color.white;
        _secondWeaponImage.sprite = _emptyWeaponSprite;
        _secondWeaponImage.color = Color.white;
        _templateWeaponImage.sprite = _emptyWeaponSprite;
        _templateWeaponImage.color = Color.white;
    }
    private void UpdateWeaponOutline()
    {
        _firstWeaponOutline.sprite = _firstWeaponImage.sprite;
        _firstWeaponOutline.color = Color.black;
        _secondtWeaponOutline.sprite = _secondWeaponImage.sprite;
        _secondtWeaponOutline.color = Color.black;
        _templateWeaponOutline.sprite = _templateWeaponImage.sprite;
        _templateWeaponOutline.color = Color.black;
        if (_playerWeaponSystem.WeaponIdx == 0)
        {
            _firstWeaponOutline.color = Color.white;
        }
        else if (_playerWeaponSystem.WeaponIdx == 1)
        {
            _secondtWeaponOutline.color = Color.white;
        }
        else if (_playerWeaponSystem.WeaponIdx == 2)
        {
            _templateWeaponOutline.color = Color.white;
        }
    }
    private void UpdateAmmoUI()
    {
        int currentAmmo = _currentWeapon.AmmoInMagazine;
        int reserveAmmo = _playerWeaponSystem.ReserveAmmo;
        _textMeshPro.SetText(currentAmmo + "/" + reserveAmmo + "\n" 
            + _playerWeaponSystem.AmmoReserve.ReserveAmmo(AmmoType.Light) + '/'
            + _playerWeaponSystem.AmmoReserve.ReserveAmmo(AmmoType.Heavy) + '/'
            + _playerWeaponSystem.AmmoReserve.ReserveAmmo(AmmoType.Energy) + '/' 
            + _playerWeaponSystem.AmmoReserve.ReserveAmmo(AmmoType.Explosive) );
    }
    private void UpdateReloadTimerUI()
    {
        if (_reloadBar != null)
        {
            if(_timeSpentReloading > _currentWeapon.WeaponData._reloadTime)
            {
                _timeSpentReloading = _currentWeapon.WeaponData._reloadTime;
            }
            _reloadBar.SetSize((_currentWeapon.WeaponData._reloadTime - _timeSpentReloading) / _currentWeapon.WeaponData._reloadTime);
        }
    }
}
