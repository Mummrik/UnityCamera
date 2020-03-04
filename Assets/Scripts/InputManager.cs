using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CameraController cameraController;

    private Transform playerTransform;
    private float moveSpeed = 3.0f;
    private bool isMoving;

    private bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }
    private void Awake()
    {
        playerTransform = GameManager.instance.player.transform;

        if (Camera.main.GetComponent<CameraController>() == null)
        {
            Debug.LogWarning("Main camera was missing a CameraController script, please add one to get rid of this warning! (InputMannager added one automatically)");
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

        if (Input.mouseScrollDelta.y != 0 && IsMouseOverGameWindow)
        {
            cameraController.Zoom(Input.mouseScrollDelta.y);
        }

        if (Input.GetButton("Fire2"))
        {
            Cursor.visible = false;

            cameraController.Rotate(Input.GetAxis("Mouse X"));
            cameraController.Pitch(Input.GetAxis("Mouse Y"));
           
            if (Input.GetButton("Fire1"))
            {
                //TODO: Fix the player rotation
                playerTransform.rotation = new Quaternion(playerTransform.rotation.x, cameraController.transform.rotation.y, playerTransform.rotation.z, playerTransform.rotation.w);
                Movement(1.0f, 0.0f);
            }
        }
        else
        {
            Cursor.visible = true;
        }

        if (Input.GetAxis("RotateWhitKey") != 0)
        {
            cameraController.Rotate(Input.GetAxis("RotateWhitKey"));
        }
    }

    //Apply input to physics
    private void FixedUpdate()
    {
        if (isMoving)
        {
            Movement(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }
    }

    private void Movement(float x, float y)
    {
        Vector3 movement = (playerTransform.forward * x * Time.deltaTime * moveSpeed) + (playerTransform.right * y * Time.deltaTime * moveSpeed);
        playerTransform.position += movement;
        cameraController.Move(movement);
        isMoving = false;
    }

}
