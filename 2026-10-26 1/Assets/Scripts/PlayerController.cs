using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [Header("Look")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxPitch = 85f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float stamina = 10f;
    [SerializeField] private float staminaRegenPerSecond = 2f;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashStaminaCost = 3f;

    public bool IsGameOver { get; private set; }
    public float Stamina => stamina;
    public float MaxStamina => maxStamina;

    private Rigidbody rb;
    private Vector3 input;
    private bool dashPressed;
    private bool dashing;
    private float dashTime;
    private float cooldownLeft;
    private Vector3 dashDir;
    private float pitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        stamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (IsGameOver) { input = Vector3.zero; return; }

        float x = 0f, z = 0f;
        Vector2 look = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.dKey.isPressed) x += 1f;
            if (k.aKey.isPressed) x -= 1f;
            if (k.wKey.isPressed) z += 1f;
            if (k.sKey.isPressed) z -= 1f;
            if (Mouse.current != null) look = Mouse.current.delta.ReadValue();
            if (k.leftShiftKey.wasPressedThisFrame || k.rightShiftKey.wasPressedThisFrame) dashPressed = true;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10f;
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) dashPressed = true;
#endif

        transform.Rotate(0f, look.x * mouseSensitivity, 0f);
        pitch = Mathf.Clamp(pitch - look.y * mouseSensitivity, -maxPitch, maxPitch);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        input = (transform.right * x + transform.forward * z).normalized;

        if (cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
        if (stamina < maxStamina)
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenPerSecond * Time.deltaTime);

        if (dashPressed)
        {
            dashPressed = false;
            if (!dashing && cooldownLeft <= 0f && stamina >= dashStaminaCost && input != Vector3.zero)
            {
                dashing = true;
                dashTime = 0f;
                dashDir = input;
                cooldownLeft = dashCooldown;
                stamina -= dashStaminaCost;
            }
        }
    }

    private void FixedUpdate()
    {
        var v = rb.linearVelocity;
        if (dashing)
        {
            // Half-sine velocity profile: eases in/out and covers exactly speed units.
            float distance = speed;
            float t = dashTime / dashDuration;
            float s = distance * Mathf.PI * 0.5f / dashDuration * Mathf.Sin(Mathf.PI * t);
            rb.linearVelocity = new Vector3(dashDir.x * s, v.y, dashDir.z * s);
            dashTime += Time.fixedDeltaTime;
            if (dashTime >= dashDuration) dashing = false;
            return;
        }
        rb.linearVelocity = new Vector3(input.x * speed, v.y, input.z * speed);
    }

    private void OnCollisionEnter(Collision collision) => HandleCollision(collision);

    private void OnCollisionStay(Collision collision) => HandleCollision(collision);

    private void HandleCollision(Collision collision)
    {
        if (IsGameOver || dashing || collision.collider.GetComponent<FallingSphere>() == null) return;
        IsGameOver = true;
        dashing = false;
        input = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Game Over");
    }
}
