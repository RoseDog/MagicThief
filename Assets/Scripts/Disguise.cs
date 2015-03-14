public class Disguise : MagicianTrickAction 
{
    float speedCache;
    UnityEngine.GameObject FakeGuard_prefab;
    UnityEngine.GameObject FakeGuard;
    UnityEngine.GameObject TrickTimerPrefab;
    UnityEngine.GameObject TrickTimer;
    SpriteSheet sheetCache = null;
    Cocos2dAction stopAction;
    Cocos2dAction trickCastAction;
    public override void Awake()
    {
        base.Awake();
        FakeGuard_prefab = UnityEngine.Resources.Load("Avatar/FakeGuard") as UnityEngine.GameObject;
        TrickTimerPrefab = UnityEngine.Resources.Load("UI/FakeGuardTimer") as UnityEngine.GameObject;
        actor.spriteSheet.CreateAnimationByName("disguise");
    }

    public override void Excute()
    {
        if (actor.currentAction == this)
        {
            TrickTimer.GetComponent<TrickTimer>().AddFrameTime(data.duration);
            actor.RemoveAction(ref stopAction);
            stopAction = actor.SleepThenCallFunction(TrickTimer.GetComponent<TrickTimer>().GetLastFrameTime(), () => Stop());
        }
        else
        {
            base.Excute();
            mage.spriteSheet.Play("disguise");
            trickCastAction = actor.SleepThenCallFunction(mage.spriteSheet.GetAnimationLength("disguise"), () => TrickActionEnd());
        }
    }

    public void TrickActionEnd()
    {
        FakeGuard = UnityEngine.GameObject.Instantiate(FakeGuard_prefab) as UnityEngine.GameObject;
        FakeGuard.transform.position = transform.position;
        FakeGuard.transform.rotation = transform.rotation;
        FakeGuard.transform.parent = transform;
        sheetCache = actor.spriteSheet;
        sheetCache.enabled = false;
        actor.spriteSheet = FakeGuard.GetComponent<SpriteSheet>();
        actor.spriteSheet._sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/FakeGuard_Sprite");
        actor.spriteSheet._actor = actor;
        actor.spriteSheet.initialized = true;
        actor.spriteSheet.CreateAnimationByName("idle");
        actor.spriteSheet.CreateAnimationByName("moving");


        TrickTimer = UnityEngine.GameObject.Instantiate(TrickTimerPrefab) as UnityEngine.GameObject;
        TrickTimer.GetComponent<TrickTimer>().BeginCountDown(FakeGuard, data.duration, new UnityEngine.Vector3(0, 1.1f, 0));

        speedCache = actor.moving.speed;
        actor.moving.speed = 0.03f;
        gameObject.layer = 23;

        stopAction = actor.SleepThenCallFunction(data.duration, () => Stop());
        actor.moving.canMove = true;

        foreach (Guard guard in Globals.maze.guards)
        {
            if ((guard as Dog) != null && guard.spot.target != null)
            {
                guard.wandering.Excute();
            }
        }
    }
   
    public override void Stop()
    {
        base.Stop();
        actor.RemoveAction(ref stopAction);
        actor.RemoveAction(ref trickCastAction);
                
        if (sheetCache != null)
        {
            DestroyObject(FakeGuard);
            DestroyObject(TrickTimer);
            sheetCache.enabled = true;
            actor.spriteSheet = sheetCache;
            actor.moving.speed = speedCache;
            gameObject.layer = 11;
            sheetCache = null;
        }               
    }
}
