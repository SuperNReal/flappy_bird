using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PipeMover : MonoBehaviour
{
    [SerializeField] float speed = 2.5f;
    [SerializeField] float destroyX = -10f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Kinematic in all Unity versions
#if UNITY_6000_0_OR_NEWER
        rb.bodyType = RigidbodyType2D.Kinematic;
#else
        rb.isKinematic = true;
#endif
        rb.gravityScale = 0f;
        SetVel(Vector2.zero);
    }

    void Update()
    {
        // Run/stop based on GameManager (works even if there isn't one)
        if (ShouldRun())
            SetVel(Vector2.left * speed);
        else
            SetVel(Vector2.zero);

        // Cleanup when off-screen
        if (transform.position.x < destroyX)
            Destroy(gameObject);
    }

    bool ShouldRun()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.GameRunning && !GameManager.Instance.GameOver;
        return true; // no GameManager: always move
    }

    // ---- linear velocity helper (Unity 6+) with fallback ----
#if UNITY_6000_0_OR_NEWER
    void SetVel(Vector2 v) => rb.linearVelocity = v;
#else
    void SetVel(Vector2 v) => rb.velocity = v;
#endif
}
