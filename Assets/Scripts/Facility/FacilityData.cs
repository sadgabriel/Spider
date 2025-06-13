using UnityEngine;

[CreateAssetMenu(menuName = "Facility/FacilityData")]
class FacilityData : ScriptableObject
{
    public FacilityType facilityType;
    public PillarSize size;
    public bool IsUnique;
    public Texture2D iconTexture;
    public string facilityName;
    [TextArea] public string facilityDescription;
}