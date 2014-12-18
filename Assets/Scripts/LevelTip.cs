public class LevelTip : AlphaFadeUI 
{
    public float fadeDuration = 1;
    public float waitDuration = 1.5f;
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
        AddAction(new Sequence(new FadeUI(this, 0, 1, fadeDuration), new SleepFor(waitDuration),
            new FadeUI(this, 1, 0, fadeDuration), new FunctionCall(this, "FadeOver")));
    }

    void FadeOver()
    {
        gameObject.SetActive(false);
    }
}
