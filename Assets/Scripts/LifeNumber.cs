public class LifeNumber : Actor
{
    public int LifeAmount = 100;
    public int LifeCurrent;
    UnityEngine.UI.Text lifeText;
    float scaleTo = 1.6f;
    float scaleTotalTime = 0.2f;
    public override void Awake()
    {
        base.Awake();
        lifeText = GetComponent<UnityEngine.UI.Text>();
    }
	
    public void Reset()
    {
        LifeCurrent = LifeAmount;
        UpdateText();
    }

    void UpdateText()
    {
        lifeText.text = LifeCurrent.ToString() + "/" + LifeAmount.ToString();
    }

    public void ChangeLife(int delta)
    {
        AddAction(new Sequence(
                    new ScaleTo(transform, new UnityEngine.Vector3(scaleTo, scaleTo, scaleTo), scaleTotalTime / 2.0f),
                    new ScaleTo(transform, new UnityEngine.Vector3(1f, 1f, 1f), scaleTotalTime / 2.0f)));
        LifeCurrent += delta;
        LifeCurrent = UnityEngine.Mathf.Clamp(LifeCurrent, 0, LifeAmount);
        UpdateText();
        if (LifeCurrent == 0)
        {
            Globals.LevelController.MagicianLifeOver();
        }
    }
}
