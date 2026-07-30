using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private Collider2D triggerZone;
    private ContactFilter2D  contactFilter = new ContactFilter2D();
    public int Damage;
    public float Timing;
    public float damageCooldown;
    private Animator anim;
    private float timer;
    private bool isWaiting;
    private bool damageAvailiable = true;
    private bool isUP;
    

    public void Start()
    {
        anim = GetComponent<Animator>();
        triggerZone = GetComponent<Collider2D>();
        timer = 0;
        isUP = false;
    }

    public void OnEnable()
    {
        StopAllCoroutines();
        isWaiting = false;
        damageAvailiable = true;
        isUP = false;
    }

    public void Update()
    {
        if (!isWaiting)
        {
            if (isUP)
            {
                isUP = false;
            }
            else
            {
                isUP = true;
            }
            isWaiting = true;
            StartCoroutine(WaitForChange());
            anim.SetBool("isUP", isUP);
        }

        if (isUP && damageAvailiable)
        {
            List<Collider2D> result = new List<Collider2D>();
            triggerZone.Overlap(contactFilter,result);
            foreach (Collider2D coll in result)
            {
                if (coll.gameObject.CompareTag("Player"))
                {
                    coll.GetComponent<PartyMember>().TakeDamage(Damage);
                }
                damageAvailiable = false;
                StartCoroutine(WaitForDamage());
            }
        }
    }
    
    IEnumerator WaitForChange()
    {
        yield return new WaitForSeconds(Timing);
        isWaiting = false;
    }

    IEnumerator WaitForDamage()
    {
        yield return new WaitForSeconds(damageCooldown);
        damageAvailiable = true;
    }
    
}
