using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const String GROUND_TAG = "Ground";

    [Header("Default Movement")]
    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private Collider defaultCollider;

    [SerializeField]
    private float rotateSpeed;

    [Header("Jumping")]
    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpSpeed;

    [Header("Crouching")]
    [SerializeField]
    private float crouchWalkSpeed;

    [SerializeField]
    private Collider crouchCollider;

    [Header("Hanging")]
    [SerializeField]
    private float climbingSpeed;

    public enum MovementState
    {
        Default,
        Crouching,
    }

    private Rigidbody rb;
    private Vector3 moveDir;
    private Vector3 moveDirRaw;
    private PlayerLedgeInteraction pli;
    private bool isOnGround;
    private bool isHanging;
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
        pli = GetComponent<PlayerLedgeInteraction>();

        pli.OnIsHanging += PLI_OnIsHanging;

        crouchCollider.enabled = false;
        targetJumpY = 0;
    }

    private void FixedUpdate()
    {
        RelateToCamera();

        if (isHanging)
        {
            HandleHangingMovement();
            return;
        }

        if (!isOnGround)
            pli.LedgeGrab();

        HandleRotation();
        HandleMovement();
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

    private void HandleHangingMovement()
    {
        Vector3 currentPos = rb.position;
        Vector3 newPos = currentPos + moveDir * climbingSpeed * Time.deltaTime;

        if (
            !pli.TestForLedge(
                newPos,
                transform.forward,
                pli.GetFrontOffset() + 0.1f,
                out RaycastHit downHit,
                out RaycastHit fwdHit
            )
        )
            return;

        newPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
        newPos += pli.GetLedgeOffset();

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
        if (isHanging)
        {
            pli.SetIsHanging(false);
            return;
        }

        if (isOnGround)
        {
            SetStateCrouching(!IsCrouching());
        }
    }

    private void PLI_OnIsHanging(object sender, bool isHanging)
    {
        this.isHanging = isHanging;
        if (isHanging)
            jumped = false;
    }
}
