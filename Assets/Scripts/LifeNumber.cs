public class LifeNumber : Actor
{    
    UnityEngine.UI.Text lifeText;
    UnityEngine.UI.RawImage LifeNumerBg;
    float scaleTo = 1.6f;
    float scaleTotalTime = 0.2f;
    public override void Awake()
    {
        base.Awake();
        lifeText = GetComponent<UnityEngine.UI.Text>();
        LifeNumerBg = GetComponentInParent<UnityEngine.UI.RawImage>();
        LifeNumerBg.gameObject.SetActive(false);
    }

    public void UpdateText(int current, int amount)
    {
        lifeText.text = current.ToString() + "/" + amount.ToString();
    }

    public void UpdateCurrentLife(int current, int amount)
    {
        AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2.0f),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2.0f)));
        UpdateText(current, amount);        
    }
}
