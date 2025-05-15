using System;
using UnityEngine;

public class PlayerMovementAnimation : MonoBehaviour
{
    private const String IS_WALKING = "IsWalking";
    private const String IS_RUNNING = "IsRunning";
    private const String IS_FALLING = "IsFalling";
    private const String IS_CROUCHING = "IsCrouching";
    private const String IS_LANDING = "IsLanding";
    private const String JUMP = "Jump";

    [SerializeField]
    private Animator animator;

    private PlayerMovement playerMovement;
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();

        // playerMovement.OnIsWalking += PlayerMovement_OnIsWalking;
        player.OnIsRunning += PlayerMovement_OnIsRunning;
        player.OnIsFalling += PlayerMovement_OnIsFalling;
        player.OnIsCrouching += PlayerMovement_OnIsCrouching;
        player.OnJumped += PlayerMovement_OnJump;
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
}
