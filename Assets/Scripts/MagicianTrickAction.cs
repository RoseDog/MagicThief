public class MagicianTrickAction : Action 
{
    public TrickData data;
    protected Magician mage;        
    public override void Awake()
    {
        mage = GetComponent<Magician>();
        base.Awake();        
    }

    public override void Excute()
    {
        base.Excute();
        actor.moving.canMove = false;
        Globals.canvasForMagician.TrickUsingHighlightOn(data);
    }

    public override void Stop()
    {
        base.Stop();
        actor.moving.canMove = true;
        Globals.canvasForMagician.TrickUsingHighlightOff();
    }
}
