public class Hitted : Action 
{    
    public override void Awake()
    {
        base.Awake();
        if (!Globals.AvatarAnimationEventNameCache.Contains(actor.name + "-repel"))
        {
            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "hitteAnimEnd";
            evt.time = animation["repel"].length;
            animation["repel"].clip.AddEvent(evt);
            Globals.AvatarAnimationEventNameCache.Add(actor.name + "-repel");
        }
    }
	public override void Excute()
    {
        UnityEngine.Debug.Log("hitted");
        base.Excute();
        actor.anim.Play("repel");                
    }
       
    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        Stop();
        actor.currentAction = null;
    }    
}
