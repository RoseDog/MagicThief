public class TryEscape : Action
{
    int Duration = 100;
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("TryEscape", -1, () => Escaped());
    }
    
    public override void Excute()
    {        
        base.Excute();
        actor.moving.canMove = false;
        actor.spriteSheet.Play("TryEscape");
        if (actor.moving.GetSeeker().GetCurrentPath() != null)
        {
            actor.moving.GetSeeker().GetCurrentPath().Reset();
        }        
    }

    void Escaped()
    {
        UnityEngine.GameObject SmokePrefab = UnityEngine.Resources.Load("Avatar/EscapeSmoke") as UnityEngine.GameObject;
        UnityEngine.GameObject smoke = UnityEngine.GameObject.Instantiate(SmokePrefab) as UnityEngine.GameObject;
        smoke.transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y-50, transform.position.z);
        Globals.stealingController.magician.OutStealing();
        Globals.stealingController.magician.Visible(false);
        actor.SleepThenCallFunction(20,()=>End());
    }

    void End()
    {
        Globals.stealingController.magician.Visible(true);
        Stop();
        Globals.stealingController.magician.escape.Go("victoryEscape");
    }    
}
