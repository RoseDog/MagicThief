public class TryEscape : Action
{
    int Duration = 100;
    public override void Awake()
    {
        base.Awake();
        actor.spriteSheet.CreateAnimationByName("TryEscape", 0.5f);
        actor.spriteSheet.AddAnimationEvent("TryEscape", -1, () => Successd());
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

    void Successd()
    {
        Stop();
        Globals.magician.OutStealing();
        Globals.magician.escape.Excute();
        (Globals.LevelController as TutorialLevelController).StealingOver();
    }
}
