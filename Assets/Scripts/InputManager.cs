using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InputManager : MonoBehaviour
{
    private CameraController cameraController;

    private Transform playerTransform;
    private float moveSpeed = 3.0f;
    private bool isMoving;


    private void Awake()
    {
        playerTransform = GameManager.instance.player.transform;

        if (Camera.main.GetComponent<CameraController>() == null)
        {
            Debug.LogWarning("Main camera was missing a CameraController, please add one to get rid of this warning!");
            cameraController = Camera.main.gameObject.AddComponent<CameraController>();
            cameraController.target = playerTransform;
        }
        else
        {
            cameraController = Camera.main.GetComponent<CameraController>();
        }

        cameraController.SetupCamera();

    }

    //Register input
    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMoving = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraController.Zoom(Input.GetAxis("Mouse ScrollWheel") * 5.0f);
        }

        if (Input.GetButton("Fire2"))
        {
            cameraController.Rotate(Input.GetAxis("Mouse X"));
            cameraController.Pitch(Input.GetAxis("Mouse Y"));
        }
    }

    //Apply input to physics
    private void FixedUpdate()
    {
        if (isMoving)
        {
            Movement();
        }

    }

    private void Movement()
    {
        playerTransform.position += (Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed) + (Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed);
        cameraController.transform.position += (Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed) + (Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed);
        isMoving = false;
    }
}
