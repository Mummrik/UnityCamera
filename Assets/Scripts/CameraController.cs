using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Tooltip("Target the camera should look at e.g. player transform")]
    public Transform target;
    [Tooltip("How far should the user be able to zoom out. (default = 10.0f)")]
    public float maxZoomRange = 10.0f;
    [Tooltip("How fast the camera should rotate or pitch. (default = 100.0f)")]
    public float rotateSpeed = 100.0f;
    [Tooltip("Invert the camera pitch (Does not change in the Edit -> project settings!)")]
    public bool invertPitch;

    private float zoomDistance;

    private void Update()
    {
        // Only for debugging remove later
        Debug.DrawLine(transform.position, target.TransformPoint(Vector3.up), Color.red);
    }
    public void SetupCamera()
    {
        transform.position = new Vector3(target.position.x, target.position.y + 3.0f, target.position.z - (maxZoomRange / 2.0f));
        zoomDistance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        transform.GetComponent<Camera>().nearClipPlane = 0.01f; // Should we force nearClipPlane?
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        if (distance == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up + (Vector3.back * 0.01f)), 0.1f);
        }

        Vector3 position = target.TransformPoint(Vector3.up) - (transform.localRotation * Vector3.forward) * zoomDistance;

        for (int i = 0; i < 2; i++)
        {
            if (Physics.Raycast(target.TransformPoint(Vector3.up * i), -(transform.localRotation * Vector3.forward), out RaycastHit hit, zoomDistance))
            {
                if (hit.transform.root != target)
                {
                    position = target.TransformPoint(Vector3.up) - (transform.localRotation * Vector3.forward) * hit.distance;
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, position, 1f);
        transform.LookAt(target.TransformPoint(Vector3.up));
    }

    ///<summary>
    ///Zoom the camera in/out to its target.
    ///</summary>
    public void Zoom(float zoomSpeed)
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        if (distance > 0 & distance < maxZoomRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), zoomSpeed);
        }
        else
        {
            if (distance > 0)
            {
                if (zoomSpeed > 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), zoomSpeed);
            }
            else if (distance >= maxZoomRange)
            {
                if (zoomSpeed < 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), zoomSpeed);
            }
        }
        zoomDistance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        UpdateCamera();
    }

    ///<summary>
    ///Rotate the camera around its target.
    ///</summary>
    public void Rotate(float amount)
    {
        transform.RotateAround(target.position, Vector3.up, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }

    ///<summary>
    ///Pitch the camera around its target.
    ///</summary>
    public void Pitch(float amount)
    {
        //TODO: Clamp the pitch of the camera
        //if (transform.eulerAngles.x > 80.0f)
        //{
        //    transform.eulerAngles = new Vector3(79.9f, transform.eulerAngles.y, transform.eulerAngles.z);
        //    return;
        //}
        transform.RotateAround(target.position, invertPitch ? Vector3.right : Vector3.left, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }

    ///<summary>
    ///Increase the camera position whit the movement vector.
    ///</summary>
    public void Move(Vector3 movement)
    {
        transform.position += movement;
        UpdateCamera();
    }
}
