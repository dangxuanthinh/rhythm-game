using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private int perfectScore = 3;
    [SerializeField] private int goodScore = 2;

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

    public void OnTileDestroyed(HitType hitType)
    {
        if (hitType == HitType.Perfect)
        {
            Score += perfectScore;
            Combo++;
        }
        else if (hitType == HitType.Good)
        {
            Score += goodScore;
            Combo++;
        }
        else if (hitType == HitType.Miss)
        {
            Combo = 0; // Reset combo on miss
        }
    }
}
