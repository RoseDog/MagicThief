public class TryEscape : Action
{
    int Duration = 100;
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("TryEscape", -1, () => Escaped());
    }
    
    public override void Excute()
    {        
        base.Excute();
        actor.moving.canMove = false;
        actor.spriteSheet.Play("TryEscape");
        if (actor.moving.GetSeeker().GetCurrentPath() != null)
        {
            actor.moving.GetSeeker().GetCurrentPath().Reset();
        }        
    }

    void Escaped()
    {
        Stop();
        Globals.magician.OutStealing();
        Globals.magician.escape.Go("victoryEscape");
        (Globals.LevelController as StealingLevelController).PvPEscaped();
    }
}
