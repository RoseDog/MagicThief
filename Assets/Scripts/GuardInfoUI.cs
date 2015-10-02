public class GuardInfoUI : CustomEventTrigger
{
    public UnityEngine.UI.Image GuardIcon;
    public UnityEngine.UI.Text Name;
    public UnityEngine.UI.Text Desc;
    public UnityEngine.UI.Text StrengthNumber;
    public UnityEngine.UI.Text VisionNumber;
    public UnityEngine.UI.Text SpeedNumber;
    public UnityEngine.UI.Text PerformingIncomeNumber;
    public override void Awake()
    {
        base.Awake();
    }

	public void SetGuard(Guard guard)
    {
        transform.parent.parent = Globals.LevelController.mainCanvas.transform;
        transform.parent.SetAsLastSibling();
        (transform.parent as UnityEngine.RectTransform).anchoredPosition = UnityEngine.Vector2.zero;

        transform.parent.localScale = UnityEngine.Vector3.one;
        Globals.languageTable.SetText(Name, guard.name);
        Globals.languageTable.SetText(Desc, guard.name + "_desc");
        GuardIcon.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + guard.data.name + "_hireInfoIcon");
        if (guard.patrol != null)
        {            
            StrengthNumber.text = guard.data.attackValue.ToString();
            VisionNumber.text = guard.eye.fovMaxDistance.ToString();
            SpeedNumber.text = guard.data.moveSpeed.ToString("F1");
            PerformingIncomeNumber.text = guard.data.income.ToString("") + "/H";
        }
        else
        {
            StrengthNumber.gameObject.SetActive(false);
            VisionNumber.gameObject.SetActive(false);
            SpeedNumber.gameObject.SetActive(false);
        }      
    }
    
    public void Close()
    {
        DestroyObject(transform.parent.gameObject);
    }
}
