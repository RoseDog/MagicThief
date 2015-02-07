public class MsgBox : AlphaFadeUI 
{
    public AlphaFadeUI MsgBoxBG;
    public UnityEngine.UI.Text msgBoxText;
    public UnityEngine.UI.Button msgBoxBtn;
    
    public override void Awake()
    {
        base.Awake();
        MsgBoxBG = Globals.getChildGameObject<AlphaFadeUI>(gameObject, "MsgBoxBG");
        msgBoxText = Globals.getChildGameObject<UnityEngine.UI.Text>(MsgBoxBG.gameObject, "Text");
        msgBoxBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(MsgBoxBG.gameObject, "Btn");
    }

    public void Msg(System.String text, UnityEngine.Events.UnityAction action = null)
    {
        transform.parent = Globals.LevelController.mainCanvas.transform;
        transform.SetAsLastSibling();
        (transform as UnityEngine.RectTransform).anchoredPosition = UnityEngine.Vector2.zero;
        transform.localScale = UnityEngine.Vector3.one;

        MsgBoxBG.UpdateAlpha(0);
        MsgBoxBG.AddAction(new FadeUI(MsgBoxBG, 0, 1, 0.7f));
        msgBoxBtn.interactable = true;
        msgBoxBtn.onClick.RemoveAllListeners();
        msgBoxText.text = text;
        if (action != null)
        {
            msgBoxBtn.onClick.AddListener(action);
        }
        msgBoxBtn.onClick.AddListener(() => MessageBoxBtnClicked());

        UpdateAlpha(0);
        AddAction(new FadeUI(this, 0, 0.4f, 0.7f));
    }

    public void MessageBoxBtnClicked()
    {
        UnityEngine.Debug.Log("MessageBoxBtnClicked");
        MsgBoxBG.AddAction(new Sequence(new FadeUI(MsgBoxBG, 1, 0, 0.7f), new FunctionCall(this, "FadeOver")));
        AddAction(new FadeUI(this, 0.4f, 0, 0.7f));
        msgBoxBtn.interactable = false;
    }

    public void FadeOver()
    {
        UnityEngine.Debug.Log("FadeOver");
        Destroy(gameObject);
    }
}
