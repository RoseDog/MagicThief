public class GuardInfoUI : CustomEventTrigger
{
    UnityEngine.UI.Image GuardIcon;
    UnityEngine.UI.Text Name;
    UnityEngine.UI.Text Desc;
    UnityEngine.UI.Text StrengthNumber;
    UnityEngine.UI.Text VisionNumber;
    UnityEngine.UI.Text SpeedNumber;
    public override void Awake()
    {
        base.Awake();
        GuardIcon = Globals.getChildGameObject<UnityEngine.UI.Image>(gameObject, "GuardIcon");
        Name = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Name");
        Desc = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Desc");
        StrengthNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "StrengthNumber");
        VisionNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "VisionNumber");
        SpeedNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "SpeedNumber");

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
            SpeedNumber.text = System.Math.Round(guard.moving.speed).ToString();
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
