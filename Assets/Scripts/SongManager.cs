using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    [field: SerializeField] public double NoteLifeTime { get; private set; } // Time from when the note spawns to when it's 100% perfect to tap
    [field: SerializeField] public double MarginOfError { get; private set; } // How far off you can be from the perfect time to get a "Perfect" tap
    [field: SerializeField] public float SongDelaySeconds { get; private set; }

    // Define base constants for note life time and margin of error
    // This makes it easier to scale the game speed if we want to
    private const double BASE_NOTE_LIFE_TIME = 1f;
    private const double BASE_MARGIN_OF_ERROR = 0.15f;

    [SerializeField] private AudioSource audioSource;

    private MidiFile midiFile;
    private bool songDistorted;

    private BeatmapData beatmapData;
    private Note[] notes;
    private float songPlayBackTimeOffset;

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

        ScaleMarginOfError();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= DistortSong;
        SceneManager.sceneLoaded -= SceneLoaded;
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
        ReadMIDIFromFile(beatmapData.midiFilePath);
    }

    private void DistortSong()
    {
        if (songDistorted) return;
        songDistorted = true;
        float pitch = 1f;
        DOTween.To(() => pitch, x => pitch = x, 0.3f, 1.3f)
            .OnUpdate(() => audioSource.pitch = pitch).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    audioSource.Stop();
                    audioSource.pitch = 1f;
                    songDistorted = false;
                });
            });
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
        if (midiFile == null)
        {
            Debug.LogError("MIDI file not loaded");
            return null;
        }
        return midiFile.GetTempoMap();
    }

    public Note[] GetNotes()
    {
        if (notes == null)
        {
            var notesFromMIDI = midiFile.GetNotes();
            notes = new Note[notesFromMIDI.Count];
            notesFromMIDI.CopyTo(notes, 0);
        }
        return notes;
    }

    private IEnumerator StartSong()
    {
        // The game starts when the song plays
        songPlayBackTimeOffset = -SongDelaySeconds;
        StartCoroutine(IncreaseSongPlayBackOffset());
        audioSource.Stop();
        yield return new WaitForSeconds(SongDelaySeconds);
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
