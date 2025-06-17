using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapCatalog : MonoBehaviour
{
    public static MapCatalog Instance;

    private List<BeatmapData> maps = new List<BeatmapData>();

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

    }
    public void LoadMaps()
    {
        maps = Resources.LoadAll<BeatmapData>("Beatmaps").ToList();
    }

    public List<BeatmapData> GetMaps()
    {
        return new List<BeatmapData>(maps);
    }
}
