public class City : UnityEngine.MonoBehaviour 
{
    UnityEngine.GameObject firstTarget;
    UnityEngine.GameObject myMazeBuilding;

    Finger fingerDownOnMap;
    Building choosenBuilding;

    System.Collections.Generic.List<UnityEngine.GameObject> targetPoses = new System.Collections.Generic.List<UnityEngine.GameObject>();
    System.Collections.Generic.List<TargetBuilding> targetBuildings = new System.Collections.Generic.List<TargetBuilding>();
    UnityEngine.GameObject canvasForCity;
    public UIMover cityEventsOpenBtn;
    public CityEventsWindow eventsWindow;    
    void Awake()
    {
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject mgrs = UnityEngine.GameObject.Instantiate(mgrs_prefab) as UnityEngine.GameObject;
        }
        Globals.input.enabled = true;

        canvasForCity = UnityEngine.GameObject.Find("CanvasForCity");
        cityEventsOpenBtn = Globals.getChildGameObject<UIMover>(canvasForCity, "CityEventsOpenBtn");        
        eventsWindow = Globals.getChildGameObject<CityEventsWindow>(canvasForCity, "CityEventsWindow");
        eventsWindow.city = this;

        // 如果教程结束了，就显示自己的家，否则就显示第一个目标建筑
        firstTarget = UnityEngine.GameObject.Find("FirstTarget");
        firstTarget.GetComponent<TargetBuilding>().city = this;
        myMazeBuilding = UnityEngine.GameObject.Find("MyMazeBuilding");
        myMazeBuilding.GetComponent<MyMazeBuilding>().city = this;

        // 可以放置目标的点
        targetPoses.AddRange(UnityEngine.GameObject.FindGameObjectsWithTag("TargetPosition"));
        foreach (UnityEngine.GameObject pos in targetPoses)
        {
            pos.renderer.enabled = false;
        }
    }

    void Start()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            firstTarget.SetActive(false);
            myMazeBuilding.SetActive(true);            

            // 生成周围的目标
            BornTargetsBuilding(Globals.unclickedTargets);
            BornTargetsBuilding(Globals.targets);
            eventsWindow.AddEvents(Globals.unclickedTargets, true);
            eventsWindow.AddEvents(Globals.targets, false);

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
    
    public void BornTargetsBuilding(System.Collections.Generic.List<System.String> targets)
    {
        // 创建新的building, 删除TargetPos
        for(int idx = 0; idx < targets.Count; ++idx)
        {
            System.String tipText = targets[idx];
            UnityEngine.GameObject targetPrefab = UnityEngine.Resources.Load("Props/Target") as UnityEngine.GameObject;
            UnityEngine.GameObject targetObject = UnityEngine.GameObject.Instantiate(targetPrefab) as UnityEngine.GameObject;
            targetObject.transform.position = GetAvailablePos();
            TargetBuilding building = targetObject.GetComponent<TargetBuilding>();
            building.tip.text = tipText;
            building.gameObject.name = tipText;
            building.city = this;
            targetBuildings.Add(building);
        }        
    }    

    public void TargetClicked(System.String clickedTarget)
    {        
        eventsWindow.EventClicked(clickedTarget);
    }

    UnityEngine.Vector3 GetAvailablePos()
    {
        foreach(UnityEngine.GameObject pos in targetPoses)
        {
            if (pos.active)
            {
                pos.SetActive(false);
                return pos.transform.position;
            }
        }
        // 没有可用的点了
        Globals.Assert(false);

        return UnityEngine.Vector3.zero;
    }

    public UnityEngine.Vector3 GetTargetPosition(System.String targetName)
    {
        foreach (TargetBuilding building in targetBuildings)
        {
            if(building.name == targetName)
            {
                return building.transform.position;
            }
        }
        Globals.Assert(false);
        return UnityEngine.Vector3.zero;
    }

    public TargetBuilding GetTargetBuilding(System.String targetName)
    {
        foreach (TargetBuilding building in targetBuildings)
        {
            if (building.name == targetName)
            {
                return building.GetComponent<TargetBuilding>();
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
