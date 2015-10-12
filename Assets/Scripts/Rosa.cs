public class Rosa : Magician 
{
    public override void Awake()
    {
        sneakingAnimSpeed = 2.5f;    
        normalMovingAnimSpeed = 1.8f;
        runningAnimSpeed = 3.0f;

        spriteSheet = GetComponentInChildren<SpriteSheet>();
        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("moving", 6, normalMovingAnimSpeed);
        spriteSheet.AddAnimationEvent("moving", 0, () => StepSound());
        spriteSheet.AddAnimationEvent("moving", 3, () => StepSound());
        spriteSheet.AddAnim("victoryEscape", 1);
        spriteSheet.AddAnim("catch_by_net", 4);
        spriteSheet.AddAnim("hitted_in_net", 1, 0.2f);
        spriteSheet.AddAnim("break_net", 8, 2.0f);
        spriteSheet.AddAnim("disguise", 8, 1.7f);
        spriteSheet.AddAnim("falling_failed", 7, 1.0f, true);
        spriteSheet.AddAnim("falling_success", 5, 1.0f, true);
        spriteSheet.AddAnim("falling", 1, 0.3f);
        spriteSheet.AddAnim("flying", 8);
        spriteSheet.AddAnim("hitted", 1, 0.3f);
        spriteSheet.AddAnim("Hypnosis", 4, 1.5f);
        spriteSheet.AddAnim("landing", 1, 0.3f);
        spriteSheet.AddAnim("lifeOver", 5, 1.5f, true);
        spriteSheet.AddAnim("lifeOverEscape", 1, 1.0f, true);
        spriteSheet.AddAnim("take_out_gun", 6, 2.8f);
        spriteSheet.AddAnim("shot_machine", 3, 1.0f);
        spriteSheet.AddAnim("shot_empty", 2, 1.0f);
        spriteSheet.AddAnim("TryEscape", 6, 1.0f);
        spriteSheet.AddAnim("falling_failed_loop", 12, 1.3f);
        spriteSheet.AddAnim("flyup_0", 7, 2.0f);
        spriteSheet.AddAnim("flyup_1", 1, 1.7f);
        spriteSheet.AddAnim("flyup_2", 6, 1.9f);
        spriteSheet.AddAnim("open_chest", 4);
        spriteSheet.AddAnimationEvent("open_chest", 0, () => OpenChestSound());
        spriteSheet.AddAnim("take_money", 16);
        spriteSheet.AddAnim("dove_trick", 12, 4.0f, true);        
        spriteSheet.AddAnim("sneaking", 6, sneakingAnimSpeed);
        base.Awake();

        spriteSheet.AddAnimationEvent("dove_trick", 7, () => releaseDove.DoveFlyOut());
    }
}
