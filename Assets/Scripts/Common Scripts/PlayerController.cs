using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour
{
    private float playerMaxMovementSpeed = 3.0f;
    private float playerMovementAcceleration = 100f;
    private float mouseSensitivity = 0.8f;
    private float cameraRotationSpeed = 45.0f;
    private float currentCameraVerticalRotation = 0;
    private float maxVerticalCameraRotation = 85;
    private Vector3 velocity = Vector3.zero;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 cameraInput = Vector2.zero;

    private Rigidbody playerRb;
    private GameObject playerCamera;
    private PlayerInput playerInput;
    private InputAction movementAction;
    private InputAction cameraAction;
    private InputAction captureMouseAction;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("Main Camera");
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Movement"];
        cameraAction = playerInput.actions["Camera"];
        captureMouseAction = playerInput.actions["CaptureMouse"];
        Cursor.visible = false;
    }

    private void Update()
    {
        if(!GlobalSettingsManager.Instance.GameOver)
            CheckCaptureMouseAction();
    }

    private void FixedUpdate()
    {
        if (!GlobalSettingsManager.Instance.GameOver && GlobalSettingsManager.Instance.CaptureMouse)
        {
            UpdateVelocity();
            Move();
            CenterMousePosition();
        }
    }

    private void LateUpdate()
    {
        if (!GlobalSettingsManager.Instance.GameOver && GlobalSettingsManager.Instance.CaptureMouse)
        {
            RotateCamera();
        }
    }

    private void UpdateVelocity()
    {
        velocity = transform.InverseTransformDirection(playerRb.velocity);
    }

    private void Move()
    {
        movementInput = movementAction.ReadValue<Vector2>();

        if(movementInput.y > 0 && velocity.z < playerMaxMovementSpeed)
        {
            playerRb.AddRelativeForce(Vector3.forward * playerMovementAcceleration * movementInput.y, ForceMode.Acceleration);
        }
        else if(movementInput.y < 0 && velocity.z > -playerMaxMovementSpeed)
        {
            playerRb.AddRelativeForce(Vector3.forward * playerMovementAcceleration * movementInput.y, ForceMode.Acceleration);
        }

        if(movementInput.x > 0 && velocity.x < playerMaxMovementSpeed)
        {
            playerRb.AddRelativeForce(Vector3.right * playerMovementAcceleration * movementInput.x, ForceMode.Acceleration);
        }
        else if(movementInput.x < 0 && velocity.x > -playerMaxMovementSpeed)
        {
            playerRb.AddRelativeForce(Vector3.right * playerMovementAcceleration * movementInput.x, ForceMode.Acceleration);
        }
    }

    private void RotateCamera()
    {
        cameraInput = cameraAction.ReadValue<Vector2>();

        if ((cameraInput.y > 0 && playerCamera.transform.localRotation.x < maxVerticalCameraRotation) || (cameraInput.y < 0 && playerCamera.transform.localRotation.x > -maxVerticalCameraRotation))
        {
            if (cameraInput.y > 0 || cameraInput.y < 0)
            {
                currentCameraVerticalRotation += cameraRotationSpeed * mouseSensitivity * -cameraInput.y * Time.deltaTime;

                currentCameraVerticalRotation = Mathf.Clamp(currentCameraVerticalRotation, -maxVerticalCameraRotation, maxVerticalCameraRotation);

                playerCamera.transform.localEulerAngles = new Vector3(currentCameraVerticalRotation, 0, 0);
            }
        }
        transform.eulerAngles += cameraRotationSpeed * mouseSensitivity * new Vector3(0, cameraInput.x, 0) * Time.deltaTime;
    }

    private void CenterMousePosition()
    {
        //https://answers.unity.com/questions/1738031/force-update-of-mouse-position-with-the-new-input-1.html?childToView=1754530#answer-1754530
        Vector2 warpPosition = Screen.safeArea.center;
        Mouse.current.WarpCursorPosition(warpPosition);
        InputState.Change(Mouse.current.position, warpPosition);
    }

    private void CheckCaptureMouseAction()
    {
        if(captureMouseAction.WasPressedThisFrame())
        {
            GlobalSettingsManager.Instance.CaptureMouse = !GlobalSettingsManager.Instance.CaptureMouse;

            if(GlobalSettingsManager.Instance.CaptureMouse)
                Cursor.visible = false;
            else
                Cursor.visible = true;
        }
    }
}
