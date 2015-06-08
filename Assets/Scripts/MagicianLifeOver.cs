public class MagicianLifeOver : LifeOver
{
    public override void Awake()
    {
        base.Awake();               
    }

    public override void Excute()
    {
        base.Excute();
        (Globals.LevelController as StealingLevelController).canvasForStealing.gameObject.SetActive(false);

        actor.AddAction(
            new Sequence(new SleepFor(actor.spriteSheet.GetAnimationLength("lifeOver")*4)
                , new FunctionCall(() => Escape())));
    }

//     void DownOnFloor()
//     {
//         actor.spriteSheet.Play("down_on_floor");
//         actor.SleepThenCallFunction(40, () => StandUp());
//     }
// 
//     void StandUp()
//     {
//         actor.spriteSheet.Play("stand_up");
//         actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLength("stand_up"), () => Escape());
//     }

    void Escape()
    {
        Globals.cameraFollowMagician.target = null;
        actor.SleepThenCallFunction((actor as Magician).escape.GetDuration(),
            () => (Globals.LevelController as StealingLevelController).AfterMagicianLifeOverEscaped());
        (actor as Magician).escape.Go("lifeOverEscape");
    }
}
