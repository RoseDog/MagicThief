public class GuardAttack : GuardAction 
{    
    bool isAtkCDing = false;

    public override void Awake()
    {
        base.Awake();
        if (!Globals.AvatarAnimationEventNameCache.Contains(guard.name + "-A"))
        {
            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "AtkEnd";
            evt.time = guard.anim["A"].length;
            guard.anim["A"].clip.AddEvent(evt);
            Globals.AvatarAnimationEventNameCache.Add(guard.name + "-A");
        }
        Globals.Assert(guard.atkCd > guard.anim["A"].length / guard.attackSpeed);
    }

    public override void Excute()
    {
        base.Excute();        
        if (guard.alertSound)
        {
            guard.alertSound.AttackAlert();
        }
        Attack();
    }

    public override void Stop()
    {
        UnityEngine.Debug.Log("guard stop attack");
        base.Stop();
        CancelInvoke("DuringAtkIdle");
        if (guard.alertSound)
        {
            guard.alertSound.StopAttackAlert();
        }
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
            Invoke("AtkCDOver", guard.atkCd);
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
            guard.spot.EnemyOutVision();            
            return false;
        }
        return true;
    }

    bool checkTargetStillClose()
    {
        System.Diagnostics.Debug.Assert(guard.spot.target != null);
        if (UnityEngine.Vector3.Distance(guard.spot.target.position, guard.transform.position) > guard.atkShortestDistance)
        {
            guard.chase.Excute();
            return false;
        }
        return true;
    }    
}