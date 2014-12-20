public class LevelTip : AlphaFadeUI 
{
    [UnityEngine.HideInInspector]
    float fade = 1.0f;
    [UnityEngine.HideInInspector]
    float wait = 1.0f;
    UnityEngine.UI.Text tip;
    
    public override void Awake()
    {
        base.Awake();
        tip = GetComponentInChildren<UnityEngine.UI.Text>();        
        UpdateAlpha(0);
    }

    public void Show(System.String text)
    {
        gameObject.SetActive(true);
        tip.text = text;
        AddAction(new Sequence(new FadeUI(this, 0, 1, fade), new SleepFor(wait),
            new FadeUI(this, 1, 0, fade), new FunctionCall(this, "FadeOver")));
    }

    void FadeOver()
    {
        gameObject.SetActive(false);
    }

    public float GetFadeDuration()
    {
        return fade;
    }

    public float GetWaitingDuration()
    {
        return wait;
    }
}
