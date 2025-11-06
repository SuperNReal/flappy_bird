using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem; // Keyboard, Mouse, Touchscreen
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Bird : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] GameStateSO gameState;
    
    [Header("Flight")]
    [SerializeField] float flapForce = 5f;
    [SerializeField] float maxFallSpeed = -12f;

    [Header("Tilt")]
    [SerializeField] float tiltFactor = 6f;     // degrees per unit of vertical velocity
    [SerializeField] float tiltLerp = 10f;      // how quickly the bird rotates toward target tilt
    [SerializeField] float maxUpTilt = 35f;
    [SerializeField] float maxDownTilt = -80f;


    private Animator anim;
    Rigidbody2D rb;

    float initialGravity;
    bool alive = false;
    bool controlEnabled;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initialGravity = rb.gravityScale;
        this.HandleGameOver();
    }    

    void Update()
    {
        if (controlEnabled)
        {
            if (WasPressedThisFrame())
            {
                rb.gravityScale = initialGravity;
                Flap();
            }

            // Clamp fall speed for stability
            if (this.rb.linearVelocity.y < maxFallSpeed)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);

            float targetZ = Mathf.Clamp(rb.linearVelocity.y * tiltFactor, maxDownTilt, maxUpTilt);
            float z = Mathf.LerpAngle(transform.eulerAngles.z, targetZ, Time.deltaTime * tiltLerp);
            transform.rotation = Quaternion.Euler(0f, 0f, z);
        }
        

        if (this.rb.linearVelocity.y < 0)
        {
            anim.SetBool("is_moving_up", false);
        } else
        {
            anim.SetBool("is_moving_up", true);
        }
    }

    public void HandleGameOver()
    {
        controlEnabled = false;
        transform.position = new Vector3(-5.02f, 2.08f, 0f);
        rb.gravityScale = initialGravity;
        rb.gravityScale = 0f;
    }

    public void begin()
    {
        controlEnabled = true;
        transform.position = new Vector3(-5.02f, 2.08f, 0f);
        rb.gravityScale = initialGravity;
        rb.gravityScale = 0f;
    }
    
    void Flap()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D _)
    {
        if (!gameState.GameOver) gameState.EndGame();
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
