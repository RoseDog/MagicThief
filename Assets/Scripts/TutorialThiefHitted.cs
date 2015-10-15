public class TutorialThiefHitted : Hitted
{
    // 这个无效。。可能是unity版本的问题，也可能是动画太老了的问题
    public override void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        base.hitteAnimEnd();
        if(actor.IsLifeOver())
        {
            Globals.maze.GuardsTargetVanish(gameObject);
        }
    }
}
