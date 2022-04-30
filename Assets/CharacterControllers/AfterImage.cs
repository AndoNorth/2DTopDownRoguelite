using System.Collections;
using UnityEngine;
using System;

public class AfterImage : MonoBehaviour
{
    private Action<AfterImage> _killAction;
    [SerializeField] private Color _afterImageColorTint;
    [SerializeField] private float _strength;
    [SerializeField] private float _fadeTime;
    private Color _afterImageColor;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    public void SetupAfterImage(Vector3 position, Color color)
    {
        transform.position = position;
        _afterImageColor = Color.Lerp(color, _afterImageColorTint, _strength);
        StartCoroutine(FadeTo(0.0f, _fadeTime));
    }
    IEnumerator FadeTo(float aValue, float fadeTime)
    {
        float alpha = _afterImageColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(_afterImageColor.r, _afterImageColor.g, _afterImageColor.b, Mathf.Lerp(alpha, aValue, t));
            _spriteRenderer.color = newColor;
            yield return null;
        }
        _killAction(this);
    }
    public void InitKillAction(Action<AfterImage> killAction)
    {
        _killAction = killAction;
    }
}
