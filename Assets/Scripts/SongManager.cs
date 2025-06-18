using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    [field: SerializeField, Range(0.5f, 1.2f)] public double NoteLifeTime { get; private set; } // Time from when the note spawns to when it's 100% perfect to tap
    [field: SerializeField, Range(0.15f, 0.25f)] public double MarginOfError { get; private set; } // How far off you can be from the perfect time to tap
    [field: SerializeField] public float SongDelaySeconds { get; private set; }

    [SerializeField] private AudioSource audioSource;

    private MidiFile midiFile;
    private bool songDistorted;

    private BeatmapData beatmapData;
    private Note[] notes;
    private float songPlayBackTimeOffset;

    public UnityAction OnSongFinished;
    private bool songFinishedNotified = true;

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
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= DistortSong;
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void Update()
    {
        TrackSongCompletion();
    }

    private void TrackSongCompletion()
    {
        if (audioSource.clip == null || songFinishedNotified) return;

        if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length - 0.01f)
        {
            songFinishedNotified = true;
            Debug.Log("SONG FINISED");
            OnSongFinished?.Invoke();
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Gameplay")
        {
            StartCoroutine(StartSong());
        }
    }

    public void InitializeMap(BeatmapData beatmapData)
    {
        this.beatmapData = beatmapData;
        audioSource.clip = beatmapData.music;
        InitializeNotes();
    }

    private void DistortSong()
    {
        if (songDistorted) return;
        songDistorted = true;
        float pitch = 1f;
        DOTween.To(() => pitch, x => pitch = x, 0.3f, 1f)
            .OnUpdate(() => audioSource.pitch = pitch).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    // Check scene in case the player exits too quickly
                    if (SceneManager.GetActiveScene().name == "Gameplay")
                        audioSource.Stop();
                    audioSource.pitch = 1f;
                    songDistorted = false;
                });
            });
    }

    private void InitializeNotes()
    {
        midiFile = MapCatalog.Instance.GetMidiFile(beatmapData);
        var notesFromMIDI = midiFile.GetNotes();
        notes = new Note[notesFromMIDI.Count];
        notesFromMIDI.CopyTo(notes, 0);
    }

    public TempoMap GetTempoMap()
    {
        if (midiFile == null)
        {
            Debug.LogError("MIDI file not loaded");
            return null;
        }
        return midiFile.GetTempoMap();
    }

    public Note[] GetNotes()
    {
        return notes;
    }

    private IEnumerator StartSong()
    {
        // The game starts when the song plays
        songPlayBackTimeOffset = -SongDelaySeconds;
        StartCoroutine(IncreaseSongPlayBackOffset());
        audioSource.Stop();
        yield return new WaitForSeconds(SongDelaySeconds);
        songFinishedNotified = false;
        audioSource.Play();
    }

    IEnumerator IncreaseSongPlayBackOffset()
    {
        while (songPlayBackTimeOffset < 0)
        {
            songPlayBackTimeOffset += Time.deltaTime;
            yield return null;
        }
        songPlayBackTimeOffset = 0f;
    }

    public double GetSongPlaybackTime()
    {
        return ((double)audioSource.timeSamples / audioSource.clip.frequency) + songPlayBackTimeOffset;
    }

    public void PlaySongPreview(AudioClip music, float startPoint)
    {
        audioSource.Stop();
        audioSource.clip = music;
        audioSource.time = startPoint * audioSource.clip.length;
        audioSource.Play();
    }

    public int GetSongBPM()
    {
        return beatmapData.bpm;
    }
}
