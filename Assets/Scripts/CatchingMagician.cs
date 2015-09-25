public class CatchingMagician : GuardAction 
{
    public override void Awake()
    {
        base.Awake();
        guard.spriteSheet.CreateAnimationByName("catching_magician");
        guard.spriteSheet.CreateAnimationByName("pushed_away");
        guard.spriteSheet.CreateAnimationByName("getup");
    }
}
