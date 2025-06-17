using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum HitType
{
    Perfect,
    Good,
    Miss
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }

    [SerializeField] private int perfectScore = 3;
    [SerializeField] private int goodScore = 2;

    public UnityAction OnScoreChanged;

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
        Tile.OnTileDestroyed += OnTileDestroyed;
        GameManager.Instance.OnGameStart += RefreshScore;
    }

    private void OnDestroy()
    {
        Tile.OnTileDestroyed -= OnTileDestroyed;
        GameManager.Instance.OnGameStart -= RefreshScore;
    }

    public void OnTileDestroyed(Tile tile, HitType hitType)
    {
        if (hitType == HitType.Perfect)
        {
            Score += perfectScore;
            IncreaseCombo();
        }
        else if (hitType == HitType.Good)
        {
            Score += goodScore;
            IncreaseCombo();
        }
        else if (hitType == HitType.Miss)
        {
            Combo = 0; // Reset combo on miss
        }
        OnScoreChanged?.Invoke();
    }

    private void IncreaseCombo()
    {
        Combo++;
        if (MaxCombo < Combo)
        {
            MaxCombo = Combo;
        }
    }

    private void RefreshScore()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        OnScoreChanged?.Invoke();
    }
}
