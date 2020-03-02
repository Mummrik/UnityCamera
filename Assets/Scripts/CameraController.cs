using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Target the camera should look at e.g. player transfrom")]
    public Transform target;
    [Tooltip("How far should the user be able to zoom out. (defalut = 10.0f)")]
    public float maxZoomRange = 10.0f;
    [Tooltip("How fast the camera should rotate or pitch. (default = 100.0f)")]
    public float rotateSpeed = 100.0f;
    [Tooltip("Invert the camera pitch (Does not change in the Edit -> project settings!)")]
    public bool invertPitch;
    //private float maxPitchAngle = 15.0f;

    public void SetupCamera()
    {
        transform.position = new Vector3(target.position.x, target.position.y + 3.0f, target.position.z - (maxZoomRange / 2.0f));
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        transform.LookAt(target.TransformPoint(Vector3.up));
    }

    public void Zoom(float CameraZoom)
    {
        float distance = -Vector3.Distance(target.position, transform.position);
        if (distance < 0 & distance > -maxZoomRange)
        {
            //transform.position += Vector3.forward * CameraZoom;
            transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
            Debug.Log(distance);
        }
        else
        {
            if (distance >= 0)
            {
                if (CameraZoom < 0)
                    //transform.position += Vector3.forward * CameraZoom;
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
            }
            else if (distance >= -maxZoomRange)
            {
                if (CameraZoom > 0)
                    //transform.position += Vector3.forward * CameraZoom;
                    transform.position = Vector3.MoveTowards(transform.position, target.TransformPoint(Vector3.up), CameraZoom);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z - maxZoomRange);
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
