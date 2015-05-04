public class MagicianLifeOver : LifeOver
{
    public override void Awake()
    {
        base.Awake();        
        actor.spriteSheet.AddAnimationEvent("die", -1, () => DownOnFloor());
    }

    public override void Excute()
    {
        base.Excute();
        (Globals.LevelController as TutorialLevelController).canvasForStealing.gameObject.SetActive(false);
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
        actor.SleepThenCallFunction((actor as Magician).escape.GetDuration(),
            () => Globals.LevelController.AfterMagicianLifeOverEscaped());
        (actor as Magician).escape.Excute();
    }
}
