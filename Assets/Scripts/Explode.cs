public class Explode : GuardAction 
{
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("Atk", 2, () => Fire());
        actor.spriteSheet.AddAnimationEvent("Atk", -1, () => ExplodeEnd());
    }

    public override void Excute()
    {
        base.Excute();
        actor.spriteSheet.Play("Atk");
    }

    public void Fire()
    {
        Actor tar = guard.spot.target.GetComponent<Actor>();        
        if (tar.currentAction != tar.lifeOver)
        {
            tar.ChangeLife(-guard.data.attackValue);
            tar.hitted.Excute();
        }               
    }

    public void ExplodeEnd()
    {
        guard.spriteSheet.enabled = false;        
        Actor.to_be_remove.Add(guard);
    }
}
