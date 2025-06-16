using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public float timeStamp;
    private double spawnTime;
    private SpriteRenderer spriteRenderer;

    private Vector2 startPosition;
    private Vector2 perfectPosition;
    private Vector2 endPosition;

    private float distanceToPerfectPosition;
    private float distanceToEndPosition;
    private float speed;

    public static UnityAction<HitType> OnTileDestroyed;

    void Update()
    {
        double elapsed = SongManager.Instance.GetSongPlaybackTime() - spawnTime;

        // Calculate how far the tile should have traveled at constant speed
        float traveled = (float)elapsed * speed;

        // Clamp to the total distance
        if (traveled >= distanceToEndPosition)
        {
            Destroy(gameObject);
            return;
        }

        // Interpolate position along the path
        float t = traveled / distanceToEndPosition;
        transform.position = Vector2.Lerp(startPosition, endPosition, t);

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    public void Setup(Vector2 lanePosition, float timeStamp)
    {
        this.timeStamp = timeStamp;
        spawnTime = SongManager.Instance.GetSongPlaybackTime();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = new Vector2(lanePosition.x, Camera.main.ViewportToWorldPoint(Vector2.one).y + 1);
        perfectPosition = new Vector2(lanePosition.x, lanePosition.y);
        endPosition = new Vector2(lanePosition.x, Camera.main.ViewportToWorldPoint(Vector2.zero).y - 1);

        distanceToPerfectPosition = Vector2.Distance(startPosition, perfectPosition);
        distanceToEndPosition = Vector2.Distance(startPosition, endPosition);

        // Speed so that tile reaches perfectPosition at noteLifeTime
        speed = distanceToPerfectPosition / (float)SongManager.Instance.noteLifeTime;
    }

    public void DestroyTile(HitType hitType)
    {
        OnTileDestroyed?.Invoke(hitType);
        LeanPool.Despawn(this);
    }
}