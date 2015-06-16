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
        if (atkJumpAction != null)
        {
            guard.RemoveAction(ref atkJumpAction);
        }        
        
        guard.spriteSheet.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        guard.spriteRenderer.gameObject.layer = 13;
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

    public void AtkEnd()
    {
        UnityEngine.Debug.Log("atk end");
        atkJumpAction = null;
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
            guard.gameObject.layer = 13;
        }
    }

    void DuringAtkIdle()
    {
        // 因为check的时候可能会切换动作
        if (guard.currentAction == this)
        {
            checkTargetStillAlive();
        }
        if (guard.currentAction == this)
        {
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

    Cocos2dAction atkJumpAction;
    public virtual void AtkAnimation()
    {
        guard.FaceTarget(guard.spot.target);
        if (!checkIfTargetPressDown())
        {
            guard.spriteSheet.Play("Atk");

            atkJumpAction = new Sequence(new JumpTo(transform, guard.spot.target.transform.position, 1.0f,
            guard.spriteSheet.GetAnimationLengthWithSpeed("Atk") + 15), new FunctionCall(() => AtkEnd()));
            guard.AddAction(atkJumpAction);
            guard.spriteRenderer.gameObject.layer = 21;
        }
        else
        {
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

    public virtual void FireTheHit()
    {
        UnityEngine.Vector3 magicianDir = guard.spot.target.position - guard.transform.position;
        Actor targetActor = guard.spot.target.GetComponent<Actor>();
        if (!checkIfTargetPressDown() && !targetActor.IsLifeOver())
        {
            if (magicianDir.magnitude < guard.data.atkShortestDistance + 0.3f)
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
                    targetActor.ChangeLife(-guard.data.attackValue);
                    targetActor.hitted.Excute();
                    targetActor.FaceDir(magicianDir);
                }
            }
        }
    }

    public virtual bool checkTargetStillAlive()
    {
        if (guard.spot.target.GetComponent<Actor>().IsLifeOver())
        {
            guard.wandering.Excute();
            return false;
        }
        return true;
    }

    public virtual bool checkTargetStillClose()
    {
        System.Diagnostics.Debug.Assert(guard.spot.target != null);
        if (UnityEngine.Vector3.Distance(guard.spot.target.position, guard.transform.position) > guard.data.atkShortestDistance)
        {
            guard.chase.Excute();
            return false;
        }
        return true;
    }
}