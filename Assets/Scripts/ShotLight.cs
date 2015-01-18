public class ShotLight : MagicianTrickAction 
{    
    public UnityEngine.GameObject targetLight;
        
    public override void Awake()
    {
        animationName = "A_Skill_1";        
        castTrickFuncName = "ShotBulb";
        base.Awake();
        actor.anim[animationName].speed = 2.0f;
    }

    public void Shot(UnityEngine.GameObject light)
    {
        targetLight = light;
        AimTarget(new UnityEngine.Vector3(light.transform.position.x, 4.0f, light.transform.position.z));        
        Excute();
    }

    public void ShotBulb()
    {
        targetLight.GetComponentInParent<Lamp>().BulbBroken();
    }

    public override void TrickActionEnd()
    {
        base.TrickActionEnd();                
        targetLight = null;
    }
}