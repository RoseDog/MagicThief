public class BeenHypnosised : GuardAction
{    
    TrickTimer timer;
    public override void Awake()
    {
        base.Awake();        
        guard.spriteSheet.CreateAnimationByName("BeenHypnosised",0.5f, true);
    }

    public void GoToSleep(int duration)
    {
        base.Excute();
        actor.spriteSheet.Play("BeenHypnosised");
        actor.moving.canMove = false;
        guard.HideBtns();
        guard.spot.target = null;
        guard.RemoveAction(ref guard.spot.outVisionCountDown);
        guard.EnableEyes(false);
        guard.gameObject.layer = 0;

        timer = (UnityEngine.GameObject.Instantiate(Globals.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(gameObject, duration, new UnityEngine.Vector3(0, 1.0f, 0));

        guard.SleepThenCallFunction(duration, ()=>Stop());
    }

    public override void Stop()
    {
        base.Stop();
        DestroyObject(timer.gameObject);
        guard.gameObject.layer = 13;
        guard.wakeFromHypnosised.Excute();
    }
}
