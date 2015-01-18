public class MagicianHitted : Hitted 
{
    public override void Excute()
    {
        base.Excute();        
        (actor as Magician).UnRegistEvent();
    }

    
    public override void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        (actor as Magician).RegistEvent();        
        base.hitteAnimEnd();        
    }
}
