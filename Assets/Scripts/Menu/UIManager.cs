using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Exploration HUD")]
    public CanvasGroup explorationGroup;
    public Text regionText;
    public RectTransform compassPanel;

    [Header("Carrying HUD")]
    public CanvasGroup carryingGroup;
    public Text artifactNameText;
    public Text weightText;
    public Text coordinatesText;

    private Player player;

    void Awake()
    {
        Instance = this;
        player = FindObjectOfType<Player>();

        PlayerInteraction.OnArtifactPickedUp += ShowCarryingHUD;
        PlayerInteraction.OnArtifactDropped += HideCarryingHUD;

        if (explorationGroup != null) explorationGroup.alpha = 1;
        if (carryingGroup != null)
        {
            carryingGroup.alpha = 0;
            carryingGroup.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        PlayerInteraction.OnArtifactPickedUp -= ShowCarryingHUD;
        PlayerInteraction.OnArtifactDropped -= HideCarryingHUD;
    }

    void Update()
    {
        if (player != null)
        {
            if (carryingGroup != null && carryingGroup.alpha > 0)
            {
                coordinatesText.text = $"Lat: {player.transform.position.x:F2} Lon: {player.transform.position.z:F2}";
            }

            if (compassPanel != null)
            {
                Vector3 north = Vector3.up;
                Vector3 playerUp = player.transform.position.normalized;
                Vector3 forwardOnSphere = Vector3.ProjectOnPlane(player.transform.forward, playerUp).normalized;
                Vector3 northOnSphere = Vector3.ProjectOnPlane(north, playerUp).normalized;
                
                if (forwardOnSphere != Vector3.zero && northOnSphere != Vector3.zero)
                {
                    float angle = Vector3.SignedAngle(forwardOnSphere, northOnSphere, playerUp);
                    compassPanel.localRotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }

    public void ShowCarryingHUD(ArtifactData data)
    {
        if (artifactNameText != null) artifactNameText.text = data.artifactName;
        if (weightText != null) weightText.text = $"Weight: {data.weight}x";
        
        if (carryingGroup != null) StartCoroutine(TransitionHUD(carryingGroup, true, true));
    }

    public void HideCarryingHUD()
    {
        if (carryingGroup != null) StartCoroutine(TransitionHUD(carryingGroup, false, true));
    }

    // --- Legacy Stub Methods to Fix Compilation Errors ---
    public void TogglePause() {}
    public void ToggleMap() {}
    // -----------------------------------------------------

    IEnumerator TransitionHUD(CanvasGroup group, bool show, bool slide)
    {
        if (show) group.gameObject.SetActive(true);
        
        float duration = 0.4f;
        float elapsed = 0f;
        float startAlpha = group.alpha;
        float endAlpha = show ? 1f : 0f;

        RectTransform rt = group.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = show ? Vector2.zero : new Vector2(500, 0); // Slide right

        if (!slide) endPos = startPos;
        if (!show) startPos = Vector2.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            if (slide && rt != null) rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        group.alpha = endAlpha;
        if (slide && rt != null) rt.anchoredPosition = endPos;

        if (!show) group.gameObject.SetActive(false);
    }
}
