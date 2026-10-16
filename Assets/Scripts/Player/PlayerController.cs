using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    public float maxSpeed = 8f;
    public float drag = 0.5f;

    Vector2 moveInput;
    bool thrusting;
    Rigidbody2D rb;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        thrusting = value.isPressed;
    }

    void Update()
    {
        float rotation = -moveInput.x;
        transform.Rotate(0, 0, rotation * rotationSpeed * Time.deltaTime);

        if (moveInput.y > 0.1f)
            thrusting = true;
        else if (moveInput.y < -0.1f)
            thrusting = false;

        bool moving = thrusting || moveInput.sqrMagnitude > 0.01f;
        if (animator != null)
        {
            animator.SetBool("Move", moving);
            if (moving) animator.SetTrigger("Move");
        }
    }

    void FixedUpdate()
    {
        if (thrusting)
        {
            rb.AddForce(transform.up * moveSpeed, ForceMode2D.Force);
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        }
        else
        {
            rb.linearVelocity *= (1f - drag * Time.fixedDeltaTime);
        }
    }

    public void Setup(ShipData data)
    {
        moveSpeed = data.moveSpeed * 3f;
        rotationSpeed = data.rotationSpeed;
        maxSpeed = data.moveSpeed * 1.5f;
    }
}
