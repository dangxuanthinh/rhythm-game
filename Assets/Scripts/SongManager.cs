using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    public MidiFile midiFile;
    public double noteLifeTime = 2f; // Time from when the note spawns to when it disappears
    public double marginOfError = 0.3f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float songDelaySeconds;
    [SerializeField] private string fileLocation;

    public double inputDelayInMilliseconds;

    [SerializeField] private List<Lane> lanes = new List<Lane>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        ReadMIDIFromFile();
        InitializeMapFromMIDI();
        yield return new WaitForSeconds(0f);
        StartSong();
    }

    private void ReadMIDIFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
    }

    public void InitializeMapFromMIDI()
    {
        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);
        foreach (Lane lane in lanes)
        {
            lane.SetTimeStamps(array);
        }
    }

    private void StartSong()
    {
        // The game starts when the song plays
        audioSource.Play();
    }

    public double GetSongPlaybackTime()
    {
        return (double)audioSource.timeSamples / audioSource.clip.frequency;
    }
}
