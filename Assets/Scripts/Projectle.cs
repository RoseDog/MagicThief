public class Projectle : Actor
{
    protected Actor targetActor;
    protected Cocos2dAction jumpAction;
    protected double hit_target_dis;
    protected int jumpDuration;

    protected bool activeNet = false;
    
    public void Fire(Actor target)
    {
        targetActor = target;        
        jumpAction = new Sequence(new JumpTo(transform, targetActor.transform.position, 100.0f,
            jumpDuration),
            new FunctionCall(() => HitMiss()));
        AddAction(jumpAction);
        AddAction(SleepThenCallFunction((int)(jumpDuration * 0.7f), () => Active()));
    }

    void Active()
    {
        activeNet = true;

        System.String content_test = gameObject.name;
        content_test += " Active";
        Globals.record("testReplay", content_test);
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        if (activeNet)
        {
            if (targetActor != null && !targetActor.IsLifeOver())
            {
                float tar_dis = UnityEngine.Vector3.Distance(transform.position, targetActor.transform.position);
                if (Globals.DEBUG_REPLAY)
                {
                    System.String content_test = gameObject.name + " target distance " + transform.position.ToString("F5") + targetActor.transform.position.ToString("F5");
                    Globals.record("testReplay", content_test);
                }
                
                if (tar_dis < hit_target_dis)
                {
                    HitTarget();
                }
            }
        }
    }

    public virtual void HitTarget()
    {

    }

    void HitMiss()
    {
        UnityEngine.Debug.Log("NetCatchMiss");
        jumpAction = null;
        
        to_be_remove.Add(this);

        System.String content = gameObject.name;
        content += " NetCatchMiss";
        Globals.record("testReplay", content);
    }

}
