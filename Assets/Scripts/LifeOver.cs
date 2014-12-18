public class LifeOver : Action
{
    public override void Excute()
    {
        base.Excute();
        actor.OutStealing();        
        actor.anim.Play("die");
    }
}
