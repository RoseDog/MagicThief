
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public SelectGuard selectGuard;
    public UnityEngine.UI.Text tutorialText;
    public UnityEngine.GameObject CashNumerBg;
    public Cash cashNumber;
    public LifeNumber lifeNumber;
    public UnityEngine.GameObject LifeNumerBg;
    public UnityEngine.UI.Text RestartText;
	// Use this for initialization
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;
        selectGuard = GetComponentInChildren<SelectGuard>();
        tutorialText = UnityEngine.GameObject.Find("TutorialText").GetComponent<UnityEngine.UI.Text>();
        CashNumerBg = UnityEngine.GameObject.Find("CashNumerBg");
        cashNumber = GetComponentInChildren<Cash>();

        LifeNumerBg = UnityEngine.GameObject.Find("LifeNumerBg");
        lifeNumber = GetComponentInChildren<LifeNumber>();
        
        RestartText = UnityEngine.GameObject.Find("RestartText").GetComponent<UnityEngine.UI.Text>();
        

        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject mgrs = UnityEngine.GameObject.Instantiate(mgrs_prefab) as UnityEngine.GameObject;
        }
        InitUIStats();
	}

    void InitUIStats()
    {
        RestartText.gameObject.SetActive(false);
    }

    public void SetLifeVisible(bool Visible)
    {
        LifeNumerBg.SetActive(Visible);
    }

    public void SetCashVisible(bool Visible)
    {
        CashNumerBg.SetActive(Visible);
    }
}
