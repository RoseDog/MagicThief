public class Dog : Guard 
{    
    public override void Awake()
    {        
        base.Awake();        
        bGoChaseDove = true;        
    }

    public override bool CheckIfChangeTarget(UnityEngine.GameObject newTar)
    {
        // 如果本来的target是Dove，则不换目标
        return base.CheckIfChangeTarget(newTar) && (spot.target == null ||  spot.target.gameObject.layer != 20);
    }
}
