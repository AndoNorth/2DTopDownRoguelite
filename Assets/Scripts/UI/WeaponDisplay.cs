using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponDisplay : MonoBehaviour
{
    private Image _currentWeaponImage;
    private PlayerWeaponSystem _playerWeaponSystem;
    private TextMeshProUGUI _textMeshPro;
    private WeaponData _weapon;
    private float _timeSpentReloading=0f;
    private GeneralUtility.UI_Bar _reloadBar;
    [SerializeField] Vector2 barSize;
    [SerializeField] Vector2 anchorPosition;
    void Start()
    {
        _currentWeaponImage = GetComponentInChildren<Image>();
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        _playerWeaponSystem = GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerWeaponSystem>();
        _playerWeaponSystem.OnWeaponChanged += UpdateWeaponUI;
        UpdateWeaponUI();
        _playerWeaponSystem.OnAmmoChanged += UpdateAmmoUI;
        UpdateAmmoUI();
        GeneralUtility.UI_Bar.Outline outline = new GeneralUtility.UI_Bar.Outline(1f, Color.black);
        _reloadBar = new GeneralUtility.UI_Bar(transform, anchorPosition, barSize, Color.black, Color.white, 1f, outline);
        _reloadBar.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
    private void Update()
    {
        if(_playerWeaponSystem.IsReloading)
        {
            _timeSpentReloading += Time.deltaTime;
        }
        else
        {
            _timeSpentReloading = 0;
        }
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
        _weapon = _playerWeaponSystem.CurrentWeapon;
        if (_weapon != null)
        {
            _currentWeaponImage.sprite = _weapon._sprite;
            _currentWeaponImage.color = _weapon._color;
        }
    }
    private void UpdateAmmoUI()
    {
        if (_weapon != null)
        {
            int currentAmmo = _weapon.AmmoInMagazine;
            int reserveAmmo = _playerWeaponSystem.ReserveAmmo;
            _textMeshPro.SetText(currentAmmo + "/" + reserveAmmo);
        }
    }
    private void UpdateReloadTimerUI()
    {
        if (_reloadBar != null)
        {
            if(_timeSpentReloading > _weapon._reloadTime)
            {
                _timeSpentReloading = _weapon._reloadTime;
            }
            _reloadBar.SetSize((_weapon._reloadTime - _timeSpentReloading) / _weapon._reloadTime);
        }
    }
}
