public class SpiderNet : Projectle 
{
    public Spider spider;
    public override void Awake()
    {
        base.Awake();
        hit_target_dis = 110f;
        jumpDuration = 10;
        spriteSheet.init();
        spriteSheet.AddAnim("play", spriteSheet._sprites.Length, 5.0f);
        spriteSheet.Play("play");        
    }

    public override void Start()
    {
        base.Start();
        ((jumpAction as Sequence).actions[0] as JumpTo)._changeDir = true;
    }

    public override void HitTarget()
    {
        base.HitTarget();

        Actor.to_be_remove.Add(this);
        RemoveAction(ref jumpAction);

        if (targetActor.catchByNet != null && (targetActor.gameObject.layer == 11 || targetActor.gameObject.layer == 23))
        {
            targetActor.catchByNet.Catched(spider);
        }

        System.String content = gameObject.name;
        content += " net mage";
        Globals.record("testReplay", content);
    }
}
