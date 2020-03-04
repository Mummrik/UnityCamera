using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Tooltip("Target the camera should look at e.g. player transform")]
    public Transform target;
    //[Range(0.1f, 1.0f)]
    [Tooltip("How fast the camera should auto zoom when obstacle is in the way. (default = 0.4)")]
    [SerializeField]
    private float autoZoomSpeed = 0.4f;
    [Tooltip("How far should the user be able to zoom out. (default = 10.0)")]
    [SerializeField]
    private float maxZoomRange = 10.0f;
    [Range(1.0f, 10000.0f)]
    [Tooltip("How fast the camera should rotate or pitch. (default = 500.0)")]
    [SerializeField]
    public float rotateSpeed = 500.0f;
    [Tooltip("Invert the camera pitch (Does not change in the Edit -> project settings!)")]
    [SerializeField]
    private bool invertPitch;
    [Range(-2.5f, 2.5f)]
    [Tooltip("Set the camera focus offset. (center = 0)\nNOT IMPLEMENTED!")]
    [SerializeField]
    private float shoulderOffset = 0.0f;


    private float zoomDistance;

    private void Update()
    {
        // Only for debugging remove later
        Debug.DrawLine(transform.position, target.TransformPoint(Vector3.up), Color.red);
        Debug.DrawLine(transform.position, target.position, Color.blue);
        Debug.DrawLine(transform.position, target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), Color.green);
    }
    public void SetupCamera()
    {
        transform.position = new Vector3(target.position.x + shoulderOffset, target.position.y + 3.0f, target.position.z - (maxZoomRange / 2.0f));
        zoomDistance = Vector3.Distance(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), transform.position);
        transform.GetComponent<Camera>().nearClipPlane = 0.01f; // Should we force nearClipPlane?
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), transform.position);
        Vector3 position = target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)) - (transform.localRotation * Vector3.forward) * zoomDistance;
        if (distance < 1.0f)
        {
            if (zoomDistance < 1.0f)
                zoomDistance = 1.0f;

            position = target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)) - (transform.localRotation * Vector3.forward) * zoomDistance;
            transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed > 0 ? autoZoomSpeed : 1.0f);
        }

        for (int i = 0; i < 2; i++)
        {
            // Cast two rays, one from the center of the target and one from the top of the target towards the camera.
            if (Physics.Raycast(target.TransformPoint((Vector3.up * i) + (Vector3.right * shoulderOffset)), -(transform.localRotation * Vector3.forward), out RaycastHit hit, zoomDistance))
            {
                if (hit.transform.root != target)
                {
                    position = target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)) - (transform.localRotation * Vector3.forward) * hit.distance;
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, position, autoZoomSpeed);
        transform.LookAt(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)));
    }

    ///<summary>
    ///Zoom the camera in/out to its target.
    ///</summary>
    public void Zoom(float scrollDelta)
    {
        float distance = Vector3.Distance(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), transform.position);
        if (distance > 0 & distance < maxZoomRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), scrollDelta);
        }
        else
        {
            if (distance > 0)
            {
                if (scrollDelta > 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), scrollDelta);
            }
            else if (distance >= maxZoomRange)
            {
                if (scrollDelta < 0)
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), scrollDelta);
            }
        }
        zoomDistance = Vector3.Distance(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), transform.position);
        UpdateCamera();
    }

    ///<summary>
    ///Rotate the camera around its target.
    ///</summary>
    public void Rotate(float amount)
    {
        transform.RotateAround(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), Vector3.up, amount * Time.deltaTime * rotateSpeed);
        UpdateCamera();
    }

    ///<summary>
    ///Pitch the camera around its target.
    ///</summary>
    public void Pitch(float amount)
    {
        //TODO: Clamp the pitch of the camera
        transform.RotateAround(target.TransformPoint(Vector3.up + (Vector3.right * shoulderOffset)), invertPitch ? Vector3.right : Vector3.left, amount * Time.deltaTime * rotateSpeed);
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
