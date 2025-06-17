using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GameOver { get; private set; }

    public UnityAction OnGameOver;
    public UnityAction OnGameStart;

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
        float timeScale = 1f;
        DOTween.To(() => timeScale, x => timeScale = x, 0.3f, 1f);
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
