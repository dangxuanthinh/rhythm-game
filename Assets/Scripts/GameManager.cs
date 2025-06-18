using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GameOver { get; private set; }

    public UnityAction OnGameOver;
    public UnityAction OnGameStart;
    public UnityAction OnGameVictory;

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
        Tile.OnTileMissed += EndGame;
        SongManager.Instance.OnSongFinished += GameVictory;

        Application.targetFrameRate = 120;
    }

    private void OnDestroy()
    {
        Tile.OnTileMissed -= EndGame;
        SongManager.Instance.OnSongFinished -= GameVictory;
    }

    private void GameVictory()
    {
        OnGameVictory?.Invoke();
    }

    public void EndGame()
    {
        if (GameOver) return;
        GameOver = true;
        OnGameOver?.Invoke();
    }

    public void StartGame(BeatmapData beatmapData)
    {
        OnGameStart?.Invoke();
        SongManager.Instance.InitializeMap(beatmapData);
        SceneLoader.Instance.LoadScene("Gameplay");
        GameOver = false;
    }
}
