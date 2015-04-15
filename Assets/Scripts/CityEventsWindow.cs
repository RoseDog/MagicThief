public class CityEventsWindow : Actor 
{
    public System.Collections.Generic.List<CityEvent> cityEvents = new System.Collections.Generic.List<CityEvent>();    
    UnityEngine.GameObject eventPrefab;
    public City city;
    public UnityEngine.UI.Text unclickedCount;
    UnityEngine.RectTransform ReplayDetail;
    MultiLanguageUIText cash_back_then;
    MultiLanguageUIText stealing_cash;
    UnityEngine.UI.Button replay_btn;
    public override void Awake()
    {
        eventPrefab = UnityEngine.Resources.Load("UI/CityEvent") as UnityEngine.GameObject;
        UnityEngine.Debug.Log(eventPrefab);
        unclickedCount = UnityEngine.GameObject.Find("UnclickedCount").GetComponent<UnityEngine.UI.Text>();
        unclickedCount.transform.parent.gameObject.SetActive(false);

        ReplayDetail = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "ReplayDetail");
        cash_back_then = Globals.getChildGameObject<MultiLanguageUIText>(ReplayDetail.gameObject, "cash_back_then");
        stealing_cash = Globals.getChildGameObject<MultiLanguageUIText>(ReplayDetail.gameObject, "stealing_cash");
        replay_btn = Globals.getChildGameObject<UnityEngine.UI.Button>(ReplayDetail.gameObject, "replay");        
        ReplayDetail.localScale = UnityEngine.Vector3.zero;
        base.Awake();
    }    

    public void ReplayEventBtnClicked(ReplayData replay, CityEvent ce)
    {
        Globals.languageTable.SetText(cash_back_then,"cash_back_then",
            new System.String[] { replay.cashAmount.ToString("F0") });
        Globals.languageTable.SetText(stealing_cash, "stealing_cash",
            new System.String[] { replay.StealingCash.ToString("F0") });        
        ReplayDetail.parent = ce.gameObject.transform;
        ReplayDetail.localScale = UnityEngine.Vector3.one;
        ReplayDetail.anchoredPosition = new UnityEngine.Vector2(
            ce.GetComponent<UnityEngine.RectTransform>().rect.width,0);
        replay_btn.onClick.RemoveAllListeners();
        replay_btn.onClick.AddListener(() => ReplayClicked(replay));
        ce.newText.enabled = false;
        Globals.self.ReplayClicked(replay);
        Globals.UpdateUnclickedRedPointsText(unclickedCount);
    }

    public void ReplayClicked(ReplayData replay)
    {
        Globals.replay_key = replay.date.ToString();
        CloseBtnClcked();        
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
   
    public CityEvent AddEvent(Building building)
    {
        CityEvent ce = AddEvent(building.data.targetName, building.data.everClickedTarget);
        UnityEngine.UI.Button eventBtn = ce.GetComponent<UnityEngine.UI.Button>();
        eventBtn.onClick.AddListener(() => EventBtnClicked(eventBtn));
        return ce;
    }

    public CityEvent AddEvent(System.String text, bool everClicked)
    {
        CityEvent ce = (Instantiate(eventPrefab) as UnityEngine.GameObject).GetComponent<CityEvent>();
        UnityEngine.RectTransform ceTransform = ce.GetComponent<UnityEngine.RectTransform>();
        ceTransform.SetParent(transform);
        ceTransform.localScale = new UnityEngine.Vector3(1, 1, 1);
        Globals.languageTable.SetText(ce.uiText, text);
        ce.name = text;
        ce.newText.enabled = !everClicked;
        cityEvents.Add(ce);        

        float event_y_pos = 136;
        float padding = 3;
        for (int idx = cityEvents.Count - 1; idx >= 0; --idx)
        {
            cityEvents[idx].rectTransform.localPosition = new UnityEngine.Vector3(0.0f, event_y_pos, 0.0f);
            event_y_pos -= cityEvents[idx].rectTransform.rect.height;
            event_y_pos -= padding;
        }

        Globals.UpdateUnclickedRedPointsText(unclickedCount);

        return ce;
    }

    public void EventBtnClicked(UnityEngine.UI.Button btn)
    {
        CityEvent ce = btn.GetComponentInParent<CityEvent>();
        System.String clickedBuilding = ce.name;

        // 相机移动
        Globals.cameraFollowMagician.MoveToPoint(city.GetTargetPosition(ce.name), Globals.cameraMoveDuration);
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
        GetComponent<UIMover>().BeginMove(Globals.uiMoveAndScaleDuration);
    }

    public void CloseBtnClcked()
    {
        GetComponent<UIMover>().Goback(Globals.uiMoveAndScaleDuration);
        ReplayDetail.localScale = UnityEngine.Vector3.zero;
    }
}
