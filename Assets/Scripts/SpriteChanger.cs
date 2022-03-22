using System.Collections;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public static SpriteChanger instance;
    private void Awake()
    {
        // singleton pattern
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
    public void ChangeSpriteColorForTime(SpriteRenderer sprite, Color newColor, Color oldColor, float time)
    {
        StartCoroutine(ChangeSpriteColorForTimeRoutine(sprite, newColor, oldColor, time));
    }
    private IEnumerator ChangeSpriteColorForTimeRoutine(SpriteRenderer sprite, Color newColor, Color oldColor, float time)
    {
        sprite.color = newColor;
        yield return new WaitForSeconds(time);
        sprite.color = oldColor;
    }
}
