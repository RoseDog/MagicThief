public class RushAtMagician : GuardAction
{
    Cocos2dAction jumpAction;
    public bool rushing = false;

    int duration_stay_down_floor = 100;
    double rushing_speed = 0.7f;
    double pressing_mage_dis = 0.5f;

    UnityEngine.SpriteRenderer shadow;
    Actor targetActor;
    public override void Awake()
    {
        base.Awake();
        guard.spriteSheet.CreateAnimationByName("walking");
        guard.spriteSheet.CreateAnimationByName("running");

        guard.spriteSheet.CreateAnimationByName("takeoff",0.7f);
        guard.spriteSheet.AddAnimationEvent("takeoff", -1, ()=>JumpUp());
        guard.spriteSheet.CreateAnimationByName("rushing_at_magician", rushing_speed);
        guard.spriteSheet.AddAnimationEvent("rushing_at_magician",0,()=>Rushing());
        guard.spriteSheet.CreateAnimationByName("pushed_away", 1.0f, true);
        guard.spriteSheet.CreateAnimationByName("rushing_miss", 1.0f, true);
        guard.spriteSheet.CreateAnimationByName("stand_up",0.8f);
        guard.spriteSheet.AddAnimationEvent("stand_up", -1, () => Stop());

        guard.spriteSheet.CreateAnimationByName("pressing_mage");

        shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(gameObject, "shadow");        
    }

    public override void Excute()
    {
        base.Excute();
        UnityEngine.Debug.Log("takeoff:" + guard.moving.endReachedDistance.ToString("F2"));
        guard.spriteSheet.Play("takeoff");
        guard.FaceTarget(guard.spot.target);

        UnityEngine.Debug.Log(
            UnityEngine.Vector3.Distance(guard.transform.position, guard.spot.target.position).ToString("F2"));

        System.String content = gameObject.name;
        content += " takeoff";
        Globals.record("testReplay", content);
        shadow.gameObject.SetActive(false);
        targetActor = guard.spot.target.GetComponent<Actor>();
    }
    UnityEngine.Vector3 rushingDir;
    void JumpUp()
    {
        UnityEngine.Debug.Log("JumpUp");
        rushingDir = guard.spot.target.position - guard.transform.position;
        jumpAction = new Sequence(new JumpTo(guard.transform, targetActor.transform.position, 1.0f,
            guard.spriteSheet.GetAnimationLengthWithSpeed("rushing_at_magician") ),
            new FunctionCall(()=>RushMiss()));        
        guard.AddAction(jumpAction);       

        guard.spriteSheet.Play("rushing_at_magician");
        guard.EnableEyes(false);        
    }

    void Rushing()
    {
        rushing = true;
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        if (rushing)
        {            
            if (!targetActor.IsLifeOver())
            {
                if (UnityEngine.Vector3.Distance(guard.transform.position, guard.spot.target.position) < pressing_mage_dis)
                {
                    rushing = false;
                    guard.RemoveAction(ref jumpAction);
                    guard.spriteSheet.Play("pressing_mage");
                    targetActor.beenPressDown.PressedByGuard(guard);

                    System.String content = gameObject.name;
                    content += " pressing_mage";
                    Globals.record("testReplay", content);
                }
            }            
        }
    }    

    public void PushedAway()
    {
        UnityEngine.Debug.Log("pushed_away");

        guard.spriteSheet.Play("pushed_away");
        UnityEngine.Vector3 to = guard.transform.position - rushingDir.normalized * 0.5f;
        // 移动曲线
        Cocos2dAction pushedAwayAction = new Sequence(
            new JumpTo(guard.transform, to, 0.7f,
            guard.spriteSheet.GetAnimationLength("pushed_away")));
        guard.AddAction(pushedAwayAction);
        // 躺一会儿之后站起来
        guard.SleepThenCallFunction(guard.spriteSheet.GetAnimationLength("pushed_away") + duration_stay_down_floor, () => StandUp());

        System.String content = gameObject.name;
        content += " PushedAway";
        Globals.record("testReplay", content);
    }

    void RushMiss()
    {
        rushing = false;
        UnityEngine.Debug.Log("RushMiss");
        jumpAction = null;
        guard.spriteSheet.Play("rushing_miss");
        guard.SleepThenCallFunction(guard.spriteSheet.GetAnimationLength("rushing_miss") + duration_stay_down_floor, () => StandUp());

        System.String content = gameObject.name;
        content += " RushMiss";
        Globals.record("testReplay", content);
    }

    void StandUp()
    {
        shadow.gameObject.SetActive(true);
        UnityEngine.Debug.Log("StandUp");
        guard.EnableEyes(true);        
        guard.spriteSheet.Play("stand_up");

        System.String content = gameObject.name;
        content += " stand_up";
        Globals.record("testReplay", content);
    }

    public override void Stop()
    {
        rushing = false;
        shadow.gameObject.SetActive(true);        
        if (jumpAction != null)
        {
            guard.RemoveAction(ref jumpAction);
        }
        
        base.Stop();
        
        if(guard.beenHypnosised.timer == null)
        {
            // 没被催眠打断的话，才执行这个动作
            guard.wandering.Excute();
        }        
    }
}
