public class LifeNumber : Actor
{    
    public UnityEngine.UI.Text numberText;
    public System.String prefix;
    UnityEngine.UI.RawImage LifeNumerBg;
    float scaleTo = 1.6f;
    int scaleTotalTime = 8;
    public override void Awake()
    {
        base.Awake();
        numberText = GetComponent<UnityEngine.UI.Text>();
        LifeNumerBg = GetComponentInParent<UnityEngine.UI.RawImage>();
        //LifeNumerBg.gameObject.SetActive(false);
    }

    public void UpdateText(System.String current, float amount)
    {
        numberText.text = prefix + current + "/" + amount.ToString("F0");
    }

    public void UpdateCurrentLife(System.String current, float amount, bool needScale = true)
    {
        if (GetActionAmount() == 0 && needScale)
        {
            AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2)));
        }        
        UpdateText(current, amount);        
    }
}
