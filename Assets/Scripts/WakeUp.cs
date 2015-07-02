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
        guard.EnableEyes(true);
        actor.spriteSheet.Play("wakeUp");
    }

    public void wakeUpEnd()
    {
        Stop();        
        guard.wandering.Excute();
    }
}
