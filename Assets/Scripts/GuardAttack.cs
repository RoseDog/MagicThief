public class GuardAttack : Attack 
{
    public override void Awake()
    {
        base.Awake();

        actor.spriteSheet.CreateAnimationByName("Atk", 0.7f);
        actor.spriteSheet.AddAnimationEvent("Atk", -1, () => AtkEnd());
        actor.spriteSheet.AddAnimationEvent("Atk", 3, () => FireTheHit());
    }
}