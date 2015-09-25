public class Hypnosis : MagicianTrickAction 
{
    Guard target;
    UnityEngine.AudioClip fingerSnap;
    public override void Awake()
    {
        base.Awake();
        fingerSnap = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/fingerSnap_fx_03");
    }

    public void Start()
    {
        mage.spriteSheet.AddAnimationEvent("Hypnosis", 1, () => Fire());
        mage.spriteSheet.AddAnimationEvent("Hypnosis", -1, () => TrickActionEnd());
    }

    public void Cast(Guard guard)
    {
        if (Globals.stealingController.magician.ChangePower(-data.powerCost))
        {
            target = guard;
            Excute();
            mage.audioSource.PlayOneShot(fingerSnap);
            mage.spriteSheet.Play("Hypnosis");
            mage.FaceTarget(target.transform);
        }        
    }

    void Fire()
    {
        float distance = UnityEngine.Vector3.Distance(target.transform.position, mage.transform.position);
        target.beenHypnosised.GoToSleep(data.CalcTrickDurationBasedOnDistance(distance));
        mage.EnemyStopChasing(target);
    }

    public void TrickActionEnd()
    {
        
        target = null;
        base.Stop();
    }
}
