public class LifeNumber : Actor
{    
    UnityEngine.UI.Text lifeText;
    UnityEngine.UI.RawImage LifeNumerBg;
    float scaleTo = 1.6f;
    int scaleTotalTime = 8;
    public override void Awake()
    {
        base.Awake();
        lifeText = GetComponent<UnityEngine.UI.Text>();
        LifeNumerBg = GetComponentInParent<UnityEngine.UI.RawImage>();
        //LifeNumerBg.gameObject.SetActive(false);
    }

    public void UpdateText(float current, float amount)
    {
        lifeText.text = current.ToString("F0") + "/" + amount.ToString("F0");
    }

    public void UpdateCurrentLife(float current, float amount)
    {
        if (GetActionAmount() == 0)
        {
            AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2)));
        }        
        UpdateText(current, amount);        
    }
}
