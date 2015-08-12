public class Falling : Action
{
    public UnityEngine.Vector3 from;
    public UnityEngine.Vector3 to;
    [UnityEngine.HideInInspector]
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("falling", -1, ()=>FallingOver());
        actor.spriteSheet.AddAnimationEvent("landing", -1, ()=>LandingOver());
    }
    public override void Excute()
    {
        base.Excute();        
        actor.characterController.enabled = false;
        transform.position = from;
        actor.moving.canMove = false;
        actor.shadow.enabled = false;
        actor.AddAction(new MoveTo(actor.transform, to,
            actor.spriteSheet.GetAnimationLengthWithSpeed("falling")));
        actor.transform.localScale = new UnityEngine.Vector3(actor.scaleCache.x, 4000, 2400);
        actor.AddAction(new ScaleTo(actor.transform, actor.scaleCache,
            actor.spriteSheet.GetAnimationLengthWithSpeed("falling")));
        actor.spriteSheet.Play("falling");
        actor.head_on_minimap.SetActive(false);
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
        actor.shadow.enabled = true;
        actor.characterController.enabled = true;
        actor.spriteSheet.Play("idle");
        actor.head_on_minimap.SetActive(true);
        Stop();
    }
}
