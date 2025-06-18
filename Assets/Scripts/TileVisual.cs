using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject noteHoldIndicator;
    [SerializeField] private SpriteRenderer noteHoldLineSpriteRenderer;
    [SerializeField] private Color holdNoteTopGradient;
    [SerializeField] private Color tapNoteTopGradient;
    [SerializeField] private Color holdNoteBottomGradient;
    [SerializeField] private Color tapNoteBottomGradient;
    [SerializeField] private Color missColor;

    private Material material;
    private float initialSizeY;

    public const float TILE_BASE_HEIGHT = 1.5f;

    private void Awake()
    {
        material = spriteRenderer.material;
    }

    public void SetupTap()
    {
        noteHoldIndicator.SetActive(false);
        initialSizeY = spriteRenderer.size.y;
        spriteRenderer.size = new Vector2(1f, TILE_BASE_HEIGHT);

        material.SetColor("_Color", Color.white);
        material.SetColor("_TopColor", tapNoteTopGradient);
        material.SetColor("_BottomColor", tapNoteBottomGradient);
    }

    public void SetupHold(float sizeY)
    {
        noteHoldIndicator.SetActive(true);
        initialSizeY = sizeY;
        Vector2 size = spriteRenderer.size;
        size.y = initialSizeY;
        spriteRenderer.size = size;
        noteHoldLineSpriteRenderer.size = new Vector2(noteHoldLineSpriteRenderer.size.x, initialSizeY - TILE_BASE_HEIGHT);

        material.SetColor("_Color", Color.white);
        material.SetColor("_TopColor", holdNoteTopGradient);
        material.SetColor("_BotttomColor", holdNoteBottomGradient);
    }

    public void Show()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = Color.white;
    }

    public void PlayHitAnimation(UnityAction onComplete)
    {
        Color startColor = Color.white;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        material.SetColor("_Color", startColor);

        DOTween.To(() => startColor,
                   x => {
                       startColor = x;
                       material.SetColor("_Color", x);
                   }, endColor, 0.3f).OnComplete(() => onComplete?.Invoke());
    }

    public void PlayMissAnimation(UnityAction onComplete)
    {
        Color startColor = Color.white;
        Color endColor = missColor;

        material.SetColor("_Color", startColor);

        DOTween.To(() => startColor,
                   x => {
                       startColor = x;
                       material.SetColor("_Color", x);
                       material.SetColor("_TopColor", x);
                       material.SetColor("_BottomColor", x);
                   }, endColor, 0.3f).OnComplete(() => onComplete?.Invoke());
    }
}
