public class GunShot : MagicianTrickAction 
{    
    public UnityEngine.GameObject target;

    public UnityEngine.AudioClip take_out_gun;
    public UnityEngine.AudioClip shot_machine;
    public UnityEngine.AudioClip shot_empty;
        
    public override void Awake()
    {
        base.Awake();

        mage.spriteSheet.AddAnimationEvent("take_out_gun", -1, () => TakenOut());
        mage.spriteSheet.AddAnimationEvent("shot_machine", -1, () => ShotTargetEnd());
        mage.spriteSheet.AddAnimationEvent("shot_empty", 1, () => ShotEmptyEnd());
    }

    public void Shot(UnityEngine.GameObject tar)
    {        
        Excute();
        if (tar != null)
        {
            target = tar;
            mage.FaceTarget(target.transform);
        }
        mage.audioSource.Stop();
        mage.audioSource.PlayOneShot(take_out_gun);
        mage.spriteSheet.Play("take_out_gun");
    }

    public void TakenOut()
    {
        if (target != null)
        {
            mage.spriteSheet.Play("shot_machine");
            mage.audioSource.PlayOneShot(shot_machine);
        }
        else
        {
            mage.spriteSheet.Play("shot_empty");
            mage.audioSource.PlayOneShot(shot_empty);
        }
    }

    public void ShotTargetEnd()
    {
        target.GetComponentInParent<Machine>().Broken(data.duration);
        target = null;
     
        base.Stop();
    }

    public void ShotEmptyEnd()
    {        
        BarkSoundWave wave = (UnityEngine.GameObject.Instantiate(Globals.wave_prefab) as UnityEngine.GameObject).GetComponent<BarkSoundWave>();        
        wave.transform.position = transform.position;
        wave.radiusLimit = 2000;
        wave.radiusLimit = 2500;
        wave.oneWaveDuration = 6;

        base.Stop();
    }
}