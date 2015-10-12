public class Attack : GuardAction
{
    bool isAtkCDing = false;
    Cocos2dAction checkFunc;
    
    public override void Excute()
    {
        base.Excute();
        Atk();
    }    

    public override void Stop()
    {
        UnityEngine.Debug.Log("guard stop attack");
        base.Stop();
        guard.RemoveAction(ref checkFunc);        
    }

    void Atk()
    {
        guard.RemoveAction(ref checkFunc);
        if (!isAtkCDing)
        {
            UnityEngine.Debug.Log("Guard Attacking");
            

            AtkAnimation();

            isAtkCDing = true;
            guard.SleepThenCallFunction(guard.data.atkCd, () => AtkCDOver());

            System.String content = gameObject.name;
            content += " Attack";
            Globals.record("testReplay", content);
        }
        else
        {
            UnityEngine.Debug.Log("Guard Attacking Gap");
            if (guard.spriteSheet.HasAnimation("atkReady"))
            {
                guard.spriteSheet.Play("atkReady");
            }
            else
            {
                guard.spriteSheet.Play("idle");
            }
        }
    }    

    protected bool checkIfTargetPressDown()
    {
        Actor targetMage = guard.spot.target.GetComponent<Actor>();
        Magician mage = targetMage as Magician;
        if (mage == null)
        {
            return false;
        }
        if (mage.gameObject.layer == 11 && mage.currentAction == mage.beenPressDown)
        {
            return true;
        }
        return false;
    }    

    public virtual void AtkEnd()
    {
        UnityEngine.Debug.Log("atk end");        
        if (guard.currentAction == this)
        {
            if (guard.spriteSheet.HasAnimation("atkReady"))
            {
                guard.spriteSheet.Play("atkReady");
            }
            else
            {
                guard.spriteSheet.Play("idle");
            }
            
            checkFunc = guard.RepeatingCallFunction(5, () => DuringAtkIdle());            
        }
    }

    void DuringAtkIdle()
    {
        // 因为check的时候可能会切换动作
        if (guard.currentAction == this)
        {
            checkTargetStillAlive();
            checkTargetStillClose();
        }        
    }

    void AtkCDOver()
    {
        System.String content = gameObject.name;
        content += " AtkCDOver";
        Globals.record("testReplay", content);

        isAtkCDing = false;
        if (guard.currentAction == this)
        {
            if (checkTargetStillClose() && checkTargetStillAlive())
            {
                Atk();
            }
        }
    }

    
    public virtual void AtkAnimation()
    {
       
    }

    public virtual void FireTheHit()
    {
        Actor targetActor = guard.spot.target.GetComponent<Actor>();
        if (targetActor != null)
        {
            UnityEngine.Vector3 magicianDir = targetActor.GetWorldCenterPos() - guard.transform.position;
            if (!checkIfTargetPressDown())
            {
                if (magicianDir.magnitude < guard.data.atkShortestDistance + 50f)
                {
                    UnityEngine.Vector3 faceDir = UnityEngine.Vector3.left;
                    if (transform.localEulerAngles.y > 179)
                    {
                        faceDir = UnityEngine.Vector3.right;
                    }
                    faceDir.z = 0;
                    double angle = UnityEngine.Vector3.Angle(magicianDir, faceDir);
                    if (angle < 100 && angle > -100)
                    {
                        targetActor.hitted.Excute();
                        targetActor.ChangeLife(-guard.data.attackValue);                        
                        targetActor.FaceDir(magicianDir);
                    }
                }
            }
        }        
    }

    public virtual bool checkTargetStillAlive()
    {
        if (guard.spot.target == null || guard.spot.target.GetComponent<Actor>().IsLifeOver())
        {
            guard.wandering.Excute();
            return false;
        }
        return true;
    }

    public virtual bool checkTargetStillClose()
    {
        if (guard.spot.target == null || guard.spot.target.GetComponent<Actor>().IsLifeOver())
        {
            return false;
        }
        System.Diagnostics.Debug.Assert(guard.spot.target != null);

        float atkShortestDistance = guard.data.atkShortestDistance;
        if (guard.spriteSheet.HasAnimation("kick") && Globals.stealingController != null && Globals.stealingController.magician.currentAction == Globals.stealingController.magician.catchByNet)
        {
            atkShortestDistance = 50f;
        }
        if (UnityEngine.Vector3.Distance(guard.spot.target.position, guard.transform.position) > atkShortestDistance)
        {
            guard.chase.Excute();
            return false;
        }
        return true;
    }
}