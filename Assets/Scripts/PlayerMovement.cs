using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const String GROUND_TAG = "Ground";

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float sprintSpeed;

    [SerializeField]
    private float crouchWalkSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpSpeed;

    private enum MovementState
    {
        Default,
        Sprinting,
        Crouching,
    }

    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isOnGround;
    private MovementState state = MovementState.Default;
    private float targetJumpY;
    private bool jumped;

    // public event EventHandler<bool> OnIsWalking;
    public event EventHandler<bool> OnIsRunning;
    public event EventHandler<bool> OnIsFalling;
    public event EventHandler<bool> OnIsCrouching;
    public event EventHandler<bool> OnIsSprinting;
    public event EventHandler OnJumped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetJumpY = 0;
    }

    private void Update() { }

    private void FixedUpdate()
    {
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
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
        Vector3 newPos;
        float speedVal;

        switch (state)
        {
            case MovementState.Crouching:
                speedVal = crouchWalkSpeed;
                break;
            case MovementState.Sprinting:
                speedVal = sprintSpeed;
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
        state = (isCrouching) ? MovementState.Crouching : change;
        OnIsCrouching?.Invoke(this, isCrouching);
    }

    public bool IsCrouching()
    {
        return state == MovementState.Crouching;
    }

    private void SetStateSprinting(bool isSprinting, MovementState change = MovementState.Default)
    {
        state = (isSprinting) ? MovementState.Sprinting : change;
        OnIsSprinting?.Invoke(this, isSprinting);
    }

    public bool IsSprinting()
    {
        return state == MovementState.Sprinting;
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
            // OnIsWalking?.Invoke(this, false);
            if (IsSprinting())
                SetStateSprinting(false);
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
            SetStateSprinting(
                false,
                (IsCrouching()) ? MovementState.Crouching : MovementState.Default
            );
        }
    }

    private void OnSprint()
    {
        if (isOnGround)
        {
            SetStateSprinting(!IsSprinting());
            SetStateCrouching(
                false,
                (IsSprinting()) ? MovementState.Sprinting : MovementState.Default
            );
        }
    }
}
