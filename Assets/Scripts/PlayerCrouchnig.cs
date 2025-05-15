using UnityEngine;

public class PlayerCrouchnig : MonoBehaviour
{
    [SerializeField]
    private float crouchColliderRadius;

    [SerializeField]
    private float crouchColliderHeight;

    [SerializeField]
    private Vector3 crouchColliderOffset;

    private Player player;
    private CapsuleCollider playerCollider;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        playerCollider = player.GetPlayerCollider();
    }

    private void Update()
    {
        if (player.IsCrouching())
            SetUpCrouchCollider();
    }

    public void SetUpCrouchCollider()
    {
        if (playerCollider.radius < crouchColliderRadius)
        {
            float currentRadius = playerCollider.radius;
            playerCollider.radius = Mathf.Lerp(
                currentRadius,
                crouchColliderRadius,
                Time.deltaTime * player.GetColliderWarpSpeed()
            );
        }
        if (playerCollider.height > crouchColliderHeight)
        {
            float currentHeight = playerCollider.height;
            playerCollider.height = Mathf.Lerp(
                currentHeight,
                crouchColliderHeight,
                Time.deltaTime * player.GetColliderWarpSpeed()
            );
        }
        if (playerCollider.center != crouchColliderOffset)
        {
            Vector3 currentOffset = playerCollider.center;
            playerCollider.center = Vector3.Lerp(
                currentOffset,
                crouchColliderOffset,
                Time.deltaTime * player.GetColliderWarpSpeed()
            );
        }
    }

    private void OnCrouch()
    {
        if (player.IsHanging())
        {
            player.SetIsHanging(false);
            return;
        }

        if (player.IsOnGround())
        {
            player.SetIsCrouching(!player.IsCrouching());
        }
    }
}
