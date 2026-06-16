using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStartPoint
    {
        public CoordinateDegrees coordinate;
        public float angle;
        [Range(0, 1)] public float elevationT;
    }

    [Header("Startup Settings")]
    public bool worldIsSpherical = true;
    public Transform model;
    public Transform[] navigationLights;
    public Transform[] ailerons;
    public Transform propeller;
    public Transform packageDropPoint;
    public WorldLookup worldLookup;
    public Transform sunLight;
    public TerrainGeneration.TerrainHeightSettings heightSettings;

    [Header("Locomotion")]
    public float moveSpeed = 25f;
    public float turnSpeed = 10f;

    // --- Legacy Stub Variables to Fix Compilation Errors ---
    public bool debug_lockMovement;
    public float maxPitchAngle;
    public float currentPitchAngle;
    public float TargetSpeedT;
    public float BoostRemainingT;
    public bool BoosterIsCharging;
    public bool IsBoosting;
    public float SpeedT;
    public System.Collections.Generic.List<Vector3> positionHistory = new System.Collections.Generic.List<Vector3>();
    public float distanceTravelledKM;

    public void UpdateMovementInput(Vector2 input) {}
    public void SetPitch(float pitch) {}
    public void SetStartPos(PlayerStartPoint pos) {}
    public void DropPackage() {}
    public void AddBoost(float amount) {}
    // --------------------------------------------------------
    
    public float currentSpeed { get; private set; }
    public float currentWeightModifier = 1f;

    Transform cameraTransform;

    void Start()
    {
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!GameController.IsState(GameState.Playing)) return;

        Vector3 gravityUp = transform.position.normalized;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

        if (inputDir.magnitude >= 0.1f && cameraTransform != null)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, gravityUp).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, gravityUp).normalized;
            
            Vector3 moveDir = (camForward * inputDir.y + camRight * inputDir.x).normalized;

            Quaternion targetRot = Quaternion.LookRotation(moveDir, gravityUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            
            float speed = moveSpeed * currentWeightModifier;
            currentSpeed = speed;
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            currentSpeed = 0f;
        }

        float radius = heightSettings != null ? heightSettings.worldRadius : 150f;
        
        // Raycast to snap to terrain
        if (Physics.Raycast(transform.position + gravityUp * 500f, -gravityUp, out RaycastHit hit, 1000f))
        {
            transform.position = hit.point;
            gravityUp = hit.normal; // Align to surface normal for extra polish
        }
        else
        {
            transform.position = transform.position.normalized * radius;
        }

        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
    }
}
