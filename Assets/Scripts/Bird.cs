using Unity.VisualScripting;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem; // Keyboard, Mouse, Touchscreen
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Bird : MonoBehaviour, IRestartable
{
    [Header("Code Stuff")]
    [SerializeField] GameStateSO gameState;

    [Header("SFX")]
    [SerializeField] AudioClip sfxFly;
    [SerializeField] AudioClip sfxFail;
    
    [Header("Flight")]
    [SerializeField] float flapForce = 5f;
    [SerializeField] float maxFallSpeed = -12f;

    [Header("Tilt")]
    [SerializeField] float tiltFactor = 6f;
    [SerializeField] float tiltLerp = 10f;
    [SerializeField] float maxUpTilt = 35f;
    [SerializeField] float maxDownTilt = -80f;


    private Animator anim;
    Rigidbody2D rb;
    Vector3 original_pos;

    float initialGravity;
    bool controlEnabled;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        initialGravity = rb.gravityScale;
        original_pos = transform.position;
        rb.gravityScale = 0f;
    }    

    void Update()
    {
        if (!controlEnabled) return;

        if (WasPressedThisFrame())
        {
            rb.gravityScale = initialGravity;
            Flap();
        }

        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);

        float targetZ = Mathf.Clamp(rb.linearVelocity.y * tiltFactor, maxDownTilt, maxUpTilt);
        float z = Mathf.LerpAngle(transform.eulerAngles.z, targetZ, Time.deltaTime * tiltLerp);
        transform.rotation = Quaternion.Euler(0f, 0f, z);

        Camera cam = Camera.main;
        if (transform.position.y <= cam.transform.position.y - cam.orthographicSize & rb.linearVelocityY < 0) Flap(2);
        if (transform.position.y >= cam.transform.position.y + cam.orthographicSize & rb.linearVelocityY > 0) Flap(-2);
        

        if (rb.linearVelocity.y > 0)
        {
            anim.SetBool("is_moving_up", true);
        } else
        {
            anim.SetBool("is_moving_up", false);
        }
    }

    void OnEnable()
    {
        gameState.OnGameStart += HandleStart;
        gameState.OnGameOver += HandleGameOver;
    }
    void OnDisable()
    {
        gameState.OnGameStart -= HandleStart;
        gameState.OnGameOver -= HandleGameOver;
    }
    
    public void HandleStart()
    {
        controlEnabled = true;
        rb.gravityScale = initialGravity;
    }
    public void HandleGameOver()
    {
        anim.SetBool("is_dead", true);
        controlEnabled = false;
    }
    public void OnRestart()
    {
        anim.SetBool("is_dead", false);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;

        transform.position = original_pos;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    } 
    
    void Flap(float power = 1)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * flapForce * power, ForceMode2D.Impulse);
        gameState.PlaySound(sfxFly, transform);
    }

    void OnCollisionEnter2D(Collision2D _)
    {
        if (gameState.GameOver) return;
        gameState.EndGame();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y*2);
        gameState.PlaySound(sfxFail, transform);
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
