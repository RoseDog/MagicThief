public class GunShot : MagicianTrickAction 
{    
    public UnityEngine.GameObject target;
        
    public override void Awake()
    {
        base.Awake();
        
        mage.spriteSheet.AddAnimationEvent("Shot", -1, () => TrickActionEnd());
    }

    public void Shot(UnityEngine.GameObject tar)
    {
        target = tar;        
        Excute();
        mage.spriteSheet.Play("Shot");
    }

    public void TrickActionEnd()
    {
        target.GetComponentInParent<Machine>().Broken();
        target = null;
        base.Stop();
    }
}