using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [SerializeField] GameStateSO gameState;
    [SerializeField] AudioClip sfxScore;
    
    bool counted = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (counted) return;
        if (other.GetComponent<Bird>() != null)
        {
            counted = true;
            gameState.AddScore(1);
            gameState.PlaySound(sfxScore, transform);
        }
    }
}
