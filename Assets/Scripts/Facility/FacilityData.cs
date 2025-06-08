using UnityEngine;

[CreateAssetMenu(menuName = "Facility/FacilityData")]
public class FacilityData : ScriptableObject
{
    public FacilityType facilityType;
    public Texture2D iconTexture;
    public string facilityName;
    [TextArea] public string facilityDescription;
}