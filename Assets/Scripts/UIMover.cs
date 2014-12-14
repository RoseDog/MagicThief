public class UIMover : Actor
{
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 originPos;

    public float movingDuration = 1.2f;

    public UnityEngine.Vector3 to;

    public override void Awake()
    {
        originPos = transform.position;
        base.Awake();
    }

    public void BeginMove()
    {
        AddAction(new EaseOut(transform, to, movingDuration));
    }

    public void Goback()
    {
        AddAction(new EaseOut(transform, originPos, movingDuration));
    }
}
