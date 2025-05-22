using UnityEngine;

class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event System.Action<int, Vector3, GameObject> OnMouseClickedWhenIdle;
    public event System.Action<int, Vector3, GameObject> OnMouseClickedWhenSpecialAction;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            GameObject clickedGO = null;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                clickedGO = hit.collider.gameObject;
            }

            if (GameStateManager.Instance.CurrentState == GameState.Idle)
            {
                OnMouseClickedWhenIdle?.Invoke(0, Input.mousePosition, clickedGO);
            }
            else if (GameStateManager.Instance.CurrentState == GameState.SpecialAction)
            {
                OnMouseClickedWhenSpecialAction?.Invoke(0, Input.mousePosition, clickedGO);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStateManager.Instance.UseSpecialAction();
        }
    }
}