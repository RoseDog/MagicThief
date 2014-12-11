public class Hitted : Action 
{
	public override void Excute()
    {
        base.Excute();        
        actor.anim.Play("repel");
        UnityEngine.Debug.Log("hitted");
    }

    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        Stop();
        actor.currentAction = null;
    }
}
