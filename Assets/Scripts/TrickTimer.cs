public class TrickTimer : Actor 
{
    public UnityEngine.GameObject actor;
    
    GUIBarScript gui_bar_script;
    UnityEngine.UI.Text timerText;

    float duration;
    float currentTime;
    UnityEngine.Vector3 posOffset;
    public override void Awake()
    {
        base.Awake();
        gui_bar_script = GetComponent<GUIBarScript>();
        timerText = GetComponentInChildren<UnityEngine.UI.Text>();
    }

    public void BeginCountDown(UnityEngine.GameObject a, float durationTime, UnityEngine.Vector3 timerOffset)
    {
        actor = a;
        duration = durationTime;
        currentTime = durationTime;
        posOffset = timerOffset;
        transform.position = actor.transform.position + posOffset;
    }

    public void AddTime(float time)
    {
        currentTime += time;
        duration += time;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (actor != null)
        {
            transform.position = actor.transform.position + posOffset;
            currentTime -= UnityEngine.Time.fixedDeltaTime;
            float val = (float)currentTime / duration;
            if (gui_bar_script != null)
            {
                gui_bar_script.Value = val;
            }            
            if (currentTime <= 0.0f)
            {
                currentTime = 0.0f;
            }
            timerText.text = currentTime.ToString("F1");
        }
    }    
}
