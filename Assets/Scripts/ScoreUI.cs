using System;
using TMPro;
using UnityEngine;


[DisallowMultipleComponent]
public class ScoreUI : MonoBehaviour
{
[SerializeField] GameStateSO gameState;
[SerializeField] TextMeshProUGUI scoreText;
[SerializeField] GameObject startPanel;
[SerializeField] GameObject gameOverPanel;

String scoreContent;


void Awake()
{
    scoreContent = scoreText.text;
}

void OnEnable()
{
gameState.OnScoreChanged += UpdateScore;
gameState.OnGameStart += ShowRun;
gameState.OnGameOver += ShowGameOver;
}


void OnDisable()
{
gameState.OnScoreChanged -= UpdateScore;
gameState.OnGameStart -= ShowRun;
gameState.OnGameOver -= ShowGameOver;
}


void Start()
{
UpdateScore(gameState != null ? gameState.Score : 0);
ShowMenu();
}


void UpdateScore(int s) => scoreText?.SetText(scoreContent, s);


void ShowMenu()
{
if (startPanel) startPanel.SetActive(true);
if (gameOverPanel) gameOverPanel.SetActive(false);
}


void ShowRun()
{
if (startPanel) startPanel.SetActive(false);
if (gameOverPanel) gameOverPanel.SetActive(false);
}


void ShowGameOver()
{
if (gameOverPanel) gameOverPanel.SetActive(true);
}
}