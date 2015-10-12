public class Explode : GuardAction 
{
    public UnityEngine.AudioClip explode;
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("Atk", 1, () => Fire());
        actor.spriteSheet.AddAnimationEvent("Atk", -1, () => ExplodeEnd());
    }

    public override void Excute()
    {
        base.Excute();
        guard.EnableEyes(false);
        actor.spriteSheet.Play("Atk");
        actor.audioSource.PlayOneShot(explode);
        UnityEngine.Debug.Log("Explode");
    }

    public void Fire()
    {        
        Actor tar = guard.spot.target.GetComponent<Actor>();
        tar.EnemyStopChasing(guard);
        if (!tar.IsLifeOver())
        {
            tar.hitted.Excute();
            tar.ChangeLife(-guard.data.attackValue);            
        }               
    }

    public void ExplodeEnd()
    {
        guard.spriteSheet.enabled = false;        
        Actor.to_be_remove.Add(guard);
    }
}
