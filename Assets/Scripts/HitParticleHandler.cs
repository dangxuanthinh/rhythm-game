using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticleHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticlePrefab;

    private void Start()
    {
        Tile.OnTileDestroyed += SpawnTapParticle;
    }

    private void OnDestroy()
    {
        Tile.OnTileDestroyed -= SpawnTapParticle;
    }

    private void SpawnTapParticle(Tile tile, HitType hitType)
    {
        if (hitType == HitType.Miss) return;
        Vector2 spawnPosition = new Vector2(tile.transform.position.x, Lane.CoordinateY);
        ParticleSystem tapParticle = LeanPool.Spawn(hitParticlePrefab, spawnPosition, Quaternion.identity);
        tapParticle.Play();
        LeanPool.Despawn(tapParticle, 3f);
    }
}
