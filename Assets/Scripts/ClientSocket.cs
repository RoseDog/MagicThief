public class ClientSocket : UnityEngine.MonoBehaviour 
{
    public WebSocketSharp.WebSocket ws;
    public System.Collections.Generic.List<UnityEngine.Events.UnityAction> threadTempActions = new System.Collections.Generic.List<UnityEngine.Events.UnityAction>();
    public System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>> serverReplyActions =
        new System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>>();
    public System.Collections.Generic.List<System.String> msgCache = new System.Collections.Generic.List<System.String>();

    UnityEngine.GameObject WaitingForServer_prefab;
    UnityEngine.GameObject WaitingForServer;
    bool ready = true;
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

        ws = new WebSocketSharp.WebSocket("ws://96.126.116.192:42788");
        //ws = new WebSocketSharp.WebSocket("ws://127.0.0.1:42788");

        ws.OnMessage += OnMessage;
        ws.OnError += OnError;
        ws.OnClose += OnClose;        

        ws.ConnectAsync();

        serverReplyActions.Add("login", (reply) => OnLoginReply(reply));
        if (!fromLoginScene)
        {
            Invoke("Login", 0.5f);
        }
    }    

	void Start () 
    {
        
	}

    void Login()
    {
        System.String name = "玫瑰狗";
        Send("login"+ Globals.self.separator + name + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier);
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
            Globals.canvasForLogin.DiveInBtn.gameObject.SetActive(true);
            Globals.tipDisplay.Msg("name_duplicated", 0.1f, Globals.canvasForLogin.transform);
        }
        Globals.socket.CloseWaitingUI();
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
        
        City city = Globals.LevelController as City;
        if (city)
        {
            city.Start();
        }
        if (Globals.maze != null)
        {
            Globals.maze.Start();
        }
        Globals.canvasForMagician.Start();
    }

    void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }

    // Update is called once per frame
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
            msgCache.Add(msg);
            if (msgCache.Count == 1)
            {
                ws.SendAsync(msg, OnSendComplete);
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

    void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
    {
        UnityEngine.Debug.Log(e.Data);
        threadTempActions.Add(() => MessageInvoke(e.Data));
    }

    void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
    {
        UnityEngine.Debug.Log(e.Message);
        threadTempActions.Add(() => ErrorInvoke(e));
    }

    void OnClose(object sender, WebSocketSharp.CloseEventArgs e)
    {
        UnityEngine.Debug.Log(e.Reason);
        threadTempActions.Add(() => CloseInvoke(e));
    }    

    void SendCompleteInvoke(bool complete)
    {
        if (complete)
        {
            msgCache.RemoveAt(0);
            if (msgCache.Count != 0)
            {
                ws.SendAsync(msgCache[0], OnSendComplete);
            }            
        }
        else
        {
            Globals.MessageBox("Send Error", () => BackToLoginScene());
        }
    }

    void ErrorInvoke(WebSocketSharp.ErrorEventArgs e)
    {
        CloseWaitingUI();
        Globals.MessageBox("cannot_connect_server",() => BackToLoginScene());
    }

    void CloseInvoke(WebSocketSharp.CloseEventArgs e)
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
