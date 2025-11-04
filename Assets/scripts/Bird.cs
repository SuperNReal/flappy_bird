using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem; // Keyboard, Mouse, Touchscreen
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bird : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] float flapForce = 5f;
    [SerializeField] float maxFallSpeed = -12f;

    [Header("Tilt")]
    [SerializeField] float tiltFactor = 6f;     // degrees per unit of vertical velocity
    [SerializeField] float tiltLerp = 10f;      // how quickly the bird rotates toward target tilt
    [SerializeField] float maxUpTilt = 35f;
    [SerializeField] float maxDownTilt = -80f;

    [Header("Behavior")]
    [SerializeField] bool startOnFirstInput = true;

    Rigidbody2D rb;
    bool alive = true;
    float initialGravity;
    bool localGameBegan = false; // used if GameManager is present but not required

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravity = Mathf.Max(1.5f, rb.gravityScale);
        if (startOnFirstInput) rb.gravityScale = 0f; // no gravity until first tap
    }

    void Update()
    {
        // Start game on first input
        if (!IsGameRunning() && alive && WasPressedThisFrame())
        {
            BeginGame();
            rb.gravityScale = initialGravity;
            Flap();
        }
        // Flap while running
        else if (IsGameRunning() && alive && WasPressedThisFrame())
        {
            Flap();
        }

        // Clamp fall speed for stability
        if (alive && rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);

        // Smooth tilt based on vertical speed
        if (alive)
        {
            float targetZ = Mathf.Clamp(rb.linearVelocity.y * tiltFactor, maxDownTilt, maxUpTilt);
            float z = Mathf.LerpAngle(transform.eulerAngles.z, targetZ, Time.deltaTime * tiltLerp);
            transform.rotation = Quaternion.Euler(0f, 0f, z);
        }
    }

    bool IsGameRunning()
    {
#if UNITY_2020_1_OR_NEWER
        // Safely handle absence of GameManager
        if (GameManager.Instance != null)
            return GameManager.Instance.GameRunning && !GameManager.Instance.GameOver;
#endif
        return localGameBegan;
    }

    void BeginGame()
    {
#if UNITY_2020_1_OR_NEWER
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Begin();
            return;
        }
#endif
        localGameBegan = true;
    }

    void Flap()
    {
        // zero-out vertical speed for crisp, consistent jumps
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D _)
    {
        if (!alive) return;
        alive = false;

#if UNITY_2020_1_OR_NEWER
        if (GameManager.Instance != null)
            GameManager.Instance.TriggerGameOver();
#endif
        // If no GameManager, you could disable controls here or show UI via another script.
    }

    bool WasPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        return false;
#else
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
        return false;
#endif
    }
}
