public class Escape : Action
{
    public int duration = 80;
    public override void Excute()
    {
        UnityEngine.Debug.Log("magician Escape");
        Globals.maze.GuardsTargetVanish(actor.gameObject);
        base.Excute();        
        actor.spriteSheet.Play("flying");
        transform.position = transform.position - new UnityEngine.Vector3(0,0,0.6f);
        actor.AddAction(
            new Sequence(new MoveTo(transform, transform.position + new UnityEngine.Vector3(0, 15, 0), duration)
                ,new FunctionCall(()=>EscapeOver())));        
    }

    void EscapeOver()
    {        
        Stop();
        Globals.magician.gameObject.SetActive(false);
    }
}
