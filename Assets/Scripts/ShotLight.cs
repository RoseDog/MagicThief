public class ShotLight : MagicianTrickAction 
{    
    public UnityEngine.GameObject targetLight;
        
    public override void Awake()
    {
        base.Awake();
        mage.spriteSheet.CreateAnimationByName("Shot");
        mage.spriteSheet.AddAnimationEvent("Shot", -1, () => TrickActionEnd());
    }

    public void Shot(UnityEngine.GameObject light)
    {
        targetLight = light;        
        Excute();
        mage.spriteSheet.Play("Shot");
    }

    public void TrickActionEnd()
    {
        targetLight.GetComponentInParent<Lamp>().BulbBroken();
        targetLight = null;
        base.Stop();
    }
}