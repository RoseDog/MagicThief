public class Falling : Action
{
    public UnityEngine.Vector3 from;
    public UnityEngine.Vector3 to;
    [UnityEngine.HideInInspector]
    public double duration;
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("falling", -1, ()=>FallingOver());
        actor.spriteSheet.AddAnimationEvent("landing", -1, ()=>LandingOver());
    }
    public override void Excute()
    {
        base.Excute();        
        GetComponent<UnityEngine.CharacterController>().enabled = false;
        transform.position = from;
        actor.moving.canMove = false;
        actor.AddAction(new MoveTo(actor.transform, to,
            actor.spriteSheet.GetAnimationLengthWithSpeed("falling")));
        actor.transform.localScale = new UnityEngine.Vector3(actor.scaleCache.x, 12, 12);
        actor.AddAction(new ScaleTo(actor.transform, actor.scaleCache,
            actor.spriteSheet.GetAnimationLengthWithSpeed("falling")));
        actor.spriteSheet.Play("falling");
    }

    public void FallingOver()
    {
        actor.spriteSheet.Play("landing");        
    }

    public void LandingOver()
    {
        if( UnityEngine.Application.loadedLevelName != "City")
        {
            Globals.LevelController.AfterMagicianFalling();
        }
        actor.moving.canMove = true;
        GetComponent<UnityEngine.CharacterController>().enabled = true;
        actor.spriteSheet.Play("idle");
        Stop();
    }
}
