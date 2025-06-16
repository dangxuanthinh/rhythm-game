using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    [SerializeField] private GameObject perfectText;
    [SerializeField] private GameObject greatText;

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
        Tile.OnTileDestroyed += PlayTextAnimation;
        UpdateScore();
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        Tile.OnTileDestroyed -= PlayTextAnimation;
    }

    public void UpdateScore()
    {
        if (ScoreManager.Instance.Score == 0)
        {
            scoreText.text = "";
            comboText.text = "";
            return;
        }
        scoreText.text = ScoreManager.Instance.Score.ToString();
        comboText.text = $"<size=60>x</size>{ScoreManager.Instance.Combo}";
        AnimateText();
    }

    private void AnimateText()
    {
        scoreText.rectTransform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() => scoreText.rectTransform.DOScale(Vector3.one, 0.1f));

        comboText.rectTransform.DOKill();
        comboText.rectTransform.localScale = Vector3.zero;
        comboText.rectTransform.DOScale(Vector3.one, 0.2f).OnComplete(() => comboText.rectTransform.DOScale(Vector3.zero, 0.15f).SetDelay(0.5f));
    }

    private void PlayTextAnimation(HitType hitType)
    {
        if (hitType == HitType.Miss) return;
        perfectText.SetActive(false);
        greatText.SetActive(false);
        if (hitType == HitType.Perfect)
        {
            perfectText.SetActive(true);
        }
        else if (hitType == HitType.Good)
        {
            greatText.SetActive(true);
        }
    }
}
