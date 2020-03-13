using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Tooltip("Target the camera should look at e.g. player transform")]
    public Transform target;
    [Tooltip("How fast the camera should auto zoom when obstacle is in the way. (default = 15)")]
    [SerializeField]
    private float autoZoomSpeed = 15f;
    [Tooltip("How far should the user be able to zoom out. (default = 10.0)")]
    [SerializeField]
    private float maxZoomRange = 10f;
    [Range(0f, 10000f)]
    [Tooltip("How fast the camera should rotate or pitch. (default = 500.0)")]
    [SerializeField]
    public float rotateSpeed = 500f;
    [Tooltip("Invert the camera pitch (Doesn't change in Edit -> project settings!)")]
    [SerializeField]
    private bool invertPitch = false;
    [Range(-2.5f, 2.5f)]
    [Tooltip("Set the camera focus offset. (center = 0)")]
    [SerializeField]
    private float shoulderOffset = 0f;

    private float zoomDistance;
    private Transform focusPoint;

    // Use LateUpdate for camera, if something got updated in a regular Update method this code will execute after
    private void LateUpdate()
    {
        UpdateCamera();

        // Only for debugging, should be removed
        Debug.DrawLine(transform.position, focusPoint.TransformPoint(Vector3.up), Color.red);
        Debug.DrawLine(transform.position, focusPoint.position, Color.blue);
        Debug.DrawLine(transform.position, focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset)), Color.green);
    }
    public void SetupCamera()
    {
        Assert.IsNotNull(target, "Couldn't find a target GameObject! Please add one to the script.");
        try
        {
            focusPoint = target.Find("FocusPoint").transform;
        }
        catch
        {
            GameObject focusObject = new GameObject("FocusPoint");
            focusObject.transform.SetParent(target);
            focusPoint = focusObject.transform;
        }
        transform.GetComponent<Camera>().nearClipPlane = 0.01f;
        transform.position = new Vector3(focusPoint.position.x + shoulderOffset, focusPoint.position.y + 3.0f, focusPoint.position.z - (maxZoomRange / 2.0f));
        zoomDistance = Vector3.Distance(focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset)), transform.position);
    }

    public void UpdateCamera()
    {
        Vector3 point = focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset));
        Vector3 position = point - (transform.localRotation * Vector3.forward) * zoomDistance;

        for (int i = 0; i < 2; i++)
        {
            // Cast two rays, one from the center of the target and one from the top of the target towards the camera.
            if (Physics.Raycast(
                focusPoint.TransformPoint((focusPoint.up * i) + (focusPoint.right * shoulderOffset)),
                -(transform.localRotation * Vector3.forward), out RaycastHit hit, zoomDistance))
            {
                if (hit.transform.root != target)
                {
                    position = point - (transform.localRotation * Vector3.forward) * hit.distance;
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed * Time.deltaTime);
        transform.LookAt(point);
    }

    ///<summary>
    ///Zoom the camera in/out to its target.
    ///</summary>
    public void Zoom(float scrollDelta)
    {
        Vector3 point = focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset));
        float distance = Vector3.Distance(point, transform.position);
        if (distance > 0 & distance < maxZoomRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, point, scrollDelta);
        }
        else
        {
            if (distance > 0)
            {
                if (scrollDelta > 0)
                    transform.position = Vector3.MoveTowards(transform.position, point, scrollDelta);
            }
            else if (distance >= maxZoomRange)
            {
                if (scrollDelta < 0)
                    transform.position = Vector3.MoveTowards(transform.position, point, scrollDelta);
            }
        }
        zoomDistance = Vector3.Distance(point, transform.position);
    }

    ///<summary>
    ///Rotate the camera around its target.
    ///</summary>
    public void Rotate(float amount)
    {
        Vector3 point = focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset));
        focusPoint.rotation = new Quaternion(focusPoint.rotation.x, transform.rotation.y, focusPoint.rotation.z, transform.rotation.w);
        transform.RotateAround(point, Vector3.up, amount * Time.deltaTime * rotateSpeed);
    }

    ///<summary>
    ///Pitch the camera around its target.
    ///</summary>
    public void Pitch(float amount)
    {
        Vector3 point = focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset));
        float angle = Quaternion.Angle(transform.rotation, focusPoint.rotation);
        if (amount < 0)
        {
            if (angle < 89f && angle > 0f)
                transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, amount);
        }
        else
        {
            if (angle < 1f) { return; }
            transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, amount);
        }
    }

    ///<summary>
    ///Increase the camera position whit the movement vector.
    ///</summary>
    public void Move(Vector3 movement)
    {
        transform.position += movement;
    }
}
