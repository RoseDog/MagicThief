public class GunShot : MagicianTrickAction 
{    
    public UnityEngine.GameObject target;
        
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
        
        mage.spriteSheet.Play("take_out_gun");
    }

    public void TakenOut()
    {
        if (target != null)
        {
            mage.spriteSheet.Play("shot_machine");
        }
        else
        {
            mage.spriteSheet.Play("shot_empty");
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
        wave.radiusLimit = 500;
        wave.radiusLimit = 1500;

        base.Stop();
    }
}