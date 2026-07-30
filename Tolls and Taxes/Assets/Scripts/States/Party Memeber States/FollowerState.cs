using UnityEngine;

/// <summary>
/// The state for party members who aren't the party leader.
/// </summary>
public class FollowerState : State
{
    // Owner reference for casting.
    protected PartyMember partyMember;
    public Vector3 DebugTarget;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set up owner and cast.
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

    /// <inheritdoc/>
    public override void EnterState()
    {
        // Set up owner and cast
        Owner = this.GetComponent<Character>();
        partyMember = Owner as PartyMember;
        partyMember.Crumb = this.transform.position;
    }

    /// <inheritdoc/>
    public override void ExitState()
    {
        
    }

    /// <inheritdoc/>
    public override void UpdateState()
    {
        // Use queue and follow the vector crumbs.
        switch (partyMember.Status)
        {
            case PartyMember.LineStatus.middle:
                partyMember.FollowTarget = partyMember.PreviousMember.FollowCrumbs.Dequeue();
                DebugTarget = partyMember.PreviousMember.FollowTarget;
                CheckDistance();
                partyMember.FollowCrumbs.Enqueue(partyMember.Crumb);
                break;
            case PartyMember.LineStatus.second:
                partyMember.FollowTarget = partyMember.Leader.FollowCrumbs.Dequeue();
                DebugTarget =  partyMember.FollowTarget;
                CheckDistance();
                partyMember.FollowCrumbs.Enqueue(partyMember.Crumb);
                break;
        }
        partyMember.Crumb = this.transform.position;
    }

    public override void UnsubcribeState()
    {
        
    }

    public override void ResubscribeState()
    {
        
    }

    /// <summary>
    /// Checks if it's time to follow the person in front
    /// </summary>
    public bool CheckDistance()
    {
        Vector3 MoveDirection = partyMember.FollowTarget - this.transform.position;
        
        // Ensure that we don't get to far away from the party leader without being on top of him.
        if (MoveDirection.sqrMagnitude > partyMember.distance * partyMember.distance)
        {
            // Let's get moving!
            Owner.body.linearVelocity = MoveDirection.normalized * partyMember.Leader.speed;
            if (MoveDirection.x < 0)
            {
                Owner.spriteRenderer.flipX = true;
            } else if (MoveDirection.x > 0)
            {
                Owner.spriteRenderer.flipX = false;
            }
            return true;
        }
        Owner.body.linearVelocity = Vector2.zero;
        return false;
    }
}
