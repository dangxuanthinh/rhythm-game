using Lean.Pool;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] private AudioClip hitAudio;
    [SerializeField] private AudioClip missAudio;
    [SerializeField] private NoteHeldVisual noteHeldVisualPrefab;
    [SerializeField] private TileVisual tileVisual;

    private AudioSource audioSource;

    private Vector2 startPosition;
    private Vector2 perfectPosition;

    private float moveSpeed;

    public bool IsHolding { get; private set; }

    public bool Held { get; private set; }

    private float holdElapsed;
    private float holdProgress;

    private NoteHeldVisual currentHeldVisual;
    private TileData tileData;

    public static UnityAction<TileData> OnHoldAutoComplete;
    public static UnityAction<HitType> OnTileDestroyed;
    public static UnityAction OnTileMissed;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        tileVisual.Show();
    }

    private void Update()
    {
        MoveTile();
        HandleTileHolding();
    }

    private void HandleTileHolding()
    {
        if (tileData.tileType == TileType.Hold && IsHolding)
        {
            holdElapsed += Time.deltaTime;
            holdProgress = Mathf.Clamp01((float)(holdElapsed / tileData.length));

            if (holdProgress >= 1f)
            {
                EndHoldAutoComplete();
            }
        }
    }

    private void MoveTile()
    {
        float newY = transform.position.y - moveSpeed * Time.deltaTime;
        transform.position = new Vector2(startPosition.x, newY);
    }

    public void Setup(Vector2 lanePosition, TileData data)
    {
        this.tileData = data;
        Held = false;

        startPosition = new Vector2(lanePosition.x, Camera.main.ViewportToWorldPoint(Vector2.one).y + 1);
        perfectPosition = new Vector2(lanePosition.x, lanePosition.y);
        transform.position = startPosition;

        float distanceToPerfectPosition = Vector2.Distance(startPosition, perfectPosition);
        moveSpeed = distanceToPerfectPosition / (float)SongManager.Instance.NoteLifeTime;

        if (tileData.tileType == TileType.Hold)
        {
            float initialSizeY = moveSpeed * (float)tileData.length;
            tileVisual.SetupHold(initialSizeY);
            holdElapsed = 0f;
            IsHolding = false;
        }
        else
        {
            tileVisual.SetupTap();
        }
    }

    public void StartHold()
    {
        if (tileData.tileType == TileType.Hold)
        {
            Held = true;
            IsHolding = true;
            holdElapsed = 0f;

            NoteHeldVisual heldVisual = LeanPool.Spawn(noteHeldVisualPrefab, new Vector2(transform.position.x, Lane.CoordinateY), Quaternion.identity);
            heldVisual.Setup(moveSpeed, transform.position);
            currentHeldVisual = heldVisual;
        }
    }

    private void EndHoldAutoComplete()
    {
        OnHoldAutoComplete?.Invoke(tileData);
        EndHold();
    }

    public void EndHold()
    {
        IsHolding = false;
        HitType hitType = holdProgress > 0.8f ? HitType.Perfect : HitType.Good;
        if (currentHeldVisual != null)
        {
            currentHeldVisual.Hide();
            currentHeldVisual = null;
        }
        DestroyTile(hitType);
    }

    public void DestroyTile(HitType hitType)
    {
        OnTileDestroyed?.Invoke(hitType);

        if (hitType == HitType.Miss)
        {
            Debug.Log("MISS");
            OnTileMissed?.Invoke();
            tileVisual.PlayMissAnimation(() => LeanPool.Despawn(this, 2f));
        }
        else
        {
            tileVisual.PlayHitAnimation(() => LeanPool.Despawn(this, 2f));
        }
    }

    public NoteName GetNoteName()
    {
        return tileData.noteName;
    }

    public TileType GetTileType()
    {
        return tileData.tileType;
    }
}