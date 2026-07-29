using System;
using System.Collections;
using UnityEngine;

public class Hazards : MonoBehaviour
{

    private Collider2D Dectector;
    private Rigidbody2D Body;
    public int Damage;
    public float Speed;
    public float cooldown;
    private bool IsActive;
    

    public void Start()
    {
        Dectector = gameObject.GetComponent<Collider2D>();
        Body = gameObject.GetComponent<Rigidbody2D>();
        IsActive = false;
    }

    public void Update()
    {
        Body.linearVelocity = new Vector2(1*Speed, 0);
    }
    
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsActive)
            {
                other.GetComponent<PartyMember>().TakeDamage(Damage);
                IsActive = true;
                StartCoroutine(damageCoolDown(cooldown));
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsActive)
            {
                other.GetComponent<PartyMember>().TakeDamage(Damage);
                IsActive = true;
                StartCoroutine(damageCoolDown(cooldown));
            }
        }
    }

    IEnumerator damageCoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        IsActive = false;
    }
}
