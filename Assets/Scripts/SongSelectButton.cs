using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SongSelectButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI songName;
    [SerializeField] private TextMeshProUGUI songDuration;
    [SerializeField] private TextMeshProUGUI artistName;
    [SerializeField] private Button playButton;
    [SerializeField] private Button previewSongButton;
    private BeatmapData beatmapData;

    public void Setup(BeatmapData beatmapData, UnityAction<BeatmapData> OnButtonClicked)
    {
        this.beatmapData = beatmapData;
        songName.text = beatmapData.songName;

        float duration = beatmapData.music.length;
        TimeSpan timeSpan = TimeSpan.FromSeconds(duration);
        songDuration.text = "Duration: " + string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        artistName.text = beatmapData.artistName;

        playButton.onClick.AddListener(SelectSongAndPlay);
        previewSongButton.onClick.AddListener(() => OnButtonClicked?.Invoke(this.beatmapData));
    }

    private void SelectSongAndPlay()
    {
        GameManager.Instance.StartGame(beatmapData);
    }
}
