public class WakeUp : GuardAction 
{
    public override void Awake()
    {
        base.Awake();
        if (!Globals.AvatarAnimationEventNameCache.Contains(actor.name + "-wakeUp"))
        {
            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "wakeUpEnd";
            evt.time = animation["wakeUp"].length;
            animation["wakeUp"].clip.AddEvent(evt);
            Globals.AvatarAnimationEventNameCache.Add(actor.name + "-wakeUp");
        }
    }

    public override void Excute()
    {
        base.Excute();
        actor.anim.Play("wakeUp");
    }

    public void wakeUpEnd()
    {
        Stop();        
        actor.moving.canMove = true;
        guard.EnableEyes(true);
        guard.wandering.Excute();
    }
}
