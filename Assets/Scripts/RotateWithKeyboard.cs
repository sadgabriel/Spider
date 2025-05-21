using UnityEngine;
using UnityEngine.UIElements;

public class RotateWithKeyboard : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 40f;

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
        
        Rotate(direction.normalized);
    }

    private void Rotate(Vector3 direction)
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(direction, rotationAmount, Space.World);
    }
}
