using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class PlayerController : EntityClass
{
    [Header("Player Variables")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private int staminaDrain = 10;
    [SerializeField] private float rollSpeed = 20f;
    [SerializeField] private float rollTime = 0.25f;
    [SerializeField] private int maxHealingCharges = 5;
    [SerializeField] private int healAmount = 60;
    [Tooltip("Should be however many attacks the attack animation has")]
    [SerializeField] private int attackChainMax = 4;

    [Header("Input action references")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference roll;
    [SerializeField] private InputActionReference heal;
    [SerializeField] private InputActionReference attack;

    [Header("UI references")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private Image healImage;
    [SerializeField] private TextMeshProUGUI healText;

    [Header("Other References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject pivotPoint;

    private bool rollCoroutineRunning = false;
    private bool rollStamCooldown = false;

    private float moveSpeed;

    private int currentHealingCharges;
    private bool healCoroutineRunning = false;

    private int currentAttackChainCount = 0;
    private bool attackCoroutineRunning = false;
    

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
        if (sprint.action.IsPressed() && stamina > 0 && !healCoroutineRunning)
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
        else if (healCoroutineRunning)
        {
            moveSpeed = walkSpeed / 3;
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

    private IEnumerator PlayerRoll()
    {
        rollCoroutineRunning = true;
        rollStamCooldown = true;

        float whileTime = startTime;

        //Turn moveDirection into a vector3
        Vector3 moveDirection3 = new Vector3(moveDirection.x, 0, moveDirection.y);

        //transform worldDirection from local direction to world direction
        Vector3 worldDirection = worldDirTransform.TransformDirection(moveDirection3);

        //Normalize
        worldDirection = worldDirection.normalized;

        while (whileTime <= startTime + rollTime) 
        {
            whileTime += Time.deltaTime;
            
            characterController.Move(rollSpeed * Time.deltaTime * worldDirection);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ModifyStamina(20, false));
        yield return new WaitForSeconds(0.5f);
        rollCoroutineRunning = false;

        yield return new WaitForSeconds(0.5f);
        rollStamCooldown = false;
    }

    private IEnumerator PlayerHeal()
    {
        healCoroutineRunning = true;

        currentHealingCharges -= 1;

        StartCoroutine(ModifyHealth(healAmount, true));

        yield return new WaitForSeconds(1f);

        healCoroutineRunning = false;
    }

    private IEnumerator PlayerAttack()
    {
        attackCoroutineRunning = true;

        switch (currentAttackChainCount)
        {
            case 0:
                Debug.Log("attack 1");
                currentAttackChainCount += 1;
                StartCoroutine(ModifyStamina(staminaDrain, false));
                //do animation
                yield return new WaitForSeconds(1f);
                break;
            case 1:
                Debug.Log("attack 2");
                currentAttackChainCount += 1;
                StartCoroutine(ModifyStamina(staminaDrain, false));
                //do animation
                yield return new WaitForSeconds(1f);
                break;
            case 2:
                Debug.Log("attack 3");
                currentAttackChainCount += 1;
                StartCoroutine(ModifyStamina(staminaDrain, false));
                //do animation
                yield return new WaitForSeconds(1f);
                break;
            case 3: 
                Debug.Log("attack 4");
                currentAttackChainCount = 0;
                StartCoroutine(ModifyStamina(staminaDrain * 2, false));
                //do animation
                yield return new WaitForSeconds(1.5f);
                break;
        }

        attackCoroutineRunning = false;
    }

    private IEnumerator ResetAttackChain()
    {
        yield return new WaitForSeconds(3f);
        if(!attackCoroutineRunning && !attack.action.IsPressed())
        {
            currentAttackChainCount = 0;
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        worldDirTransform = pivotPoint.transform;

        transform.rotation = new Quaternion(0, 0, 0, 1);

        currentHealingCharges = maxHealingCharges;
        healText.text = currentHealingCharges + " / " + maxHealingCharges;
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
        if (!sprint.action.IsPressed() && !staminaCoroutineRunning && !roll.action.IsPressed() && !rollStamCooldown && !attack.action.IsPressed() && !attackCoroutineRunning && stamina < 100 )
        {
            StartCoroutine(ModifyStamina(staminaDrain * 2, true));
        }

        if (roll.action.IsPressed() && !rollCoroutineRunning && stamina >= 20)
        {
            startTime = Time.time;
            StartCoroutine(PlayerRoll());
        }

        if(heal.action.IsPressed() && currentHealingCharges > 0 && !healCoroutineRunning)
        {
            StartCoroutine(PlayerHeal());
        }

        if(attack.action.IsPressed() &&  !attackCoroutineRunning && stamina > 0)
        {
            StartCoroutine(PlayerAttack());
            StartCoroutine(ResetAttackChain());
        }

        //Update UI
        float stamBarFillAmount = stamina / 100;
        staminaBar.fillAmount = stamBarFillAmount;
        staminaText.text = stamina + " / 100";

        float healthBarFillAmount = health / 100;
        healthBar.fillAmount = healthBarFillAmount;
        healthText.text = health + " / 100";
    }

    private void FixedUpdate()
    {
        healText.text = currentHealingCharges + " / " + maxHealingCharges;
    }
}
