public class AlphaFadeUI : Actor 
{
    public UnityEngine.UI.Graphic[] graphics;
    public override void Awake()
    {
        graphics = GetComponentsInChildren<UnityEngine.UI.Graphic>();
        base.Awake();
    }    

    public void UpdateAlpha(float a)
    {
        foreach (UnityEngine.UI.Graphic graphic in graphics)
        {
            UnityEngine.Color color = graphic.color;
            color.a = a;
            graphic.color = color;
        }
    }
}
