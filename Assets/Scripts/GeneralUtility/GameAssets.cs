using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;
    [SerializeField] private List<string> layerNames = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    private void Start()
    {
        playerCharacter = FindObjectOfType<PlayerInput>().gameObject;
        SetupLayerList();
    }
    private void SetupLayerList()
    {
        layerNames = new List<string>();
        for (int i = 8; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
        {
            string layerN = LayerMask.LayerToName(i); //get the name of the layer
            if (layerN.Length > 0) //only add the layer if it has been named (comment this line out if you want every layer)
            {
                layerNames.Add(layerN);
            }
        }
    }
    [Header("static references")]
    public Sprite white1x1;
    public TrailRenderer ShotTrail;
    public TemplateProject.TextPopup pfTextPopup;
    public GameObject playerCharacter;
    public DamageProjectile templateBullet;
    public PickUpWeapon templateWeaponGO;
    public PickUpWeaponUpgrade templateWeaponUpgradeGO;
    [Header("sorting layers")]
    public string healthBarSortingLayer;
    public string timerBarSortingLayer;
    [Header("layer masks")]
    public string[] playerShootableLayers;
    public string[] enemyShootableLayers;
}
