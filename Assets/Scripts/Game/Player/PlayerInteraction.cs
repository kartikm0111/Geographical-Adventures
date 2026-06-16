using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRadius = 5f;
    public Transform handSocket; // Assign in editor or create dynamically
    
    private Player player;
    private InteractableArtifact currentlyCarrying;

    // Events for UI
    public static event System.Action<ArtifactData> OnArtifactPickedUp;
    public static event System.Action OnArtifactDropped;

    void Awake()
    {
        player = GetComponent<Player>();
        if (handSocket == null)
        {
            GameObject hand = new GameObject("HandSocket");
            hand.transform.SetParent(this.transform);
            hand.transform.localPosition = new Vector3(0, 1f, 1f);
            handSocket = hand.transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentlyCarrying != null)
            {
                DropArtifact();
            }
            else
            {
                TryPickupArtifact();
            }
        }
    }

    void TryPickupArtifact()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);
        InteractableArtifact closest = null;
        float minDst = float.MaxValue;

        foreach (var col in hitColliders)
        {
            InteractableArtifact artifact = col.GetComponentInParent<InteractableArtifact>();
            if (artifact != null && !artifact.isBeingCarried)
            {
                float dst = Vector3.Distance(transform.position, artifact.transform.position);
                if (dst < minDst)
                {
                    minDst = dst;
                    closest = artifact;
                }
            }
        }

        if (closest != null)
        {
            currentlyCarrying = closest;
            currentlyCarrying.AttachTo(handSocket);

            if (player != null && closest.data != null)
            {
                player.currentWeightModifier = 1f / closest.data.weight;
            }

            OnArtifactPickedUp?.Invoke(closest.data);
        }
    }

    void DropArtifact()
    {
        if (currentlyCarrying == null) return;

        float radius = player != null && player.heightSettings != null ? player.heightSettings.worldRadius : 150f;
        
        currentlyCarrying.Detach(Vector3.zero, radius);
        currentlyCarrying = null;

        if (player != null)
        {
            player.currentWeightModifier = 1f;
        }

        OnArtifactDropped?.Invoke();
    }
}
