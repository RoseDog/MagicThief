public class Hypnosis : MagicianTrickAction 
{
    Guard target;

    public override void Awake()
    {
        base.Awake();
        mage.spriteSheet.CreateAnimationByName("Hypnosis",1.3f);
        mage.spriteSheet.AddAnimationEvent("Hypnosis", -1, () => TrickActionEnd());
    }

    public void Cast(Guard guard)
    {
        target = guard;        
        Excute();
        mage.spriteSheet.Play("Hypnosis");
        mage.FaceTarget(target.transform);
    }   

    public void TrickActionEnd()
    {
        target.beenHypnosised.GoToSleep(data.duration);
        target = null;
        base.Stop();
    }
}
