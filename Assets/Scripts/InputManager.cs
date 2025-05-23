using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event System.Action<int, Vector3, GameObject> OnMouseButtonDown;
    public event System.Action<HashSet<KeyCode>> OnKeyDown;
    public event System.Action<HashSet<KeyCode>> OnKey;

    [SerializeField] private KeyCode[] monitoredKeys = {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.Space,
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            int button = Input.GetMouseButtonDown(0) ? 0 : 1;
            Vector3 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            GameObject clickedGO = null;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                clickedGO = hit.collider.gameObject;
            }

            OnMouseButtonDown?.Invoke(button, mousePosition, clickedGO);
        }

        if (Input.anyKeyDown)
        {
            HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
            foreach (KeyCode key in monitoredKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    pressedKeys.Add(key);
                }
            }
            if (pressedKeys.Count > 0)
            {
                OnKeyDown?.Invoke(pressedKeys);
            }
        }

        if (Input.anyKey)
        {
            HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
            foreach (KeyCode key in monitoredKeys)
            {
                if (Input.GetKey(key))
                {
                    pressedKeys.Add(key);
                }
            }
            if (pressedKeys.Count > 0)
            {
                OnKey?.Invoke(pressedKeys);
            }
        }
    }
}