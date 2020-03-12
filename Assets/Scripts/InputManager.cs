using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CameraController cameraController;

    private Transform playerTransform;
    private float moveSpeed = 3.0f;
    private bool isMoving;
    private bool isRotating;

    #region input ids
    private const string moveHorizontal = "Horizontal";
    private const string moveVertical = "Vertical";
    private const string mouse1 = "Fire1";
    private const string mouse2 = "Fire2";
    private const string rotateWhitKeys = "RotateWhitKey";
    private const string mouseHorizontal = "Mouse X";
    private const string mouseVertical = "Mouse Y";
    #endregion

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
        if (Input.GetAxis(moveHorizontal) != 0 || Input.GetAxis(moveVertical) != 0)
        {
            isMoving = true;
        }

        if (Input.mouseScrollDelta.y != 0 && IsMouseOverGameWindow)
        {
            cameraController.Zoom(Input.mouseScrollDelta.y);
        }

        if (Input.GetButton(mouse2))
        {
            Cursor.lockState = CursorLockMode.Locked;

            cameraController.Pitch(Input.GetAxisRaw(mouseVertical));
            cameraController.Rotate(Input.GetAxis(mouseHorizontal));
            //cameraController.RotateNew(Input.GetAxis(mouseVertical), Input.GetAxis(mouseHorizontal));

            if (Input.GetButton(mouse1))
            {
                isRotating = true;
                playerTransform.rotation = new Quaternion(
                    playerTransform.rotation.x,
                    cameraController.transform.rotation.y,
                    playerTransform.rotation.z,
                    cameraController.transform.rotation.w);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetAxis(rotateWhitKeys) != 0)
        {
            cameraController.Rotate(Input.GetAxis(rotateWhitKeys));
        }
    }

    //Apply input to physics
    private void FixedUpdate()
    {
        if (isMoving)
        {
            // if rotate movement is active then only strafe movement will be allowed
            Movement(isRotating ? 1f : Input.GetAxis(moveVertical), Input.GetAxis(moveHorizontal));
        }
        if (isRotating)
        {
            Movement(1f);
            isRotating = false;
        }
    }

    private void Movement(float x = 0f, float y = 0f)
    {
        Vector3 movement = (playerTransform.forward * x * Time.deltaTime * moveSpeed) + (playerTransform.right * y * Time.deltaTime * moveSpeed);
        playerTransform.position += movement;
        cameraController.Move(movement);
        isMoving = false;
    }

}
