using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    public bool IsGameOver { get; private set; }

    private Rigidbody rb;
    private Vector3 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        if (IsGameOver) { input = Vector3.zero; return; }
        float x = 0f, z = 0f;
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.dKey.isPressed) x += 1f;
            if (k.aKey.isPressed) x -= 1f;
            if (k.wKey.isPressed) z += 1f;
            if (k.sKey.isPressed) z -= 1f;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        input = new Vector3(x, 0f, z).normalized;
    }

    private void FixedUpdate()
    {
        var v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(input.x * speed, v.y, input.z * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsGameOver || collision.collider.GetComponent<FallingSphere>() == null) return;
        IsGameOver = true;
        input = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        Debug.Log("Game Over");
    }
}
