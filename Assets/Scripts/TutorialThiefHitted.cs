public class TutorialThiefHitted : Hitted
{
    float life = 100;
    public override void Excute()
    {
        base.Excute();
        life -= 60;
        if(life < 0)
        {
            actor.lifeOver.Excute();
        }        
    }

    // 这个无效。。可能是unity版本的问题，也可能是动画太老了的问题
    public override void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        base.hitteAnimEnd();
    }
}
