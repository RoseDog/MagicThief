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
        base.Excute();
        Globals.maze.GuardsTargetVanish(actor.gameObject);
        actor.characterController.enabled = false;
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
        actor.spriteRenderer.material = (actor as Magician).flyingMat;
    }

    void EscapeOver()
    {        
        Stop();
        actor.head_on_minimap.SetActive(true);
        actor.spriteRenderer.material = (actor as Magician).groundMat;
        actor.shadow.enabled = true;
        Globals.stealingController.magician.gameObject.SetActive(false);        
    }
}
