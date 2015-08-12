public class Police : Guard
{
    public override void Awake()
    {
        spriteSheet = GetComponentInChildren<SpriteSheet>();

        spriteSheet.AddAnim("BeenHypnosised", 11, 0.7f);
        spriteSheet.AddAnim("Sleeping", 9, 0.3f);
        spriteSheet.AddAnim("wakeUp", 7, 0.5f);
        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("spot", 4, 1.0f, true);
        spriteSheet.AddAnim("Atk", 7, 8.0f, true);
        spriteSheet.AddAnim("wander", 8, 1.4f);

        spriteSheet.AddAnim("walking", 6, 1.5f);
        
        
        spriteSheet.AddAnim("atkReady", 4);
        spriteSheet.AddAnim("running", 4, 1.8f);
        
        spriteSheet.AddAnim("suspicious", 4, 0.6f);
        
        
        spriteSheet.AddAnim("kick", 6, 0.8f);

        base.Awake();
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());
        spriteSheet.AddAnimationEvent("kick", 4, () => atk.FireTheHit());
        spriteSheet.AddAnimationEvent("kick", -1, () => atk.AtkEnd());
    }
}
