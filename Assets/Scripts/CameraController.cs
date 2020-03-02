using UnityEngine;

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

    [HideInInspector]
    public bool isNotInLineOfSight;

    private void Update()
    {
        // Only for debugging remove later
        Debug.DrawLine(transform.position, target.TransformPoint(Vector3.up), Color.red);
    }
    public void SetupCamera()
    {
        transform.position = new Vector3(target.position.x, target.position.y + 3.0f, target.position.z - (maxZoomRange / 2.0f));
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        if (distance == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up + (Vector3.back * 0.01f)), 0.1f);
        }

        //TODO: Zoom back to standard position
        RaycastHit hit;
        if (Physics.Linecast(transform.position, target.position, out hit))
        {
            if (hit.transform.root != target)
            {
                transform.position = Vector3.MoveTowards(transform.position, hit.point, 0.05f);
                isNotInLineOfSight = true;
            }
            else
            {
                isNotInLineOfSight = false;
            }
        }

        transform.LookAt(target.TransformPoint(Vector3.up));
    }

    public void Zoom(float CameraZoom)
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up), transform.position);
        if (distance > 0 & distance < maxZoomRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
        }
        else
        {
            if (distance > 0)
            {
                if (CameraZoom > 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
            }
            else if (distance >= maxZoomRange)
            {
                if (CameraZoom < 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
            }
        }
        UpdateCamera();
    }

    public void Rotate(float amount)
    {
        transform.RotateAround(target.position, Vector3.up, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }

    public void Pitch(float amount)
    {
        transform.RotateAround(target.position, invertPitch ? Vector3.right : Vector3.left, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }
}
