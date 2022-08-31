using UnityEngine;

public class PickUpAmmo : MonoBehaviour
{
    private AmmoType _ammoType;
    public AmmoType AmmoType => _ammoType;
    private PlayerWeaponSystem _playerWeaponSystem;
    private SpriteRenderer _ammoSprite;
    private void Awake()
    {
        _ammoSprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _playerWeaponSystem = FindObjectOfType<PlayerWeaponSystem>();
        SetAmmoType(GeneralUtility.GetRandomEnumValue<AmmoType>());
    }
    public void SetAmmoType(AmmoType ammoType)
    {
        _ammoType = ammoType;
        RefreshVisuals();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            PickUp();
        }
    }
    private void RefreshVisuals()
    {
        _ammoSprite.color = GameAssets.instance.ammoColors.GetAmmoColour(_ammoType);
    }
    private void PickUp()
    {
        _playerWeaponSystem.AddToAmmoReserve(_ammoType, GameAssets.instance.ammoAmounts.GetAmmoAmount(_ammoType));
        GameObject.Destroy(this.gameObject);
    }
}
