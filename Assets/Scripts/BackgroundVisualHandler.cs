using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundVisualHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightDecoration;

    private void Start()
    {
        Tile.OnTileDestroyed += LightUp;
    }

    private void OnDestroy()
    {
        Tile.OnTileDestroyed -= LightUp;
    }

    private void LightUp(HitType hitType)
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
    }
}
