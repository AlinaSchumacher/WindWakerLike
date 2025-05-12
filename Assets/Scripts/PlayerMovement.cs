using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const String GROUND_TAG = "Ground";

    [Header("Movement Settings")]
    [SerializeField]
    private LayerMask obstacleLayer;

    [Header("Default Movement")]
    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private Collider defaultCollider;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpSpeed;

    [Header("Crouching")]
    [SerializeField]
    private float crouchWalkSpeed;

    [SerializeField]
    private Collider crouchCollider;

    public enum MovementState
    {
        Default,
        Crouching,
    }

    private Rigidbody rb;
    private Vector3 moveDir;
    private Vector3 moveDirRaw;
    private bool isOnGround;
    private MovementState state = MovementState.Default;
    private float targetJumpY;
    private bool jumped;

    // public event EventHandler<bool> OnIsWalking;
    public event EventHandler<bool> OnIsRunning;
    public event EventHandler<bool> OnIsFalling;
    public event EventHandler<bool> OnIsCrouching;
    public event EventHandler OnJumped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetJumpY = 0;
    }

    private void Update() { }

    private void FixedUpdate()
    {
        RelateToCamera();
        HandleRotation();
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG) { }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
        {
            isOnGround = true;
            OnIsFalling?.Invoke(this, !isOnGround);
            jumped = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Collider other = collision.collider;
        if (other.tag == GROUND_TAG)
        {
            Debug.Log(collision.GetContact(0).point);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
        {
            isOnGround = false;
            OnIsFalling?.Invoke(this, !isOnGround);
        }
    }

    private void RelateToCamera()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        moveDir = cameraForward * moveDirRaw.z + cameraRight * moveDirRaw.x;
    }

    private void HandleRotation()
    {
        Vector3 currentRot = transform.forward;
        transform.forward = Vector3.Lerp(currentRot, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleMovement()
    {
        Vector3 currentPos = rb.position;
        Vector3 newPos;
        float speedVal;

        switch (state)
        {
            case MovementState.Crouching:
                speedVal = crouchWalkSpeed;
                break;
            default:
                speedVal = runSpeed;
                break;
        }

        newPos = currentPos + moveDir * speedVal * Time.deltaTime;

        //jump
        if (jumped)
            newPos += Vector3.up * jumpSpeed * Time.deltaTime;
        if (currentPos.y >= targetJumpY)
            jumped = false;

        rb.MovePosition(newPos);
    }

    private void SetStateCrouching(bool isCrouching, MovementState change = MovementState.Default)
    {
        crouchCollider.enabled = isCrouching;
        defaultCollider.enabled = !isCrouching;

        state = (isCrouching) ? MovementState.Crouching : change;
        OnIsCrouching?.Invoke(this, isCrouching);
    }

    public bool IsCrouching()
    {
        return state == MovementState.Crouching;
    }

    private void OnMove(InputValue v)
    {
        Vector2 val = v.Get<Vector2>();
        moveDirRaw = new Vector3(val.x, 0, val.y);

        // Debug.Log(val);

        if (val == new Vector2(0, 0))
        {
            OnIsRunning?.Invoke(this, false);
            // OnIsWalking?.Invoke(this, false);
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

            if (IsCrouching())
                SetStateCrouching(false);
        }
    }

    private void OnCrouch()
    {
        if (isOnGround)
        {
            SetStateCrouching(!IsCrouching());
        }
    }
}
