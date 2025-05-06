using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const String IS_WALKING = "IsWalking";

    [SerializeField]
    private Animator animator;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = transform.GetComponentInParent<PlayerMovement>();

        playerMovement.IsWalking += PlayerMovement_IsWalking;
    }

    private void PlayerMovement_IsWalking(object sender, bool isWalking)
    {
        animator.SetBool(IS_WALKING, isWalking);
    }
}
