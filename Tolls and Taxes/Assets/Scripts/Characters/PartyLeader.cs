using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] public DeckViewOverLay DeckMenu;
    
    //Speed stat.
    public int speed;
    private bool isDoubled;
    
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
        Foe.PreBattleProcessing += FoeOnPreBattleProcessing;
        Deck = data.Deck;
        Deck.Shuffle();
        HP = data.CurrentHp;
        MP = data.CurrentMp;
        Level =  data.CurrentLevel;
        XP = data.CurrentXp;
        CurrentState = this.AddComponent<PlayerMovement>();
        body = this.GetComponent<Rigidbody2D>();
        Crumb = this.transform.position;
        CurrentState.EnterState();
        speed = DataCenter.Instance.Allies[MemberName].Speed;
        isDoubled = false;
    }
    

    private void FoeOnPreBattleProcessing()
    {
        TransferCenter.Instance.SaveCharacterState(MemberName, Deck, HP, MP,Level,XP);
        PartyMember memeber = NextMember;
        while (memeber != this)
        {
            TransferCenter.Instance.SaveCharacterState(memeber.MemberName, memeber.Deck, memeber.HP, memeber.MP, memeber.Level, memeber.XP);
            memeber = memeber.NextMember;
        }
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

    public void ApplySpeedBoost()
    {
        if (!isDoubled)
        {
            isDoubled = true;
            speed = speed * 2;
        }
    }

    public void RemoveSpeedBoost()
    {
        if (isDoubled)
        {
            isDoubled = false;
            speed = speed / 2;
        }
    }
    
    public override void OnDestroy()
    {
        foreach (var item in effectRoster)
        {
            StopCoroutine(item.Value);
        }
        CurrentState.ExitState();
        SceneManager.LoadScene("Prototype GameOver");
    }

}
