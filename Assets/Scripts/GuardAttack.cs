using System.Collections;

public class GuardAttack : GuardAction 
{
    public float atkCd = 2.0f;
    bool isAtkCDing = false;

    public override void Excute()
    {
        base.Excute();        
        Attack();
    }

    public override void Stop()
    {
        UnityEngine.Debug.Log("guard stop attack");
        base.Stop();
        CancelInvoke("DuringAtkIdle");
    }

    void Attack()
    {
        CancelInvoke("DuringAtkIdle");
        if (!isAtkCDing)
        {
            UnityEngine.Debug.Log("Guard Attacking");
            guard.FaceTarget(guard.spot.target);
            guard.anim.CrossFade("A");
            isAtkCDing = true;
            Invoke("AtkCDOver", atkCd);
        }
        else
        {
            UnityEngine.Debug.Log("Guard Attacking Gap");
            guard.anim.CrossFade("atkReady");
        }
    }

    public void AtkEnd()
    {
        UnityEngine.Debug.Log("atk end");
        if (guard.currentAction == this)
        {
            guard.anim.CrossFade("atkReady");
            InvokeRepeating("DuringAtkIdle", 0.0f, 0.1f);
        }        
    }

    void DuringAtkIdle()
    {
        checkTargetStillAlive();
        checkTargetStillClose();
    }

    void AtkCDOver()
    {
        isAtkCDing = false;
        if (guard.currentAction == this)
        {
            if (checkTargetStillClose() && checkTargetStillAlive())
            {
                Attack();
            }
        }
    }

    bool checkTargetStillAlive()
    {
        if (guard.spot.target.GetComponent<Actor>().IsLifeOver())
        {
            guard.eyes.EnemyOutVision();
            return false;
        }
        return true;
    }

    bool checkTargetStillClose()
    {
        System.Diagnostics.Debug.Assert(guard.spot.target != null);
        if (UnityEngine.Vector3.Distance(guard.spot.target.position, guard.transform.position) > 3)
        {
            guard.chase.Excute();
            return false;
        }
        return true;
    }    
}