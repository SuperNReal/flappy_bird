using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Game State", fileName = "GameState")]
public class GameStateSO : ScriptableObject
{
public bool Running { get; private set; }
public bool GameOver { get; private set; }
public bool Paused { get; private set; }
public int Score { get; private set; }


public event Action OnGameStart;
public event Action OnGameOver;
public event Action<bool> OnPauseChanged;
public event Action<int> OnScoreChanged;


// Call once at play start (e.g., from GameFlowController.Awake)
public void ResetRuntime()
{
Running = false; GameOver = false; Paused = false; Score = 0;
OnScoreChanged?.Invoke(Score);
}


public void StartGame()
{
if (Running) return;
Running = true; GameOver = false; Paused = false; Score = 0;
OnGameStart?.Invoke();
OnScoreChanged?.Invoke(Score);
}


public void EndGame()
{
if (!Running || GameOver) return;
Running = false; GameOver = true;
OnGameOver?.Invoke();
}


public void SetPaused(bool paused)
{
if (Paused == paused) return;
Paused = paused;
OnPauseChanged?.Invoke(Paused);
}


public void AddScore(int delta)
{
if (!Running || GameOver) return;
Score += delta;
OnScoreChanged?.Invoke(Score);
}
}