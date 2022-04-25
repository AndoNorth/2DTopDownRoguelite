using UnityEngine;

public class PickUpWeaponUpgrade : MonoBehaviour, IInteractable
{
    private WeaponUpgrade _weaponUpgrade;
    public WeaponUpgrade WeaponUpgrade => _weaponUpgrade;
    private SpriteRenderer _sprite;
    private PlayerWeaponSystem _playerWeaponSystem;
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _playerWeaponSystem = FindObjectOfType<PlayerWeaponSystem>();
    }
    public void SetWeaponUpgrade(WeaponUpgrade weaponUpgrade)
    {
        _weaponUpgrade = weaponUpgrade;
        RefreshVisuals();
    }
    private void PickUp()
    {
        _playerWeaponSystem.PickUpWeaponUpgrade(this);
        GameObject.Destroy(this.gameObject);
    }
    private void RefreshVisuals()
    {
        _sprite.sprite = _weaponUpgrade._sprite;
        _sprite.color = _weaponUpgrade._color;
    }
    public void Interact()
    {
        PickUp();
    }
}
