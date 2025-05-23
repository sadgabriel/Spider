using UnityEngine;
using System.Collections.Generic;

class SpecialActionManager : MonoBehaviour
{
    public static SpecialActionManager Instance { get; private set; }

    private static HashSet<SpecialAction.SpecialAction> specialActions = new HashSet<SpecialAction.SpecialAction>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddSpecialAction(SpecialAction.SpecialAction action)
    {
        if (action == null) return;

        if (!specialActions.Contains(action))
        {
            specialActions.Add(action);
            action.Activate();
        }
    }

    public void RemoveSpecialAction(SpecialAction.SpecialAction action)
    {
        if (action == null) return;

        if (specialActions.Contains(action))
        {
            specialActions.Remove(action);
            action.Deactivate();
        }
    }
}