public class MouseHoverSpriter : Actor
{
    public float animSpeed = 1.0f;
    public bool DestroyAtEnd = false;
    public bool clampAnim = false;
    public override void Awake()
    {
        base.Awake();
        if(enabled)
        {
            spriteSheet.init();
            spriteSheet.AddAnim("play", spriteSheet._sprites.Length, animSpeed, clampAnim);
            spriteSheet.Play("play");

            if (DestroyAtEnd)
            {
                spriteSheet.AddAnimationEvent("play", -1, () => PlayEnded());
            }        

        }
        
    }

    void PlayEnded()
    {
        Destroy(gameObject);
    }
}
