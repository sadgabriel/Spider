using UnityEngine;
using System.Collections.Generic;

class SpecialActionManager : MonoBehaviour
{
    public static SpecialActionManager Instance { get; private set; }

    [SerializeField] private Texture2D SprintIcon;
    [SerializeField] private Texture2D DemolitionIcon;

    public SpecialAction SelectedSpecialAction => specialActions.Count > 0 && selectedActionIndex >= 0 ? specialActions[selectedActionIndex] : null;

    private readonly List<SpecialAction> specialActions = new List<SpecialAction>();
    private int selectedActionIndex = -1;

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
    }

    public void Initialize()
    {
        InputManager.Instance.OnKeyDown += HandleKeyDown;

        Sprint.Instance.Icon = SprintIcon;
        Demolition.Instance.Icon = DemolitionIcon;
    }

    public void AddSpecialAction(SpecialAction action)
    {
        if (action == null) return;

        if (!specialActions.Contains(action))
        {
            specialActions.Add(action);
            if (selectedActionIndex == -1)
            {
                action.Activate();
                selectedActionIndex = 0;
            }
        }
    }

    public void RemoveSpecialAction(SpecialAction action)
    {
        if (action == null) return;

        if (specialActions.Contains(action))
        {
            if (specialActions[selectedActionIndex] == action)
            {
                if (specialActions.Count == 1)
                {
                    selectedActionIndex = -1;
                    action.Deactivate();
                    specialActions.Clear();
                    return;
                }
                else
                {
                    SelectNextAction();
                }
            }

            if (specialActions.FindIndex(a => a == action) < selectedActionIndex)
            {
                selectedActionIndex--;
            }

            specialActions.Remove(action);
        }
    }

    public void SelectNextAction()
    {
        if (specialActions.Count <= 1) return;

        specialActions[selectedActionIndex].Deactivate();
        selectedActionIndex = (selectedActionIndex + 1) % specialActions.Count;
        specialActions[selectedActionIndex].Activate();
    }

    public void SelectPreviousAction()
    {
        if (specialActions.Count <= 1) return;

        specialActions[selectedActionIndex].Deactivate();
        selectedActionIndex = (selectedActionIndex - 1 + specialActions.Count) % specialActions.Count;
        specialActions[selectedActionIndex].Activate();
    }

    private void HandleKeyDown(HashSet<KeyCode> pressedKeys)
    {
        if (pressedKeys.Contains(KeyCode.Q))
        {
            SelectPreviousAction();
        }
        else if (pressedKeys.Contains(KeyCode.E))
        {
            SelectNextAction();
        }
    }
}