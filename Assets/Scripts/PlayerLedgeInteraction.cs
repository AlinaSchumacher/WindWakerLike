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

    private Rigidbody rb;
    private bool wasHanging;
    private float hangingResetTimer;
    private float lendgeOffset;

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

        Debug.Log(rb.linearVelocity);
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

        RaycastHit downHit;
        float fwdOffset = 0.4f;

        Vector3 lineDownStart =
            (transform.position + Vector3.up * maxGrabHeight) + transform.forward * fwdOffset;
        Vector3 lineDownEnd =
            (transform.position + Vector3.up * minGrabHeight) + transform.forward * fwdOffset;

        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, climbableLayer);
        Debug.DrawLine(lineDownStart, lineDownEnd, Color.white);

        //stop if nothing hit
        if (!downHit.collider)
            return;

        RaycastHit fwdHit;
        float downOffset = 0.1f;
        Vector3 lineFwdStart = new Vector3(
            transform.position.x,
            downHit.point.y - downOffset,
            transform.position.z
        );
        Vector3 lineFwdEnd =
            new Vector3(transform.position.x, downHit.point.y - downOffset, transform.position.z)
            + transform.forward;

        Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, climbableLayer);
        Debug.DrawLine(fwdHit.point, fwdHit.point + fwdHit.normal, Color.red, 0.5f);

        //stop if nothing hit
        if (!fwdHit.collider)
            return;

        Vector3 hangigPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
        Vector3 offset = transform.forward * -fwdOffset + transform.up * -minGrabHeight;
        hangigPos += offset;

        transform.position = hangigPos;
        SetIsHanging(true);
        transform.forward = -fwdHit.normal;
    }

    private bool TestForLedge()
    {
        RaycastHit downHit;
        float fwdOffset = 0.4f;

        Vector3 lineDownStart =
            (transform.position + Vector3.up * maxGrabHeight) + transform.forward * fwdOffset;
        Vector3 lineDownEnd =
            (transform.position + Vector3.up * minGrabHeight) + transform.forward * fwdOffset;

        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, climbableLayer);
        Debug.DrawLine(lineDownStart, lineDownEnd, Color.white);

        return (downHit.collider);
    }
}
