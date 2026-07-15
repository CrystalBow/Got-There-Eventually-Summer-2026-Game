using UnityEngine;

public class FollowerState : State
{
    protected PartyMember partyMember;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Owner = this.GetComponent<Character>();
        if (Owner is PartyMember)
        {
            partyMember = Owner as PartyMember;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        partyMember = Owner as PartyMember;
        partyMember.Crumb = this.transform.position;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        switch (partyMember.Status)
        {
            case PartyMember.LineStatus.middle:
                partyMember.FollowTarget = partyMember.PreviousMember.FollowCrumbs.Dequeue();
                CheckDistance();
                partyMember.FollowCrumbs.Enqueue(partyMember.Crumb);
                break;
            case PartyMember.LineStatus.second:
                partyMember.FollowTarget = partyMember.Leader.followCrumbs.Dequeue();
                CheckDistance();
                partyMember.FollowCrumbs.Enqueue(partyMember.Crumb);
                break;
        }
        partyMember.Crumb = this.transform.position;
    }
    
    public bool CheckDistance()
    {
        if ((partyMember.FollowTarget - this.transform.position).sqrMagnitude > partyMember.distance * partyMember.distance)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, partyMember.FollowTarget, partyMember.Leader.speed * Time.deltaTime);
            return true;
        }
        return false;
    }
}
