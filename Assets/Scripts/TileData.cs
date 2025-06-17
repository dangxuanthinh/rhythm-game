using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public TileType tileType;
    public NoteName noteName;
    public double timeStamp;
    public double length;

    public TileData(TileType tileType, double timeStamp, double length, NoteName noteName)
    {
        this.tileType = tileType;
        this.timeStamp = timeStamp;
        this.length = length;
        this.noteName = noteName;
    }
}

public enum TileType
{
    Tap,
    Hold
}
