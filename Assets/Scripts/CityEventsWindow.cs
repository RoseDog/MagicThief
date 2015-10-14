public class CityEventsWindow : CustomEventTrigger 
{
    public System.Collections.Generic.List<CityEvent> cityEvents = new System.Collections.Generic.List<CityEvent>();    
    UnityEngine.GameObject eventPrefab;
    public City city;
    public UnityEngine.UI.Text unclickedCount;
    UnityEngine.RectTransform ReplayDetail;
    MultiLanguageUIText time_stamp;
    public MultiLanguageUIText stealing_cash_info;

    UnityEngine.UI.Button replay_btn;
    UnityEngine.UI.Button defense_reward_btn;
    MultiLanguageUIText defense_reward_number;
    public UnityEngine.GameObject highLightFrame;
    public UnityEngine.UI.GridLayoutGroup layout;
    public override void Awake()
    {
        eventPrefab = UnityEngine.Resources.Load("UI/CityEvent") as UnityEngine.GameObject;
        UnityEngine.Debug.Log(eventPrefab);
        unclickedCount = UnityEngine.GameObject.Find("UnclickedCount").GetComponent<UnityEngine.UI.Text>();
        unclickedCount.transform.parent.gameObject.SetActive(false);

        ReplayDetail = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "ReplayDetail");
        time_stamp = Globals.getChildGameObject<MultiLanguageUIText>(ReplayDetail.gameObject, "time_stamp");
          
        replay_btn = Globals.getChildGameObject<UnityEngine.UI.Button>(ReplayDetail.gameObject, "replay");
        defense_reward_btn = Globals.getChildGameObject<UnityEngine.UI.Button>(ReplayDetail.gameObject, "defense_reward");
        defense_reward_number = Globals.getChildGameObject<MultiLanguageUIText>(defense_reward_btn.gameObject, "Text");
        ReplayDetail.localScale = UnityEngine.Vector3.zero;


        highLightFrame = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "highLightFrame").gameObject;
        highLightFrame.SetActive(false);

        base.Awake();
        gameObject.SetActive(false);
    }    

    public void ReplayEventBtnClicked(ReplayData replay, CityEvent ce)
    {
        System.TimeSpan date_diff = System.DateTime.Now - replay.date;

        if (date_diff.Days != 0)
        {
            Globals.languageTable.SetText(time_stamp,"few_days_ago",
            new System.String[] { date_diff.Days.ToString() });
        }
        else if (date_diff.Hours != 0)
        {
            Globals.languageTable.SetText(time_stamp, "few_hours_ago",
            new System.String[] { date_diff.Hours.ToString() });
        }
        else
        {
            Globals.languageTable.SetText(time_stamp, "few_minutes_ago");
        }

        Globals.languageTable.SetText(stealing_cash_info, "stealing_cash_info",
            new System.String[] { replay.guard.cashAmount.ToString("F0"), replay.guard.GetCashAmountOnMazeFloor().ToString("F0"),
            replay.StealingCashInSafebox.ToString("F0"), replay.PickedCash.ToString("F0")});        

        defense_reward_btn.onClick.RemoveAllListeners();

        if (replay.StealingCashInSafebox < 1 && replay.guard.name == Globals.self.name)
        {
            defense_reward_btn.gameObject.SetActive(true);
            if (replay.rewardAccepted)
            {
                Globals.languageTable.SetText(defense_reward_number, "reward_accepted");
            }
            else
            {
                defense_reward_number.text = "x" + replay.reward_rose_count.ToString();
                defense_reward_btn.onClick.AddListener(() => Reward(defense_reward_btn, defense_reward_number, replay));
            }            
        }
        else
        {
            defense_reward_btn.gameObject.SetActive(false);
        }
        
        
        ReplayDetail.localScale = UnityEngine.Vector3.one;        
        replay_btn.onClick.RemoveAllListeners();
        replay_btn.onClick.AddListener(() => ReplayClicked(replay));
        
        ce.newText.enabled = false;
        Globals.self.ReplayClicked(replay);
        Globals.UpdateUnclickedRedPointsText(unclickedCount);

        ShowHightlight(ce.GetComponent<UnityEngine.UI.Button>());
    }

    public void Reward(UnityEngine.UI.Button btn, MultiLanguageUIText btn_text, ReplayData replay)
    {
        btn.onClick.RemoveAllListeners();
        Globals.self.ChangeRose(replay.reward_rose_count);
        Globals.canvasForMagician.RoseNumber.Add(replay.reward_rose_count);
        Globals.canvasForMagician.RoseNumber.audioSource.Play();        

        Globals.languageTable.SetText(defense_reward_number, "reward_accepted");
        Globals.self.RewardAccepted(replay);
    }

    public void ReplayClicked(ReplayData replay)
    {
        Globals.playingReplay = replay;
        Globals.thiefPlayer = replay.thief;
        Globals.guardPlayer = replay.guard;
        city.Exit();
        Globals.asyncLoad.ToLoadSceneAsync("StealingLevel");
    }
   
    public CityEvent AddEvent(Building building)
    {
        CityEvent ce = AddEvent(building.data.everClickedTarget);
        Globals.languageTable.SetText(ce.uiText, "new_target_event",new System.String[] { building.data.targetName });
        ce.name = building.data.targetName;
        UnityEngine.UI.Button eventBtn = ce.GetComponent<UnityEngine.UI.Button>();
        eventBtn.onClick.AddListener(() => EventBtnClicked(eventBtn));
        return ce;
    }

    public CityEvent AddEvent(bool everClicked)
    {
        CityEvent ce = (Instantiate(eventPrefab) as UnityEngine.GameObject).GetComponent<CityEvent>();
        UnityEngine.RectTransform ceTransform = ce.GetComponent<UnityEngine.RectTransform>();
        ceTransform.SetParent(layout.transform);
        ceTransform.localScale = new UnityEngine.Vector3(1, 1, 1);        
        ce.newText.enabled = !everClicked;
        cityEvents.Add(ce);

        Globals.UpdateUnclickedRedPointsText(unclickedCount);
        
        return ce;
    }

    public void EventBtnClicked(UnityEngine.UI.Button btn)
    {
        CityEvent ce = btn.GetComponentInParent<CityEvent>();
        System.String clickedBuilding = ce.name;

        // 相机移动
        Globals.cameraFollowMagician.MoveToPoint(city.GetTargetPosition(ce.name)+new UnityEngine.Vector3(-3.0f,2.5f,0), Globals.cameraMoveDuration);
        // 选中建筑
        Building building = city.GetTargetBuilding(ce.name);
        city.ChooseBuilding(building);
        // 更新列表
        Globals.self.BuildingClicked(building.data);
        
        // 红字消失        
        ce.newText.enabled = false;
        // 红点提示未查看的目标个数
        Globals.UpdateUnclickedRedPointsText(unclickedCount);

        ReplayDetail.localScale = UnityEngine.Vector3.zero;

        ShowHightlight(btn);
    }

    void ShowHightlight(UnityEngine.UI.Button btn)
    {
        highLightFrame.SetActive(true);
        highLightFrame.transform.parent = btn.transform;
        highLightFrame.transform.localScale = UnityEngine.Vector3.one;
        highLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
        highLightFrame.transform.SetAsFirstSibling();
    }

    public CityEvent EventClicked(System.String clickedTarget)
    {
        foreach(CityEvent ce in cityEvents)
        {
            if(ce.name == clickedTarget)
            {
                // 这个函数暂时没有用                
                ce.Clicked();
                return ce;
            }
        }
        Globals.Assert(false);
        return null;
    }

    public void OpenBtnClicked()
    {
        gameObject.SetActive(true);
        highLightFrame.SetActive(false);
        city.ranksWindow.CloseBtnClcked();
        city.ChooseBuilding(null);

        // 录像                
        TargetBuilding[] targets = FindObjectsOfType<TargetBuilding>();
        foreach(TargetBuilding building in targets)
        {
            AddEvent(building);
            if(cityEvents.Count == 10)
            {
                break;
            }
        }
        foreach (ReplayData replay in Globals.self.defReplays)
        {
            city.AddOneReplayToEventWindow(replay);
            if (cityEvents.Count == 10)
            {
                break;
            }
        }
        foreach (ReplayData replay in Globals.self.atkReplays)
        {
            city.AddOneReplayToEventWindow(replay);
            if (cityEvents.Count == 10)
            {
                break;
            }
        }
        Globals.canvasForMagician.gameObject.SetActive(false);
    }

    public void CloseBtnClcked()
    {
        highLightFrame.transform.parent = transform;
        ReplayDetail.localScale = UnityEngine.Vector3.zero;
        gameObject.SetActive(false);
        foreach (CityEvent ce in cityEvents)
        {
            DestroyObject(ce.gameObject);
        }
        cityEvents.Clear();
        Globals.canvasForMagician.gameObject.SetActive(true);
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
    }
}
