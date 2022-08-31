using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Ammo Amounts", fileName = "New Ammo Ammounts")]
public class AmmoAmounts : ScriptableObject
{
    [SerializeField] private int[] ammoAmounts = new int[sizeof(AmmoType)];
    public int GetAmmoAmount(AmmoType ammoType) => ammoAmounts[(int)ammoType];
}