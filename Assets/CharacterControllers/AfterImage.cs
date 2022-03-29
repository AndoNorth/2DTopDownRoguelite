using System.Collections;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField] Color _afterImageColor;
    [SerializeField] float _startTime;
    [SerializeField] float _refreshImageRate;
    SpriteRenderer _spriteRenderer;
    // update transform every iteration
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _spriteRenderer.color = _afterImageColor;
        InvokeRepeating("UpdateAfterImage", _startTime, _refreshImageRate);
    }
    private void OnEnable()
    {
        transform.position = GameAssets.instance.playerCharacter.transform.position;
        InvokeRepeating("UpdateAfterImage", _startTime, _refreshImageRate);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    private void UpdateAfterImage()
    {
        transform.position = GameAssets.instance.playerCharacter.transform.position;
        _spriteRenderer.color = _afterImageColor;
        StartCoroutine(FadeTo(0.0f, 1.0f));
    }
    IEnumerator FadeTo(float aValue, float fadeTime)
    {
        float alpha = _spriteRenderer.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(_afterImageColor.r, _afterImageColor.g, _afterImageColor.b, Mathf.Lerp(alpha, aValue, t));
            _spriteRenderer.color = newColor;
            yield return null;
        }
    }
}
