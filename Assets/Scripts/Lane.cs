using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;
using Lean.Pool;

public class Lane : MonoBehaviour
{
    [SerializeField] private NoteName noteName;

    [SerializeField] private Tile tilePrefab;

    private List<double> timeStamps = new List<double>();

    private int spawnIndex;
    private int inputIndex;

    private List<Tile> spawnedTiles = new List<Tile>();

    public KeyCode input;

    public static float LaneY { get; private set; }

    private void Awake()
    {
        LaneY = transform.position.y;
    }

    public void SetTimeStamps(Note[] notes)
    {
        foreach (Note note in notes)
        {
            if (note.NoteName != noteName) continue;
            MetricTimeSpan metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.Instance.GetTempoMap());
            timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
        }
    }

    private void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.Instance.GetSongPlaybackTime() >= timeStamps[spawnIndex] - SongManager.Instance.NoteLifeTime)
            {
                Tile tile = LeanPool.Spawn(tilePrefab, Vector2.one * 1000f, Quaternion.identity, transform);
                spawnedTiles.Add(tile);
                tile.Setup(transform.position, (float)timeStamps[spawnIndex]);
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.MarginOfError;
            double audioTime = SongManager.Instance.GetSongPlaybackTime();

            if (Input.GetKeyDown(input))
            {
                double timingDifference = Math.Abs(audioTime - timeStamp);
                if (timingDifference < marginOfError)
                {
                    HitType hitType;
                    if (timingDifference < marginOfError / 2f)
                    {
                        hitType = HitType.Perfect;
                    }
                    else
                    {
                        hitType = HitType.Good;
                    }
                    if (spawnedTiles[inputIndex] != null)
                    {
                        spawnedTiles[inputIndex].DestroyTile(hitType);
                    }
                    inputIndex++;
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                spawnedTiles[inputIndex].DestroyTile(HitType.Miss);
                inputIndex++;
            }
        }
    }
}
