using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float rotateSpeed = 10f;
    private Rigidbody rb;
    private Vector3 moveDir;

    public event EventHandler<bool> IsWalking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        Vector3 currentRot = transform.forward;
        transform.forward = Vector3.Lerp(currentRot, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleMovement()
    {
        Vector3 currentPos = rb.position;
        Vector3 newPos = currentPos + moveDir * moveSpeed * Time.deltaTime;

        rb.MovePosition(newPos);
    }

    private void OnMove(InputValue v)
    {
        Vector2 val = v.Get<Vector2>();
        moveDir = new Vector3(val.x, 0, val.y);

        if (val == new Vector2(0, 0))
        {
            IsWalking?.Invoke(this, false);
        }
        else
        {
            IsWalking?.Invoke(this, true);
        }
    }
}
