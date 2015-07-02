public class GunShot : MagicianTrickAction 
{    
    public UnityEngine.GameObject target;
        
    public override void Awake()
    {
        base.Awake();

        mage.spriteSheet.AddAnimationEvent("take_out_gun", -1, () => TakenOut());
        mage.spriteSheet.AddAnimationEvent("shot_machine", -1, () => TrickActionEnd());
    }

    public void Shot(UnityEngine.GameObject tar)
    {
        target = tar;        
        Excute();
        mage.spriteSheet.Play("take_out_gun");
    }

    public void TakenOut()
    {
        mage.spriteSheet.Play("shot_machine");
    }

    public void TrickActionEnd()
    {
        target.GetComponentInParent<Machine>().Broken();
        target = null;
        base.Stop();
    }
}