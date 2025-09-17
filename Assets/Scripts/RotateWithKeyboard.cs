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

        direction = direction.normalized;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Rotate(direction, true);
        }
        else
        {
            Rotate(direction, false);
        }

        if (Input.GetKey(KeyCode.R))
        {
            ResetRotation();
        }
    }

    private void Rotate(Vector3 direction, bool boost = false)
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (boost)
        {
            rotationAmount *= 2f;
        }
        
        transform.Rotate(direction, rotationAmount, Space.World);
    }

    private void ResetRotation()
    {
        Vector3 originToPlayer = (Player.Instance.transform.position - transform.position).normalized;
        Vector3 originToCamera = (Camera.main.transform.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.FromToRotation(originToPlayer, originToCamera) * transform.rotation;
        transform.rotation = targetRotation;
    }
}
