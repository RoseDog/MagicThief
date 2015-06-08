public class LifeOver : Action
{
    public override void Awake()
    {
        base.Awake();                
    }
    public override void Excute()
    {
        UnityEngine.Debug.Log("magician life over");

        base.Excute();
        actor.OutStealing();
        actor.controller.enabled = false;
        //actor.moving.GetSeeker().GetCurrentPath().Reset();        
        actor.moving.canMove = false;
        actor.moving.ClearPath();
        actor.spriteSheet.Play("lifeOver");
        Globals.LevelController.MagicianLifeOver();
        foreach (Chest chest in Globals.maze.chests)
        {
            if (chest.isMagicianNear)
            {
                chest.OnTriggerExit(actor.collider);
            }
        }
    }
}
