using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartyLeader : Character
{
    [NonSerialized]
    public string LeaderName = "Samantha Pel";
    [NonSerialized]
    public int HP;
    [NonSerialized]
    public int MP;
    public static event Action<Vector2> Interaction;
    public static event Action<Vector2> Attack;
    public static event Action<Vector2,int> AOEAttack;
    public Deck Deck = new Deck();
    public PartyMember nextMember;
    public PartyMember LastMember;
    public Queue<Vector3> followCrumbs = new Queue<Vector3>();
    [NonSerialized]
    public Vector3 crumbPosition;
    public int speed;
    
    //Card UI Stuff
    public GameObject cardTray;
    public List<CardUI> cards = new List<CardUI>();
    
    //Effect Tracking
    public Dictionary<int, Coroutine> effectRoster = new Dictionary<int, Coroutine>();

    private void Awake()
    {
        followCrumbs.Enqueue(this.transform.position);
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(LeaderName);
        //Temp Renderer
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Deck = data.Deck;
        Deck.Shuffle();
        HP = data.CurrentHp;
        MP = data.CurrentMp;
        CurrentState = this.AddComponent<PlayerMovement>();
        body = this.GetComponent<Rigidbody2D>();
        crumbPosition = this.transform.position;
        CurrentState.EnterState();
        speed = DataCenter.Instance.Allies[LeaderName].Speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        followCrumbs.Enqueue(crumbPosition);
        crumbPosition = this.transform.position;
    }

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

    IEnumerator effect(float time, int id)
    {
        yield return new WaitForSeconds(time);
        removeEffect(id);
    }

    public void applyEffect(float time, int id)
    {
        if (effectRoster.ContainsKey(id))
        {
            StopCoroutine(effectRoster[id]);
            removeEffect(id);
        }
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
        switch (id)
        {
            case 7:
                speed = speed / 2;
                break;
        }
    }
}
