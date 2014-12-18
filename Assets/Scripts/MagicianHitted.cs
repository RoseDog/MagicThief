public class MagicianHitted : Hitted 
{
    public override void Excute()
    {
        base.Excute();
        (actor as Magician).UnRegistEvent();
        Globals.canvasForMagician.lifeNumber.ChangeLife(-60);
    }

    public override void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        (actor as Magician).RegistEvent();
        base.hitteAnimEnd();        
    }
}
