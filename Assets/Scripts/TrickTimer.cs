public class TrickTimer : Actor 
{
    public UnityEngine.GameObject actor;
    UnityEngine.UI.Image unlockProgressSprite;
    UnityEngine.UI.Text timerText;

    int duration;
    int lastFrames;
    UnityEngine.Vector3 posOffset;
    public override void Awake()
    {
        base.Awake();
        timerText = GetComponentInChildren<UnityEngine.UI.Text>();
        unlockProgressSprite = Globals.getChildGameObject<UnityEngine.UI.Image>(gameObject, "progress");
        if (unlockProgressSprite != null)
        {
            unlockProgressSprite.type = UnityEngine.UI.Image.Type.Filled;
            unlockProgressSprite.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            unlockProgressSprite.fillOrigin = (int)UnityEngine.UI.Image.OriginHorizontal.Left;
        }
        
    }

    public void BeginCountDown(UnityEngine.GameObject a, int durationTime, UnityEngine.Vector3 timerOffset)
    {
        actor = a;
        duration = durationTime;
        lastFrames = durationTime;
        posOffset = timerOffset;
        transform.position = actor.transform.position + posOffset;
    }

    public void AddFrameTime(int frames)
    {
        lastFrames += frames;
        duration += frames;
    }

    public int GetLastFrameTime()
    {
        return lastFrames;
    }

    public override void Update()
    {
        base.Update();
        if (actor != null)
        {
            transform.rotation = UnityEngine.Quaternion.identity;
            transform.position = actor.transform.position + posOffset;
            --lastFrames;
            if (unlockProgressSprite != null)
            {
                unlockProgressSprite.fillAmount = (float)lastFrames / duration;
            }                        
            if (lastFrames <= 0.0f)
            {
                lastFrames = 0;
            }            
        }
    }

    public void FixedUpdate()
    {
        if (actor != null)
        {
            timerText.text = (lastFrames * UnityEngine.Time.fixedDeltaTime).ToString("F1");
        }        
    }
}
