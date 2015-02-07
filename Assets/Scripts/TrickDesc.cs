public class TrickDesc : CustomEventTrigger 
{
    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
    }
}
