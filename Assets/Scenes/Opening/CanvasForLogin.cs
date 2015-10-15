public class CanvasForLogin : LevelController
{
    public UnityEngine.UI.InputField EnterName;
    public MultiLanguageUIText PlaceHolder;
    public UnityEngine.UI.Button DiveInBtn;
    UnityEngine.RectTransform caret;
    public MultiLanguageUIText version;
    public UnityEngine.GameObject RosedogBillboard;
    public override void Awake()
    {
        base.Awake();
        Globals.canvasForLogin = this;
        version.text = "ver"+Globals.versionString;
        //UnityEngine.Screen.SetResolution(332, 589, false);
    }

    public void CloseRosedogBillboard()
    {
        RosedogBillboard.SetActive(false);
        if (EnterName.text == "")
        {
            Globals.transition.BlackIn();
        }
        else
        {
            LoginUIVisible(false);
            Submit();
        }    
    }
    
	// Use this for initialization
    public override void Start() 
    {
        //base.Start();
        EnterName = Globals.getChildGameObject<UnityEngine.UI.InputField>(gameObject, "EnterName");
        
        PlaceHolder = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "PlaceHolder");
        DiveInBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "DiveInBtn");
        DiveInBtn.onClick.AddListener(() => Submit());
        DiveInBtn.interactable = false;
        EnterName.onValueChange.AddListener((str) => OnValueChange(str));
        EnterName.onValidateInput = OnValidateInput;
        Invoke("CaretOffset", 0.5f);

        if (System.IO.File.Exists(UnityEngine.Application.persistentDataPath + "/name.txt"))
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(UnityEngine.Application.persistentDataPath + "/name.txt",
               System.Text.Encoding.UTF8);
            EnterName.text = stream.ReadLine();
            stream.Close();
        }        
	}

    char OnValidateInput(string text, int charIndex, char addedChar)
    {
        if (EnterName.text.Length < 8 && 
            ((addedChar >= '0' && addedChar <= '9') || 
            (addedChar >= 'a' && addedChar <= 'z') ||
            (addedChar >= 'A' && addedChar <= 'Z')))
        {
            return addedChar;
        }
        return '\0';
    }

    public void LoginUIVisible(bool visible)
    {
        EnterName.gameObject.SetActive(visible);
        PlaceHolder.gameObject.SetActive(visible);
        DiveInBtn.gameObject.SetActive(visible);
        Invoke("CaretOffset", 0.3f);
    }

    public void CaretOffset()
    {
        caret = Globals.getChildGameObject<UnityEngine.RectTransform>(EnterName.gameObject, "EnterName Input Caret");
        if (caret != null)
        {
            caret.offsetMin = new UnityEngine.Vector2(5, 15);
            caret.offsetMax = new UnityEngine.Vector2(10, 0);
        }        
    }

    void OnValueChange(System.String str)
    {       
        if(str == "")
        {
            PlaceHolder.gameObject.SetActive(true);
            DiveInBtn.interactable = false;
        }
        else
        {
            PlaceHolder.gameObject.SetActive(false);
            DiveInBtn.interactable = true;
        }        
    }
	    
    public void Submit()
    {
//         if (Globals.socket.ws.State == WebSocketSharp.WebSocketState.Connecting)
//         {
//             Globals.socket.ws.ConnectAsync();
//         }
        Globals.socket.OpenWaitingUI();
        Globals.self.name = EnterName.text;
        Globals.socket.Send("login" + Globals.self.separator + Globals.self.name + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier + Globals.self.separator + Globals.versionString);
    }    
}
