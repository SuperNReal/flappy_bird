using UnityEngine;
using UnityEngine.SceneManagement;


[DisallowMultipleComponent]
public class GameFlowController : MonoBehaviour
{
[Header("State Asset")]
[SerializeField] GameStateSO gameState;


[Header("UI (optional)")]
[SerializeField] GameObject startPanel;
[SerializeField] GameObject gameOverPanel;


void Awake()
{
// Ensure normal time & fresh state (but DON'T subscribe here)
Time.timeScale = 1f;
gameState.ResetRuntime();


if (startPanel) startPanel.SetActive(true);
if (gameOverPanel) gameOverPanel.SetActive(false);
}


void OnEnable()
{
if (!gameState) return;
// Subscribe with named handlers so we can cleanly unsubscribe
gameState.OnGameStart += HandleGameStart;
gameState.OnGameOver += HandleGameOver;
gameState.OnGameRestart += RestartSoft;
gameState.OnPauseChanged += HandlePauseChanged;
}


void OnDisable()
{
if (!gameState) return;
// Always unsubscribe to avoid duplicate calls/memory leaks
gameState.OnGameStart -= HandleGameStart;
gameState.OnGameOver -= HandleGameOver;
gameState.OnGameRestart -= RestartSoft;
gameState.OnPauseChanged -= HandlePauseChanged;
}


void HandleGameStart()
{
if (startPanel) startPanel.SetActive(false);
if (gameOverPanel) gameOverPanel.SetActive(false);
}


void HandleGameOver()
{
if (gameOverPanel) gameOverPanel.SetActive(true);
}


void HandlePauseChanged(bool paused)
{
Time.timeScale = paused ? 0f : 1f;
}


// --- Button hooks ---
public void StartGame() => gameState.StartGame();


public void RestartSoft()
{
// 1) normalize timescale & panels
Time.timeScale = 1f;
if (startPanel) startPanel.SetActive(true);
if (gameOverPanel) gameOverPanel.SetActive(false);


// 2) reset shared state
gameState.ResetRuntime();


// 3) tell all restartables to reset
#if UNITY_2023_1_OR_NEWER
var list = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
var list = FindObjectsOfType<MonoBehaviour>();
#endif
foreach (var mb in list)
if (mb is IRestartable r) r.OnRestart();
}


public void RestartHard()
=> SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


public void TogglePause()
=> gameState.SetPaused(!gameState.Paused);
}