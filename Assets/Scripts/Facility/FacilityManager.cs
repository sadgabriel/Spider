using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FacilityType
{
    Dummy,
    MaxHp,
    Sprint,
    Upgrade,
    Teleport,
    Demolition,
}

[System.Serializable]
public class FacilityEntry
{
    public FacilityType type;
    public GameObject prefab;
    public FacilityData data;
}

public class FacilityManager : MonoBehaviour
{
    public static FacilityManager Instance { get; private set; }
    [SerializeField] private List<FacilityEntry> facilityEntries;

    public List<Facility> Facilities { get; private set; } = new List<Facility>();

    private Dictionary<FacilityType, GameObject> facilityPrefabMap;
    private Dictionary<FacilityType, FacilityData> facilityDataMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        facilityPrefabMap = facilityEntries.ToDictionary(entry => entry.type, entry => entry.prefab);
        facilityDataMap = facilityEntries.ToDictionary(entry => entry.type, entry => entry.data);
    }

    public Facility BuildFacility(Pillar pillar, FacilityType type)
    {
        if (pillar == null || pillar.HasFacility)
            return null;

        if (!facilityPrefabMap.TryGetValue(type, out GameObject facilityPrefab))
        {
            Debug.LogError($"No prefab found for FacilityType {type}");
            return null;
        }

        GameObject facilityGO = Instantiate(facilityPrefab, pillar.transform);
        Facility facility = facilityGO.GetComponent<Facility>();
        facility.BuildOn(pillar);

        Facilities.Add(facility);

        AudioManager.Instance.PlayBuildSfx();

        return facility;
    }
    
    public void DestroryFacility(Facility facility)
    {
        if (facility == null || !Facilities.Contains(facility))
            return;

        Facilities.Remove(facility);
        facility.Demolish();
    }

    public List<FacilityData> GetAllAvailableFacilityData()
    {
        List<FacilityData> allFacilityData = facilityDataMap.Values.ToList();
        List<FacilityType> existingUniqueFacilityTypes = Facilities.Where(facility => facility.Data.IsUnique)
                                                                  .Select(facility => facility.Data.FacilityType)
                                                                  .ToList();

        return allFacilityData.Where(facilityData => !existingUniqueFacilityTypes.Contains(facilityData.FacilityType)).ToList();
    }
}