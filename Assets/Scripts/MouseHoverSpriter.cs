public class MouseHoverSpriter : Actor
{
    public override void Awake()
    {
        base.Awake();
        spriteSheet.init();
        spriteSheet.AddAnim("play", spriteSheet._sprites.Length);
        spriteSheet.Play("play");
    }    
}
