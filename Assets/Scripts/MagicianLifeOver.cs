public class MagicianLifeOver : LifeOver
{
    public override void Awake()
    {
        base.Awake();        
        actor.spriteSheet.AddAnimationEvent("die", -1, () => DownOnFloor());
    }

    void DownOnFloor()
    {
        actor.spriteSheet.Play("down_on_floor");
        actor.SleepThenCallFunction(40, () => StandUp());
    }

    void StandUp()
    {
        actor.spriteSheet.Play("stand_up");
        actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLength("stand_up"), () => Escape());
    }

    void Escape()
    {
        Globals.cameraFollowMagician.target = null;
        //Globals.cameraFollowMagician.StaringMagician((actor as Magician).escape.duration - 0.8f);        
        actor.SleepThenCallFunction((actor as Magician).escape.duration,
            () => Globals.LevelController.AfterMagicianLifeOverEscaped());
        (actor as Magician).escape.Excute();
    }
}
