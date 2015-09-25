public class Police : Guard
{
    public override void Awake()
    {
        spriteSheet = GetComponentInChildren<SpriteSheet>();

        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("walking", 6, 1.5f);
        spriteSheet.AddAnim("spot", 4, 1.0f, true);
        spriteSheet.AddAnim("running", 10, 2.5f);
        spriteSheet.AddAnim("atkReady", 4);
        spriteSheet.AddAnim("Atk", 7, 8.0f, true);
        spriteSheet.AddAnim("BeenHypnosised", 11, 0.7f);
        spriteSheet.AddAnim("Sleeping", 9, 0.3f);
        spriteSheet.AddAnim("wakeUp", 1, 0.5f);
        spriteSheet.AddAnim("suspicious", 6, 0.6f);
        spriteSheet.AddAnim("wander", 8, 1.4f);
               
        base.Awake();
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());        
    }
}
