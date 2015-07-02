public class Dog : Guard 
{    
    public override void Awake()
    {        
        base.Awake();        
        bGoChaseDove = true;

        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("walking", 4);
        spriteSheet.AddAnim("spot", 8, 1.0f, true);
        spriteSheet.AddAnim("atkReady", 7);
        spriteSheet.AddAnim("running", 3, 1.3f);
        spriteSheet.AddAnim("wander", 12, 0.5f);        
        spriteSheet.AddAnim("Atk", 6, 1.0f, true);
    }

    public override bool CheckIfChangeTarget(UnityEngine.GameObject newTar)
    {
        // 如果本来的target是Dove，则不换目标
        return base.CheckIfChangeTarget(newTar) && (spot.target == null ||  spot.target.gameObject.layer != 20);
    }
}
