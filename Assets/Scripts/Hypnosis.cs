public class Hypnosis : MagicianTrickAction 
{
    Guard target;

    public override void Awake()
    {
        animationName = "A_Attack_2";
        castTrickFuncName = "GoToSleep";
        base.Awake();
    }

    public void Cast(Guard guard)
    {
        target = guard;        
        AimTarget(guard.transform.position + new UnityEngine.Vector3(0, 1, 0));
        Excute();
    }

    public void GoToSleep()
    {
        target.beenHypnosised.GoToSleep(data.duration);
    }

    public override void TrickActionEnd()
    {
        base.TrickActionEnd();
        target = null;
    }
}
