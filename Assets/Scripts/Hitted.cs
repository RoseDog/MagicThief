public class Hitted : Action 
{    
    public virtual void Start()
    {
        
    }

	public override void Excute()
    {
        if (!actor.IsLifeOver())
        {
            UnityEngine.Debug.Log("hitted");
            base.Excute();
            actor.moving.canMove = false;
            actor.spriteSheet.Play("hitted");
            actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLengthWithSpeed("hitted"), () => hitteAnimEnd());
        }        
    }
       
    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        
        if(actor.LifeCurrent < UnityEngine.Mathf.Epsilon)
        {
            if (actor.currentAction != actor.lifeOver)
            {
                actor.lifeOver.Excute();
            }        
        }
        else
        {
            Stop();
            actor.moving.canMove = true;
        }        
    }    
}
