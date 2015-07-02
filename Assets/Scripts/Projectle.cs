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
        jumpAction = new Sequence(new JumpTo(transform, targetActor.transform.position, 1.0f,
            jumpDuration),
            new FunctionCall(() => HitMiss()));
        AddAction(jumpAction);
        AddAction(SleepThenCallFunction((int)(jumpDuration * 0.7f), () => Active()));
    }

    void Active()
    {
        activeNet = true;
    }

    public override void Update()
    {
        base.Update();
        if (activeNet)
        {
            if (targetActor != null && !targetActor.IsLifeOver())
            {
                if (UnityEngine.Vector3.Distance(transform.position, targetActor.transform.position) < hit_target_dis)
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

        DestroyObject(gameObject);

        System.String content = gameObject.name;
        content += " NetCatchMiss";
        Globals.record("testReplay", content);
    }

}
