public class WakeUp : GuardAction 
{
    public override void Awake()
    {
        base.Awake();
        guard.spriteSheet.AddAnimationEvent("wakeUp", -1, () => wakeUpEnd());
    }

    public override void Excute()
    {
        base.Excute();
        actor.spriteSheet.Play("wakeUp");
    }

    public void wakeUpEnd()
    {
        Stop();
        guard.EnableEyes(true);
        guard.wandering.Excute();
    }
}
