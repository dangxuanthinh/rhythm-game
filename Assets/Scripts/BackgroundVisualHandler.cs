using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundVisualHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightDecoration;
    [SerializeField] private SpriteRenderer hitLineDecoration;

    private void Start()
    {
        Tile.OnTileDestroyed += LightUp;

        lightDecoration.transform.position = new Vector2(0f, Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.9f, Camera.main.transform.position.z)).y);
    }

    private void OnDestroy()
    {
        Tile.OnTileDestroyed -= LightUp;
    }

    private void LightUp(Tile tile, HitType hitType)
    {
        if(hitType == HitType.Miss) // No VFX on miss
        {
            return;
        }
        lightDecoration.DOKill();
        lightDecoration.DOFade(1f, 0.2f).OnComplete(() =>
        {
            lightDecoration.DOFade(0.6f, 0.1f).SetDelay(0.05f);
        });

        hitLineDecoration.DOKill();
        hitLineDecoration.DOFade(1f, 0.2f).OnComplete(() =>
        {
            hitLineDecoration.DOFade(0.5f, 0.4f).SetDelay(0.1f);
        });
    }
}
