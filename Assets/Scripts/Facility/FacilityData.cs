using UnityEngine;

[CreateAssetMenu(menuName = "FacilityData")]
public class FacilityData : ScriptableObject
{
    public FacilityType FacilityType;
    public PillarSize Size;
    public bool IsUnique;
    public Texture2D IconTexture;
    public string Name;
    public int Count;
    [TextArea] public string Description;
    [TextArea] public string BuildEffectDescription;
}