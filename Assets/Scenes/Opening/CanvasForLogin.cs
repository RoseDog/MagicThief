public class CanvasForLogin : UnityEngine.MonoBehaviour 
{
    public UnityEngine.UI.InputField EnterName;
    public MultiLanguageUIText PlaceHolder;
    public UnityEngine.UI.Button DiveInBtn;

    void Awake()
    {
        UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
        UnityEngine.GameObject.Instantiate(mgrs_prefab);
        Globals.canvasForLogin = this;
        //UnityEngine.Screen.SetResolution(332, 589, false);
    }
    
	// Use this for initialization
	void Start () 
    {        
        EnterName = Globals.getChildGameObject<UnityEngine.UI.InputField>(gameObject, "EnterName");
        PlaceHolder = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "PlaceHolder");
        
        EnterName.onValueChange.AddListener((str) => OnValueChange(str));
        DiveInBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "DiveInBtn");
        DiveInBtn.onClick.AddListener(()=>Submit());
        DiveInBtn.enabled = false;
        Globals.transition.BlackIn();        
	}

    void OnValueChange(System.String str)
    {
        if(str == "")
        {
            PlaceHolder.gameObject.SetActive(true);
            DiveInBtn.enabled = false;
        }
        else
        {
            PlaceHolder.gameObject.SetActive(false);
            DiveInBtn.enabled = true;
        }        
    }
	    
    public void Submit()
    {        
        Globals.socket.OpenWaitingUI();
        Globals.socket.Send("login" + Globals.self.separator + EnterName.text + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier);
    }    
}
