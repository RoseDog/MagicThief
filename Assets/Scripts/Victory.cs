public class Victory : Action
{
    public override void Excute()
    {        
        base.Excute();
        actor.OutStealing();
        Invoke("Escape", 0.3f);
    }

    void Escape()
    {
        //Globals.cameraFollowMagician.StaringMagician((actor as Magician).escape.duration - 0.5f);
        actor.SleepThenCallFunction((actor as Magician).escape.duration,
            () => Globals.LevelController.AfterMagicianSuccessedEscaped());
        (actor as Magician).escape.Excute();        
    }
}
