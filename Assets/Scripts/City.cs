public class City : LevelController
{
    UnityEngine.GameObject firstTarget;
    UnityEngine.GameObject myMazeBuilding;

    Finger fingerDownOnMap;
    Building choosenBuilding;

    System.Collections.Generic.List<UnityEngine.GameObject> buildingPosHolders = new System.Collections.Generic.List<UnityEngine.GameObject>();
    System.Collections.Generic.List<TargetBuilding> targetBuildings = new System.Collections.Generic.List<TargetBuilding>();
    System.Collections.Generic.List<PoorBuilding> poorBuildings = new System.Collections.Generic.List<PoorBuilding>();
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

        // 可以放置目标的点
        buildingPosHolders.AddRange(UnityEngine.GameObject.FindGameObjectsWithTag("BuildingPosition"));
        foreach (UnityEngine.GameObject pos in buildingPosHolders)
        {
            pos.renderer.enabled = false;
        }
    }

    void Start()
    {
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
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {            
            firstTarget.SetActive(false);
            myMazeBuilding.SetActive(true);

            if (Globals.unclickedBuildingAchives.Count == 0 && Globals.buildingAchives.Count == 0)
            {
                System.Collections.Generic.List<IniFile> newAchives = new System.Collections.Generic.List<IniFile>() { 
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition2\n" + Globals.TargetBuildingDescriptionKey + "=poker_face"),
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition1\n" + Globals.TargetBuildingDescriptionKey + "=cat_eye_lady"),
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition3\n" + Globals.TargetBuildingDescriptionKey + "=cash_eye")};
                Globals.AddNewTargetBuildingAchives(newAchives);

//                 IniFile poorAchive = new IniFile();
//                 poorAchive.set(Globals.PosHolderKey, "BuildingPosition4");
//                 Globals.AddPoorBuildingAchives(poorAchive);
// 
//                 IniFile roseAchive = new IniFile();
//                 roseAchive.set(Globals.PosHolderKey, "BuildingPosition3");
//                 Globals.AddRoseBuilding(roseAchive);
            }

            // 生成周围的目标
            Globals.UpdateUnclickedRedPointsText(eventsWindow.unclickedCount);
            BornTargetsBuilding(Globals.unclickedBuildingAchives);
            BornTargetsBuilding(Globals.buildingAchives);
            BornRoseBuilding(Globals.roseBuildingAchives);
            BornPoorsBuilding(Globals.poorsBuildingAchives);
            eventsWindow.AddEvents(Globals.unclickedBuildingAchives, true);
            eventsWindow.AddEvents(Globals.buildingAchives, false);

            // 显示出城市事件列表的按钮
            cityEventsOpenBtn.BeginMove(Globals.uiMoveAndScaleDuration);
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

    public void BornRoseBuilding(System.Collections.Generic.List<IniFile> poorBuildingsAchives)
    {
        // 创建新的building, 删除TargetPos
        for (int idx = 0; idx < poorBuildingsAchives.Count; ++idx)
        {
            BornBuilding(poorBuildingsAchives[idx], "Props/RoseBuilding");
        }
    }

    public void BornPoorsBuilding(System.Collections.Generic.List<IniFile> poorBuildingsAchives)
    {
        // 创建新的building, 删除TargetPos
        for (int idx = 0; idx < poorBuildingsAchives.Count; ++idx)
        {
           PoorBuilding poorBuilding = BornBuilding(poorBuildingsAchives[idx], "Props/PoorBuilding") as PoorBuilding;
           poorBuildings.Add(poorBuilding);
        }
    }
    
    public void BornTargetsBuilding(System.Collections.Generic.List<IniFile> targetAchives)
    {
        // 创建新的building, 删除TargetPos
        for(int idx = 0; idx < targetAchives.Count; ++idx)
        {
            TargetBuilding building = BornBuilding(targetAchives[idx], "Props/TargetBuilding") as TargetBuilding;            
            System.String tipTextKey = targetAchives[idx].get(Globals.TargetBuildingDescriptionKey);
            Globals.languageTable.SetText(building.tip, tipTextKey);
            building.gameObject.name = tipTextKey;            
            targetBuildings.Add(building);
        }        
    }

    public Building BornBuilding(IniFile achive, System.String prefabFile)
    {
        UnityEngine.GameObject posHolder = GetPosHolder(achive.get(Globals.PosHolderKey));
        UnityEngine.GameObject buildingPrefab = UnityEngine.Resources.Load(prefabFile) as UnityEngine.GameObject;
        UnityEngine.GameObject targetBuildingObject = UnityEngine.GameObject.Instantiate(buildingPrefab) as UnityEngine.GameObject;
        Building building = targetBuildingObject.GetComponent<Building>();
        building.transform.position = posHolder.transform.position;
        building.buildingAchive = achive;
        building.city = this;
        return building;
    }

    public void DestroyPoorBuilding(IniFile achive)
    {
        foreach(PoorBuilding building in poorBuildings)
        {
            if (building.buildingAchive == achive)
            {
                poorBuildings.Remove(building);
                DestroyObject(building.gameObject);
                return;
            }
        }
    }

    public void TargetClicked(System.String clickedTarget)
    {        
        eventsWindow.EventClicked(clickedTarget);
    }

    UnityEngine.GameObject GetPosHolder(System.String idString)
    {
        foreach(UnityEngine.GameObject pos in buildingPosHolders)
        {
            if (pos.name == idString)
            {
                return pos;
            }
        }
        // 没有可用的点了
        Globals.Assert(false);

        return null;
    }

    public UnityEngine.Vector3 GetTargetPosition(System.String targetName)
    {
        foreach (BuildingCouldDivedIn building in targetBuildings)
        {
            if(building.name == targetName)
            {
                return building.transform.position;
            }
        }
        Globals.Assert(false);
        return UnityEngine.Vector3.zero;
    }

    public BuildingCouldDivedIn GetTargetBuilding(System.String targetName)
    {
        foreach (BuildingCouldDivedIn building in targetBuildings)
        {
            if (building.name == targetName)
            {
                return building.GetComponent<BuildingCouldDivedIn>();
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
            Building building = Globals.FingerRayToObj<Building>(
                Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), 16, fingerDownOnMap);

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
            if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over && building.gameObject != firstTarget)
            {
                return ;
            }

            building.Choosen();
            choosenBuilding = building;
        }
    }
}
