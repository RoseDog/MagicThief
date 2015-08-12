public class GuardAlertSound : Actor 
{
    public UnityEngine.GameObject wave;
    Guard owner;
    double oneWaveDuration;
    double radiusStart;
    double radiusLimit = 2000;
    double radius;
    bool onlyOneWave = false;
    UnityEngine.GameObject wave_prefab;
    Cocos2dAction waving_action = null;
    public override void Awake()
    {        
        base.Awake();
        owner = GetComponentInParent<Guard>();
        wave_prefab = UnityEngine.Resources.Load("Avatar/DogBark/barkSoundWave") as UnityEngine.GameObject;        
    }

    public void SetRadius(double radius)
    {
        radiusLimit = radius;
    }

    void CreateOneWave()
    {
        radiusStart = 1;
        radius = radiusLimit;

        wave = UnityEngine.GameObject.Instantiate(wave_prefab) as UnityEngine.GameObject;
        //wave.GetComponent<BarkSoundWave>().owner = owner;
        
        if (owner)
        {
            wave.transform.position = owner.transform.position;
            wave.transform.SetParent(owner.transform);
            radius /= owner.transform.transform.localScale.x;
            radiusStart /= owner.transform.transform.localScale.x;
            onlyOneWave = false;
        }
        else
        {
            wave.transform.position = transform.position;
            onlyOneWave = true;
        }

        wave.transform.localScale = UnityEngine.Vector3.zero;
        oneWaveDuration = 30;
    }

    int start_frame;
    public void Waving()
    {
        float scale = UnityEngine.Mathf.Lerp((float)radiusStart, (float)radius, (Globals.LevelController.frameCount - start_frame) / (float)oneWaveDuration);
        wave.transform.localScale = new UnityEngine.Vector3(scale, scale, scale);

        if (Globals.DEBUG_REPLAY)
        {
            System.String content = "Alert Sound radius";
            content += scale.ToString("F5");
            Globals.record("testReplay", content);
        }
        
        if (Globals.LevelController.frameCount - start_frame > oneWaveDuration)
        {
            Actor.to_be_remove.Add(wave.GetComponent<Actor>());            
            wave = null;
            if (!onlyOneWave)
            {
                start_frame = Globals.LevelController.frameCount;
                CreateOneWave();
            }
            else
            {
                Actor.to_be_remove.Add(this);
            }
        }
    }

	public void StartAlert()
    {
        if (waving_action == null)
        {
            start_frame = Globals.LevelController.frameCount;
            CreateOneWave();
            waving_action = RepeatingCallFunction(0,()=>Waving());
        }        
    }

    public void ChaseAlert()
    {
        
        //StartCoroutine(Waving());
    }


    public void StopAlert()
    {
        RemoveAction(ref waving_action);
        if (wave != null)
        {
            Actor.to_be_remove.Add(wave.GetComponent<Actor>());            
            wave = null;
        }        
    }
}
