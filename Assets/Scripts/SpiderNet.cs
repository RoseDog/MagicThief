public class SpiderNet : Projectle 
{
    public Spider spider;
    public override void Awake()
    {
        base.Awake();
        hit_target_dis = 1.1f;
        jumpDuration = 60;
    }

    public override void HitTarget()
    {
        base.HitTarget();
        DestroyObject(gameObject);

        if (targetActor.catchByNet != null && targetActor.gameObject.layer == 11)
        {
            targetActor.catchByNet.Catched(spider);
        }

        System.String content = gameObject.name;
        content += " net mage";
        Globals.record("testReplay", content);
    }
}
