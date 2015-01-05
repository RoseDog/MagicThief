public class Escape : Action
{
    public float duration = 1.8f;
    public override void Excute()
    {
        UnityEngine.Debug.Log("magician Escape");
        base.Excute();
        actor.anim.CrossFade("A_Falling_1");                        
        actor.AddAction(
            new Sequence(new MoveTo(transform, transform.position + new UnityEngine.Vector3(0, 15, 0), duration)
                ,new FunctionCall(this, "EscapeOver")));        
    }

    void EscapeOver()
    {        
        Stop();
    }
}
