using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Ammo Colours", fileName = "New Ammo Colours")]
public class AmmoColours : ScriptableObject
{
    [SerializeField] private Color[] ammoColors = new Color[sizeof(AmmoType)];
    public Color GetAmmoColour(AmmoType ammoType) => ammoColors[(int) ammoType];
}
