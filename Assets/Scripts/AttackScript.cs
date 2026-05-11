using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [Header("Variables for attack")]
    [SerializeField] private int attack1Damage = 20;
    [SerializeField] private int attack2Damage = 30;

    [Header("Object references")]
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider capsuleCollider;

    [HideInInspector] public int attackDamage = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("IsAttacking"))
        {
            capsuleCollider.enabled = true;

            if (animator.GetBool("Attack1"))
            {
                attackDamage = attack1Damage;
            }

            else if (animator.GetBool("Attack2"))
            {
                attackDamage = attack2Damage;
            }
        }
        else
        {
            capsuleCollider.enabled = false;
        }
    }
}
