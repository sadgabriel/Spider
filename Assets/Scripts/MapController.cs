using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;

    private void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.up;
        }
        
        RotateMap(direction.normalized);
    }

    private void RotateMap(Vector3 direction)
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(direction, rotationAmount, Space.World);
    }
}
