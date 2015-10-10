public class Explode : GuardAction 
{
    public UnityEngine.AudioClip explode;
    public void Start()
    {
        actor.spriteSheet.AddAnimationEvent("Atk", 2, () => Fire());
        actor.spriteSheet.AddAnimationEvent("Atk", -1, () => ExplodeEnd());
    }

    public override void Excute()
    {
        base.Excute();
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
