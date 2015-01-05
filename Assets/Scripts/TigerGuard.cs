public class TigerGuard : Guard 
{    
    public override void Awake()
    {        
        base.Awake();
        magicianOutVisionTime = 5.5f;
        atkCd = 1.2f;
        attackValue = 30;
        atkShortestDistance = 2.0f;
        doveOutVisionTime = 5.0f;        
        attackSpeed = 2.0f;
    }

    public override void SpotEnemy(UnityEngine.GameObject gameObj)
    {
        if (gameObj.layer == 11)
        {
            spot.SpotMagician(gameObj, magicianOutVisionTime, true);
        }
        else if (gameObj.layer == 20)
        {
            spot.SpotMagician(gameObj, doveOutVisionTime,true);
        }        
    }
}
