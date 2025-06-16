using UnityEngine;

public class Bridge : Node
{
    public void Initialize(Vector3 from, Vector3 to, Vector3 upwards = default)
    {
        Vector3 diff = to - from;
        float distance = diff.magnitude;
        Vector3 forward = diff.normalized;

        Vector3 position = from + forward * (distance / 2);

        if (upwards == default)
        {
            upwards = Vector3.up;
        }

        upwards = Vector3.ProjectOnPlane(upwards, forward).normalized;

        Quaternion rotation = Quaternion.LookRotation(forward, upwards);

        transform.position = position;
        transform.rotation = rotation;
        
        Vector3 scale = transform.localScale;
        scale.z = distance;
        transform.localScale = scale;
    }
}
