using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GameOver { get; private set; }

    public UnityAction OnGameOver;

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
    }

    private void OnDestroy()
    {
        Tile.OnTileMissed -= EndGame;
    }

    public void EndGame()
    {
        if (GameOver) return;
        GameOver = true;
        OnGameOver?.Invoke();
    }

    public void StartGame(BeatmapData beatmapData)
    {
        SongManager.Instance.InitializeMap(beatmapData);
    }
}
