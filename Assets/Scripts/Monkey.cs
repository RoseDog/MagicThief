public class Monkey : Guard 
{
    public override void Awake()
    {
        spriteSheet = GetComponentInChildren<SpriteSheet>();

        spriteSheet.AddAnim("idle", 4, 0.23f);
        spriteSheet.AddAnim("walking", 4, 1.0f);
        spriteSheet.AddAnim("spot", 2, 1.0f, true);        
        spriteSheet.AddAnim("running", 4, 1.5f);
        spriteSheet.AddAnim("atkReady", 4);
        spriteSheet.AddAnim("Atk", 9, 0.5f, true);
        spriteSheet.AddAnim("BeenHypnosised", 4, 0.5f);
        spriteSheet.AddAnim("Sleeping", 7, 0.2f);
        spriteSheet.AddAnim("wander", 4, 0.25f);
        spriteSheet.CreateAnimationBySprites(spriteSheet._animationList["wander"].spriteList,"wakeUp",1.0f,false);


        base.Awake();
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());
        spriteSheet.AddAnimationEvent("Atk", -1, () => atk.AtkEnd());
    }
}
