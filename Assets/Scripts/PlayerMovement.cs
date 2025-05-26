using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float crouchWalkSpeed;

    private Player player;
    private Rigidbody rb;
    private Vector3 moveDir;
    private Vector3 moveDirRaw;

    // public event EventHandler<bool> OnIsWalking;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RelateToCamera();

        if (player.IsHanging())
            return;

        HandleRotation();
        HandleMovement();
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
        Vector3 lookAt = Vector3.Lerp(currentRot, moveDir, Time.deltaTime * rotateSpeed);

        Quaternion rotate = Quaternion.LookRotation(lookAt, Vector3.up);
        rb.rotation = rotate;
    }

    private void HandleMovement()
    {
        Vector3 newPos;
        float speedVal;

        if (player.IsCrouching())
        {
            speedVal = crouchWalkSpeed;
        }
        else
        {
            speedVal = runSpeed;
        }

        newPos = CalcNewPosition(speedVal);

        rb.MovePosition(newPos);
    }

    public Vector3 CalcNewPosition(float speed)
    {
        return rb.position + moveDir * speed * Time.deltaTime;
    }

    private void OnMove(InputValue v)
    {
        Vector2 val = v.Get<Vector2>();
        moveDirRaw = new Vector3(val.x, 0, val.y);

        if (val == new Vector2(0, 0))
        {
            player.SetIsRunning(false);
            // OnIsWalking?.Invoke(this, false);
        }
        else
            player.SetIsRunning(true);
    }
}
