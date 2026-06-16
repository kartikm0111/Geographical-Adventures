using UnityEngine;

public class Player : MonoBehaviour
{
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
