using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class PlayerController : EntityClass
{
    [Header("Variables for movement")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private int staminaDrain = 10;
    [SerializeField] private float rollSpeed = 20f;
    [SerializeField] private float rollTime = 0.25f;

    [Header("Input action references")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference roll;

    [Header("UI references")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;

    [Header("Other References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject pivotPoint;

    private bool rollCoroutineRunning = false;

    private float moveSpeed;
    private float startTime;

    //Variable that transforms get assigned to for the TransformDirection line in MovePlayer
    private Transform worldDirTransform;

    private Vector2 moveDirection;

    //Toggles whatever transform gets used for worldDirection
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
        Vector3 moveForce;

        //Turn moveDirection into a vector3
        Vector3 moveDirection3 = new Vector3(moveDirection.x, 0, moveDirection.y);

        //transform worldDirection from local direction to world direction
        Vector3 worldDirection = worldDirTransform.TransformDirection(moveDirection3);

        //Normalize
        worldDirection = worldDirection.normalized;

        //If sprinting is pressed and stamina greater than 0
        if (sprint.action.IsPressed() && stamina > 0)
        {
            //Start sprinting
            moveSpeed = sprintSpeed;
            if (!staminaCoroutineRunning)
            {
                //Drain stamina
                StartCoroutine(ModifyStamina(staminaDrain, false));
            }
        }
        else if (sprint.action.IsPressed() && stamina <= 0 )
        {
            moveSpeed = walkSpeed;
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

    //Damage
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Attack"))
        {
            int attackDamage = other.GetComponent<AttackScript>().attackDamage;
            if (!healthCoroutineRunning && !rollCoroutineRunning)
            {
                StartCoroutine(ModifyHealth(attackDamage, false));
            }
        }
    }

    private IEnumerator SprintRollDecision()
    {
        yield return new WaitForSeconds(0.5f);
        if (sprint.action.IsPressed())
        {
            moveSpeed = sprintSpeed;
        }
        else if (!sprint.action.IsPressed())
        {
            StartCoroutine(PlayerRoll());
        }
    }


    private IEnumerator PlayerRoll()
    {
        rollCoroutineRunning = true;
        Debug.Log(Time.time + " Regular time");
        Debug.Log(startTime + rollTime+ " roll end time");

        float whileTime = startTime;

        //Turn moveDirection into a vector3
        Vector3 moveDirection3 = new Vector3(moveDirection.x, 0, moveDirection.y);

        //transform worldDirection from local direction to world direction
        Vector3 worldDirection = worldDirTransform.TransformDirection(moveDirection3);

        //Normalize
        worldDirection = worldDirection.normalized;

        Debug.Log(moveDirection);

        while (whileTime <= startTime + rollTime) 
        {
            whileTime += Time.deltaTime;
            
            characterController.Move(rollSpeed * Time.deltaTime * worldDirection);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ModifyStamina(20, false));
        rollCoroutineRunning = false;
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

        //Movement
        if (move.action.IsPressed() && moveDirection != new Vector2(0, 0))
        {
            MovePlayer(moveDirection);
        }

        //Stamina
        if (!sprint.action.IsPressed() && !staminaCoroutineRunning && !roll.action.IsPressed() && !rollCoroutineRunning && stamina < 100 )
        {
            StartCoroutine(ModifyStamina(staminaDrain * 2, true));
        }

        if (roll.action.IsPressed() && !rollCoroutineRunning && stamina > 20)
        {
            startTime = Time.time;
            StartCoroutine(PlayerRoll());
        }

        //Update UI
        float stamBarFillAmount = stamina / 100;
        staminaBar.fillAmount = stamBarFillAmount;
        staminaText.text = stamina + " / 100";

        float healthBarFillAmount = health / 100;
        healthBar.fillAmount = healthBarFillAmount;
        healthText.text = health + " / 100";
    }
}
