using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SetupProject : Editor
{
    [MenuItem("Geographical Adventures/1. Setup UI and Mechanics")]
    public static void SetupScene()
    {
        // 1. Setup UI
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null) {
            canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        UIManager uiManager = canvasGO.GetComponent<UIManager>();
        if (uiManager == null) uiManager = canvasGO.AddComponent<UIManager>();

        // Destroy old UI if exists
        foreach (Transform child in canvasGO.transform)
        {
            if (child.name == "ExplorationHUD" || child.name == "CarryingHUD")
                DestroyImmediate(child.gameObject);
        }

        // Create Exploration HUD
        GameObject exploreHUD = new GameObject("ExplorationHUD");
        exploreHUD.transform.SetParent(canvasGO.transform, false);
        CanvasGroup exploreGroup = exploreHUD.AddComponent<CanvasGroup>();
        RectTransform exploreRt = exploreHUD.GetComponent<RectTransform>();
        exploreRt.anchorMin = new Vector2(0, 1);
        exploreRt.anchorMax = new Vector2(0, 1);
        exploreRt.pivot = new Vector2(0, 1);
        exploreRt.anchoredPosition = new Vector2(20, -20);
        exploreRt.sizeDelta = new Vector2(400, 200);

        Image exploreBg = exploreHUD.AddComponent<Image>();
        exploreBg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f); // Futuristic Dark Glass

        GameObject regionTxt = new GameObject("RegionText");
        regionTxt.transform.SetParent(exploreHUD.transform, false);
        Text rText = regionTxt.AddComponent<Text>();
        rText.text = "Region: Sector 7";
        rText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        rText.color = new Color(0.2f, 0.8f, 1f, 1f); // Cyan
        rText.fontSize = 24;
        RectTransform rRt = regionTxt.GetComponent<RectTransform>();
        rRt.anchoredPosition = new Vector2(10, -10);

        GameObject compass = new GameObject("Compass");
        compass.transform.SetParent(exploreHUD.transform, false);
        Image compImg = compass.AddComponent<Image>();
        compImg.color = new Color(0.2f, 0.8f, 1f, 1f);
        RectTransform compRt = compass.GetComponent<RectTransform>();
        compRt.anchoredPosition = new Vector2(50, -80);
        compRt.sizeDelta = new Vector2(50, 50);

        // Create Carrying HUD
        GameObject carryingHUD = new GameObject("CarryingHUD");
        carryingHUD.transform.SetParent(canvasGO.transform, false);
        CanvasGroup carryGroup = carryingHUD.AddComponent<CanvasGroup>();
        RectTransform carryRt = carryingHUD.GetComponent<RectTransform>();
        carryRt.anchorMin = new Vector2(1, 0.5f);
        carryRt.anchorMax = new Vector2(1, 0.5f);
        carryRt.pivot = new Vector2(1, 0.5f);
        carryRt.anchoredPosition = new Vector2(-20, 0);
        carryRt.sizeDelta = new Vector2(300, 200);

        Image carryBg = carryingHUD.AddComponent<Image>();
        carryBg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        GameObject artName = new GameObject("ArtifactName");
        artName.transform.SetParent(carryingHUD.transform, false);
        Text aText = artName.AddComponent<Text>();
        aText.text = "Artifact";
        aText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        aText.color = Color.yellow;
        aText.fontSize = 28;
        aText.alignment = TextAnchor.UpperCenter;
        RectTransform aRt = artName.GetComponent<RectTransform>();
        aRt.anchoredPosition = new Vector2(0, -20);
        aRt.sizeDelta = new Vector2(300, 50);

        GameObject artWeight = new GameObject("Weight");
        artWeight.transform.SetParent(carryingHUD.transform, false);
        Text wText = artWeight.AddComponent<Text>();
        wText.text = "Weight: 1x";
        wText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        wText.color = Color.white;
        RectTransform wRt = artWeight.GetComponent<RectTransform>();
        wRt.anchoredPosition = new Vector2(20, -80);

        GameObject artCoord = new GameObject("Coordinates");
        artCoord.transform.SetParent(carryingHUD.transform, false);
        Text cText = artCoord.AddComponent<Text>();
        cText.text = "Lat: 0 Lon: 0";
        cText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        cText.color = Color.white;
        RectTransform cRt = artCoord.GetComponent<RectTransform>();
        cRt.anchoredPosition = new Vector2(20, -120);

        uiManager.explorationGroup = exploreGroup;
        uiManager.regionText = rText;
        uiManager.compassPanel = compRt;
        
        uiManager.carryingGroup = carryGroup;
        uiManager.artifactNameText = aText;
        uiManager.weightText = wText;
        uiManager.coordinatesText = cText;

        // 2. Setup Player and Camera
        Player player = GameObject.FindObjectOfType<Player>();
        if (player != null) {
            if (player.GetComponent<PlayerInteraction>() == null)
                player.gameObject.AddComponent<PlayerInteraction>();
            
            Rigidbody prb = player.GetComponent<Rigidbody>();
            if (prb != null) prb.isKinematic = true;
            
            // Give player a simple capsule model
            if (player.transform.Find("Capsule") == null)
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.name = "Capsule";
                capsule.transform.SetParent(player.transform);
                capsule.transform.localPosition = new Vector3(0, 1, 0);
                DestroyImmediate(capsule.GetComponent<Collider>());
            }
        }

        // Create Artifact Data
        if (!Directory.Exists("Assets/Data")) Directory.CreateDirectory("Assets/Data");
        
        ArtifactData sword = AssetDatabase.LoadAssetAtPath<ArtifactData>("Assets/Data/Sword.asset");
        if (sword == null) {
            sword = ScriptableObject.CreateInstance<ArtifactData>();
            sword.artifactName = "Ancient Sword";
            sword.weight = 1.2f;
            AssetDatabase.CreateAsset(sword, "Assets/Data/Sword.asset");
        }

        ArtifactData gun = AssetDatabase.LoadAssetAtPath<ArtifactData>("Assets/Data/Gun.asset");
        if (gun == null) {
            gun = ScriptableObject.CreateInstance<ArtifactData>();
            gun.artifactName = "Laser Blaster";
            gun.weight = 1.5f;
            AssetDatabase.CreateAsset(gun, "Assets/Data/Gun.asset");
        }

        // Spawn Artifacts in Scene
        SpawnArtifact("Ancient Sword", sword, new Vector3(10, 0, 10));
        SpawnArtifact("Laser Blaster", gun, new Vector3(-10, 0, 15));

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Scene and Mechanics Setup Complete!");
    }

    static void SpawnArtifact(string name, ArtifactData data, Vector3 localOffset)
    {
        if (GameObject.Find(name) != null) return;

        Player player = GameObject.FindObjectOfType<Player>();
        Vector3 center = Vector3.zero;
        if (player != null) center = player.transform.position.normalized * 150f; 

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = center + localOffset; 
        
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.isKinematic = true; // Use programmatic gravity
        
        InteractableArtifact ia = obj.AddComponent<InteractableArtifact>();
        ia.data = data;
        
        // Ensure it's snapped to the ground
        ia.Detach(Vector3.zero, 150f);
    }
}
