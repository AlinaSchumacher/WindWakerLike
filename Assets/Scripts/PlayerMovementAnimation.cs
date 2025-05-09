using System;
using UnityEngine;

public class PlayerMovementAnimation : MonoBehaviour
{
    private const String IS_WALKING = "IsWalking";
    private const String IS_RUNNING = "IsRunning";
    private const String IS_FALLING = "IsFalling";
    private const String IS_CROUCHING = "IsCrouching";
    private const String IS_SPRINTING = "IsSprinting";
    private const String IS_LANDING = "IsLanding";
    private const String JUMP = "Jump";

    [SerializeField]
    private Animator animator;

    private Transform player;

    private void Start()
    {
        player = transform.parent;

        if (player.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            // playerMovement.OnIsWalking += PlayerMovement_OnIsWalking;
            playerMovement.OnIsRunning += PlayerMovement_OnIsRunning;
            playerMovement.OnIsFalling += PlayerMovement_OnIsFalling;
            playerMovement.OnIsCrouching += PlayerMovement_OnIsCrouching;
            playerMovement.OnIsSprinting += PlayerMovement_OnIsSprinting;
            playerMovement.OnJumped += PlayerMovement_OnJump;
        }
    }

    // private void PlayerMovement_OnIsWalking(object sender, bool isWalking)
    // {
    //     animator.SetBool(IS_WALKING, isWalking);
    // }

    private void PlayerMovement_OnIsRunning(object sender, bool isRunning)
    {
        animator.SetBool(IS_RUNNING, isRunning);
    }

    private void PlayerMovement_OnIsFalling(object sender, bool isFalling)
    {
        animator.SetBool(IS_FALLING, isFalling);
        // if (!isFalling)
        //     animator.SetTrigger(IS_LANDING);
    }

    private void PlayerMovement_OnJump(object sender, EventArgs e)
    {
        animator.SetTrigger(JUMP);
    }

    private void PlayerMovement_OnIsCrouching(object sender, bool isCrouching)
    {
        animator.SetBool(IS_CROUCHING, isCrouching);
    }

    private void PlayerMovement_OnIsSprinting(object sender, bool isSprinting)
    {
        animator.SetBool(IS_SPRINTING, isSprinting);
    }
}
