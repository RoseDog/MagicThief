public class PoorBuilding : Building 
{
    UnityEngine.RectTransform poorsNeedMoneyTip;
    
    UnityEngine.UI.Button yesBtn;
    UnityEngine.UI.Text costText;
    float cost = 1000;
    UnityEngine.UI.Button noBtn;
    UnityEngine.Vector3 tipScaleCache;
    
    public override void Awake()
    {        
        base.Awake();
        poorsNeedMoneyTip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "PoorsNeedMoney");
        tipScaleCache = poorsNeedMoneyTip.transform.localScale;
        poorsNeedMoneyTip.localScale = UnityEngine.Vector3.zero;
        
        yesBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "Yes");
        yesBtn.onClick.AddListener(() => YesBtnClicked());
        costText = Globals.getChildGameObject<UnityEngine.UI.Text>(yesBtn.gameObject, "cost");
        costText.text = cost.ToString("F0");
        noBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "No");
        noBtn.onClick.AddListener(() => NoBtnClicked());
    }


    public override void Choosen()
    {
        Cocos2dParallel actions = new Cocos2dParallel();

        if (poorsNeedMoneyTip.localScale.x < UnityEngine.Mathf.Epsilon)
        {
            AddAction(new Sequence(
            new ScaleTo(poorsNeedMoneyTip.transform, tipScaleCache * 1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(poorsNeedMoneyTip.transform, tipScaleCache, Globals.uiMoveAndScaleDuration / 4)));

        }
        AddAction(actions);
        base.Choosen();
    }

    public override void Unchoose()
    {
        AddAction(new ScaleTo(poorsNeedMoneyTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        base.Unchoose();
    }


    public void YesBtnClicked()
    {        
        if (Globals.canvasForMagician.ChangeCash(-cost))
        {
            Globals.self.TurnPoorToRose(data);
            yesBtn.interactable = false;
        }
    }

    public void NoBtnClicked()
    {
        AddAction(new ScaleTo(poorsNeedMoneyTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
    }
}
