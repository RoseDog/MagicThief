public class Flash : Actor
{
    public override void Awake()
    {
        base.Awake();
        spriteSheet.CreateAnimationByName("flash", 7.0f, true);
        spriteSheet.AddAnimationEvent("flash", -1, () => FlashEnd());
        spriteSheet.Play("flash");
    }

    void FlashEnd()
    {
        Destroy(gameObject);
    }
}
