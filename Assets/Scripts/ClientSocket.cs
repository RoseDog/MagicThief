public class ClientSocket : UnityEngine.MonoBehaviour 
{
    public WebSocketSharp.WebSocket ws;
    public System.Collections.Generic.List<UnityEngine.Events.UnityAction> threadTempActions = new System.Collections.Generic.List<UnityEngine.Events.UnityAction>();
    public System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>> serverReplyActions =
        new System.Collections.Generic.Dictionary<System.String, UnityEngine.Events.UnityAction<System.String[]>>();
    UnityEngine.GameObject WaitingForServer_prefab;
    UnityEngine.GameObject WaitingForServer;
    public bool IsReady = true;
    public bool FromLogin = false;
	// Use this for initialization
    public void Awake()
    {
        Globals.socket = this;
        WaitingForServer_prefab = UnityEngine.Resources.Load("UI/WaitingForServer") as UnityEngine.GameObject;        
        if(!FromLogin && UnityEngine.GameObject.FindObjectOfType<LevelEditor>()==null)
        {
            IsReady = false;
        }
    }

	void Start () 
    {
        ws = new WebSocketSharp.WebSocket("ws://127.0.0.1:42788");

        ws.OnMessage += OnMessage;
        ws.OnError += OnError;
        ws.OnClose += OnClose;

        ws.ConnectAsync();
        Globals.socket.serverReplyActions.Add("login", (reply) => OnLoginReply(reply));
        if(!FromLogin)
        {
            Invoke("Login", 0.5f);
        }        
	}

    void Login()
    {
        System.String name = "d";
        Send("login"+ Globals.self.separator + name + Globals.self.separator + UnityEngine.SystemInfo.deviceUniqueIdentifier);
        Globals.self.name = name;
        Invoke("DownloadReady", 2.0f);
    }

    void DownloadReady()
    {
        Globals.socket.IsReady = true;
    }

    void OnLoginReply(System.String[] reply)
    {
        if (reply[0] == "ok")
        {
            Globals.self.SyncWithServer();
            if (FromLogin)
            {
                Globals.asyncLoad._ToLoadingScene("Tutorial_Levels");
            }
            else
            {
                StartCoroutine("WaitingForSyncRead");
            }
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
            if (IsReady)
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
            ws.SendAsync(msg, OnSendComplete);        
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
        threadTempActions.Add(() => MessageInvoke(e.Data));
    }

    void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
    {
        UnityEngine.Debug.Log(e.Exception.Message);
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
        }
    }

    void ErrorInvoke(WebSocketSharp.ErrorEventArgs e)
    {
        CloseWaitingUI();
        Globals.MessageBox("cannot_connect_server");
    }

    void CloseInvoke(WebSocketSharp.CloseEventArgs e)
    {
        CloseWaitingUI();
        Globals.MessageBox("cannot_connect_server");
    }

    void MessageInvoke(System.String replyData)
    {
        UnityEngine.Debug.Log(replyData);
        System.Collections.Generic.List<System.String> datas = 
            new System.Collections.Generic.List<System.String>(replyData.Split(Globals.self.separator.ToCharArray()));
        System.String protocol = datas[0];
        datas.RemoveAt(0);
        if (serverReplyActions.ContainsKey(protocol))
        {
            serverReplyActions[protocol].Invoke(datas.ToArray());
        }
    }
}
