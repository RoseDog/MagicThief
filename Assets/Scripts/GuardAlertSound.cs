public class GuardAlertSound : Actor 
{
    public UnityEngine.GameObject wave;
    Guard owner;
    double oneWaveDuration;
    double radiusStart;
    double radiusLimit;    
    bool onlyOneWave = false;
    UnityEngine.GameObject wave_prefab;
    public override void Awake()
    {        
        base.Awake();
        owner = GetComponentInParent<Guard>();
        wave_prefab = UnityEngine.Resources.Load("Avatar/DogBark/barkSoundWave") as UnityEngine.GameObject;        
    }

    void CreateOneWave()
    {
        radiusStart = 1;
        radiusLimit = 10;

        wave = UnityEngine.GameObject.Instantiate(wave_prefab) as UnityEngine.GameObject;
        wave.GetComponent<BarkSoundWave>().owner = owner;
        
        if (owner)
        {
            wave.transform.position = owner.transform.position;
            wave.transform.SetParent(owner.transform);
            radiusLimit /= owner.transform.transform.localScale.x;
            radiusStart /= owner.transform.transform.localScale.x;
            onlyOneWave = false;
        }
        else
        {
            wave.transform.position = transform.position;
            onlyOneWave = true;
        }

        wave.transform.localScale = UnityEngine.Vector3.one;
        oneWaveDuration = 30;
    }

    public System.Collections.IEnumerator Waving()
    {
        int start_frame = UnityEngine.Time.frameCount;
        CreateOneWave();
        while (true)
        {
            float scale = UnityEngine.Mathf.Lerp((float)radiusStart, (float)radiusLimit, (UnityEngine.Time.frameCount - start_frame) / (float)oneWaveDuration);
            wave.transform.localScale = new UnityEngine.Vector3(scale, scale, scale);
            System.String content = "Alert Sound radius";
            content += scale.ToString("F5");
            Globals.record("testReplay", content);
            if (UnityEngine.Time.frameCount - start_frame > oneWaveDuration)
            {
                DestroyObject(wave.gameObject);
                wave = null;
                if (!onlyOneWave)
                {                                        
                    start_frame = UnityEngine.Time.frameCount;
                    CreateOneWave();
                }
                else
                {
                    DestroyObject(gameObject);
                }                
            }
            yield return null;
        }
        
    }

    public override void Start()
    {
        base.Start();        
    }

	public void StartAlert()
    {
        if (wave == null)
        {
            StartCoroutine(Waving());
        }        
    }

    public void ChaseAlert()
    {
        
        //StartCoroutine(Waving());
    }


    public void StopAlert()
    {
        StopAllCoroutines();
        if (wave != null)
        {
            DestroyObject(wave.gameObject);
            wave = null;
        }        
    }
}
