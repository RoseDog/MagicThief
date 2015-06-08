public class SpiderNet : Actor 
{
    Actor targetActor;
    Cocos2dAction jumpAction;
    UnityEngine.Vector3 rushingDir;
    double catch_mage_dis = 1.1f;
    int jumpDuration = 60;

    bool activeNet = false;
    public Spider spider;
    public void Fire(Actor target)
    {
        targetActor = target;
        UnityEngine.Debug.Log("Net Fired");
        rushingDir = targetActor.transform.position - transform.position;
        jumpAction = new Sequence(new JumpTo(transform, targetActor.transform.position, 1.0f,
            jumpDuration),
            new FunctionCall(() => NetCatchMiss()));
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
                if (UnityEngine.Vector3.Distance(transform.position, targetActor.transform.position) < catch_mage_dis)
                {
                    DestroyObject(gameObject);

                    if (targetActor.catchByNet != null)
                    {
                        targetActor.catchByNet.Catched(spider);
                    }                    

                    System.String content = gameObject.name;
                    content += " net mage";
                    Globals.record("testReplay", content);
                }
            }
        }
    }

    void NetCatchMiss()
    {
        UnityEngine.Debug.Log("NetCatchMiss");
        jumpAction = null;

        DestroyObject(gameObject);

        System.String content = gameObject.name;
        content += " NetCatchMiss";
        Globals.record("testReplay", content);
    }

}
