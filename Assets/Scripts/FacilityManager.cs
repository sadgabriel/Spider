using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FacilityType
{
    Dummy,
    MaxHP,
    Sprint,
    Upgrade
}

[System.Serializable]
class FacilityEntry
{
    public FacilityType type;
    public GameObject prefab;
    public FacilityData data;
}

class FacilityManager : MonoBehaviour
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
        if (pillar == null || pillar.HasFacility) return null;

        if (facilityPrefabMap.TryGetValue(type, out GameObject facilityPrefab))
        {
            GameObject facilityGO = Instantiate(facilityPrefabMap[type], pillar.transform);
            Facility facility = facilityGO.GetComponent<Facility>();

            facility.BuildOn(pillar);

            pillar.HasFacility = true;

            Facilities.Add(facility);

            return facility;
        }
        else
        {
            Debug.LogError("No Such Facility in FacilityMananger.");
            return null;
        }
    }

    public List<FacilityData> GetAllAvailableFacilityData()
    {
        List<FacilityData> allFacilityData = facilityDataMap.Values.ToList();
        List<FacilityType> ExistingUniqueFacilityType = Facilities.Where(facility => facility.Data.IsUnique)
                                                                  .Select(facility => facility.Data.facilityType)
                                                                  .ToList();
        
        return allFacilityData.Where(facilityData => !ExistingUniqueFacilityType.Contains(facilityData.facilityType)).ToList(); 
    }
}