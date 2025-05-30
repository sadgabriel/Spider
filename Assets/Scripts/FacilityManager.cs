using UnityEngine;

public enum FacilityType
{
    Test,
}

class FacilityManager : MonoBehaviour
{
    public static FacilityManager Instance { get; private set; }

    [SerializeField] private GameObject testFacilityPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public Facility InstallFacility(Pillar pillar, FacilityType type)
    {
        if (pillar == null) return null;

        if (pillar.HasFacility) return null;

        GameObject facilityGO = Instantiate(testFacilityPrefab, pillar.transform);
        Facility facility = facilityGO.GetComponent<Facility>();

        facility.InstallOn(pillar);

        pillar.HasFacility = true;

        return facility;
    }
}