using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpSpeed;

    private Player player;
    private Rigidbody rb;
    private float targetJumpY;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        targetJumpY = 0;
    }

    private void FixedUpdate()
    {
        if (player.DidAJump())
            HandleJump();
    }

    private void HandleJump()
    {
        rb.position += Vector3.up * jumpSpeed * Time.deltaTime;
        if (rb.position.y >= targetJumpY)
            player.SetJumped(false);
    }

    private void OnJump()
    {
        if (player.IsOnGround())
        {
            targetJumpY = rb.position.y + jumpHeight;
            player.SetJumped(true);
        }
    }
}
