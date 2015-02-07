public class UIMover : Actor
{
    UnityEngine.Vector3 originPosition;
    public UnityEngine.Vector3 to;

    public override void Awake()
    {        
        base.Awake();
        originPosition = (transform as UnityEngine.RectTransform).anchoredPosition;
    }

    public void BeginMove(float movingDuration)
    {
        AddAction(new EaseOut(transform, to, movingDuration));
    }

    public void Goback(float movingDuration)
    {
        AddAction(new EaseOut(transform, originPosition, movingDuration));
    }

    public void Jump()
    {        
        AddAction(new RepeatForever(
            new MoveTo(transform, to, Globals.uiMoveAndScaleDuration*0.5f, true),
            new MoveTo(transform, originPosition, Globals.uiMoveAndScaleDuration * 0.5f, true)));
    }

    public void ForeverMoving(float movingDuration)
    {        
        AddAction(new RepeatForever(
            new MoveTo(transform, to, movingDuration, true), new FunctionCall(this, "RecoverPos")));
    }

    public void RecoverPos()
    {
        (transform as UnityEngine.RectTransform).anchoredPosition = originPosition;
    }
}
