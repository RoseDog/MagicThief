public class GuardAlertSound : Actor 
{
    UnityEngine.SphereCollider waveCollider;
    UnityEngine.GameObject wave;
    Guard owner;
    float oneWaveDuration;
    float radiusMax;
    float radiusLimit;
    UnityEngine.ParticleSystem waveParticle;
    public override void Awake()
    {        
        base.Awake();
        owner = GetComponentInParent<Guard>();
        UnityEngine.GameObject wave_prefab = UnityEngine.Resources.Load("Avatar/Tiger/barkSoundWave") as UnityEngine.GameObject;
        wave = UnityEngine.GameObject.Instantiate(wave_prefab) as UnityEngine.GameObject;
        wave.GetComponent<BarkSoundWave>().owner = owner;
        wave.transform.SetParent(owner.transform);
        waveCollider = wave.GetComponent<UnityEngine.SphereCollider>();
        radiusMax = waveCollider.radius;
        waveCollider.enabled = false;
        waveParticle = wave.GetComponent<UnityEngine.ParticleSystem>();        
        oneWaveDuration = waveParticle.duration;
    }

    System.Collections.IEnumerator Waving()
    {
        waveCollider.enabled = true;
        float _start_time = UnityEngine.Time.time;
        waveParticle.Emit(1);
        while (true)
        {
            UnityEngine.ParticleSystem.Particle[] particles = new UnityEngine.ParticleSystem.Particle[1];
            waveParticle.GetParticles(particles);
            waveCollider.radius = UnityEngine.Mathf.Lerp(waveParticle.startSize, radiusLimit, (UnityEngine.Time.time - _start_time) / oneWaveDuration);

            // Reached target position
            if (UnityEngine.Time.time - _start_time >= oneWaveDuration)
            {
                _start_time = UnityEngine.Time.time;
                waveCollider.radius = waveParticle.startSize;
                waveParticle.Emit(1);
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
        // 这里改成invoke会好一些
        StartCoroutine(Waving());
    }

    public void StopSpotAlert()
    {
        StopAlert();
    }

    public void ChaseAlert()
    {
        radiusLimit = radiusMax;
        StartCoroutine(Waving());
    }

    public void StopChaseAlert()
    {
        StopAlert();
    }

    public void AttackAlert()
    {
        radiusLimit = radiusMax;
        StartCoroutine(Waving());
    }

    public void StopAttackAlert()
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
