public class GuardAlertSound : GuardAction
{
    public UnityEngine.GameObject wave;
    Cocos2dAction waving_action = null;

    int oneWaveTimeGap = 30;

    void CreateOneWave()
    {
        BarkSoundWave wave = (UnityEngine.GameObject.Instantiate(Globals.wave_prefab) as UnityEngine.GameObject).GetComponent<BarkSoundWave>();
        wave.transform.position = guard.transform.position;
        wave.transform.SetParent(guard.transform);

        wave.radiusLimit = 600;
        wave.radiusStart = 300;

        wave.radiusLimit /= guard.transform.transform.localScale.x;
        wave.radiusStart /= guard.transform.transform.localScale.x;
    }

	public void StartAlert(bool repeat)
    {
        if (waving_action == null)
        {
            CreateOneWave();
            if (repeat)
            {
                waving_action = guard.RepeatingCallFunction(oneWaveTimeGap, () => CreateOneWave());
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
            guard.RemoveAction(ref waving_action);
        }        
    }
}
