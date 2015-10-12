public class MagicianHitted : Hitted 
{
    public override void Start()
    {
        base.Start();
    }

    public override void Excute()
    {
        foreach (Chest chest in Globals.maze.chests)
        {
            if (chest.isMagicianNear)
            {
                chest.TouchOut(actor);
            }
        }
        // 被网住的状态
        if (actor.currentAction == actor.catchByNet && actor.catchByNet.jumpSequence == null)
        {
            actor.spriteSheet.Play("hitted_in_net");
            actor.SleepThenCallFunction(actor.spriteSheet.GetAnimationLengthWithSpeed("hitted_in_net"), ()=>hitteInNetEnd());
        }
        else
        {
            base.Excute();
            (actor as Magician).UnRegistEvent();
        }
    }
    
    public override void hitteAnimEnd()
    {
        (actor as Magician).RegistEvent();        
        base.hitteAnimEnd();        
    }

    public void hitteInNetEnd()
    {
        if (actor.LifeCurrent < UnityEngine.Mathf.Epsilon)
        {
            actor.lifeOver.Excute();
        }
        else if (actor.catchByNet.jumpSequence == null)
        {
            actor.spriteSheet.Play("catch_by_net");
        }
    }
}
