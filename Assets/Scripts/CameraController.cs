using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float minSize = 8f;
    public float maxSize = 20f;

    private Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        }
    }
}
