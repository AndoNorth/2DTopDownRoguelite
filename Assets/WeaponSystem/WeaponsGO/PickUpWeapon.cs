using UnityEngine;

public class PickUpWeapon : MonoBehaviour, IInteractable
{
    private Weapon _weapon;
    public Weapon Weapon => _weapon;
    private SpriteRenderer _weaponSprite;
    private PlayerWeaponSystem _playerWeaponSystem;
    private void Awake()
    {
        _weaponSprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _playerWeaponSystem = FindObjectOfType<PlayerWeaponSystem>();
    }
    public void SetWeapon(Weapon weapon)
    {
        _weapon = weapon;
        RefreshVisuals();
    }
    private void PickUp()
    {
        _playerWeaponSystem.PickUpWeapon(this);
        GameObject.Destroy(this.gameObject);
    }
    private void RefreshVisuals()
    {
        _weaponSprite.sprite = _weapon.WeaponData._sprite;
        _weaponSprite.color = _weapon.WeaponData._color;
    }
    public void Interact()
    {
        PickUp();
    }
}
