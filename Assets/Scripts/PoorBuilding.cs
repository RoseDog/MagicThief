public class PoorBuilding : Building 
{
    UnityEngine.RectTransform poorsNeedMoneyTip;
    UnityEngine.RectTransform helpMark;
    UnityEngine.UI.Button yesBtn;
    UnityEngine.UI.Button noBtn;
    UnityEngine.Vector3 tipScaleCache;
    UnityEngine.Vector3 markScaleCache;
    public override void Awake()
    {        
        base.Awake();
        poorsNeedMoneyTip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "PoorsNeedMoney");
        tipScaleCache = poorsNeedMoneyTip.transform.localScale;
        poorsNeedMoneyTip.localScale = UnityEngine.Vector3.zero;
        helpMark = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "Help");
        markScaleCache = helpMark.transform.localScale;
        helpMark.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => HelpMarkClicked());
        yesBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "Yes");
        yesBtn.onClick.AddListener(() => YesBtnClicked());
        noBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "No");
        noBtn.onClick.AddListener(() => NoBtnClicked());
    }

    public override void Choosen()
    {
        Cocos2dParallel actions = new Cocos2dParallel();
        if (helpMark.localScale.x < UnityEngine.Mathf.Epsilon)
        {
            actions.actions.Add(new Sequence(
            new ScaleTo(helpMark.transform, markScaleCache*1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(helpMark.transform, markScaleCache, Globals.uiMoveAndScaleDuration / 4)));
        }

        if (poorsNeedMoneyTip.localScale.x > UnityEngine.Mathf.Epsilon)
        {
            actions.actions.Add(new ScaleTo(poorsNeedMoneyTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        }
        AddAction(actions);
        base.Choosen();
    }

    public override void Unchoose()
    {
        AddAction(new ScaleTo(helpMark.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        AddAction(new ScaleTo(poorsNeedMoneyTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        base.Unchoose();
    }

    public void HelpMarkClicked()
    {
        AddAction(new ScaleTo(helpMark.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        AddAction(new Sequence(
            new ScaleTo(poorsNeedMoneyTip.transform, tipScaleCache*1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(poorsNeedMoneyTip.transform, tipScaleCache, Globals.uiMoveAndScaleDuration / 4)));
    }

    public void YesBtnClicked()
    {
        Globals.AddRoseBuilding(buildingAchive);
        city.DestroyPoorBuilding(buildingAchive);
        city.BornBuilding(buildingAchive, "Props/RoseBuilding");
    }

    public void NoBtnClicked()
    {
        AddAction(new ScaleTo(poorsNeedMoneyTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
    }
}
