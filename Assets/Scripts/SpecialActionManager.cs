using UnityEngine;
using System.Collections.Generic;

class SpecialActionManager : MonoBehaviour
{
    public static SpecialActionManager Instance { get; private set; }

    private static HashSet<SpecialAction> specialActions = new HashSet<SpecialAction>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddSpecialAction(SpecialAction action)
    {
        if (action == null) return;

        if (!specialActions.Contains(action))
        {
            specialActions.Add(action);
            action.Activate();
        }
    }

    public void RemoveSpecialAction(SpecialAction action)
    {
        if (action == null) return;

        if (specialActions.Contains(action))
        {
            specialActions.Remove(action);
            action.Deactivate();
        }
    }
}