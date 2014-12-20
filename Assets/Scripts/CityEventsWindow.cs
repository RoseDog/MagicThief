public class CityEventsWindow : Actor 
{
    public System.Collections.Generic.List<CityEvent> cityEvents = new System.Collections.Generic.List<CityEvent>();    
    UnityEngine.GameObject eventPrefab;
    public City city;
    public UnityEngine.UI.Text unclickedTargetCount;
    public override void Awake()
    {
        eventPrefab = UnityEngine.Resources.Load("UI/CityEvent") as UnityEngine.GameObject;
        UnityEngine.Debug.Log(eventPrefab);
        unclickedTargetCount = UnityEngine.GameObject.Find("UnclickedTargetCount").GetComponent<UnityEngine.UI.Text>();
        Globals.UpdateUnclickedRedPointsText(unclickedTargetCount);
        base.Awake();
    }

    public void AddEvents(System.Collections.Generic.List<System.String> events, bool bNew)
    {
        foreach (System.String eventText in events)
        {
            AddEvent(eventText, bNew);
        }
        
        float event_y_pos = 206;
        float padding = 3;
        for (int idx = cityEvents.Count - 1; idx >= 0; --idx)
        {
            cityEvents[idx].rectTransform.localPosition = new UnityEngine.Vector3(0.0f, event_y_pos, 0.0f);
            event_y_pos -= cityEvents[idx].rectTransform.rect.height;
            event_y_pos -= padding;
        }
    }

    public CityEvent AddEvent(System.String eventText, bool bNew)
    {
        CityEvent ce = (Instantiate(eventPrefab) as UnityEngine.GameObject).GetComponent<CityEvent>();
        UnityEngine.RectTransform ceTransform = ce.GetComponent<UnityEngine.RectTransform>();
        ceTransform.SetParent(transform);
        ceTransform.localScale = new UnityEngine.Vector3(1,1,1);
        ce.uiText.text = eventText;
        ce.name = eventText;
        ce.newText.enabled = bNew;
        cityEvents.Add(ce);
        UnityEngine.UI.Button eventBtn = ce.GetComponent<UnityEngine.UI.Button>();
        eventBtn.onClick.AddListener(() => EventBtnClicked(eventBtn));
        return ce;
    }

    public void EventBtnClicked(UnityEngine.UI.Button btn)
    {
        CityEvent ce = btn.GetComponentInParent<CityEvent>();
        System.String clickedTarget = ce.uiText.text;

        EventClicked(clickedTarget);
        // 相机移动
        Globals.cameraFollowMagician.MoveToPoint(
            city.GetTargetPosition(ce.uiText.text) + Globals.cameraFollowMagician.GetHorForward() * 15 + 
            -Globals.cameraFollowMagician.GetHorRight() * 10, 
            Globals.cameraFollowMagician.disOffset, 0.7f);
        // 选中建筑
        city.ChooseBuilding(city.GetTargetBuilding(ce.uiText.text));
        // 更新列表
        if (Globals.unclickedTargets.Remove(clickedTarget))
        {
            Globals.targets.Add(clickedTarget);
        }
                
        // 红字消失        
        ce.newText.enabled = false;
        // 红点提示未查看的目标个数
        Globals.UpdateUnclickedRedPointsText(unclickedTargetCount);
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
    }
}
