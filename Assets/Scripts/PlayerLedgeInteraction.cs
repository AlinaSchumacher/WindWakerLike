using System;
using System.Collections.Generic;
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

    private Rigidbody rb;
    private bool wasHanging;
    private float hangingResetTimer;
    private Vector3 ledgeOffset;

    public event EventHandler<bool> OnIsHanging;

    private void Awake()
    {
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
        OnIsHanging?.Invoke(this, val);

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
        SetIsHanging(true);
        transform.forward = -fwdHit.normal;
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
