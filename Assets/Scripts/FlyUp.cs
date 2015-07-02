public class FlyUp : MagicianTrickAction
{
    Guard target;
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 up = new UnityEngine.Vector3(0,2.5f,0);
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 destination = UnityEngine.Vector3.zero;
    Cocos2dAction inAirCD;
    TrickTimer timer;
    UnityEngine.SpriteRenderer shadow;
    UnityEngine.Vector3 shadowPosCache;
    UnityEngine.Vector3 shadowScaleCache;
    public override void Awake()
    {
        base.Awake();

        mage.spriteSheet.AddAnimationEvent("flyup_0", -1, () => Up());
        mage.spriteSheet.AddAnimationEvent("flyup_2", -1, () => InAir());

        shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(gameObject, "shadow");
        shadowPosCache = shadow.transform.localPosition;
        shadowScaleCache = shadow.transform.localScale;
    }

    public override void Excute()
    {
        base.Excute();

        actor.moving.ClearPath();
        actor.moving.canMove = false;
        actor.gameObject.layer = 26;
        actor.spriteRenderer.gameObject.layer = 26;
        actor.spriteSheet.Play("flyup_0");

        Globals.canvasForMagician.HideTricksPanel();

        System.String content = gameObject.name;
        content += " Fly up";
        Globals.record("testReplay", content);
    }
    int up_duration = 10;
    public void Up()
    {
        

        actor.AddAction(new MoveTo(actor.transform, actor.transform.position + up, up_duration));
        actor.AddAction(new MoveTo(shadow.transform, -up / 2, up_duration));
        actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache / 2, up_duration));
        destination = actor.transform.position;
        actor.spriteSheet.Play("flyup_1");
        actor.SleepThenCallFunction(up_duration, () => UnfoldUmbrella());
    }

    public void UnfoldUmbrella()
    {
        actor.spriteSheet.Play("flyup_2");
        actor.AddAction(new MoveTo(actor.transform, actor.transform.position - new UnityEngine.Vector3(0, 0.4f, 0), actor.spriteSheet.GetAnimationLengthWithSpeed("flyup_2")));
    }

    public void InAir()
    {
        timer = (UnityEngine.GameObject.Instantiate(Globals.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(gameObject, data.duration, new UnityEngine.Vector3(0, 1.7f, 0));
        inAirCD = actor.SleepThenCallFunction(data.duration, () => FoldGlider());
        actor.spriteSheet.Play("flying");
        UnityEngine.Debug.Log("flying");
    }

    public void Update()
    {
        if (inAirCD != null)
        {
            UnityEngine.Vector3 dir = (destination + up) - actor.transform.position;
            if (dir.magnitude > 0.1f)
            {
                actor.controller.Move(dir.normalized * (float)actor.moving.speed);
                actor.FaceDir(dir);
            }        
        }        
    }

    public void FoldGlider()
    {        
        DestroyObject(timer.gameObject);
        UnityEngine.Debug.Log("FoldGlider");
        Pathfinding.Node node = Globals.maze.pathFinder.GetSingleNode(actor.transform.position - up, true);
        
        // 可以落下
        if (node != null)
        {
            UnityEngine.GameObject UmbrellaPrefab = UnityEngine.Resources.Load("Avatar/FoldUmbrella") as UnityEngine.GameObject;
            UnityEngine.GameObject umbrella = UnityEngine.GameObject.Instantiate(UmbrellaPrefab) as UnityEngine.GameObject;
            umbrella.transform.position = actor.transform.position
                + new UnityEngine.Vector3(0,1.0f,0);

            actor.spriteSheet.Play("falling_success");
            actor.AddAction(new Sequence(
                new MoveTo(actor.transform, actor.transform.position - up, 20),
                new FunctionCall(() => SuccessDownOnFloor())));
            actor.AddAction(new MoveTo(shadow.transform, shadowPosCache, 20));
            actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache, 20));
        }
        else
        {
            actor.spriteSheet.Play("falling_failed_loop");
            UnityEngine.Vector3 fall_pos = Globals.GetPathNodePos(Globals.maze.pathFinder.GetNearestWalkableNode(actor.transform.position - up));
            actor.AddAction(new Sequence(
                new MoveToWithSpeed(actor.transform, fall_pos, 0.05f),
                new FunctionCall(() => FailedDownOnFloor())));
            actor.AddAction(new MoveToWithSpeed(shadow.transform, shadowPosCache, 0.05f));
            shadow.transform.localScale = shadowScaleCache;
        }

        inAirCD = null;

        System.String content = gameObject.name;
        content += " FoldGlider";
        Globals.record("testReplay", content);
    }

    public void SuccessDownOnFloor()
    {
        standUpAct = actor.SleepThenCallFunction(20, () => StandUp());

        System.String content = gameObject.name;
        content += " SuccessDownOnFloor";
        Globals.record("testReplay", content);
    }
    Cocos2dAction standUpAct;
    public void FailedDownOnFloor()
    {
        actor.gameObject.layer = 11;
        actor.spriteRenderer.gameObject.layer = 11;

        shadow.transform.localPosition = shadowPosCache;
        actor.spriteSheet.Play("falling_failed");
        standUpAct = actor.SleepThenCallFunction(100, ()=>StandUp());

        System.String content = gameObject.name;
        content += " FailedDownOnFloor";
        Globals.record("testReplay", content);
    }

    public void StandUp()
    {
        standUpAct = null;
        actor.spriteSheet.Play("landing");
        actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLength("landing"), () => Landed());
    }

    public void Landed()
    {        
        System.String content = gameObject.name;
        content += " Landed";
        Globals.record("testReplay", content);

        Stop();
    }

    public override void Stop()
    {
        if (standUpAct != null)
        {
            actor.RemoveAction(ref standUpAct);
        }
        actor.moving.canMove = true;
        actor.gameObject.layer = 11;
        actor.spriteRenderer.gameObject.layer = 11;
        Globals.canvasForMagician.ShowTricksPanel();

        base.Stop();
    }
}
