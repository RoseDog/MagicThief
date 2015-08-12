public class City : LevelController
{
    UnityEngine.GameObject firstTarget;
    UnityEngine.GameObject myMazeBuilding;

    Finger fingerDownOnMap;
    Building choosenBuilding;
    
    public System.Collections.Generic.List<Building> buildings = new System.Collections.Generic.List<Building>();
    UnityEngine.GameObject canvasForCity;
    public UIMover cityEventsOpenBtn;
    public UIMover rankOpenBtn;
    public CityEventsWindow eventsWindow;
    public RanksWindow ranksWindow;
    public UnityEngine.UI.Text whoIsYourTarget;
    UIMover go_add_box;
    public BeenStolenReport beenStolenReportUI;
    public override void Awake()
    {
        Globals.city = this;
        base.Awake();        
        canvasForCity = UnityEngine.GameObject.Find("CanvasForCity");
        mainCanvas = canvasForCity.GetComponent<UnityEngine.Canvas>();
        cityEventsOpenBtn = Globals.getChildGameObject<UIMover>(canvasForCity, "CityEventsOpenBtn");
        rankOpenBtn = Globals.getChildGameObject<UIMover>(canvasForCity, "RankOpenBtn");
        whoIsYourTarget = Globals.getChildGameObject<UnityEngine.UI.Text>(canvasForCity, "who_is_your_target");
        whoIsYourTarget.gameObject.SetActive(false);
        eventsWindow = Globals.getChildGameObject<CityEventsWindow>(canvasForCity, "CityEventsWindow");
        eventsWindow.city = this;
        
        ranksWindow = Globals.getChildGameObject<RanksWindow>(canvasForCity, "RanksWindow");
        ranksWindow.viewRankPlayer = Globals.getChildGameObject<ViewRankPlayer>(canvasForCity, "ViewRankPlayer");
        ranksWindow.viewRankPlayer.city = this;

        // 如果教程结束了，就显示自己的家，否则就显示第一个目标建筑
        firstTarget = UnityEngine.GameObject.Find("FirstTarget");
        firstTarget.GetComponent<Building>().city = this;
        myMazeBuilding = UnityEngine.GameObject.Find("MyMazeBuilding");
        myMazeBuilding.GetComponent<MyMazeBuilding>().city = this;

        if (Globals.buildingSprites == null)
        {
            Globals.buildingSprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/City Night/city-0");
        }

        go_add_box = Globals.getChildGameObject<UIMover>(canvasForCity, "go_add_box");
        go_add_box.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => GoAddBox());
        go_add_box.gameObject.SetActive(false);
        beenStolenReportUI.transform.parent.parent.gameObject.SetActive(false);
    }

    public void GoAddBox()
    {
        Exit();
        Globals.asyncLoad.ToLoadSceneAsync("MyMaze");
    }

    public UnityEngine.Sprite GetBuildingSprite(System.String sprite_name)
    {
        foreach (UnityEngine.Sprite sprite in Globals.buildingSprites)
        {
            if(sprite.name == sprite_name)
            {
                return sprite;
            }
        }
        return null;
    }

    public override void Start()
    {
        if (!Globals.socket.IsFromLogin() && !Globals.socket.IsReady())
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
        Globals.canvasForMagician.SetLifeVisible(true);
        Globals.canvasForMagician.SetPowerVisible(true);
        Globals.canvasForMagician.SetCashVisible(true);
        Globals.canvasForMagician.SetRoseVisible(true);
        Globals.canvasForMagician.ShowTricksPanel();
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        Globals.magician.ResetLifeAndPower(Globals.self);
                
        ranksWindow.viewRankPlayer.OnTouchUpOutside(null);
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {            
            firstTarget.SetActive(false);
            myMazeBuilding.SetActive(true);

            // 生成周围的目标
            for (int idx = 0; idx < Globals.self.buildingDatas.Count; ++idx)
            {
                UnityEngine.GameObject building = UnityEngine.GameObject.Find("building_" + Globals.self.buildingDatas[idx].posID);
                building.GetComponentInChildren<Building>().data = Globals.self.buildingDatas[idx];
                UpdateBuilding(building.GetComponentInChildren<Building>(), Globals.self.buildingDatas[idx]);
            }           

            
            
            // 录像
            AddReplaysToEventWindow(Globals.self.defReplays);
            AddReplaysToEventWindow(Globals.self.atkReplays);

            // 排行榜
            foreach(PlayerInfo playOnRank in Globals.playersOnRank)
            {
                ranksWindow.AddRecord(playOnRank);
            }
        }
        else
        {
            //whoIsYourTarget.gameObject.SetActive(false);
            firstTarget.SetActive(true);
            myMazeBuilding.SetActive(false);
        }

        // 云雾的状态
        for (int idx = 0; idx < Globals.self.cloudDatas.Count; ++idx)
        {
            UnityEngine.GameObject cloud = UnityEngine.GameObject.Find("clouds_" + idx.ToString());
            cloud.GetComponent<Cloud>().SyncData(Globals.self.cloudDatas[idx]);
        }           

        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(0);
            finger.Evt_Down += OnDragFingerDown;
            finger.Evt_Moving += OnDragFingerMoving;
            finger.Evt_Up += OnDragFingerUp;
        }
        Globals.UpdateUnclickedRedPointsText(eventsWindow.unclickedCount);
        
        MoneyFull(Globals.canvasForMagician.money_full.activeSelf);

        Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/city_bgm");
        Globals.cameraFollowMagician.audioSource.Play();
        Globals.cameraFollowMagician.audioSource.volume = 0.25f;

        if (Globals.self.beenStolenReports.Count != 0)
        {
            BeenStolen();
        }        
    }

    public void AddReplaysToEventWindow(System.Collections.Hashtable replays)
    {
        foreach (System.Collections.DictionaryEntry entry in replays)
        {
            ReplayData replay = entry.Value as ReplayData;

            CityEvent ce = null;
            if (replay.thief.name == Globals.self.name)
            {
                ce = eventsWindow.AddEvent(replay.everClicked);
                Globals.languageTable.SetText(ce.uiText, "you_stole_others_event", new System.String[] { replay.guard.name });
            }
            else
            {
                ce = eventsWindow.AddEvent(replay.everClicked);
                if (replay.StealingCash > 0)
                {
                    ce.uiText.color = UnityEngine.Color.red;
                    Globals.languageTable.SetText(ce.uiText, "stolen_by_others_event_guards_failed", new System.String[] { replay.thief.name, replay.thief.roseCount.ToString()});
                }
                else
                {
                    ce.uiText.color = UnityEngine.Color.green;
                    Globals.languageTable.SetText(ce.uiText, "stolen_by_others_event_guards_success", new System.String[] { replay.thief.name, replay.thief.roseCount.ToString() });
                }                
            }

            UnityEngine.UI.Button eventBtn = ce.GetComponent<UnityEngine.UI.Button>();
            eventBtn.onClick.AddListener(() => eventsWindow.ReplayEventBtnClicked(replay, ce));
        }
    }

    public void BeenStolen()
    {
        beenStolenReportUI.Open();
        Globals.self.beenStolenReports.Clear();        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(0);
            finger.Evt_Down -= OnDragFingerDown;
            finger.Evt_Moving -= OnDragFingerMoving;
            finger.Evt_Up -= OnDragFingerUp;
        }
        Globals.city = null;
    }

    public Building UpdateBuilding(Building old_building,BuildingData new_data)
    {
        System.String new_building_name = "";
        if (new_data.isPvP)
        {
            new_building_name = "City/pvp";
        }
        else
        {
            int idx = UnityEngine.Random.Range(0, 4);
            new_building_name = "City/pve_" + idx.ToString();
        }
        new_building_name += new_data.type;

        UnityEngine.GameObject buildingPrefab = UnityEngine.Resources.Load("Props/" + new_data.type + "Building") as UnityEngine.GameObject;
        UnityEngine.GameObject targetBuildingObject = UnityEngine.GameObject.Instantiate(buildingPrefab) as UnityEngine.GameObject;
        Building new_building = targetBuildingObject.GetComponentInChildren<Building>();
        new_building.gameObject.name = new_building_name;
        new_building.transform.parent.name = new_data.posID;
        new_building.transform.parent.parent = old_building.transform.parent.parent;
        new_building.transform.position = old_building.transform.position;
        new_building.data = new_data;
        new_building.city = this;
        buildings.Add(new_building);
        buildings.Remove(old_building);
        DestroyObject(old_building.transform.parent.gameObject);

        return new_building;
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
        // 这里是不是服务器有bug，会在timer结束之后多发一次rose grow？
        RoseBuilding rose_b = b as RoseBuilding;
        if (rose_b != null)
        {
            rose_b.RoseGrow();
        }        
    }

    public void RoseBuildingEnd(BuildingData data)
    {
        Building b = GetBuilding(data);
        UpdateBuilding(b, data);        
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

    public void Exit()
    {
        cityEventsOpenBtn.gameObject.SetActive(false);
        rankOpenBtn.gameObject.SetActive(false);
        eventsWindow.CloseBtnClcked();
        ranksWindow.CloseBtnClcked();
        go_add_box.gameObject.SetActive(false);
        //whoIsYourTarget.gameObject.SetActive(false);
    }

    public void DestroyRosePickTip(UnityEngine.GameObject RosePickedTip)
    {
        DestroyObject(RosePickedTip);
    }

    public override void MoneyFull(bool full)
    {
        base.MoneyFull(full);
        go_add_box.gameObject.SetActive(full);
        if(full)
        {
            go_add_box.ClearAllActions();
            go_add_box.Scale();
        }
    }
}
