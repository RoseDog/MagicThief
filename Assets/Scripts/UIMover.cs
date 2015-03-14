public class UIMover : Actor
{
    UnityEngine.Vector3 originPosition;
    public UnityEngine.Vector3 to;

    public override void Awake()
    {        
        base.Awake();
        originPosition = (transform as UnityEngine.RectTransform).anchoredPosition;
    }

    public void BeginMove(int movingDuration)
    {
        AddAction(new EaseOut(transform, to, movingDuration));
    }

    public void Goback(int movingDuration)
    {
        AddAction(new EaseOut(transform, originPosition, movingDuration));
    }

    public void Jump()
    {        
        AddAction(new RepeatForever(
            new MoveTo(transform, to, Globals.uiMoveAndScaleDuration/2, true),
            new MoveTo(transform, originPosition, Globals.uiMoveAndScaleDuration/2, true)));
    }

    public void ForeverMoving(int movingDuration)
    {        
        AddAction(new RepeatForever(
            new MoveTo(transform, to, movingDuration, true), new FunctionCall(()=>RecoverPos())));
    }

    public void RecoverPos()
    {
        (transform as UnityEngine.RectTransform).anchoredPosition = originPosition;
    }
}
