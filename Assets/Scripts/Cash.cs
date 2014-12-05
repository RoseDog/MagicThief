public class Cash : Actor
{
    UnityEngine.UI.Text cashNumer;
    UnityEngine.UI.RawImage cashIcon;
    int cashAmont = 0;
    float scaleTo = 1.6f;
    float scaleTotalTime = 0.2f;
    public override void Awake()
    {
        base.Awake();
        cashNumer = GetComponent<UnityEngine.UI.Text>();
        cashIcon = UnityEngine.GameObject.Find("CashIcon").GetComponent<UnityEngine.UI.RawImage>();
    }
    public void Add(int cash)
    {
        cashAmont += cash;
        cashNumer.text = cashAmont.ToString();
        
        AddAction(new Sequence(
                    new ScaleTo(new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime/2.0f),
                    new ScaleTo(new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2.0f)));
    }    
}
