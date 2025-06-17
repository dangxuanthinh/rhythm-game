using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectPanel : MonoBehaviour
{
    [SerializeField] private Transform songsHolder;
    [SerializeField] private SongSelectButton songSelectButtonPrefab;
    private BeatmapData selectedMap;

    private void Start()
    {
        SpawnSongs();
    }

    public void SpawnSongs()
    {
        foreach (Transform child in songsHolder)
        {
            Destroy(child.gameObject);
        }
        List<BeatmapData> maps = MapCatalog.Instance.GetMaps();
        foreach (BeatmapData map in maps)
        {
            SongSelectButton songSelectButton = Instantiate(songSelectButtonPrefab, songsHolder);
            songSelectButton.Setup(map, OnSongButtonClicked);
        }
    }

    private void OnSongButtonClicked(BeatmapData beatmapData)
    {
        selectedMap = beatmapData;
        SongManager.Instance.PlaySongPreview(beatmapData.music, beatmapData.songPreviewPoint);
    }
}
