using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHeldVisual : MonoBehaviour
{
    private float speed;
    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y + speed * 2 * Time.deltaTime);
    }

    public void Setup(float tileMovingSpeed, Vector2 currentTilePosition)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        float initialSizeY = Mathf.Abs(Lane.CoordinateY - currentTilePosition.y);
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, initialSizeY);
        this.speed = tileMovingSpeed;
    }

    public void Hide()
    {
        spriteRenderer.DOFade(0f, 0.5f).OnComplete(() =>
        {
            LeanPool.Despawn(this);
        });
    }
}
