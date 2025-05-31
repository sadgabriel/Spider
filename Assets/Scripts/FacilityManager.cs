using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FacilityType
{
    Test,
}

[System.Serializable]
public class FacilityEntry
{
    public FacilityType type;
    public GameObject prefab;
}

class FacilityManager : MonoBehaviour
{
    public static FacilityManager Instance { get; private set; }
    [SerializeField] private List<FacilityEntry> facilityEntries;

    public List<Facility> Facilities { get; private set; } = new List<Facility>();

    private Dictionary<FacilityType, GameObject> facilityPrefabs;

    private void Awake()
    {
        Instance = this;
        facilityPrefabs = facilityEntries.ToDictionary(entry => entry.type, entry => entry.prefab);
    }

    public Facility InstallFacility(Pillar pillar, FacilityType type)
    {
        if (pillar == null || pillar.HasFacility) return null;

        GameObject facilityGO = Instantiate(facilityPrefabs[type], pillar.transform);
        Facility facility = facilityGO.GetComponent<Facility>();

        facility.InstallOn(pillar);

        pillar.HasFacility = true;

        Facilities.Add(facility);

        return facility;
    }
}