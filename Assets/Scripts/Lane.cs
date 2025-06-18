using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;
using Lean.Pool;

public class Lane : MonoBehaviour
{
    [SerializeField] private NoteName noteName;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private KeyCode input; // Keyboard input

    private List<TileData> tilesData = new List<TileData>();
    private List<Tile> spawnedTiles = new List<Tile>();

    private int spawnIndex;
    private int inputIndex;

    public static float CoordinateY { get; private set; }

    private void Awake()
    {
        CoordinateY = transform.position.y;
    }

    private void Start()
    {
        Tile.OnHoldAutoComplete += OnHoldAutoComplete;
        Setup(SongManager.Instance.GetNotes());
    }

    private void OnDestroy()
    {
        Tile.OnHoldAutoComplete -= OnHoldAutoComplete;
    }

    private void OnHoldAutoComplete(TileData tileData)
    {
        if(tileData.noteName == noteName)
        {
            AdvanceInputIndex();
        }
    }

    public void Setup(Note[] notes)
    {
        if (notes == null)
        {
            Debug.LogWarning("MIDI Notes are not initialized, cannot spawn notes");
            return;
        }
        foreach (Note note in notes)
        {
            if(note.NoteName != NoteName.F && note.NoteName != NoteName.G && note.NoteName != NoteName.A && note.NoteName != NoteName.B)
            {
                Debug.Log(note.NoteName);
                Debug.LogError("CHECK MAP");
            }
            if (note.NoteName != noteName) continue;

            MetricTimeSpan length = LengthConverter.ConvertTo<MetricTimeSpan>(note.Length, note.Time, SongManager.Instance.GetTempoMap());
            double tileLength = length.Minutes * 60f + length.Seconds + (double)length.Milliseconds / 1000f;
            TileType tileType = tileLength > 0.5f ? TileType.Hold : TileType.Tap;

            MetricTimeSpan timeStamp = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.Instance.GetTempoMap());
            double tileTimeStamp = (double)timeStamp.Minutes * 60f + timeStamp.Seconds + (double)timeStamp.Milliseconds / 1000f;

            tilesData.Add(new TileData(tileType, tileTimeStamp, tileLength, note.NoteName));
        }
    }

    private void Update()
    {
        TrySpawnTile();
    }

    private void TrySpawnTile()
    {
        if (spawnIndex >= tilesData.Count) return;

        if (SongManager.Instance.GetSongPlaybackTime() >= tilesData[spawnIndex].timeStamp - SongManager.Instance.NoteLifeTime)
        {
            Tile tile = LeanPool.Spawn(tilePrefab, Vector2.one * 1000f, Quaternion.identity, transform);
            spawnedTiles.Add(tile);
            tile.Setup(transform.position, tilesData[spawnIndex]);
            spawnIndex++;
        }
    }

    public bool HasTileToProcess => inputIndex < tilesData.Count && inputIndex < spawnedTiles.Count;
    public Tile CurrentTile => spawnedTiles[inputIndex];
    public TileData CurrentTileData => tilesData[inputIndex];
    public void AdvanceInputIndex() => inputIndex++;
}
