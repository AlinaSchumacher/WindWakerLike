using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const String IS_WALKING = "IsWalking";
    private const String IS_RUNNING = "IsRunning";

    [SerializeField]
    private Animator animator;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = transform.GetComponentInParent<PlayerMovement>();

        playerMovement.OnIsWalking += PlayerMovement_OnIsWalking;
        playerMovement.OnIsRunning += PlayerMovement_OnIsRunning;
    }

    private void PlayerMovement_OnIsWalking(object sender, bool isWalking)
    {
        animator.SetBool(IS_WALKING, isWalking);
    }

    private void PlayerMovement_OnIsRunning(object sender, bool isRunning)
    {
        animator.SetBool(IS_RUNNING, isRunning);
    }
}
