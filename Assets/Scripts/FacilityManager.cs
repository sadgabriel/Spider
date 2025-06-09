using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FacilityType
{
    Test1,
    Test2,
    Test3,
}

[System.Serializable]
public class FacilityEntry
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

        GameObject facilityGO = Instantiate(facilityPrefabMap[type], pillar.transform);
        Facility facility = facilityGO.GetComponent<Facility>();

        facility.BuildOn(pillar);

        pillar.HasFacility = true;

        Facilities.Add(facility);

        facility.UpgradeLevel = 1;

        return facility;
    }

    public List<FacilityData> GetAllFacilityData()
    {
        return facilityDataMap.Values.ToList();
    }
}