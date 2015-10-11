public class ChickenBullet : Projectle
{
    public Guard monkey;
    public override void Awake()
    {
        base.Awake();
        hit_target_dis = 110f;
        jumpDuration = 30;
        spriteSheet.AddAnim("flying", 2);
        spriteSheet.Play("flying");
    }

    public override void HitTarget()
    {
        base.HitTarget();
        Actor.to_be_remove.Add(this);

        targetActor.hitted.Excute();
        targetActor.ChangeLife(-monkey.data.attackValue);
        

        System.String content = gameObject.name;
        content += "chicken bullet hit mage";
        Globals.record("testReplay", content);
    }
}
