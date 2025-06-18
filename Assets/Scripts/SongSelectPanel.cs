using DG.Tweening;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectPanel : MonoBehaviour
{
    [SerializeField] private Transform songsHolder;
    [SerializeField] private SongSelectButton songSelectButtonPrefab;
    [SerializeField] private Transform songSelectText;
    private BeatmapData selectedMap;
    private float beatInterval = 0.5f;
    private Coroutine pulseCoroutine;

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
        OnSongButtonClicked(maps[0]); // Preview the first song by default
    }

    private void OnSongButtonClicked(BeatmapData beatmapData)
    {
        selectedMap = beatmapData;
        SongManager.Instance.PlaySongPreview(beatmapData.music, beatmapData.songPreviewPoint);

        beatInterval = 60f / beatmapData.bpm;
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        pulseCoroutine = StartCoroutine(PulseLoop());
    }

    IEnumerator PulseLoop()
    {
        float delayToNextBeat = beatInterval - (selectedMap.bpm % beatInterval);
        yield return new WaitForSeconds(delayToNextBeat);

        while (true)
        {
            DoPulse();
            yield return new WaitForSeconds(beatInterval);
        }
    }

    private void DoPulse()
    {
        songSelectText.DOKill();
        songSelectText.DOScale(1.05f, 0.07f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
                songSelectText.DOScale(1f, 0.15f).SetEase(Ease.InQuad)
            );
    }

    private void OnDestroy()
    {
        songSelectText.DOKill();
    }
}
