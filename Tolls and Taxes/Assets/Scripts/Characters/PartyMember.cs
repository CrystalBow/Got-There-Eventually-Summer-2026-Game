using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartyMember : Character
{
    //Stat Stuff and Deck
    public string MemberName = "Samantha Pel";
    [NonSerialized]
    public int HP;
    [NonSerialized]
    public int MP;
    [NonSerialized] public int XP;
    [NonSerialized] public int Level;
    public Deck Deck = new Deck();

    public static event Action MaficEffciencyExpire;
    public static event Action MaficIneffciencyExpire;
    
    // Linked List Links
    public PartyMember NextMember;
    public PartyMember PreviousMember;
    public PartyLeader Leader;
    
    //Movement Data
    [NonSerialized]
    public LineStatus Status;
    public Queue<Vector3> FollowCrumbs = new Queue<Vector3>();
    [NonSerialized]
    public Vector3 FollowTarget;
    [NonSerialized]
    public Vector3 Crumb;
    public float distance;
    // Movement Enum
    public enum LineStatus
    {
        middle,
        second,
    }
    //Card UI Stuff
    public GameObject cardTray;
    public List<CardUI> cards = new List<CardUI>();
    
    // Arrow UI
    public GameObject ArrowPointer;
    // Effect Tracking
    public Dictionary<int, Coroutine> effectRoster = new Dictionary<int, Coroutine>();
    public Dictionary<int, Coroutine> expiringEffectRoster = new Dictionary<int, Coroutine>();
    
    protected override void Start()
    {
        //Fetch from transfer center
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(MemberName);
        //Base Class Initializations
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        //Deploy from transfer center
        Deck = data.Deck;
        HP = data.CurrentHp;
        MP = data.CurrentMp;
        Level = data.CurrentLevel;
        XP = data.CurrentXp;
        //Shuffle the deck and prep the follow queue
        Deck.Shuffle();
        FollowTarget = this.transform.position;
        FollowCrumbs.Enqueue(this.transform.position);
        if (PreviousMember == null)
        {
            Status = LineStatus.second;
        }
        else
        {
            Status = LineStatus.middle;
        }
        //Initialize the follower state
        CurrentState = this.AddComponent<FollowerState>();
        CurrentState.EnterState();
    }

    protected override void Update()
    {
    }

    public void OnEnable()
    {
        if (CurrentState == null)
        {
            return;
        }
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(MemberName);
        Deck = data.Deck;
        HP = data.CurrentHp;
        MP = data.CurrentMp;
        Level = data.CurrentLevel;
        XP = data.CurrentXp;
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
            expiringEffectRoster.Add(id, effectRoster[id]);
            effectRoster.Remove(id);
            removeEffect(id);
        }
        // Logic to make it happen.
        switch (id)
        {
            case 7:
                Leader.ApplySpeedBoost();
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
                Leader.RemoveSpeedBoost();
                break;
            case 8:
                MaficEffciencyExpire?.Invoke();
                break;
            case 12:
                MaficIneffciencyExpire?.Invoke();
                break;
        }
    }
    
    public void TakeDamage(int damage)
    {
        Double defense = DataCenter.Instance.DefenseCalculation(DataCenter.Instance.Allies[MemberName], Level);
        if (effectRoster.ContainsKey(13)||effectRoster.ContainsKey(10))
        {
            defense = Math.Floor(defense / 2);
        } else if (effectRoster.ContainsKey(6))
        {
            defense *= 2;
        }
        int defesneInt = (int)defense;
        HP -= damage - defesneInt;
        if (HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        foreach (var item in effectRoster)
        {
            StopCoroutine(item.Value);
        }
        effectRoster.Clear();
        expiringEffectRoster.Clear();
        PreviousMember.NextMember = NextMember;
        NextMember.PreviousMember = PreviousMember;
        CurrentState.ExitState();
    }
}
