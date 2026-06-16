using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public event System.Action<Camera> gameCameraUpdateComplete;
    public enum ViewMode { TopDown, LookingForward, LookingBehind, MainMenu }
    public ViewMode activeView = ViewMode.LookingForward;

    [Header("Orbit Settings")]
    public float distance = 15f;
    public bool topDownMode; // Legacy stub
    public float height = 5f;
    public float followSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;
    public float mouseSensitivity = 2f;

    [Header("References")]
    public Camera cam;
    public Player player;

    private float pitch = 20f;
    private float yaw;
    private Vector3 currentVelocity;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    public void SetActiveView(ViewMode viewMode) {
        activeView = viewMode;
    }

    public void InitView() { }

    void LateUpdate()
    {
        if (!GameController.IsState(GameState.Paused) && player != null)
        {
            // Mouse drag for orbit
            if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, -20f, 80f);
            }

            Vector3 targetPos = player.transform.position;
            Vector3 gravityUp = targetPos.normalized;

            // Prevent gimbal lock by computing relative to gravityUp
            Quaternion gravityRotation = Quaternion.FromToRotation(Vector3.up, gravityUp);
            Quaternion localRotation = Quaternion.Euler(pitch, yaw, 0f);
            Quaternion targetRotation = gravityRotation * localRotation;

            Vector3 offset = targetRotation * new Vector3(0, 0, -distance) + gravityUp * height;

            transform.position = Vector3.SmoothDamp(transform.position, targetPos + offset, ref currentVelocity, followSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-rotationSmoothTime * Time.deltaTime * 60f));

            gameCameraUpdateComplete?.Invoke(cam);
        }
    }
}