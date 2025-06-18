using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private RectTransform texts;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [SerializeField] private Button exitButton;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameOver += ShowGameOver;
        GameManager.Instance.OnGameVictory += ShowGameVictory;

        exitButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.LoadScene("MapSelect");
        });
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= ShowGameOver;
        GameManager.Instance.OnGameVictory -= ShowGameVictory;
        texts.DOKill();
    }

    private void ShowGameVictory()
    {
        gameOverText.text = "WELL DONE";
        ShowUI();
    }

    public void ShowGameOver()
    {
        gameOverText.text = "GAME OVER";
        ShowUI();
    }  
    
    private void ShowUI()
    {
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });

        texts.anchoredPosition = new Vector2(0f, -150f);
        texts.DOAnchorPosY(0f, 0.4f).SetEase(Ease.OutBack);

        scoreText.text = $"Score: {ScoreManager.Instance.Score}";
        comboText.text = $"Combo: {ScoreManager.Instance.MaxCombo}";
    }
}
