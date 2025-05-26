using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const String GROUND_TAG = "Ground";

    [SerializeField]
    private CapsuleCollider playerCollider;

    [SerializeField]
    private float defaultColliderRadius;

    [SerializeField]
    private float defaultColliderHeight;

    [SerializeField]
    private Vector3 defaultColliderOffset;

    [SerializeField]
    private float colliderWarpSpeed;

    public event EventHandler<bool> OnIsRunning;
    public event EventHandler<bool> OnIsFalling;
    public event EventHandler<bool> OnIsCrouching;
    public event EventHandler OnJumped;

    private bool isHanging;
    private bool isOnGround;
    private bool isCrouching;
    private bool jumped;
    private bool isRunning;

    private PlayerLedgeInteraction pli;
    private PlayerMovement pm;

    private void Awake()
    {
        TryGetComponent<PlayerLedgeInteraction>(out pli);
        TryGetComponent<PlayerMovement>(out pm);
    }

    private void Update()
    {
        if (!isCrouching)
            SetUpDefaultCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
        {
            SetJumped(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
        {
            SetIsOnGround(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GROUND_TAG)
        {
            SetIsOnGround(false);
        }
    }

    public CapsuleCollider GetPlayerCollider()
    {
        return playerCollider;
    }

    public float GetColliderWarpSpeed()
    {
        return colliderWarpSpeed;
    }

    public bool IsHanging()
    {
        return isHanging;
    }

    public bool IsOnGround()
    {
        return isOnGround;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public bool DidAJump()
    {
        return jumped;
    }

    public bool IsMoving()
    {
        return isRunning;
    }

    private void SetUpDefaultCollider()
    {
        if (playerCollider.radius > defaultColliderRadius)
        {
            float currentRadius = playerCollider.radius;
            playerCollider.radius = Mathf.Lerp(
                currentRadius,
                defaultColliderRadius,
                Time.deltaTime * colliderWarpSpeed
            );
        }
        if (playerCollider.height < defaultColliderHeight)
        {
            float currentHeight = playerCollider.height;
            playerCollider.height = Mathf.Lerp(
                currentHeight,
                defaultColliderHeight,
                Time.deltaTime * colliderWarpSpeed
            );
        }
        if (playerCollider.center != defaultColliderOffset)
        {
            Vector3 currentOffset = playerCollider.center;
            playerCollider.center = Vector3.Lerp(
                currentOffset,
                defaultColliderOffset,
                Time.deltaTime * colliderWarpSpeed
            );
        }
    }

    public void SetIsRunning(bool isRunning)
    {
        this.isRunning = isRunning;
        OnIsRunning?.Invoke(this, isRunning);
    }

    public void SetIsHanging(bool isHanging)
    {
        this.isHanging = isHanging;
        pli.SetIsHanging(isHanging);

        if (isHanging)
            SetJumped(false);
    }

    public void SetIsOnGround(bool isOnGround)
    {
        this.isOnGround = isOnGround;
        OnIsFalling?.Invoke(this, !isOnGround);
    }

    public void SetIsCrouching(bool isCrouching)
    {
        this.isCrouching = isCrouching;
        OnIsCrouching?.Invoke(this, isCrouching);
    }

    public void SetJumped(bool jumped)
    {
        this.jumped = jumped;
        if (jumped)
            OnJumped?.Invoke(this, EventArgs.Empty);

        if (isCrouching)
            SetIsCrouching(false);
    }

    public Vector3 CalculateNewPosition(float speed) => pm.CalcNewPosition(speed);

    public void GrabLedge(Vector3 hangingPos, Vector3 forward) =>
        pli.LedgeGrab(hangingPos, forward);
}
