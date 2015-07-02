public class Explode : GuardAction 
{
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("Atk", 2, () => Fire());
        actor.spriteSheet.AddAnimationEvent("Atk", -1, () => AtkEnd());
    }

    public override void Excute()
    {
        base.Excute();
        actor.spriteSheet.Play("Atk");
    }

    public void Fire()
    {
        Actor actor = guard.spot.target.GetComponent<Actor>();
        actor.ChangeLife(-guard.data.attackValue);
        actor.hitted.Excute();
    }

    public void AtkEnd()
    {
        guard.spriteSheet.enabled = false;
        Globals.DestroyGuard(guard);
    }
}
