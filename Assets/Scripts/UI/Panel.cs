using UnityEngine;

public interface IPanel
{
    void Show(object data);
    void Hide();
    bool IsVisible { get; }
}

public abstract class Panel<T> : MonoBehaviour, IPanel
{
    public bool IsVisible => gameObject.activeSelf;

    public void Show(object data)
    {
        if (data == null)
        {
            Show(default);
        }
        else if (data is T typedData)
        {
            Show(typedData);
        }
        else
        {
            Debug.LogError($"[{name} / Panel<{typeof(T).Name}>] Invalid data type: received {data.GetType().Name}, expected {typeof(T).Name}.");
        }
    }

    public abstract void Show(T data);

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
