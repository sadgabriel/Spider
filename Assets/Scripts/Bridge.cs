using UnityEngine;

public class Bridge : Node
{
    public void Initialize(Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from;
        float distance = diff.magnitude;
        Vector3 direction = diff.normalized;

        Vector3 position = from + direction * (distance / 2);
        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.position = position;
        transform.rotation = rotation;
        
        Vector3 scale = transform.localScale;
        scale.z = distance;
        transform.localScale = scale;
    }
}
