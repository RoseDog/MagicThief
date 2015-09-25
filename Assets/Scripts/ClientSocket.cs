public class ClientSocket : UnityEngine.MonoBehaviour 
{
    //public WebSocketSharp.WebSocket ws;
    public WebSocket4Net.WebSocket ws;
    public System.Collections.Generic.List<UnityEngine.Events.UnityAction> threadTempActions = new System.Collections.Generic.List<UnityEngine.Events.UnityAction>();
    public System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>> serverReplyActions =
        new System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>>();
    public System.Collections.Generic.List<System.String> msgCache = new System.Collections.Generic.List<System.String>();

    UnityEngine.GameObject WaitingForServer_prefab;
    UnityEngine.GameObject WaitingForServer;
    bool ready = false;
    public bool IsReady() { return ready; }
    public void SetReady(bool value) { ready = value; }
    bool fromLoginScene = false;
    public bool IsFromLogin() { return fromLoginScene; }
	// Use this for initialization
    public void Awake()
    {
        Globals.socket = this;
        WaitingForServer_prefab = UnityEngine.Resources.Load("UI/WaitingForServer") as UnityEngine.GameObject;        
        if(!fromLoginScene && UnityEngine.GameObject.FindObjectOfType<LevelEditor>()==null)
        {
            ready = false;
        }

        //
//         ws = new WebSocketSharp.WebSocket("ws://127.0.0.1:42788");
// 
//         ws.OnMessage += OnMessage;
//         ws.OnError += OnError;
//         ws.OnClose += OnClose;        
// 
//         ws.ConnectAsync();

        //ws = new WebSocket4Net.WebSocket("ws://96.126.116.192:42788");
        ws = new WebSocket4Net.WebSocket("ws://192.168.1.4:42788/");

        ws.Closed += new System.EventHandler(OnClose);
        ws.MessageReceived += new System.EventHandler<WebSocket4Net.MessageReceivedEventArgs>(OnMessage);
        ws.Error += new System.EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(OnError);

        if (ws.State == WebSocket4Net.WebSocketState.None)
        {
            ws.Open();
        }        

        serverReplyActions.Add("login", (reply) => OnLoginReply(reply));
        Globals.self.MsgActions();

        if (!fromLoginScene)
        {
            Invoke("Login", 1.5f);
        }
    }    

	void Start () 
    {
        
	}

    void Login()
    {
        System.String name = "rosedog";
        Send("login" + Globals.self.separator + name + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier + Globals.self.separator + Globals.versionString);
        Globals.self.name = name;
    }    

    void OnLoginReply(System.String[] reply)
    {                
        if (reply[0] == "ok")
        {            
            Globals.self.SyncWithServer();
            System.IO.StreamWriter stream = new System.IO.StreamWriter(UnityEngine.Application.persistentDataPath + "/name.txt",
                false, System.Text.Encoding.UTF8);
            stream.WriteLine(Globals.self.name);
            stream.Close();            
        }
        else if (reply[0] == "duplicated")
        {
            Globals.canvasForLogin.LoginUIVisible(true);
            Globals.MessageBox("name_duplicated");
        }
        else if (reply[0] == "version_error")
        {            
            Globals.MessageBox("version_error", ()=>VersionErrorTips());
        }
        Globals.socket.CloseWaitingUI();
    }

    public void VersionErrorTips()
    {
        Globals.canvasForLogin.version.text = Globals.languageTable.GetText("version_error") + "\n" + Globals.canvasForLogin.version.text;
    }

    System.Collections.IEnumerator WaitingForSyncRead()
    {
        while (true)
        {
            yield return new UnityEngine.WaitForSeconds(0.1f);
            if (ready)
            {
                break;
            }
        }

        if (Globals.city && Globals.city.canvasForCity.activeSelf)
        {
            Globals.city.Start();
        }
        if (Globals.maze != null)
        {
            Globals.maze.Start();
        }
        Globals.canvasForMagician.Start();
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

    void Update()
    {
        if (threadTempActions.Count != 0)
        {
            threadTempActions[0].Invoke();
            threadTempActions.RemoveAt(0);
        }
    }
    
    public void Send(System.String msg)
    {        
        if (ws != null)
        {
//            msgCache.Add(msg);
//            if (msgCache.Count == 1)
            {
                UnityEngine.Debug.Log(msg);
                ws.Send(msg);
            }            
        }        
    }

    public void OpenWaitingUI()
    {
        WaitingForServer = UnityEngine.GameObject.Instantiate(WaitingForServer_prefab) as UnityEngine.GameObject;
    }

    public void CloseWaitingUI()
    {
        if (WaitingForServer != null)
        {
            DestroyObject(WaitingForServer);
        }        
    }

    void OnSendComplete(bool complete)
    {
        threadTempActions.Add(() => SendCompleteInvoke(complete));
    }

    void OnMessage(object sender, WebSocket4Net.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log(e.Message);
        threadTempActions.Add(() => MessageInvoke(e.Message));
    }

    void OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        UnityEngine.Debug.Log(e.Exception.Message);
        UnityEngine.Debug.Log(e.Exception.Source);
        UnityEngine.Debug.Log(e.Exception.Data);
        threadTempActions.Add(() => ErrorInvoke(e));
    }
    
    void OnClose(object sender, System.EventArgs e)
    {
        UnityEngine.Debug.Log(e);
        threadTempActions.Add(() => CloseInvoke(e));
    }    

    void SendCompleteInvoke(bool complete)
    {
        if (complete)
        {
            msgCache.RemoveAt(0);
            if (msgCache.Count != 0)
            {
                UnityEngine.Debug.Log(msgCache[0]);
                ws.Send(msgCache[0]);
            }            
        }
        else
        {
            Globals.MessageBox("Send Error", () => BackToLoginScene());
        }
    }

    void ErrorInvoke(System.EventArgs e)
    {
        CloseWaitingUI();
        Globals.MessageBox("cannot_connect_server",() => BackToLoginScene());
    }

    void CloseInvoke(System.EventArgs e)
    {
        CloseWaitingUI();
        Globals.MessageBox("cannot_connect_server", () => BackToLoginScene());
    }

    void MessageInvoke(System.String replyData)
    {        
        System.Collections.Generic.List<System.String> datas = 
            new System.Collections.Generic.List<System.String>(replyData.Split(Globals.self.separator.ToCharArray()));
        System.String protocol = datas[0];
        datas.RemoveAt(0);
        if (serverReplyActions.ContainsKey(protocol))
        {
            serverReplyActions[protocol].Invoke(datas.ToArray());
        }
    }

    void BackToLoginScene()
    {
        UnityEngine.Application.LoadLevel("Login");
    }
}
