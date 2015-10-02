public class ViewRankPlayer : CustomEventTrigger
{
    PlayerInfo player;
    UnityEngine.UI.Text PlayerName;
    UnityEngine.UI.Button VisitBtn;
    public System.Collections.Generic.List<CityEvent> stealingRecords = new System.Collections.Generic.List<CityEvent>();    
    UnityEngine.GameObject recordPrefab;

    UnityEngine.RectTransform ReplayDetail;
    public MultiLanguageUIText stealing_cash_info;

    UnityEngine.UI.Button replay_btn;
    public City city;
    public UnityEngine.GameObject highLightFrame;
    public override void Awake()
    {
        base.Awake();
        PlayerName = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "PlayerName");
        VisitBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "Visit");
        VisitBtn.onClick.AddListener(()=>VisitMaze());

        recordPrefab = UnityEngine.Resources.Load("UI/OtherPlayerStealRecord") as UnityEngine.GameObject;

        ReplayDetail = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "ReplayDetail");
        replay_btn = Globals.getChildGameObject<UnityEngine.UI.Button>(ReplayDetail.gameObject, "replay");
        ReplayDetail.localScale = UnityEngine.Vector3.zero;

        highLightFrame = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "highLightFrame").gameObject;
        highLightFrame.SetActive(false);
    }	

    public void Open(PlayerInfo playerOnRank)
    {
        highLightFrame.transform.parent = transform;
        highLightFrame.SetActive(false);
        ReplayDetail.transform.parent = transform;
        foreach (CityEvent record in stealingRecords)
        {
            DestroyObject(record.gameObject);
        }
        stealingRecords.Clear();

        gameObject.SetActive(true);
        player = playerOnRank;
        PlayerName.text = playerOnRank.name;

        foreach (System.Collections.DictionaryEntry entry in playerOnRank.atkReplays)
        {
            ReplayData replay = entry.Value as ReplayData;
            AddStealingRecord(replay);
        }        
    }

    public CityEvent AddStealingRecord(ReplayData replay)
    {
        CityEvent record = (Instantiate(recordPrefab) as UnityEngine.GameObject).GetComponent<CityEvent>();
        UnityEngine.RectTransform ceTransform = record.GetComponent<UnityEngine.RectTransform>();
        ceTransform.SetParent(transform);
        ceTransform.localScale = new UnityEngine.Vector3(1, 1, 1);
        replay.guard.name = new System.String('?', replay.guard.name.Length);
        Globals.languageTable.SetText(record.uiText, "stealing_event", new System.String[] { replay.thief.name, replay.guard.name });        
        record.newText.enabled = false;
        stealingRecords.Add(record);

        UnityEngine.UI.Button eventBtn = record.GetComponent<UnityEngine.UI.Button>();
        eventBtn.onClick.AddListener(() => ReplayEventBtnClicked(replay, record, eventBtn));

        float event_y_pos = 27;
        float padding = 3;
        for (int idx = stealingRecords.Count - 1; idx >= 0; --idx)
        {
            stealingRecords[idx].rectTransform.localPosition = new UnityEngine.Vector3(79,event_y_pos, 0.0f);
            event_y_pos -= stealingRecords[idx].rectTransform.rect.height;
            event_y_pos -= padding;
        }

        return record;
    }
    public void ReplayEventBtnClicked(ReplayData replay, CityEvent ce, UnityEngine.UI.Button eventBtn)
    {
        Globals.languageTable.SetText(stealing_cash_info, "stealing_cash_info",
            new System.String[] { replay.guard.cashAmount.ToString("F0"), replay.guard.GetCashAmountOnMazeFloor().ToString("F0"),
            replay.StealingCashInSafebox.ToString("F0"), replay.PickedCash.ToString("F0")});           
        
        ReplayDetail.localScale = UnityEngine.Vector3.one;        
        replay_btn.onClick.RemoveAllListeners();
        replay_btn.onClick.AddListener(() => ReplayClicked(replay));


        highLightFrame.SetActive(true);
        highLightFrame.transform.parent = eventBtn.transform;
        highLightFrame.transform.localScale = UnityEngine.Vector3.one;
        highLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
        highLightFrame.transform.SetAsFirstSibling();
    }

    public void ReplayClicked(ReplayData replay)
    {
        Globals.playingReplay = replay;
        Globals.thiefPlayer = replay.thief;
        Globals.guardPlayer = replay.guard;
        city.Exit();
        Globals.asyncLoad.ToLoadSceneAsync("StealingLevel");
    }
    
    public void VisitMaze()
    {
        city.Exit();
        Globals.visitPlayer = player;
        Globals.self.DownloadOtherPlayer(Globals.visitPlayer, null);
        Globals.asyncLoad.ToLoadSceneAsync("VisitOtherPlayer");        
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
    }
}
