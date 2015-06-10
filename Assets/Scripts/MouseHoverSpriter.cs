public class MouseHoverSpriter : Actor
{
    public override void Awake()
    {
        base.Awake();
        spriteSheet.AddAnim("play", 4);
        spriteSheet.Play("play");
    }    
}
