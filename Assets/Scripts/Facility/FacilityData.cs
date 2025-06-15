using UnityEngine;

[CreateAssetMenu(menuName = "Facility/FacilityData")]
public class FacilityData : ScriptableObject
{
    public FacilityType FacilityType;
    public PillarSize Size;
    public bool IsUnique;
    public Texture2D IconTexture;
    public string Name;
    [TextArea] public string Description;
}