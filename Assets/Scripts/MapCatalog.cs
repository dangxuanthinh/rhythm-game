using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MapCatalog : MonoBehaviour
{
    public static MapCatalog Instance;
    private List<BeatmapData> maps = new List<BeatmapData>();
    private Dictionary<BeatmapData, MidiFile> midiFileTable = new Dictionary<BeatmapData, MidiFile>();

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
        LoadMaps();
        foreach (BeatmapData map in maps)
        {
            StartCoroutine(LoadMIDI(map));
        }
    }

    private void LoadMaps()
    {
        maps = Resources.LoadAll<BeatmapData>("Beatmaps").ToList();
    }

    public List<BeatmapData> GetMaps()
    {
        return new List<BeatmapData>(maps);
    }

    public MidiFile GetMidiFile(BeatmapData beatmapData)
    {
        if (midiFileTable.TryGetValue(beatmapData, out var midiFile))
        {
            return midiFile;
        }
        else
        {
            Debug.LogError("Cannot get midi file " + beatmapData.midiFilePath);
            return null;
        }
    }

    private IEnumerator LoadMIDI(BeatmapData beatmapData)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, beatmapData.midiFilePath);
        MidiFile midiFile;
#if UNITY_ANDROID && !UNITY_EDITOR
    // On Android, use UnityWebRequest
    using (var www = UnityWebRequest.Get(path))
    {
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            var midiData = www.downloadHandler.data;
            using (var stream = new System.IO.MemoryStream(midiData))
            {
                midiFile = MidiFile.Read(stream);
                midiFileTable[beatmapData] = midiFile;
            }
        }
        else
        {
            Debug.LogError("Failed to load MIDI file: " + www.error);
            yield break;
        }
    }
#else
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("MIDI file not found: " + path);
            yield break;
        }
        using (var stream = System.IO.File.OpenRead(path))
        {
            midiFile = MidiFile.Read(stream);
            midiFileTable[beatmapData] = midiFile;
        }
#endif
    }
}
