using UnityEngine;

public static class Utils
{
    public static Node GetNodeFromGameObject(GameObject go)
    {
        Node node = go?.GetComponent<Node>();

        if (node == null)
        {
            Facility facility = go?.GetComponentInParent<Facility>();
            node = facility?.CurrentPillar;
        }

        if (node == null)
        {
            Unit unit = go?.GetComponentInParent<Unit>();
            node = unit?.CurrentNode;
        }

        return node;
    }
}