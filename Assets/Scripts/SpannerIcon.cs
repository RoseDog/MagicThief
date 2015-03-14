public class SpannerIcon : AlphaFadeUI 
{
    public override void Start()
    {
        base.Start();
        FadeAgain();
    }

    public void FadeAgain()
    {
        AddAction(new Sequence(new FadeUI(this, 1.0f, 0.0f, 0.7f), new FadeUI(this, 0.0f, 1.0f, 0.7f), new FunctionCall(()=>FadeAgain())));
    }
}
