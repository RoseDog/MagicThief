public class Escape : Action
{
    int Duration = 80;
    public int GetDuration()
    {
        return Duration;
    }
    public void Go(System.String anim)
    {
        UnityEngine.Debug.Log("magician Escape");
        Globals.maze.GuardsTargetVanish(actor.gameObject);
        base.Excute();
        actor.shadow.enabled = false;
        actor.moving.canMove = false;
        actor.spriteSheet.Play(anim);
        transform.position = transform.position - new UnityEngine.Vector3(0,0,0.6f);
        actor.AddAction(
            new Sequence(new MoveTo(transform, transform.position + new UnityEngine.Vector3(0, 1500, 0), Duration)
                ,new FunctionCall(()=>EscapeOver())));
        if (actor.moving.GetSeeker().GetCurrentPath() != null)
        {
            actor.moving.GetSeeker().GetCurrentPath().Reset();
        }

        actor.head_on_minimap.SetActive(false);
    }

    void EscapeOver()
    {        
        Stop();
        actor.head_on_minimap.SetActive(true);
        actor.shadow.enabled = true;
        Globals.magician.gameObject.SetActive(false);        
    }
}
