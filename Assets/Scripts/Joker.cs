
public class Joker : Guard 
{
    public override void Awake()
    {
        spriteSheet = GetComponentInChildren<SpriteSheet>();

        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("walking", 6,1.5f);
        spriteSheet.AddAnim("spot", 7, 2.5f, true);
        spriteSheet.AddAnim("wander", 3, 0.4f);
        spriteSheet.AddAnim("atkReady", 4);
        spriteSheet.AddAnim("running", 4, 1.8f);
        spriteSheet.AddAnim("Sleeping", 4, 0.3f);
        spriteSheet.AddAnim("wakeUp", 2, 0.5f);
        spriteSheet.AddAnim("suspicious", 4, 0.6f);
        spriteSheet.AddAnim("BeenHypnosised", 4, 0.7f);
        spriteSheet.AddAnim("Atk", 8, 8.0f, true);
        spriteSheet.AddAnim("kick", 6,0.8f);
                
        base.Awake();
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());
        spriteSheet.AddAnimationEvent("kick", 4, () => atk.FireTheHit());
        spriteSheet.AddAnimationEvent("kick", -1, () => atk.AtkEnd());
    }
}
