public class Hitted : Action 
{    
    public virtual void Start()
    {
        
    }

	public override void Excute()
    {
        UnityEngine.Debug.Log("hitted");
        base.Excute();
        actor.moving.canMove = false;
        actor.spriteSheet.Play("hitted");
        actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLengthWithSpeed("hitted"), () => hitteAnimEnd());
    }
       
    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        Stop();
        actor.moving.canMove = true;
        if (actor.LifeCurrent < UnityEngine.Mathf.Epsilon)
        {
            actor.lifeOver.Excute();
        }        
    }    
}
