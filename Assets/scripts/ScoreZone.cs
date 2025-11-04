using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    bool counted = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (counted) return;
        if (other.GetComponent<Bird>() != null)
        {
            counted = true;
            GameManager.Instance.AddScore(1);
        }
    }
}
