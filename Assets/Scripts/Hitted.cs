using System.Collections;

public class Hitted : Action 
{
	public override void Excute()
    {
        base.Excute();
        // 后面放到Magician moving里面去
        (actor as Magician).isMoving = false;
        (actor as Magician).UnRegistEvent();
        actor.anim.Play("repel");
        UnityEngine.Debug.Log("hitted");
    }

    public void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        (actor as Magician).RegistEvent();
        Stop();
        actor.currentAction = null;
    }
}
