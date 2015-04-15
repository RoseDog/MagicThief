public class MsgBox : AlphaFadeUI 
{
    public AlphaFadeUI MsgBoxBG;
    public UnityEngine.UI.Text msgBoxText;
    public UnityEngine.UI.Button YesBtn;
    public UnityEngine.UI.Button NoBtn;
    
    public override void Awake()
    {
        base.Awake();
        MsgBoxBG = Globals.getChildGameObject<AlphaFadeUI>(gameObject, "MsgBoxBG");
        msgBoxText = Globals.getChildGameObject<UnityEngine.UI.Text>(MsgBoxBG.gameObject, "Text");
        YesBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(MsgBoxBG.gameObject, "YesBtn");
        NoBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(MsgBoxBG.gameObject, "NoBtn");
    }

    public void Msg(System.String text, UnityEngine.Events.UnityAction yesAction = null, bool bNeedCancel = false)
    {        
        MsgBoxBG.UpdateAlpha(0);
        MsgBoxBG.AddAction(new FadeUI(MsgBoxBG, 0, 1, 0.2f));        
        msgBoxText.text = text;

        YesBtn.interactable = true;
        YesBtn.onClick.RemoveAllListeners();
        if (yesAction != null)
        {
            YesBtn.onClick.AddListener(yesAction);
        }
        YesBtn.onClick.AddListener(() => MessageBoxBtnClicked());

        NoBtn.interactable = true;
        NoBtn.onClick.RemoveAllListeners();
        if (bNeedCancel)
        {
            NoBtn.onClick.AddListener(() => MessageBoxBtnClicked());
        }
        else
        {
            NoBtn.gameObject.SetActive(false);
            UnityEngine.Vector2 pos = YesBtn.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
            YesBtn.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(0, pos.y);
        }
        
        UpdateAlpha(0);
        AddAction(new FadeUI(this, 0, 0.4f, 0.2f));
    }

    public void MessageBoxBtnClicked()
    {
        UnityEngine.Debug.Log("MessageBoxBtnClicked");
        MsgBoxBG.AddAction(new Sequence(new FadeUI(MsgBoxBG, 1, 0, 0.7f), new FunctionCall(()=>FadeOver())));
        AddAction(new FadeUI(this, 0.4f, 0, 0.7f));
        YesBtn.interactable = false;
    }

    public void FadeOver()
    {
        UnityEngine.Debug.Log("FadeOver");
        Destroy(transform.parent.gameObject);
    }
}
