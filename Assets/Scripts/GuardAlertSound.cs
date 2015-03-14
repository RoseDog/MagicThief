public class GuardAlertSound : Actor 
{
    public UnityEngine.SphereCollider waveCollider;
    public UnityEngine.GameObject wave;
    Guard owner;
    float oneWaveDuration;
    public float radiusMax;
    float radiusLimit;
    UnityEngine.ParticleSystem waveParticle;
    bool onlyOneWave = false;
    public override void Awake()
    {        
        base.Awake();
        owner = GetComponentInParent<Guard>();
        UnityEngine.GameObject wave_prefab = UnityEngine.Resources.Load("Avatar/DogBark/barkSoundWave") as UnityEngine.GameObject;
        wave = UnityEngine.GameObject.Instantiate(wave_prefab) as UnityEngine.GameObject;
        wave.GetComponent<BarkSoundWave>().owner = owner;
        waveCollider = wave.GetComponent<UnityEngine.SphereCollider>();
        if (owner)
        {
            wave.transform.position = owner.transform.position;
            wave.transform.SetParent(owner.transform);
            radiusMax = waveCollider.radius / owner.transform.localScale.x;
            onlyOneWave = false;
        }
        else
        {
            onlyOneWave = true;
            radiusMax = waveCollider.radius * 2.0f;
        }
        
        wave.transform.localScale = UnityEngine.Vector3.one;                
        waveCollider.enabled = false;
        waveParticle = wave.GetComponent<UnityEngine.ParticleSystem>();        
        oneWaveDuration = 30;
    }

    System.Collections.IEnumerator Waving()
    {
        waveCollider.enabled = true;
        int start_frame = UnityEngine.Time.frameCount;
        waveParticle.Emit(1);
        while (true)
        {
            UnityEngine.ParticleSystem.Particle[] particles = new UnityEngine.ParticleSystem.Particle[1];
            waveParticle.GetParticles(particles);
            waveCollider.radius = UnityEngine.Mathf.Lerp(waveParticle.startSize, radiusLimit, (UnityEngine.Time.frameCount - start_frame) / (float)oneWaveDuration);
            System.String content = "Alert Sound radius";
            content += waveCollider.radius.ToString("F5");
            Globals.record("testReplay", content);
            if (UnityEngine.Time.frameCount - start_frame > oneWaveDuration)
            {
                if (onlyOneWave)
                {
                    DestroyObject(gameObject);
                    DestroyObject(wave.gameObject);
                }
                else
                {
                    start_frame = UnityEngine.Time.frameCount;
                    waveCollider.radius = waveParticle.startSize;
                    waveParticle.Emit(1);
                }                
            }
            yield return null;
        }
        
    }

    public override void Start()
    {
        base.Start();        
    }

	public void SpotAlert()
    {
        radiusLimit = radiusMax;        
        StartCoroutine(Waving());        
    }

    public void StopSpotAlert()
    {
        StopAlert();
    }

    public void ChaseAlert()
    {
        radiusLimit = radiusMax;
        //StartCoroutine(Waving());
    }

    public void StopChaseAlert()
    {
        StopAlert();
    }

    public void StopAlert()
    {
        waveParticle.Stop();
        StopAllCoroutines();
        waveCollider.enabled = false;
    }
}
