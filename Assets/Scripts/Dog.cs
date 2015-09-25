public class Dog : Guard 
{
    UnityEngine.GameObject appearSpot;
    public override void Awake()
    {        
        base.Awake();        
        bGoChaseDove = true;

        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("walking", 4, 1.3f);
        spriteSheet.AddAnim("spot", 8, 1.0f, true);
        spriteSheet.AddAnim("atkReady", 7);
        spriteSheet.AddAnim("running", 3, 1.5f);
        spriteSheet.AddAnim("wander", 12, 0.5f);        
        spriteSheet.AddAnim("Atk", 6, 1.0f, true);

        appearSpot = Globals.getChildGameObject(gameObject, "appearSpot");
        appearSpot.SetActive(false);
    }

    public override bool CheckIfChangeTarget(UnityEngine.GameObject newTar)
    {
        // 如果本来的target是Dove，则不换目标
        return spot.target != newTar.transform && (spot.target == null || spot.target.gameObject.layer != 20);
    }

    public override void SetInFog(bool infog)
    {
        base.SetInFog(infog);
        shadow.enabled = !infog;
        spriteRenderer.enabled = !infog;

        if(currentAction == spot)
        {
            eye.SetVisonConesVisible(true);
        }        
    }
}
