using System;
using UnityEngine;

public class PlayerLedgeInteraction : MonoBehaviour
{
    [SerializeField]
    private LayerMask climbableLayer;

    [SerializeField]
    private float coolDownTime;

    [SerializeField]
    private float maxGrabHeight;

    [SerializeField]
    private float minGrabHeight;

    [SerializeField]
    private float frontOffset = 0.4f;

    [SerializeField]
    float downOffset = 0.1f;

    [SerializeField]
    private float climbingSpeed;

    private Rigidbody rb;
    private Player player;

    private bool wasHanging;
    private float hangingResetTimer;
    private Vector3 ledgeOffset;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (wasHanging && hangingResetTimer <= 0)
        {
            wasHanging = false;
        }
        hangingResetTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (player.IsHanging())
            HandleHangingMovement();
        else if (!player.IsOnGround())
            LedgeGrab();
    }

    public Vector3 GetLedgeOffset()
    {
        return ledgeOffset;
    }

    public float GetFrontOffset()
    {
        return frontOffset;
    }

    public void SetIsHanging(bool val)
    {
        rb.useGravity = !val;

        if (val)
            rb.linearVelocity = Vector3.zero;
        else
        {
            wasHanging = true;
            hangingResetTimer = coolDownTime;
        }
    }

    public void LedgeGrab()
    {
        if (wasHanging)
            return;

        if (
            !TestForLedge(
                transform.position,
                transform.forward,
                frontOffset,
                out RaycastHit downHit,
                out RaycastHit fwdHit
            )
        )
            return;

        Vector3 hangigPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
        hangigPos += ledgeOffset;

        transform.position = hangigPos;
        player.SetIsHanging(true);
        transform.forward = -fwdHit.normal;
    }

    private void HandleHangingMovement()
    {
        Vector3 newPos = player.CalculateNewPosition(climbingSpeed);

        if (
            !TestForLedge(
                newPos,
                transform.forward,
                frontOffset + 0.1f,
                out RaycastHit downHit,
                out RaycastHit fwdHit
            )
        )
            return;

        newPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
        newPos += ledgeOffset;

        rb.MovePosition(newPos);
    }

    public bool TestForLedge(
        Vector3 position,
        Vector3 fwd,
        float fwdOffset,
        out RaycastHit downHit,
        out RaycastHit fwdHit
    )
    {
        downHit = new RaycastHit();
        fwdHit = new RaycastHit();

        Vector3 lineDownStart = (position + Vector3.up * maxGrabHeight) + fwd * fwdOffset;
        Vector3 lineDownEnd = (position + Vector3.up * minGrabHeight) + fwd * fwdOffset;

        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, climbableLayer);
        Debug.DrawLine(lineDownStart, lineDownEnd, Color.white);

        //stop if nothing hit
        if (!downHit.collider)
            return false;

        Vector3 lineFwdStart = new Vector3(position.x, downHit.point.y - downOffset, position.z);
        Vector3 lineFwdEnd =
            new Vector3(position.x, downHit.point.y - downOffset, position.z) + fwd;

        Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, climbableLayer);
        Debug.DrawLine(fwdHit.point, fwdHit.point + fwdHit.normal, Color.red, 0.5f);

        ledgeOffset = transform.forward * -frontOffset + transform.up * -minGrabHeight;

        return (fwdHit.collider);
    }
}
