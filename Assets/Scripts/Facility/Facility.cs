using UnityEngine;

class Facility : MonoBehaviour
{
    [SerializeField] private GameObject IconSurface;
    [SerializeField] private Material IconMaterial;

    private void Start()
    {
        ApplyIcon();
    }

    public void InstallOn(Pillar pillar)
    {
        if (pillar == null)
        {
            return;
        }

        transform.position = pillar.TopPosition;
        transform.rotation = pillar.transform.rotation;

        ResizeToMatchPillar(pillar);
    }

    public void ApplyIcon()
    {
        if (IconSurface == null || IconMaterial == null)
        {
            return;
        }

        MeshRenderer meshRenderer = IconSurface.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = IconMaterial;
        }
    }
    
    private void ResizeToMatchPillar(Pillar pillar)
    {
        float diameter = pillar.Diameter;

        float localScaleMultipler = diameter / (IconSurface.transform.lossyScale.x * 10);
        IconSurface.transform.localScale = new Vector3(IconSurface.transform.localScale.x * localScaleMultipler, 1f, IconSurface.transform.localScale.z * localScaleMultipler);
    }
}