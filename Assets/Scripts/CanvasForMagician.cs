
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject CashNumerBg;
    public Number cashNumber;
    public LifeNumber lifeNumber;    
    public UnityEngine.GameObject LifeNumerBg;
    public LifeNumber PowerNumber;
    public UnityEngine.GameObject PowerNumerBg;
    public Number RoseNumber;
    public UnityEngine.GameObject RoseNumberBg;
    public UnityEngine.UI.Text RestartText;
    public AlphaFadeUI MsgBoxMask;
    public AlphaFadeUI MsgBoxBG;
    public UnityEngine.UI.Text msgBoxText;
    public UnityEngine.UI.Button msgBoxBtn;
	// Use this for initialization
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;        
        CashNumerBg = UnityEngine.GameObject.Find("CashNumerBg");
        cashNumber = CashNumerBg.GetComponentInChildren<Number>();

        LifeNumerBg = Globals.getChildGameObject(gameObject, "LifeNumerBg");
        lifeNumber = LifeNumerBg.GetComponentInChildren<LifeNumber>();

        PowerNumerBg = Globals.getChildGameObject(gameObject, "PowerNumerBg");
        PowerNumber = PowerNumerBg.GetComponentInChildren<LifeNumber>();

        RoseNumberBg = UnityEngine.GameObject.Find("RoseNumberBg");
        RoseNumber = RoseNumberBg.GetComponentInChildren<Number>();                      

        RestartText = UnityEngine.GameObject.Find("RestartText").GetComponent<UnityEngine.UI.Text>();

        MsgBoxMask = Globals.getChildGameObject<AlphaFadeUI>(gameObject, "MsgBoxMask");
        MsgBoxBG = Globals.getChildGameObject<AlphaFadeUI>(gameObject, "MsgBoxBG");
        msgBoxText = Globals.getChildGameObject<UnityEngine.UI.Text>(MsgBoxBG.gameObject, "Text");
        msgBoxBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(MsgBoxBG.gameObject, "Btn");
        
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(mgrs_prefab);
        }
        InitUIStats();
	}

    void Start()
    {
        //InitUIStats();
    }

    void InitUIStats()
    {
        RestartText.gameObject.SetActive(false);
        msgBoxBtn.interactable = false;
        MsgBoxMask.gameObject.SetActive(false);
        MsgBoxBG.gameObject.SetActive(false);
    }

    public void SetLifeVisible(bool Visible)
    {
        LifeNumerBg.SetActive(Visible);
        PowerNumerBg.SetActive(Visible);
    }

    public void SetCashVisible(bool Visible)
    {
        CashNumerBg.SetActive(Visible);
    }

    public void MessageBox(System.String text, UnityEngine.Events.UnityAction action = null)
    {        
        MsgBoxBG.gameObject.SetActive(true);
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

        MsgBoxMask.gameObject.SetActive(true);
        MsgBoxMask.UpdateAlpha(0);        
        MsgBoxMask.AddAction(new FadeUI(MsgBoxMask, 0, 0.4f, 0.7f));
    }

    public void MessageBoxBtnClicked()
    {
        UnityEngine.Debug.Log("MessageBoxBtnClicked");
        MsgBoxBG.AddAction(new Sequence(new FadeUI(MsgBoxBG, 1, 0, 0.7f), new FunctionCall(this, "FadeOver")));
        MsgBoxMask.AddAction(new FadeUI(MsgBoxMask, 0.4f, 0, 0.7f));
        msgBoxBtn.interactable = false; 
    }

    public void FadeOver()
    {
        UnityEngine.Debug.Log("FadeOver");
        MsgBoxMask.gameObject.SetActive(false);
        MsgBoxBG.gameObject.SetActive(false);
    }
}
