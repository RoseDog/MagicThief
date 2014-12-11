public class LifeOver : Action
{
    public override void Excute()
    {
        base.Excute();
        actor.Dead();        
        actor.anim.Play("die");
        actor.collider.enabled = false;
        UnityEngine.Debug.Log("magician life over");
    }
}
