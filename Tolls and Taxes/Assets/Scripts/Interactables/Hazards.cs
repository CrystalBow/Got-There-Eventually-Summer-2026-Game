using System;
using System.Collections;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    private Collider2D Dectector;
    private Rigidbody2D Body;
    public int Damage;
    public Vector2 Speed;
    public float cooldown;
    private bool IsActive;

    [HideInInspector] public float maxLifetime;
    private float lifetimeTimer;

    public void Awake()
    {
        Dectector = gameObject.GetComponent<Collider2D>();
        Body = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        float angle = Vector2.SignedAngle(Vector2.right, Speed);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        IsActive = false;
        
        // Reset the timer every time it spawns
        lifetimeTimer = 0f; 
    }

    public void Update()
    {
        Body.linearVelocity = Speed;

        // Track lifetime here! It automatically pauses when the scene is disabled.
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= maxLifetime)
        {
            gameObject.SetActive(false); // Turning it off automatically returns it to the List pool
        }
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
