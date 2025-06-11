using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

abstract class Facility : MonoBehaviour
{
    [SerializeField] private GameObject iconSurface;
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private FacilityData facilityData;
    [SerializeField] private float firstRingOffset = -0.5f;
    [SerializeField] private float ringGap = 0.3f;

    public Texture2D IconTexture
    {
        get => facilityData?.iconTexture;
    }

    public string FacilityName
    {
        get => facilityData?.facilityName;
    }

    public string FacilityDescription
    {
        get => facilityData?.facilityDescription;
    }

    private List<GameObject> rings = new List<GameObject>();

    public Pillar CurrentPillar { get; set; }

    public int UpgradeLevel
    {
        get
        {
            if (CurrentPillar != null)
            {
                return CurrentPillar.FacilityUpgradeLevel;
            }
            else
            {
                Debug.LogError("Facility is Not On Pillar");
                return -1;
            }

        }

        set
        {
            CurrentPillar.FacilityUpgradeLevel = value;
        }
    }

    public void BuildOn(Pillar pillar)
    {
        if (pillar == null)
        {
            Debug.LogError("Cannot install facility on a null pillar.");
            return;
        }
        if (pillar.HasFacility)
        {
            Debug.LogError("Pillar already has a facility installed.");
            return;
        }
        if (CurrentPillar != null)
        {
            Debug.LogError("Facility is already installed on a pillar.");
            return;
        }

        transform.position = pillar.TopPosition;
        transform.rotation = pillar.transform.rotation;

        CurrentPillar = pillar;
        pillar.HasFacility = true;

        ResizeToMatchPillar(pillar);
        ApplyIconTexture();
        pillar.OnFacilityUpgradeLevelChange += HandleUpgradeLevelChange;

        Initialize();
    }

    private void ResizeToMatchPillar(Pillar pillar)
    {
        float diameter = pillar.Diameter;

        float localScaleMultipler = diameter / (iconSurface.transform.lossyScale.x * 10) * 0.7f;
        iconSurface.transform.localScale = new Vector3(iconSurface.transform.localScale.x * localScaleMultipler, 1f, iconSurface.transform.localScale.z * localScaleMultipler);
    }

    private void ApplyIconTexture()
    {
        if (iconSurface != null && IconTexture != null)
        {
            Renderer renderer = iconSurface.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = IconTexture;
            }
            else
            {
                Debug.LogError("Icon surface does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Icon surface or icon texture is not set.");
        }
    }

    private void HandleUpgradeLevelChange(int value)
    {
        while (rings.Count < value)
        {
            IncreaseRing();
        }

        while (rings.Count > 0 && rings.Count > value)
        {
            DecreaseRing();
        }

        ApplyUpgradeLevel(value);
    }

    private void IncreaseRing()
    {
        Vector3 firstRingPosition = CurrentPillar.TopPosition + firstRingOffset * CurrentPillar.transform.up;
        Vector3 ringPosition = firstRingPosition - rings.Count * ringGap * CurrentPillar.transform.up;

        GameObject ring = Instantiate(ringPrefab, ringPosition, CurrentPillar.transform.rotation, transform);
        float ringDiameter = CurrentPillar.Diameter + 0.01f;

        float localScaleMultiplier = ringDiameter / ring.transform.lossyScale.x;

        ring.transform.localScale = new Vector3(ring.transform.localScale.x * localScaleMultiplier, ring.transform.localScale.y, ring.transform.localScale.z * localScaleMultiplier);

        rings.Add(ring);
    }

    private void DecreaseRing()
    {
        if (rings.Count == 0) return;

        GameObject ring = rings[rings.Count - 1];
        rings.RemoveAt(rings.Count - 1);
        Destroy(ring);
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void ApplyUpgradeLevel(int value)
    {

    }

    protected virtual void Act()
    {

    }
}