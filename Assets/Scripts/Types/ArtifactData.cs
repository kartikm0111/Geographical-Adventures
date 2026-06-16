using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Geographical Adventures/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    [TextArea(3, 10)]
    public string description;
    public float weight = 1f; // 1 = normal speed, >1 = slower speed, <1 = faster speed
    public Sprite icon;
    public Vector3 holdOffset = new Vector3(0, 0.5f, 0.5f);
}
