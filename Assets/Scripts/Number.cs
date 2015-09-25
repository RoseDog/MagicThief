public class Number : Actor
{
    UnityEngine.UI.Text numberText;
    public float numberAmont = 0;
    float scaleTo = 1.6f;
    int scaleTotalTime = 6;
    public override void Awake()
    {
        base.Awake();
        numberText = GetComponent<UnityEngine.UI.Text>();
    }

    public void SetToZero()
    {
        numberAmont = 0;
        numberText.text = numberAmont.ToString("F0");
    }

    public void Add(float cash)
    {
        SetNumber(numberAmont + cash);
        ClearAllActions();
        AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2)));
    }

    public void SetNumber(float number)
    {
        numberAmont = number;
        numberText.text = numberAmont.ToString("F0");
    }
}
