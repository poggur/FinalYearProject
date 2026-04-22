using System.Collections;
using UnityEngine;

public class EntityClass : MonoBehaviour
{
    [Header("Entity max values, default 100")]
    public int maxHealth = 100;
    public int maxStamina = 100;

    //Entity's health and stamina
    [HideInInspector] public float health;
    [HideInInspector] public float stamina;

    //Bool for detecting if hit
    [HideInInspector] public bool hit = false;
    protected bool healthCoroutineRunning = false;
    protected bool staminaCoroutineRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        health = maxHealth;
        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected IEnumerator ModifyHealth(int number, bool adding)
    {
        healthCoroutineRunning = true;
        if (adding)
        {
            //Doing the healing in 3 parts
            for (int i = 0; i < 3; i++)
            {
                health += (number / 3);
                yield return new WaitForSeconds(0.3f);
            }
        }
        else if (!adding)
        {
            health -= number;
            yield return new WaitForSeconds(1f);
        }

        health = Mathf.Clamp(health, 0, 100);
        healthCoroutineRunning = false;
    }

    protected IEnumerator ModifyStamina(int number, bool adding)
    {
        staminaCoroutineRunning = true;
        if (stamina > 0 && !adding)
        {
            stamina -= number;
            stamina = Mathf.Clamp(stamina, 0, 100);
            yield return new WaitForSeconds(0.5f);
        }
        else if (adding)
        {
            yield return new WaitForSeconds(0.75f);
            stamina += number;
            stamina = Mathf.Clamp(stamina, 0, 100);
        }
        staminaCoroutineRunning = false;
    }
}
