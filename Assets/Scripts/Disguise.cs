public class Disguise : MagicianTrickAction 
{
    double speedCache;
    UnityEngine.GameObject FakeGuard_prefab;
    UnityEngine.GameObject FakeGuard;
    UnityEngine.GameObject TrickTimerPrefab;
    UnityEngine.GameObject TrickTimer;
    SpriteSheet sheetCache = null;
    Cocos2dAction stopAction;
    Cocos2dAction trickCastAction;
    public UnityEngine.AudioClip disguise;
    UnityEngine.Vector3 shadowPosCache;
    public override void Awake()
    {
        base.Awake();
        FakeGuard_prefab = UnityEngine.Resources.Load("Avatar/FakeGuard") as UnityEngine.GameObject;
        TrickTimerPrefab = UnityEngine.Resources.Load("UI/FakeGuardTimer") as UnityEngine.GameObject;
        shadowPosCache = actor.shadow.transform.localPosition;        
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
            trickCastAction = actor.SleepThenCallFunction(mage.spriteSheet.GetAnimationLengthWithSpeed("disguise"), () => TrickActionEnd());
        }

        mage.audioSource.PlayOneShot(disguise);
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
        if(Globals.guardPlayer.isBot)
        {
            actor.spriteSheet._sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/FakeGuard_pve_Sprite");
            actor.spriteSheet._actor = actor;
            actor.spriteSheet.initialized = true;
            actor.spriteSheet.AddAnim("idle", 4, 1.5f);
            actor.spriteSheet.AddAnim("moving", 4, 1.8f);
            actor.spriteSheet.AddAnim("open_chest", 4);
            actor.spriteSheet.AddAnim("take_money", 16);
        }
        else
        {
            actor.spriteSheet._sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/FakeGuard_Sprite");
            actor.spriteSheet._actor = actor;
            actor.spriteSheet.initialized = true;
            actor.spriteSheet.AddAnim("idle", 4, 1.5f);
            actor.spriteSheet.AddAnim("moving", 6, 1.8f);
            actor.spriteSheet.AddAnim("open_chest", 4);
            actor.spriteSheet.AddAnim("take_money", 16);
        }
        
        actor.shadow.transform.localPosition = new UnityEngine.Vector3(0.041f,0.01f,0);
        (actor as Magician).OnKeyUpR("");

        TrickTimer = UnityEngine.GameObject.Instantiate(TrickTimerPrefab) as UnityEngine.GameObject;
        TrickTimer.GetComponent<TrickTimer>().BeginCountDown(FakeGuard, data.duration, new UnityEngine.Vector3(0, 110f, 0));

        (actor as Magician).speedModifier = 0.5f;
        gameObject.layer = 23;

        stopAction = actor.SleepThenCallFunction(data.duration, () => Stop());
        actor.moving.canMove = true;

//         // 易容可以逃脱狗和Armed
//         foreach (Guard guard in Globals.maze.guards)
//         {
//             if (guard.data.name != "guard" && guard.spot != null && guard.spot.target == actor.transform)
//             {
//                 guard.wandering.Excute();
//             }
//         }
    }
   
    public override void Stop()
    {
        base.Stop();
        actor.RemoveAction(ref stopAction);
        actor.RemoveAction(ref trickCastAction);
        actor.shadow.transform.localPosition = shadowPosCache;
        
        if (sheetCache != null)
        {                        
            DestroyObject(FakeGuard);
            Actor.to_be_remove.Add(TrickTimer.GetComponent<Actor>());
            sheetCache.enabled = true;
            actor.spriteSheet = sheetCache;
            (actor as Magician).speedModifier = 1.0f;
            gameObject.layer = 11;
            sheetCache = null;
        }               
    }
}
