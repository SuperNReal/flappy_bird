using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GameRunning { get; private set; }
    public bool GameOver { get; private set; }
    public int Score { get; private set; }

    [Header("UI (TMP only)")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI scoreTMP;

    void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Time.timeScale = 1f;
        GameRunning = false;
        GameOver = false;
        Score = 0;
        UpdateScoreUI();

        if (startPanel) startPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void Begin()
    {
        if (GameRunning || GameOver) return;
        GameRunning = true;
        if (startPanel) startPanel.SetActive(false);
    }

    public void AddScore(int amount = 1)
    {
        if (!GameRunning || GameOver) return;
        Score += amount;
        UpdateScoreUI();
    }

    public void TriggerGameOver()
    {
        if (GameOver) return;
        GameOver = true;
        GameRunning = false;
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    void UpdateScoreUI()
    {
        if (scoreTMP) scoreTMP.SetText("{0}", Score);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
