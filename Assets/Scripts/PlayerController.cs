using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Variables for movement")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;

    //[SerializeField] private float lookRange = 80f;

    [Header("Input action references")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;


    [Header("Other References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject pivotPoint;

    private Transform worldDirTransform;


    private Vector2 moveDirection;

    public void ToggleWorldDirection(string reqTransform)
    {
        //switch to locked on transform
        if(reqTransform == "locked")
        {
            worldDirTransform = mainCamera.transform;
        }
        //switch to unlocked transform
        else if (reqTransform == "unlocked")
        {
            worldDirTransform = pivotPoint.transform;
        }
    }

    private void MovePlayer(Vector2 moveDirection)
    {
        float moveSpeed;

        Vector3 moveForce;
        Vector3 moveDirection3 = new Vector3(moveDirection.x, 0, moveDirection.y);

        Vector3 worldDirection = worldDirTransform.TransformDirection(moveDirection3);

        worldDirection = worldDirection.normalized;

        if (sprint.action.IsPressed())
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        moveForce.x = worldDirection.x * moveSpeed;
        moveForce.y = 0;
        moveForce.z = worldDirection.z * moveSpeed;

        characterController.Move(moveForce * Time.deltaTime);
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        worldDirTransform = pivotPoint.transform;

        transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        if (move.action.IsPressed() && moveDirection != new Vector2(0, 0))
        {
            MovePlayer(moveDirection);
        }
    }
}
