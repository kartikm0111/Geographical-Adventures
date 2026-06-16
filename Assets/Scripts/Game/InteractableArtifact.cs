using UnityEngine;

public class InteractableArtifact : MonoBehaviour
{
    public ArtifactData data;
    public bool isBeingCarried { get; private set; }

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Handled manually for planetary gravity
        }
    }

    public void AttachTo(Transform handSocket)
    {
        isBeingCarried = true;
        transform.SetParent(handSocket);
        
        // Use offset from ScriptableObject
        Vector3 offset = data != null ? data.holdOffset : Vector3.zero;
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;

        if (rb != null) rb.isKinematic = true;
    }

    public void Detach(Vector3 planetCenter, float planetRadius)
    {
        isBeingCarried = false;
        transform.SetParent(null);

        // Calculate drop point on the sphere
        Vector3 gravityUp = (transform.position - planetCenter).normalized;
        
        // Raycast down to find terrain
        if (Physics.Raycast(transform.position + gravityUp * 50f, -gravityUp, out RaycastHit hit, 100f))
        {
            transform.position = hit.point;
            gravityUp = hit.normal;
        }
        else
        {
            transform.position = planetCenter + gravityUp * planetRadius;
        }

        // Align exactly to the surface
        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
    }
}
