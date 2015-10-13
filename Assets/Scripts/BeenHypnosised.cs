public class BeenHypnosised : GuardAction
{    
    public TrickTimer timer;
    public override void Awake()
    {
        base.Awake();                
        actor.spriteSheet.AddAnimationEvent("BeenHypnosised", -1, () => Sleeping());
    }

    public void GoToSleep(int duration)
    {       
        base.Excute();
        timer = (UnityEngine.GameObject.Instantiate(Globals.stealingController.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        actor.spriteSheet.Play("BeenHypnosised");
        actor.moving.canMove = false;
        guard.HideBtns();
        guard.spot.target = null;
        guard.RemoveAction(ref guard.spot.outVisionCountDown);
        guard.EnableEyes(false);
        guard.moving.ClearPath();
        guard.gameObject.layer = 0;

        
        timer.BeginCountDown(gameObject, duration, new UnityEngine.Vector3(0, 100f, 0));

        guard.SleepThenCallFunction(duration, ()=>Stop());
    }

    public void Sleeping()
    {
        actor.spriteSheet.Play("Sleeping");
    }

    public override void Stop()
    {
        base.Stop();
        if (timer != null)
        {
            Actor.to_be_remove.Add(timer);
            timer = null;
        }        
        guard.gameObject.layer = 13;
        guard.wakeFromHypnosised.Excute();
    }
}
