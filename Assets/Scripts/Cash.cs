public class Cash : Actor
{
    UnityEngine.UI.Text cashNumer;
    public float cashAmont = 0;
    float scaleTo = 1.6f;
    float scaleTotalTime = 0.2f;
    public override void Awake()
    {
        base.Awake();
        cashNumer = GetComponent<UnityEngine.UI.Text>();
    }

    public void SetToZero()
    {
        cashAmont = 0;
        cashNumer.text = cashAmont.ToString("F0");
    }

    public void Add(float cash)
    {
        SetNumber(cashAmont + cash);
        ClearAllActions();
        AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2.0f),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2.0f)));
    }

    public void SetNumber(float number)
    {
        cashAmont = number;
        cashNumer.text = cashAmont.ToString("F0");
    }
}
