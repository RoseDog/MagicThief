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
            guard.FaceTarget(guard.spot.target);
            guard.anim.Stop();
            guard.anim.Play("A");
            isAtkCDing = true;
            Invoke("AtkCDOver", atkCd);
        }
        else
        {
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
        //guard.FaceTarget(guard.spot.target);
        checkTargetStillClose();
    }

    void AtkCDOver()
    {
        isAtkCDing = false;
        if (guard.currentAction == this)
        {
            if (checkTargetStillClose())
            {
                Attack();
            }
        }
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