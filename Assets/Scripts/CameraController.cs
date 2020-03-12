using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Tooltip("Target the camera should look at e.g. player transform")]
    public Transform target;
    //[Range(0.1f, 1.0f)]
    [Tooltip("How fast the camera should auto zoom when obstacle is in the way. (default = 10)")]
    [SerializeField]
    private float autoZoomSpeed = 10f;
    [Tooltip("How far should the user be able to zoom out. (default = 10.0)")]
    [SerializeField]
    private float maxZoomRange = 10f;
    [Range(1f, 10000f)]
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
    private bool isZooming;
    private Transform focusPoint;

    // Use LateUpdate for camera, if something got updated in the regular update method this code will execute after
    private void LateUpdate()
    {
        // Only for debugging remove later
        Debug.DrawLine(transform.position, focusPoint.TransformPoint(Vector3.up), Color.red);
        Debug.DrawLine(transform.position, focusPoint.position, Color.blue);
        Debug.DrawLine(transform.position, focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), Color.green);
    }
    public void SetupCamera()
    {
        focusPoint = target.Find("FocusPoint").transform;
        Assert.IsNotNull(focusPoint, "Couldn't find a 'FocusPoint' GameObject on the target! Please add it as a child.");
        transform.position = new Vector3(focusPoint.position.x + shoulderOffset, focusPoint.position.y + 3.0f, focusPoint.position.z - (maxZoomRange / 2.0f));
        zoomDistance = Vector3.Distance(focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), transform.position);
        transform.GetComponent<Camera>().nearClipPlane = 0.01f; // Should we force nearClipPlane?
        UpdateCamera();
        cameraX = transform.position.x;
        cameraY = transform.position.y;
    }

    public void UpdateCamera()
    {
        Vector3 point = focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset));
        float distance = Vector3.Distance(point, transform.position);
        Vector3 position = point - (transform.localRotation * Vector3.forward) * zoomDistance;
        if (distance < 1.0f)
        {
            if (zoomDistance < 1.0f)
                zoomDistance = 1.0f;

            position = point - (transform.localRotation * Vector3.forward) * zoomDistance;
            transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed > 0 ? autoZoomSpeed : 1.0f);
        }

        for (int i = 0; i < 2; i++)
        {
            // Cast two rays, one from the center of the target and one from the top of the target towards the camera.
            if (Physics.Raycast(
                focusPoint.TransformPoint((Vector3.up * i) + (Vector3.right * shoulderOffset)),
                -(transform.localRotation * Vector3.forward), out RaycastHit hit, zoomDistance))
            {
                if (hit.transform.root != focusPoint)
                {
                    position = point - (transform.localRotation * Vector3.forward) * hit.distance;
                }
            }
        }

        if (isZooming)
        {
            while (transform.position != position)
                transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed * Time.deltaTime);
            isZooming = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed * Time.deltaTime);
        }
        transform.LookAt(point);
    }

    ///<summary>
    ///Zoom the camera in/out to its target.
    ///</summary>
    public void Zoom(float scrollDelta)
    {
        isZooming = true;
        Vector3 point = focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset));
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
        UpdateCamera();
    }

    private float cameraX;
    private float cameraY;
    public void RotateNew(float x, float y)
    {
        float minAngle = -15f;
        float maxAngle = 89f;

        cameraX += invertPitch ? x : -x;
        cameraY += y;
        cameraX = Mathf.Clamp(cameraX, minAngle, maxAngle);
        Vector3 dir = new Vector3(0f, 0f, -zoomDistance);
        Quaternion rotation = Quaternion.Euler(cameraX, cameraY, 0);
        transform.position = focusPoint.position + rotation * dir;
        transform.LookAt(focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)));
        UpdateCamera();
    }

    ///<summary>
    ///Rotate the camera around its target.
    ///</summary>
    public void Rotate(float amount)
    {
        Vector3 point = focusPoint.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset));
        focusPoint.rotation = new Quaternion(focusPoint.rotation.x, transform.rotation.y, focusPoint.rotation.z, transform.rotation.w);
        transform.RotateAround(point, Vector3.up, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }

    ///<summary>
    ///Pitch the camera around its target.
    ///</summary>
    public void Pitch(float amount)
    {
        //TODO: Clamp the pitch of the camera
        //Debug.Log(focusPoint.localEulerAngles);
        Vector3 point = focusPoint.TransformPoint(focusPoint.up + (focusPoint.right * shoulderOffset));
        //transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, amount * Time.deltaTime * rotateSpeed);

        float angle = Quaternion.Angle(transform.rotation, focusPoint.rotation);
        //Debug.Log(angle);
        amount = amount * Time.deltaTime * rotateSpeed;
        if (amount < 0)
        {
            if (angle < 89f && angle > 0f)
            {
                transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, amount);
            }
        }
        else
        {
            if (angle < 1f)
            {
                transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, -amount * 0.01f);
                return;
            }
            transform.RotateAround(point, invertPitch ? focusPoint.right : -focusPoint.right, amount);
        }
        //transform.position += new Vector3(0f, amount * rotateSpeed * 0.001f, 0f);
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
