public class Flash : Actor
{
    public override void Awake()
    {
        base.Awake();
        spriteSheet.CreateAnimationByName("flash", 4.0f, true);
        spriteSheet.AddAnimationEvent("flash", -1, () => FlashEnd());
        spriteSheet.Play("flash");
    }

    void FlashEnd()
    {
        Destroy(gameObject);
    }
}
