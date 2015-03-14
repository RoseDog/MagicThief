public class LifeOver : Action
{
    public override void Awake()
    {
        base.Awake();
        actor.spriteSheet.CreateAnimationByName("die", 1.0f, true);
    }
    public override void Excute()
    {
        UnityEngine.Debug.Log("magician life over");

        base.Excute();
        actor.OutStealing();
        actor.controller.enabled = false;
        //actor.moving.GetSeeker().GetCurrentPath().Reset();        
        actor.moving.ClearPath();
        actor.spriteSheet.Play("die");
        Globals.LevelController.MagicianLifeOver();
    }
}
