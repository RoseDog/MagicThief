public class Hitted : Action 
{    
    public override void Awake()
    {
        base.Awake();
        actor.spriteSheet.CreateAnimationByName("hitted");
        actor.spriteSheet.AddAnimationEvent("hitted", -1, () => hitteAnimEnd());
    }
	public override void Excute()
    {
        UnityEngine.Debug.Log("hitted");
        base.Excute();
        actor.moving.canMove = false;
        actor.spriteSheet.Play("hitted");
    }
       
    public virtual void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        Stop();
        actor.moving.canMove = true;
    }    
}
