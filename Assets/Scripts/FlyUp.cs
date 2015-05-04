public class FlyUp : MagicianTrickAction
{
    Guard target;
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 up = new UnityEngine.Vector3(0,1.5f,0);
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
        mage.spriteSheet.CreateAnimationByName("flyup");
        mage.spriteSheet.AddAnimationEvent("flyup", -1, () => InAir());

        mage.spriteSheet.CreateAnimationByName("flying");
        System.Collections.Generic.List<UnityEngine.Sprite> sprites = new System.Collections.Generic.List<UnityEngine.Sprite>();
        sprites.Add(mage.spriteSheet._animationList["falling"].spriteList[0]);
        sprites.Add(mage.spriteSheet._animationList["falling"].spriteList[1]);
        sprites.Add(mage.spriteSheet._animationList["falling"].spriteList[2]);
        mage.spriteSheet.CreateAnimationBySprites(sprites, "falling_success");
        mage.spriteSheet.CreateAnimationByName("falling_failed");

        sprites.Clear();
        sprites.Add(mage.spriteSheet._animationList["landing"].spriteList[0]);        
        mage.spriteSheet.CreateAnimationBySprites(sprites, "success_down_on_floor");

        mage.spriteSheet.CreateAnimationByName("down_on_floor");
        mage.spriteSheet.CreateAnimationByName("stand_up");

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
        actor.AddAction(new MoveTo(actor.transform, actor.transform.position + up, actor.spriteSheet.GetAnimationLength("flyup")));
        actor.AddAction(new MoveTo(shadow.transform, -up/2, actor.spriteSheet.GetAnimationLength("flyup")));
        actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache / 2, actor.spriteSheet.GetAnimationLength("flyup")));
        
        actor.spriteSheet.Play("flyup");

        destination = actor.transform.position;

        Globals.canvasForMagician.HideTricksPanel();

        System.String content = gameObject.name;
        content += " Fly up";
        Globals.record("testReplay", content);
    }

    public void InAir()
    {
        timer = (UnityEngine.GameObject.Instantiate(Globals.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(gameObject, data.duration, new UnityEngine.Vector3(0, 1.4f, 0));
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
            actor.spriteSheet.Play("falling_success");
            actor.AddAction(new Sequence(
                new MoveTo(actor.transform, actor.transform.position - up, 20),
                new FunctionCall(() => SuccessDownOnFloor())));
            actor.AddAction(new MoveTo(shadow.transform, shadowPosCache, 20));
            actor.AddAction(new ScaleTo(shadow.transform, shadowScaleCache, 20));
        }
        else
        {
            actor.spriteSheet.Play("falling_failed");
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
        actor.spriteSheet.Play("success_down_on_floor");
        actor.SleepThenCallFunction(20, () => StandUp());

        System.String content = gameObject.name;
        content += " SuccessDownOnFloor";
        Globals.record("testReplay", content);
    }
   
    public void FailedDownOnFloor()
    {
        shadow.transform.localPosition = shadowPosCache;        
        actor.spriteSheet.Play("down_on_floor");
        actor.SleepThenCallFunction(60, ()=>StandUp());

        System.String content = gameObject.name;
        content += " FailedDownOnFloor";
        Globals.record("testReplay", content);
    }

    public void StandUp()
    {
        actor.spriteSheet.Play("stand_up");
        actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLength("stand_up"), () => Landed());
    }

    public void Landed()
    {
        actor.moving.canMove = true;
        actor.gameObject.layer = 11;
        actor.spriteRenderer.gameObject.layer = 11;
        Globals.canvasForMagician.ShowTricksPanel();

        System.String content = gameObject.name;
        content += " Landed";
        Globals.record("testReplay", content);

        base.Stop();
    }
}
