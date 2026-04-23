using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossController : EntityClass
{
    [Header("Navmesh stuff")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTarget;

    [Header("Boss variables")]
    [SerializeField] private float stopDistance = 5;

    private int currentAttackChainCount = 0;
    private bool attacking = false;

    private IEnumerator BasicAttack()
    {
        attacking = true;

        Debug.Log("boss attack 1");
        currentAttackChainCount += 1;
        //StartCoroutine(ModifyStamina(staminaDrain, false));
        //do animation
        yield return new WaitForSeconds(1f);

        Debug.Log("boss attack 2");
        currentAttackChainCount += 1;
        //StartCoroutine(ModifyStamina(staminaDrain, false));
        //do animation
        yield return new WaitForSeconds(1f);

        Debug.Log("boss attack 3");
        currentAttackChainCount += 1;
        //StartCoroutine(ModifyStamina(staminaDrain, false));
        //do animation
        yield return new WaitForSeconds(1f);

        Debug.Log("boss attack 4");
        currentAttackChainCount = 0;
        //StartCoroutine(ModifyStamina(staminaDrain * 2, false));
        //do animation
        yield return new WaitForSeconds(1.5f);

        attacking = false;
    }

    private IEnumerator RunningAttack()
    {
        attacking = true;

        Debug.Log("Running attack");
        yield return new WaitForSeconds(4f);

        attacking = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerTarget);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

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
        if(!attacking && Vector3.Distance(transform.position, playerTarget.position) > stopDistance * 2 && Random.Range(1, 30) == 1)
        {
            StartCoroutine(RunningAttack());
        }

        if (!attacking && Vector3.Distance(transform.position, playerTarget.position) < stopDistance && Random.Range(1, 20) == 1)
        {
            StartCoroutine(BasicAttack());
        }
    }
}
