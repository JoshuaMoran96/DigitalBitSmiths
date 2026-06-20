using System.Collections;
using UnityEngine;

public interface StateMachine
{
    void Enter(SuperiorJoe joe);

    void Update(SuperiorJoe joe);

    void Exit(SuperiorJoe joe);
}

// --------------
// IDLE STATE
// --------------


public class IdleState : StateMachine
{
    public void Enter(SuperiorJoe joe)
    {
        joe.rb.linearVelocity = new Vector2(0, joe.rb.linearVelocity.y);
    }

    public void Exit(SuperiorJoe joe)
    {
     
    }

    public void Update(SuperiorJoe joe)
    {
        float dist = Mathf.Abs(joe.target.position.x - joe.transform.position.x);
    
        if(dist < joe.chaseDistance)
        {
            joe.ChangeState(new ChaseState());
        }
    }
}

// --------------
// CHASE STATE
// --------------

public class ChaseState : StateMachine
{
    public void Enter(SuperiorJoe joe)
    {
        
    }

    public void Exit(SuperiorJoe joe)
    {
        
    }

    public void Update(SuperiorJoe joe)
    {
        float dist = Mathf.Abs(joe.target.position.x - joe.transform.position.x);
        
        if(dist > joe.chaseDistance)
        {
            joe.ChangeState(new IdleState());
        }

        if(dist < joe.barrageRange && Time.time >= joe.nextAttackTime)
        {
            joe.ChangeState(new BarrageAttackState());
            return;
        }

        if (dist < joe.burstRange && Time.time >= joe.nextAttackTime)
        {
            joe.ChangeState(new BurstAttackState());
        }

        joe.HandleMovement();
        joe.HandleJumping();
    }
}

// --------------
// BURST STATE
// --------------


public class BurstAttackState : StateMachine
{

    bool finished = false;
    public void Enter(SuperiorJoe joe)
    {
       joe.StartCoroutine(BurstRoutine(joe));
    
    }

    IEnumerator BurstRoutine(SuperiorJoe joe)
    {

        for (int i = 0; i < joe.burstCount; i++)
        {
            joe.FireBullet();
            yield return new WaitForSeconds(joe.burstDelay);
        }

        joe.nextAttackTime = Time.time + joe.burstCD;
        finished = true;
        
    }

    public void Exit(SuperiorJoe joe)
    {
        
    }

    public void Update(SuperiorJoe joe)
    {
        if(finished)
        {
            joe.ChangeState(new ChaseState());
        }
    }
}

// --------------
// BARRAGE STATE
// --------------

public class BarrageAttackState : StateMachine
{

    private bool finished = false;

    public void Enter(SuperiorJoe joe)
    {
        joe.StartCoroutine(BarrageAttack(joe));
    }

    IEnumerator BarrageAttack(SuperiorJoe joe)
    {
        for (int i = 0; i < joe.barrageCount; i++)
        {
            joe.FireBullet();
            yield return new WaitForSeconds(joe.barrageDelay);
        }

        joe.nextAttackTime = Time.time + joe.barrageCooldown;
        finished = true;
    }

    public void Exit(SuperiorJoe joe)
    {
     
    }

    public void Update(SuperiorJoe joe)
    {
        if(finished)
        {
            joe.ChangeState(new ChaseState());    
        }
    }
}

