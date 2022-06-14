using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float playerMaxMovementSpeed = 3.0f;
    private float playerMovementAcceleration = 100f;
    private float mouseSensitivity = 1.0f;
    private Vector3 velocity = Vector3.zero;
    private Vector2 movementInput = Vector2.zero;

    private Rigidbody playerRb;
    private GameObject playerCamera;
    private PlayerInput playerInput;
    private InputAction movementAction;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("Main Camera");
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Movement"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        Move();
    }

    private void LateUpdate()
    {
        RotateCamera();
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

    }
}
