
public class Joker : Guard 
{
    public override void Awake()
    {
        spriteSheet = GetComponentInChildren<SpriteSheet>();

        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("walking", 6);
        spriteSheet.AddAnim("spot", 7, 1.0f, true);
        spriteSheet.AddAnim("wander", 3, 0.25f);
        spriteSheet.AddAnim("atkReady", 4);
        spriteSheet.AddAnim("running", 4, 1.3f);
        spriteSheet.AddAnim("Sleeping", 4, 0.2f);
        spriteSheet.AddAnim("wakeUp", 6, 0.4f);
        spriteSheet.AddAnim("BeenHypnosised", 4, 0.5f);
        spriteSheet.AddAnim("Atk", 8, 2.0f, true);
                
        base.Awake();
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());
    }
}
