using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
/// <summary>
/// For the party leader during environment exploration
/// </summary>
public class PartyLeader : PartyMember
{
    // Events for Attacks and interactions
    public static event Action<Vector2> Interaction;
    public static event Action<Vector2> Attack;
    public static event Action<Vector2,int> AOEAttack;
    
    //Speed stat.
    public int speed;
    
    //Start the queue
    private void Awake()
    {
        FollowCrumbs.Enqueue(this.transform.position);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(MemberName);
        //Temp Renderer
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Deck = data.Deck;
        Deck.Shuffle();
        HP = data.CurrentHp;
        MP = data.CurrentMp;
        CurrentState = this.AddComponent<PlayerMovement>();
        body = this.GetComponent<Rigidbody2D>();
        Crumb = this.transform.position;
        CurrentState.EnterState();
        speed = DataCenter.Instance.Allies[MemberName].Speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Set up the crumbs
        FollowCrumbs.Enqueue(Crumb);
        Crumb = this.transform.position;
    }
    
    // ------------------------------------------------- Activation Functions
    // They expose the actions as public methods, so states can use them.
    public void InteractionActivation()
    {
        Interaction?.Invoke(new Vector2(this.transform.position.x, this.transform.position.y));
    }

    public void AttackActivation()
    {
        Attack?.Invoke(new Vector2(this.transform.position.x, this.transform.position.y));
    }

    public void AOEAttackActivation(int damage)
    {
        AOEAttack?.Invoke(new Vector2(this.transform.position.x, this.transform.position.y), damage);
    }
    
    // ----------------------------------------------------- Effects stuff
    //Coroutine for effects 
    IEnumerator effect(float time, int id)
    {
        yield return new WaitForSeconds(time);
        removeEffect(id);
    }

    public void applyEffect(float time, int id)
    {
        if (effectRoster.ContainsKey(id))
        {
            //Override duplicates to prevent stacking
            StopCoroutine(effectRoster[id]);
            removeEffect(id);
        }
        // Logic to make it happen.
        switch (id)
        {
            case 7:
                speed = speed * 2;
                break;
        }
        effectRoster.Add(id, StartCoroutine(effect(time, id)));
    }
    
    
    public void removeEffect(int id)
    {
        effectRoster.Remove(id);
        //Logic upon removal.
        switch (id)
        {
            case 7:
                speed = speed / 2;
                break;
        }
    }
}
