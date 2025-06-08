using UnityEngine;

enum PanelType
{
    Status,
    FacilitySelection,
    FacilityBuilding
}

interface IPanel
{
    void Hide();
}

abstract class Panel<T> : MonoBehaviour, IPanel
{
    public bool IsVisible => gameObject.activeSelf;

    public abstract void Show(T data = default);

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}