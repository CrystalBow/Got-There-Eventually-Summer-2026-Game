using UnityEngine;

public class Patrol : State
{

    private bool _isEnemy;
    private int _currentPartrolRouteIndex;
    
    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        _isEnemy = Owner is Foe;
        _currentPartrolRouteIndex = 0;
    }

    public override void ExitState()
    {
        Destroy(this);
    }

    public override void UpdateState()
    {
        if (Owner == null)
        {
            return;
        }

        Vector2 direction = Owner.PartrolRoute[_currentPartrolRouteIndex].transform.position - Owner.transform.position;
        Debug.DrawRay(Owner.transform.position, direction, Color.red);
        float angle = Vector2.SignedAngle(Vector2.left, direction);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (direction.magnitude < 1f)
        {
            _currentPartrolRouteIndex++;
            if (_currentPartrolRouteIndex >= Owner.PartrolRoute.Count)
            {
                _currentPartrolRouteIndex = 0;
            }
        }
        else
        {
            Owner.body.linearVelocity = direction.normalized * 2;
        }
    }

    public override void UnsubcribeState()
    {
        
    }

    public override void ResubscribeState()
    {
        
    }

    public void Update()
    {
        UpdateState();
    }
}
