using UnityEngine;

interface IPanel
{
    void Show(object data);
    void Hide();
}

abstract class Panel<T> : MonoBehaviour, IPanel
{
    public bool IsVisible => gameObject.activeSelf;

    public void Show(object data)
    {
        if (data == null)
        {
            Show(default(T));
        }
        else if (data is T typedData)
        {
            Show(typedData);
        }
        else
        {
            Debug.LogWarning($"[Panel<{typeof(T)}>] 잘못된 타입 전달됨: {data.GetType()} → {typeof(T)}");
        }
    }

    public abstract void Show(T data);

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
