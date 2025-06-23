using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Facility : MonoBehaviour
{
    [SerializeField] private GameObject iconSurface;
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private FacilityData facilityData;
    [SerializeField] private float firstRingOffset = -0.5f;
    [SerializeField] private float ringGap = 0.3f;

    public FacilityData Data => facilityData;

    public Pillar CurrentPillar { get; private set; }

    public abstract int MaxUpgradeLevel { get; }
    public abstract int MinUpgradeLevel { get; }
    public int ClampedUpgradeLevel
    {
        get
        {
            if (CurrentPillar == null)
            {
                Debug.LogError("Facility is Not Built");
                return -1;
            }

            int pillarUpgradeLevel = CurrentPillar.FacilityUpgradeLevel;
            return Mathf.Clamp(pillarUpgradeLevel, MinUpgradeLevel, MaxUpgradeLevel);
        }
    }
    private int lastUpgradeLevel = 0;

    private readonly List<GameObject> rings = new();

    protected Action<int> OnUpgradeLevelIncrease;
    protected Action<int> OnUpgradeLevelDecrease;

    private void OnDestroy()
    {
        if (CurrentPillar != null)
        {
            CurrentPillar.OnFacilityUpgradeLevelChange -= HandleUpgradeLevelChange;
            CurrentPillar.BuiltFacility = null;
            CurrentPillar = null;
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

        MoveToPillar(pillar);
        ConnectToPillar(pillar);

        ResizeToMatchPillar(pillar);
        ApplyIconTexture();

        SetHandlersOnUpgradeLevel(); // Must be called BEFORE ApplyUpgradeLevel() to set upgrade delegates
        ApplyUpgradeLevel(ClampedUpgradeLevel);

        Initialize();
    }

    public virtual void Demolish()
    {
        ApplyUpgradeLevel(0);
        Destroy(gameObject);
    }

    private void MoveToPillar(Pillar pillar)
    {
        transform.position = pillar.TopPosition;
        transform.rotation = pillar.transform.rotation;
    }

    private void ConnectToPillar(Pillar pillar)
    {
        CurrentPillar = pillar;
        pillar.BuiltFacility = this;

        pillar.OnFacilityUpgradeLevelChange += HandleUpgradeLevelChange;
    }

    private void ResizeToMatchPillar(Pillar pillar)
    {
        float diameter = pillar.Diameter;

        float localScaleMultipler = diameter / (iconSurface.transform.lossyScale.x * 10) * 0.7f;
        iconSurface.transform.localScale = new Vector3(iconSurface.transform.localScale.x * localScaleMultipler, 1f, iconSurface.transform.localScale.z * localScaleMultipler);
    }

    private void ApplyIconTexture()
    {
        var iconTexture = Data.IconTexture;
        if (iconSurface != null && iconTexture != null)
        {
            Renderer renderer = iconSurface.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = iconTexture;
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

    private void HandleUpgradeLevelChange(int upgradeLevel)
    {
        ApplyUpgradeLevel(upgradeLevel);
    }

    private void ApplyUpgradeLevel(int pillarUpgradeLevel)
    {
        int clampedUpgradeLevel = Mathf.Clamp(pillarUpgradeLevel, MinUpgradeLevel, MaxUpgradeLevel);
        AdjustRing(clampedUpgradeLevel);

        while (lastUpgradeLevel < clampedUpgradeLevel && lastUpgradeLevel < MaxUpgradeLevel)
        {
            lastUpgradeLevel++;
            OnUpgradeLevelIncrease?.Invoke(lastUpgradeLevel);
        }

        while (lastUpgradeLevel > clampedUpgradeLevel && lastUpgradeLevel > 0)
        {
            OnUpgradeLevelDecrease?.Invoke(lastUpgradeLevel);
            lastUpgradeLevel--;
        }
    }

    private void AdjustRing(int upgradeLevel)
    {
        while (rings.Count < upgradeLevel)
        {
            IncreaseRing();
        }

        while (rings.Count > 0 && rings.Count > upgradeLevel)
        {
            DecreaseRing();
        }
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

    private void SetHandlersOnUpgradeLevel()
    {
        OnUpgradeLevelIncrease += HandleUpgradeLevelIncrease;
        OnUpgradeLevelDecrease += HandleUpgradeLevelDecrease;
    }

    protected virtual void HandleUpgradeLevelIncrease(int level)
    {

    }

    protected virtual void HandleUpgradeLevelDecrease(int level)
    {

    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Act()
    {

    }
}