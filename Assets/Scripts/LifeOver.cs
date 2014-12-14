public class LifeOver : Action
{
    public override void Excute()
    {
        base.Excute();
        actor.Dead();        
        actor.anim.Play("die");
        UnityEngine.Debug.Log("magician life over");
    }
}
