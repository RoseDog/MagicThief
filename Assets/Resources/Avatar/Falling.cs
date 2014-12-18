public class Falling : Action
{
    public UnityEngine.Vector3 from;
    public UnityEngine.Vector3 to;
    public float duration;
    public override void Excute()
    {
        base.Excute();        
        transform.position = from;
        actor.AddAction(new Sequence(new MoveTo(actor.transform, to, duration), new FunctionCall(this, "FallingOver")));
        actor.anim.Play("A_Falling_1");
    }

    void FallingOver()
    {
        actor.anim.Play("A_JumpLanding_1");
        Invoke("LandingOver", actor.anim["A_JumpLanding_1"].length);
    }

    void LandingOver()
    {
        if( UnityEngine.Application.loadedLevelName != "City")
        {
            Globals.LevelController.AfterMagicianFalling();
        }        
        Stop();
    }
}
