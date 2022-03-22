using UnityEngine;

[CreateAssetMenu(menuName = "GameAssets/Projectile Data", fileName = "New Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public string _projName;
    public PolygonCollider2D _projCollider;
    public Color _projColor;
    public TrailRenderer _projTrail;
}
