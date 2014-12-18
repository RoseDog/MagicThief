public class UIMover : Actor
{
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 originPos;    
    public UnityEngine.Vector3 to;

    public override void Awake()
    {        
        base.Awake();
        originPos = (transform as UnityEngine.RectTransform).anchoredPosition;
        UnityEngine.Debug.Log(originPos);
    }

    public void BeginMove(float movingDuration)
    {
        AddAction(new EaseOut(transform, to, movingDuration));
    }

    public void Goback(float movingDuration)
    {
        AddAction(new EaseOut(transform, originPos, movingDuration));
    }
}
