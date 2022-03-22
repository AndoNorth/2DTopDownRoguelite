using UnityEngine;
public class HealthBar
{
    private GeneralUtility.World_Bar _healthBar;
    private GameObject _gameObject;

    public HealthBar(Vector3 position, string characterName, Transform parent)
    {
        _gameObject = new GameObject(characterName + "'s Health Bar");
        _gameObject.transform.position = position;
        _gameObject.transform.parent = parent;
        GeneralUtility.World_Bar.Outline outline = new GeneralUtility.World_Bar.Outline();
        outline.size = 0.1f;

        _healthBar = new GeneralUtility.World_Bar(_gameObject.transform, new Vector3(0, 0.7f), new Vector3(1.2f, 0.12f), Color.white, Color.red, 1f, -10, GameAssets.instance.healthBarSortingLayer, outline);
    }
    public void SetHealthBar(float fillAmount)
    {
        _healthBar.SetSize(fillAmount);
    }
    public void SetInverseRotation(Quaternion rot)
    {
        _gameObject.transform.SetPositionAndRotation(_gameObject.transform.position, rot);
    }
    public void Show()
    {
        _healthBar.Show();
    }
    public void Hide()
    {
        _healthBar.Hide();
    }
}