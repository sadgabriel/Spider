using UnityEngine;

public static class Utils
{
    public static Node GetNodeFromGameObject(GameObject go)
    {
        if (go == null)
        {
            return null;
        }

        Node node = go.GetComponent<Node>();

        if (node == null)
        {
            Facility facility = go.GetComponentInParent<Facility>();
            if (facility != null)
            {
                node = facility.CurrentPillar;
            }
        }

        if (node == null)
        {
            Unit unit = go.GetComponentInParent<Unit>();
            if (unit != null)
            {
                node = unit.CurrentNode;
            }
        }

        if (node == null)
        {
            ExpOrb expOrb = go.GetComponentInParent<ExpOrb>();
            if (expOrb != null)
            {
                node = expOrb.CurrentNode;
            }   
        }

        return node;
    }
}