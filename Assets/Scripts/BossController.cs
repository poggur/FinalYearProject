using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossController : EntityClass
{
    [Header("Boss object references")]
    [SerializeField] private Animator animator;

    [Header("Navmesh stuff")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTarget;

    [Header("Boss variables")]
    [SerializeField] private float stopDistance = 5;

    [Header("UI references")]
    [SerializeField] private Image healthBar;


    private bool coroutineRunning = false;
    private int currentAttackChainCount = 0;
    private bool attacking = false;

    private IEnumerator BasicAttack()
    {
        coroutineRunning = true;

        attacking = true;
        animator.SetBool("Attack1", true);
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(5.01f);

        animator.SetBool("Attack1", false);
        animator.SetBool("IsAttacking", false);

        attacking = false;

        yield return new WaitForSeconds(1.5f);

        coroutineRunning = false;
    }

    private IEnumerator SecondAttack()
    {
        coroutineRunning = true;

        attacking = true;

        animator.SetBool("Attack2", true);
        animator.SetBool("IsAttacking", true);

        Debug.Log("Attack2");
        yield return new WaitForSeconds(4.5f);

        animator.SetBool("Attack2", false);
        animator.SetBool("IsAttacking", false);

        attacking = false;

        yield return new WaitForSeconds(2.5f);

        coroutineRunning= false;
    }

    public void TookDamage(int attackDamage)
    {
        StartCoroutine(ModifyHealth(attackDamage, false));
        Debug.Log(health);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float healthBarFillAmount = health / 100;
        healthBar.fillAmount = healthBarFillAmount;

        transform.LookAt(playerTarget);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0);

        transform.position = new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z);

        if(attacking)
        {
            stopDistance = 5;
        }
        else if (!attacking)
        {
            stopDistance = 8;
        }

        if (Vector3.Distance(transform.position, playerTarget.position) > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }   
        if(Vector3.Distance(transform.position, playerTarget.position) < stopDistance)
        {
            agent.isStopped = true;
        }
    }

    private void FixedUpdate()
    {
        if (!attacking && Vector3.Distance(transform.position, playerTarget.position) < stopDistance && Random.Range(1, 20) == 1 && !coroutineRunning)
        {
            int randomNumber = Random.Range(1, 3);

            Debug.Log(randomNumber);

            switch(randomNumber)
            {
                case 2:
                    StartCoroutine(BasicAttack());
                    break;

                case 1:
                    StartCoroutine(SecondAttack());
                    break;
                default:
                    break;
            }

            
        }
    }
}
