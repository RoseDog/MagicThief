public class GuardAlertSound : Actor 
{
    public UnityEngine.GameObject wave;
    Guard owner;   
    Cocos2dAction waving_action = null;

    int oneWaveTimeGap = 30;
    public override void Awake()
    {        
        base.Awake();
        owner = GetComponentInParent<Guard>();        
    }

    void CreateOneWave()
    {
        BarkSoundWave wave = (UnityEngine.GameObject.Instantiate(Globals.wave_prefab) as UnityEngine.GameObject).GetComponent<BarkSoundWave>();
        wave.transform.position = owner.transform.position;
        wave.transform.SetParent(owner.transform);

        wave.radiusLimit = 600;
        wave.radiusStart = 300;

        wave.radiusLimit /= owner.transform.transform.localScale.x;
        wave.radiusStart /= owner.transform.transform.localScale.x;
    }

	public void StartAlert(bool repeat)
    {
        if (waving_action == null)
        {
            CreateOneWave();
            if (repeat)
            {
                waving_action = RepeatingCallFunction(oneWaveTimeGap, () => CreateOneWave());
            }            
        }        
    }

    public void ChaseAlert()
    {
        
        //StartCoroutine(Waving());
    }


    public void StopAlert()
    {
        if (waving_action != null)
        {
            RemoveAction(ref waving_action);
        }        
    }
}
