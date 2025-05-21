using UnityEngine;

class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event System.Action<int, Vector3> OnMouseClicked;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClicked?.Invoke(0, Input.mousePosition);
        }
    }
}