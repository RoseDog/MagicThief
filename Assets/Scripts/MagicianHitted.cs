public class MagicianHitted : Hitted 
{
    public override void Excute()
    {
        base.Excute();
        (actor as Magician).UnRegistEvent();
    }

    public override void ChangeLife(int delta)
    {
        base.ChangeLife(delta);
        Globals.canvasForMagician.lifeNumber.UpdateCurrentLife(this);        
    }

    public override void hitteAnimEnd()
    {
        UnityEngine.Debug.Log("hitteAnimEnd");
        (actor as Magician).RegistEvent();
        base.hitteAnimEnd();        
    }

    public override void ResetLife()
    {
        base.ResetLife();
        Globals.canvasForMagician.lifeNumber.UpdateText(this);
    }
}
