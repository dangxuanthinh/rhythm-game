using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using DG.Tweening;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    [field: SerializeField] public double NoteLifeTime { get; private set; } // Time from when the note spawns to when it's 100% perfect to tap
    [field: SerializeField] public double MarginOfError { get; private set; } // How far off you can be from the perfect time to get a "Perfect" tap

    // Define base constants for note life time and margin of error
    // This makes it easier to scale the game speed if we want to
    private const double BASE_NOTE_LIFE_TIME = 1f;
    private const double BASE_MARGIN_OF_ERROR = 0.15f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float songDelaySeconds;
    [SerializeField] private List<Lane> lanes = new List<Lane>();

    private MidiFile midiFile;
    private bool songDistorted;

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

    private void Start()
    {
        GameManager.Instance.OnGameOver += DistortSong;

        ScaleMarginOfError();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= DistortSong;
    }

    public void InitializeMap(BeatmapData beatmapData)
    {
        audioSource.clip = beatmapData.music;
        ReadMIDIFromFile(beatmapData.midiFilePath);
        InitializeLanes();
        StartCoroutine(StartSong());
    }

    private void DistortSong()
    {
        if (songDistorted) return;
        songDistorted = true;
        float pitch = 1f;
        DOTween.To(() => pitch, x => pitch = x, 0.3f, 1f)
            .OnUpdate(() => audioSource.pitch = pitch).OnComplete(() =>
            {
                audioSource.Stop();
            }).SetDelay(2f);
    }

    // Scale margin of error, if noteLifeTime is big -> notes move slower -> more margin of error -> easier to tap
    private void ScaleMarginOfError()
    {
        MarginOfError = BASE_MARGIN_OF_ERROR * (NoteLifeTime / BASE_NOTE_LIFE_TIME);
    }

    private void ReadMIDIFromFile(string fileName)
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileName);
    }

    public TempoMap GetTempoMap()
    {
        if(midiFile == null)
        {
            Debug.LogError("MIDI file not loaded");
            return null;
        }
        return midiFile.GetTempoMap();
    }

    public void InitializeLanes()
    {
        // Get all the notes from MIDI file and give them to lanes
        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);
        foreach (Lane lane in lanes)
        {
            lane.SetTimeStamps(array);
        }
    }

    private IEnumerator StartSong()
    {
        // The game starts when the song plays
        yield return new WaitForSeconds(songDelaySeconds);
        audioSource.Play();
    }

    public double GetSongPlaybackTime()
    {
        return (double)audioSource.timeSamples / audioSource.clip.frequency;
    }
}
