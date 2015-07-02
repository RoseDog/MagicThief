public class MouseHoverSpriter : Actor
{
    public float animSpeed = 1.0f;
    public bool DestroyAtEnd = false;
    public override void Awake()
    {
        base.Awake();
        spriteSheet.init();
        spriteSheet.AddAnim("play", spriteSheet._sprites.Length, animSpeed);
        spriteSheet.Play("play");

        if (DestroyAtEnd)
        {
            spriteSheet.AddAnimationEvent("play", -1, () => PlayEnded());
        }        
    }

    void PlayEnded()
    {
        Destroy(gameObject);
    }
}
