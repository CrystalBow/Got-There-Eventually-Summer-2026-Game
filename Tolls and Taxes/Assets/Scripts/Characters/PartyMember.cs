using System;
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
    public Deck Deck = new Deck();
    
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
    // Effect Tracking
    public Dictionary<int, Coroutine> effectRoster = new Dictionary<int, Coroutine>();
    
    protected override void Start()
    {
        //Fetch from transfer center
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(MemberName);
        //Temp renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Deploy from transfer center
        Deck = data.Deck;
        HP = data.CurrentHp;
        MP = data.CurrentMp;
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
    
    
}
