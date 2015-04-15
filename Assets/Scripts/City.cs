public class City : LevelController
{
    UnityEngine.GameObject firstTarget;
    UnityEngine.GameObject myMazeBuilding;

    Finger fingerDownOnMap;
    Building choosenBuilding;
    
    public System.Collections.Generic.List<Building> buildings = new System.Collections.Generic.List<Building>();
    UnityEngine.GameObject canvasForCity;
    public UIMover cityEventsOpenBtn;
    public CityEventsWindow eventsWindow;    
    public override void Awake()
    {
        base.Awake();                   
        canvasForCity = UnityEngine.GameObject.Find("CanvasForCity");        
        cityEventsOpenBtn = Globals.getChildGameObject<UIMover>(canvasForCity, "CityEventsOpenBtn");        
        eventsWindow = Globals.getChildGameObject<CityEventsWindow>(canvasForCity, "CityEventsWindow");
        eventsWindow.city = this;

        // 如果教程结束了，就显示自己的家，否则就显示第一个目标建筑
        firstTarget = UnityEngine.GameObject.Find("FirstTarget");
        firstTarget.GetComponent<Building>().city = this;
        myMazeBuilding = UnityEngine.GameObject.Find("MyMazeBuilding");
        myMazeBuilding.GetComponent<MyMazeBuilding>().city = this;        
    }    

    public override void Start()
    {
        if (!Globals.socket.FromLogin && !Globals.socket.IsReady)
        {
            return;
        }
        base.Start();
        // 魔术师
        if (Globals.canvasForMagician == null)
        {
            UnityEngine.GameObject canvas_prefab = UnityEngine.Resources.Load("CanvasForMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(canvas_prefab);
        }
        Globals.EnableAllInput(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(true);
        Globals.canvasForMagician.SetLifeVisible(false);
        Globals.canvasForMagician.HideTricksPanel();
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {            
            firstTarget.SetActive(false);
            myMazeBuilding.SetActive(true);

            // 生成周围的目标
            for (int idx = 0; idx < Globals.self.buildingDatas.Count; ++idx)
            {
                UnityEngine.GameObject building = UnityEngine.GameObject.Find(Globals.self.buildingDatas[idx].posID);
                building.GetComponent<Building>().data = Globals.self.buildingDatas[idx];
                UpdateBuilding(building.GetComponent<Building>());
            }           

            // 显示出城市事件列表的按钮
            cityEventsOpenBtn.BeginMove(Globals.uiMoveAndScaleDuration);

            // 录像
            foreach (System.Collections.DictionaryEntry entry in Globals.self.replays)
            {
                ReplayData replay = entry.Value as ReplayData;

                CityEvent ce = null;
                if (replay.thief == Globals.self.name)
                {
                    ce = eventsWindow.AddEvent(Globals.languageTable.GetText("you_stole_others_event",
                    new System.String[] { replay.guard }), replay.everClicked);
                }
                else
                {
                    ce = eventsWindow.AddEvent(Globals.languageTable.GetText("stolen_by_others_event",
                    new System.String[] { replay.thief }), replay.everClicked);
                }                                

                UnityEngine.UI.Button eventBtn = ce.GetComponent<UnityEngine.UI.Button>();
                eventBtn.onClick.AddListener(() => eventsWindow.ReplayEventBtnClicked(replay, ce));
            }
        }
        else
        {
            firstTarget.SetActive(true);
            myMazeBuilding.SetActive(false);
        }

        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(0);
            finger.Evt_Down += OnDragFingerDown;
            finger.Evt_Moving += OnDragFingerMoving;
            finger.Evt_Up += OnDragFingerUp;
        }
        Globals.UpdateUnclickedRedPointsText(eventsWindow.unclickedCount);
    }

    public void OnDestroy()
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(0);
            finger.Evt_Down -= OnDragFingerDown;
            finger.Evt_Moving -= OnDragFingerMoving;
            finger.Evt_Up -= OnDragFingerUp;
        }
    }

    public Building UpdateBuilding(Building building)
    {
        UnityEngine.GameObject buildingPrefab = UnityEngine.Resources.Load("Props/" + building.data.type + "Building") as UnityEngine.GameObject;
        UnityEngine.GameObject targetBuildingObject = UnityEngine.GameObject.Instantiate(buildingPrefab) as UnityEngine.GameObject;
        Building newbuilding = targetBuildingObject.GetComponent<Building>();
        newbuilding.transform.position = building.transform.position;
        newbuilding.gameObject.name = building.data.posID;
        newbuilding.data = building.data;
        newbuilding.city = this;
        buildings.Add(newbuilding);
        buildings.Remove(building);
        DestroyObject(building.gameObject);
        return newbuilding;
    }

    public Building GetBuilding(BuildingData buildingdata)
    {
        foreach(Building b in buildings)
        {
            if(b.data == buildingdata)
            {
                return b;
            }
        }
        Globals.Assert(false, "no such building data");
        return null;
    }

    public void RoseGrow(BuildingData data)
    {
        Building b = GetBuilding(data);
        (b as RoseBuilding).RoseGrow();
    }

    public void RoseBuildingEnd(BuildingData data)
    {
        Building b = GetBuilding(data);
        UpdateBuilding(b);        
    }    
        
    public void TargetClicked(System.String clickedTarget)
    {        
        eventsWindow.EventClicked(clickedTarget);
    }

    public UnityEngine.Vector3 GetTargetPosition(System.String targetName)
    {
        foreach (Building building in buildings)
        {
            if(building.data.targetName == targetName)
            {
                return building.transform.position;
            }
        }
        Globals.Assert(false);
        return UnityEngine.Vector3.zero;
    }

    public Building GetTargetBuilding(System.String targetName)
    {
        foreach (Building building in buildings)
        {
            if (building.data.targetName == targetName)
            {
                return building.GetComponent<Building>();
            }
        }
        Globals.Assert(false);
        return null;
    }

    public bool OnDragFingerDown(object sender)
    {
        fingerDownOnMap = sender as Finger;
 
        return true;
    }


    public bool OnDragFingerMoving(object sender)
    {
        Globals.cameraFollowMagician.DragToMove(fingerDownOnMap);

        return true;
    }

    public bool OnDragFingerUp(object sender)
    {
        if (fingerDownOnMap.timeSinceTouchBegin < 0.5f &&
            UnityEngine.Vector2.Distance(fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition) < 10.0f)
        {
            int mask = 1 << 16;
            Building building = Globals.FingerRayToObj<Building>(
                Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, fingerDownOnMap.nowPosition);
            UnityEngine.Debug.Log(building);
            ChooseBuilding(building);
        }

        return true;
    }

    public void ChooseBuilding(Building building)
    {
        if (choosenBuilding != null && building != choosenBuilding)
        {
            choosenBuilding.Unchoose();
            choosenBuilding = null;
        }

        if (building != null)
        {
            // 如果在教程阶段，只能选择第一个目标
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over && building.gameObject != firstTarget)
            {
                return ;
            }

            building.Choosen();
            choosenBuilding = building;
        }
    }
}
