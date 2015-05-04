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
        DiveInBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "DiveInBtn");

        if (System.IO.File.Exists(UnityEngine.Application.persistentDataPath + "/name.txt"))
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(UnityEngine.Application.persistentDataPath + "/name.txt",
               System.Text.Encoding.UTF8);
            EnterName.text = stream.ReadLine();
            stream.Close();
        }
        
        if (EnterName.text == "")
        {
            EnterName.onValueChange.AddListener((str) => OnValueChange(str));
            DiveInBtn.onClick.AddListener(() => Submit());
            DiveInBtn.enabled = false;
            Globals.transition.BlackIn();        
        }
        else
        {
            EnterName.gameObject.SetActive(false);
            PlaceHolder.gameObject.SetActive(false);
            DiveInBtn.gameObject.SetActive(false);
            Invoke("Submit", 3.0f);
        }                        
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
        if (Globals.socket.ws.ReadyState != WebSocketSharp.WebSocketState.Connecting)
        {
            Globals.socket.ws.ConnectAsync();
        }
        Globals.socket.OpenWaitingUI();
        Globals.self.name = EnterName.text;
        Globals.socket.Send("login" + Globals.self.separator + EnterName.text + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier);
    }    
}
