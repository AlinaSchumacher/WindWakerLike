using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpSpeed;

    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isOnGround = true;
    private float targetJumpY;
    private bool jumped;

    public event EventHandler<bool> OnIsWalking;
    public event EventHandler<bool> OnIsRunning;
    public event EventHandler<bool> OnIsFalling;
    public event EventHandler OnJumped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetJumpY = 0;
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isOnGround = true;
            OnIsFalling?.Invoke(this, !isOnGround);
            jumped = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = false;
            OnIsFalling?.Invoke(this, !isOnGround);
        }
    }

    private void HandleRotation()
    {
        Vector3 currentRot = transform.forward;
        transform.forward = Vector3.Lerp(currentRot, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleMovement()
    {
        Vector3 currentPos = rb.position;
        Vector3 newPos = currentPos + moveDir * runSpeed * Time.deltaTime;

        //jump
        if (jumped)
            newPos += Vector3.up * jumpSpeed * Time.deltaTime;
        if (currentPos.y >= targetJumpY)
            jumped = false;

        rb.MovePosition(newPos);
    }

    private void OnMove(InputValue v)
    {
        Vector2 val = v.Get<Vector2>();
        Vector3 inputDir = new Vector3(val.x, 0, val.y);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        moveDir = cameraForward * val.y + cameraRight * val.x;

        // Debug.Log(val);

        if (val == new Vector2(0, 0))
        {
            OnIsRunning?.Invoke(this, false);
            OnIsWalking?.Invoke(this, false);
        }
        else
        {
            OnIsRunning?.Invoke(this, true);
        }
    }

    private void OnJump()
    {
        if (isOnGround)
        {
            OnJumped?.Invoke(this, EventArgs.Empty);
            targetJumpY = rb.position.y + jumpHeight;
            jumped = true;
        }
    }
}
