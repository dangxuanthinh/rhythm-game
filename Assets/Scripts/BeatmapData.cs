using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "Beatmap/Beatmap Data")]
public class BeatmapData : ScriptableObject
{
    public AudioClip music;
    public string midiFilePath;

    [Header("Music info")]
    public string songName;
    public string artistName;
    public int bpm;
    [Range(0, 1)]
    public float songPreviewPoint;
}
