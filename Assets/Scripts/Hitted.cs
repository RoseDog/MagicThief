public class Hitted : Action 
{
    public int LifeAmount = 100;
    public int LifeCurrent;
    public override void Awake()
    {
        LifeCurrent = LifeAmount;
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

    public virtual void ChangeLife(int delta)
    {
        LifeCurrent += delta;
        LifeCurrent = UnityEngine.Mathf.Clamp(LifeCurrent, 0, LifeAmount);
        if (LifeCurrent < UnityEngine.Mathf.Epsilon)
        {
            actor.lifeOver.Excute();
        }
        else
        {
            Excute();
        }
    }
    

    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        Stop();
        actor.currentAction = null;
    }

    public virtual void ResetLife()
    {
        LifeCurrent = LifeAmount;        
    }
}
