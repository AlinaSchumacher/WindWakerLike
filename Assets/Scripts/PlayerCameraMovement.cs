using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField]
    private CinemachineFollow follow;

    [SerializeField]
    private float panSpeedH;

    [SerializeField]
    private float panSpeedV;

    [SerializeField]
    private float yMax;

    [SerializeField]
    private float yMin;

    private float offsetRadius;
    private Vector2 lookDir;
    private float currentAngleH = 0f;
    private float currentAngleV = 0f;

    // private float currentAngleV = 0f;

    private void Start()
    {
        //offset depending on Cam Position
        offsetRadius = Mathf.Abs(follow.FollowOffset.z);
    }

    private void Update()
    {
        HandleLookAround();
    }

    private void HandleLookAround()
    {
        //horizontal
        currentAngleH -= lookDir.x * panSpeedH * Time.deltaTime;
        currentAngleH %= 360f; //zwischen 0 und 360°

        //vertical
        currentAngleV -= lookDir.y * panSpeedV * Time.deltaTime;
        currentAngleV = Mathf.Clamp(currentAngleV, yMin, yMax);

        Quaternion rotation = Quaternion.Euler(currentAngleV, currentAngleH, 0f);
        Vector3 newPosition = rotation * (Vector3.back * offsetRadius);

        follow.FollowOffset = Vector3.Lerp(follow.FollowOffset, newPosition, Time.deltaTime * 10);
    }

    private void OnLookAround(InputValue v)
    {
        lookDir = v.Get<Vector2>();
    }
}
