public class FlyUp : MagicianTrickAction
{
    Guard target;
    
    UnityEngine.Vector3 updis = new UnityEngine.Vector3(0,200f,0);
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
        actor.spriteRenderer.material = (actor as Magician).flyingMat;
        actor.moving.ClearPath();
        actor.moving.canMove = false;
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
        actor.gameObject.layer = 26;        
        Globals.maze.GuardsTargetVanish(gameObject);

        actor.AddAction(new MoveTo(actor.transform, actor.transform.position + updis, up_duration));
        actor.AddAction(new MoveTo(shadow.transform, -updis / 200, up_duration));
        actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache / 2, up_duration));
        destination = actor.transform.position;
        actor.spriteSheet.Play("flyup_1");
        actor.SleepThenCallFunction(up_duration, () => UnfoldUmbrella());
    }

    public void UnfoldUmbrella()
    {
        actor.spriteSheet.Play("flyup_2");
        actor.AddAction(new MoveTo(actor.transform, actor.transform.position - new UnityEngine.Vector3(0, 40f, 0), actor.spriteSheet.GetAnimationLengthWithSpeed("flyup_2")));
    }

    public void InAir()
    {
        timer = (UnityEngine.GameObject.Instantiate(Globals.stealingController.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(gameObject, data.duration, new UnityEngine.Vector3(0, 230f, 0));
        inAirCD = actor.SleepThenCallFunction(data.duration, () => FoldGlider());
        actor.spriteSheet.Play("flying");
        UnityEngine.Debug.Log("flying");
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        if (inAirCD != null)
        {
            UnityEngine.Vector3 dir = (destination + updis) - actor.transform.position;
            if (dir.magnitude > 10f)
            {
                actor.transform.position = actor.transform.position + dir.normalized * mage.data.GetNormalSpeed();
                actor.FaceDir(dir);
            }        
        }        
    }

    public void FoldGlider()
    {                
        Actor.to_be_remove.Add(timer);
        UnityEngine.Debug.Log("FoldGlider");
        Pathfinding.Node node = Globals.maze.pathFinder.GetSingleNode(actor.transform.position - updis, true);
        
        // 可以落下
        if (node != null)
        {
            UnityEngine.GameObject UmbrellaPrefab = UnityEngine.Resources.Load("Avatar/FoldUmbrella") as UnityEngine.GameObject;
            UnityEngine.GameObject umbrella = UnityEngine.GameObject.Instantiate(UmbrellaPrefab) as UnityEngine.GameObject;
            umbrella.transform.position = actor.transform.position
                + new UnityEngine.Vector3(0,100f,0);

            actor.spriteSheet.Play("falling_success");
            actor.AddAction(new Sequence(
                new MoveTo(actor.transform, actor.transform.position - updis, 20),
                new FunctionCall(() => SuccessDownOnFloor())));
            actor.AddAction(new MoveTo(shadow.transform, shadowPosCache, 20));
            actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache, 20));
        }
        else
        {
            actor.spriteSheet.Play("falling_failed_loop");
            UnityEngine.Vector3 fall_pos = Globals.GetPathNodePos(Globals.maze.pathFinder.GetNearestWalkableNode(actor.transform.position - updis));
            actor.AddAction(new Sequence(
                new MoveToWithSpeed(actor.transform, fall_pos, 5f),
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
        actor.gameObject.layer = 11;
        actor.spriteRenderer.gameObject.layer = 11;

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
        content += " " + transform.position.ToString("F3");
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
        actor.spriteRenderer.material = (actor as Magician).groundMat;
        base.Stop();
    }
}
