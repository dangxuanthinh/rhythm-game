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
            MetricTimeSpan metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.Instance.midiFile.GetTempoMap());
            timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
        }
    }

    private void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.Instance.GetSongPlaybackTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteLifeTime)
            {
                Tile tile = LeanPool.Spawn(tilePrefab, transform);
                spawnedTiles.Add(tile);
                tile.Setup(transform.position, (float)timeStamps[spawnIndex]);
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.Instance.GetSongPlaybackTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000);

            if (Input.GetKeyDown(input))
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    // Hit
                    if (spawnedTiles[inputIndex] != null)
                    {
                        spawnedTiles[inputIndex].DestroyTile(HitType.Perfect);
                    }
                    inputIndex++;
                }
                else
                {
                    //print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                // Miss
                //print($"Missed {inputIndex} note");
                spawnedTiles[inputIndex].DestroyTile(HitType.Miss);
                inputIndex++;
            }
        }
    }
}
