public class BarkSoundWave : Actor
{
    public Guard owner;
    public int oneWaveDuration = 30;
    public int fadeDuration = 10;
    public double radiusStart = 1;
    public double radiusLimit = 2000;
    int start_frame;

    public override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<UnityEngine.SpriteRenderer>();
        transform.localScale = UnityEngine.Vector3.zero;
        start_frame = Globals.LevelController.frameCount;
        RepeatingCallFunction(0, ()=>Waving());
    }

    public override void Start()
    {
        base.Start();
        SleepThenCallFunction(oneWaveDuration + fadeDuration, () => WavingEnd());
    }

    void WavingEnd()
    {
        Destroy(gameObject);
    }

    public void Waving()
    {
        if (Globals.LevelController.frameCount - start_frame < oneWaveDuration)
        {
            float scale = UnityEngine.Mathf.Lerp((float)radiusStart, (float)radiusLimit, (Globals.LevelController.frameCount - start_frame) / (float)oneWaveDuration);
            transform.localScale = new UnityEngine.Vector3(scale, scale, scale);
        }
        else
        {
            float alpha = UnityEngine.Mathf.Lerp(0.0f, 1.0f, (Globals.LevelController.frameCount - start_frame - oneWaveDuration) / (float)fadeDuration);

            spriteRenderer.color = new UnityEngine.Color(1, 1, 1, 1-alpha);
        }
        
    }


    public override void TouchBegin(Actor other)    
    {
        base.TouchBegin(other);
        Guard guard = other as Guard;
        // 声音碰到的不是自己，有听觉，没有正在追击的目标
        if (owner != guard && guard.heardAlert != null && /*guard.heardAlert.alertTeammate == null &&*/ guard.spot.target == null)
        {
            guard.heardAlert.HeardSound(transform.position);
//             if (owner == null)
//             {
//                 guard.heardAlert.HeardSound(transform.position);
//             }
//             else
//             {
//                 guard.heardAlert.Heard(owner);
//             }            
        }
    }

    public override void TouchStay(Actor other)
    {
        base.TouchStay(other);        
        Guard guard = other as Guard;
        if (owner != guard && guard.heardAlert != null && /*guard.heardAlert.alertTeammate == null &&*/ guard.spot.target == null)
        {
            guard.heardAlert.HeardSound(transform.position);

//             if (owner == null)
//             {
//                 guard.heardAlert.HeardSound(transform.position);
//             }
//             else
//             {
//                 guard.heardAlert.Heard(owner);
//             }
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {

    }    
}
