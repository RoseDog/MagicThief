public class TutorialLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 5.0f, 0.0f);
    protected float fallingDuration = 0.4f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealingBegin;
    public LevelTip LevelTip;
    UnityEngine.UI.Button LeaveBtn;
    public Number StealingCash;
    public UnityEngine.UI.Text RestartText;    
    public System.Collections.Generic.List<UnityEngine.GameObject> coinsOnFloor = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public int paperMovingDuration = 40;
    
    UnityEngine.Vector3 camOffsetInSpy = new UnityEngine.Vector3(0, 20, -7);
    UnityEngine.Vector3 camOffsetInStealing = new UnityEngine.Vector3(0, 21, -7);

    UnityEngine.GameObject mark_prefab;
    public UnityEngine.GameObject landingMark;

    public System.Collections.Generic.List<UnityEngine.GameObject> unstolenGems = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public UnityEngine.GameObject fogPlane;    
    public override void Awake()
    {
        base.Awake();
        countDownSeconds = restartInSeconds;

        canvasForStealingBegin = UnityEngine.GameObject.Find("CanvasForStealingBegin") as UnityEngine.GameObject;
        mainCanvas = canvasForStealingBegin.GetComponent<UnityEngine.Canvas>();
        LevelTip = Globals.getChildGameObject<LevelTip>(canvasForStealingBegin, "LevelTip");
        LevelTip.gameObject.SetActive(false);
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "LeaveBtn");
        LeaveBtn.gameObject.SetActive(false);
        StealingCash = Globals.getChildGameObject<Number>(canvasForStealingBegin, "StealingCash");
        StealingCash.gameObject.SetActive(false);
        RestartText = Globals.getChildGameObject<UnityEngine.UI.Text>(canvasForStealingBegin, "RestartText");
        RestartText.gameObject.SetActive(false);

        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            Globals.iniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        }
        else
        {
            Globals.Assert(Globals.iniFileName != "");
            if (Globals.iniFileName == "")
            {
                Globals.iniFileName = "poker_face";
            }
        }        
                
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);


        mark_prefab = UnityEngine.Resources.Load("Misc/LandingPositionMark") as UnityEngine.GameObject;
        landingMark = UnityEngine.GameObject.Instantiate(mark_prefab) as UnityEngine.GameObject;
        UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
        markBtn.onClick.AddListener(() => MagicianFallingDown());        
        markBtn.GetComponent<Actor>().AddAction(
            new RotateTo(
                new UnityEngine.Vector3(0.0f, 0.0f, 0.0f), 
                new UnityEngine.Vector3(0.0f, 00.0f, 360.0f), 100, true));
        landingMark.SetActive(false);

        fogPlane = UnityEngine.GameObject.Find("FogPlane");
        Globals.Assert(fogPlane != null, "no FogPlane");        
    }

    public override void BeforeGenerateMaze()
    {
        if(!Globals.PLAY_RECORDS)
        {
            Globals.ReadMazeIniFile(Globals.iniFileName, true);
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/replay.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_pvp.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");
        }
        else
        {
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");
            Globals.replay.ReadFile();            
        }
        
        base.BeforeGenerateMaze();
    }    

    public override void MazeFinished()
    {
        base.MazeFinished();        
        foreach (Chest chest in Globals.maze.chests)
        {
            chest.Visible(true);
        }
        unstolenGems.Clear();
        foreach(UnityEngine.GameObject gem in Globals.maze.gemHolders)
        {
            unstolenGems.Add(gem);
        }

        foreach(Guard guard in Globals.maze.guards)
        {
            guard.InitTricksUI();
            guard.FindGuardedGem();
        }        
       
        Globals.magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        Globals.maze.RegistChallengerEvent();

        Globals.cameraFollowMagician.Reset();

        Globals.canvasForMagician.gameObject.SetActive(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(false);

        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetGem)
        {
            Globals.magician.gameObject.SetActive(true);
            //Globals.cameraFollowMagician.disOffset = camOffsetInStealing;
            Globals.canvasForMagician.HideTricksPanel();
            // 隐藏界面                        
            StealingCash.gameObject.SetActive(false);            

            // 禁止输入        
            Globals.EnableAllInput(false);
            // 相机就位
            Globals.cameraFollowMagician.transform.position = 
                Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f + new UnityEngine.Vector3(0, 0.2f, 0);

            // 角色落下
            MagicianFallingDown();
            Globals.cameraFollowMagician.StaringMagician(fallingDuration);
        }
        else
        {
            Globals.EnableAllInput(true);
            Globals.canvasForMagician.SetLifeVisible(true);
            RestartText.gameObject.SetActive(true);
            Globals.languageTable.SetText(RestartText, "click_guard_to_show_info");
            Globals.magician.ResetLifeAndPower();

            //Globals.cameraFollowMagician.disOffset = camOffsetInSpy;
            if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetAroundGuard)
            {
                Globals.canvasForMagician.HideTricksPanel();
            }
            else
            {
                Globals.canvasForMagician.ShowTricksPanel();
            }
            
            // 有守卫，要点了潜入才能开始
            if (Globals.maze.guards.Count != 0)
            {                
                Globals.magician.gameObject.SetActive(false);                
            }
            // 没有守卫，不需要潜入按钮，直接开始
            else
            {
                Globals.magician.gameObject.SetActive(true);
                // 主角降下          
                MagicianFallingDown();
            }

            // 如果从城市地图过来，显示出离开的按钮
            if (Globals.asyncLoad.LastLevelName == "City")
            {
                LeaveBtn.gameObject.SetActive(true);
            }
            else
            {
                LeaveBtn.gameObject.SetActive(false);
            }
        }
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
    }

    public void MagicianFallingDown()
    {
        Globals.replay.RecordMageFallingDown();
        RestartText.gameObject.SetActive(false);
        LeaveBtn.gameObject.SetActive(false);
        Globals.canvasForMagician.equipBtn.gameObject.SetActive(false);
        Globals.magician.gameObject.SetActive(true);        
        if(landingMark.activeSelf)
        {
            Globals.magician.transform.position = new UnityEngine.Vector3(
                landingMark.transform.position.x, 
                landingMark.transform.position.y,
                landingMark.transform.position.z-0.01f);
        }        
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.GetGem)
        {
            UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
            markBtn.interactable = false;
            // 相机跟随                    
            Globals.cameraFollowMagician.MoveToPoint(Globals.magician.transform.position, 30);


            Globals.EnableAllInput(false);
        }

        // 主角降下     
        Globals.magician.falling.from = Globals.magician.transform.position + posOnSky;
        Globals.magician.falling.to = Globals.magician.transform.position;
        Globals.magician.falling.duration = fallingDuration;
        Globals.magician.falling.Excute();       
    }

    public override void AfterMagicianFalling()
    {        
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetGem)
        {
            // 妈比这个名字到底咋个取
            SleepThenCallFunction(80, () => TutorialOneMageGirlFallingOver());
        }
        else 
        {
            Globals.EnableAllInput(true);
            OperateMagician();
        }
       
        if (landingMark.activeSelf)
        {
            UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
            markBtn.interactable = true;
            landingMark.SetActive(false);
        }
        
        base.AfterMagicianFalling();
    }

    void TutorialOneMageGirlFallingOver()
    {
        Globals.transition.BlackOut(()=>OperateMagician());
        // 相机跟随                    
        Globals.cameraFollowMagician.MoveToPoint(Globals.magician.transform.position, 30);
    }   

    public void OperateMagician()
    {
        if (Globals.maze.LevelTipText != "")
        {
            LevelTip.Show(Globals.maze.LevelTipText);        
        }
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetGem)
        {            
            Globals.transition.BlackIn();
        }
        
        StealingCash.gameObject.SetActive(true);
        Globals.canvasForMagician.tricksInUseTip.SetActive(false);
        // 魔术师出场   
        Globals.magician.InStealing();                
    }

    public override void MagicianLifeOver()
    {
        float forceFactor = 2.0f;
        float rotateForceFactor = 115.0f;

        UnityEngine.GameObject dropPrefab = UnityEngine.Resources.Load("Props/DroppedCoin") as UnityEngine.GameObject;
        for (int i = 0; i < 15; ++i)
        {
            UnityEngine.GameObject coin = UnityEngine.GameObject.Instantiate(dropPrefab,
                Globals.magician.transform.position + new UnityEngine.Vector3(0, Globals.magician.controller.bounds.size.y * 0.5f, 0),
                Globals.magician.transform.rotation) as UnityEngine.GameObject;

            UnityEngine.Vector3 randForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(1.0f, 4.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f));
            randForce *= forceFactor;

            UnityEngine.Vector3 randRotForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
                                                UnityEngine.Random.Range(-1.0f, 1.0f),
                                                UnityEngine.Random.Range(-1.0f, 1.0f));
            randRotForce *= rotateForceFactor;
            coin.rigidbody.velocity = randForce;
            coin.rigidbody.angularVelocity = randRotForce;
            coinsOnFloor.Add(coin);
        }


        StealingCash.SetToZero();
        StealingCash.gameObject.SetActive(false);
        Globals.canvasForMagician.gameObject.SetActive(false);
        base.MagicianLifeOver();
        if(Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            InvokeRepeating("RestartCount", 4.0f, 1.0f);
        }        
    }

    void RestartCount()
    {
        RestartText.gameObject.SetActive(true);
        if (countDownSeconds >= 0)
        {
            Globals.languageTable.SetText(RestartText, "restart_tutorial_level_tip", 
                new System.String[] { countDownSeconds.ToString() });
            --countDownSeconds;
        }
        else
        {
            foreach (UnityEngine.GameObject coin in coinsOnFloor)
            {
                Destroy(coin);
            }
            coinsOnFloor.Clear();
            countDownSeconds = restartInSeconds;
            CancelInvoke("RestartCount");

            Globals.magician.gameObject.SetActive(false);            
            Globals.maze.ClearMaze();
            Start();
            Globals.maze.Start();
        }
    }

    public override void LevelPassed()
    {
        base.LevelPassed();
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            ++Globals.TutorialLevelIdx;
        }        
        Globals.iniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);
        Globals.canvasForMagician.ChangeCash(StealingCash.numberAmont);
        Globals.canvasForMagician.HideTricksPanel();
        StealingCash.SetToZero();
        UnityEngine.Debug.Log("level passed:" + Globals.cashAmount.ToString());
        Globals.magician.victory.Excute();
    }

    public override void AfterMagicianSuccessedEscaped()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.InitMyMaze || Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            canvasForStealingBegin.SetActive(false);
            Globals.transition.BlackOut(()=>Newsreport());
        }
        else if (Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstTarget)
        {
            canvasForStealingBegin.SetActive(false);
            Globals.asyncLoad.ToLoadSceneAsync("City");
        }
        else
        {
            Globals.magician.gameObject.SetActive(false);
            Globals.maze.ClearMaze();
            Start();
            Globals.maze.Start();
        }
        base.AfterMagicianSuccessedEscaped();
    }

    public override void AfterMagicianLifeOverEscaped()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            Leave();
        }
        base.AfterMagicianLifeOverEscaped();
    }

    UIMover[] papers;
    void Newsreport()
    {        
        UnityEngine.Debug.Log("Newsreport");
        // 禁止输入        
        Globals.EnableAllInput(false);

        UnityEngine.GameObject MoonNightThief_prefab = UnityEngine.Resources.Load("MoonNightThief") as UnityEngine.GameObject;
        UnityEngine.GameObject MoonNightThief = UnityEngine.GameObject.Instantiate(MoonNightThief_prefab) as UnityEngine.GameObject;
        // 远离迷宫
        MoonNightThief.transform.position = new UnityEngine.Vector3(1000, 0, 1000);

        // 站立点
        UnityEngine.GameObject Stand = Globals.getChildGameObject(MoonNightThief, "StandPos");
        Globals.magician.transform.parent = Stand.transform.parent;
        Globals.magician.transform.position = Stand.transform.position;
        Globals.magician.transform.localRotation = Stand.transform.localRotation;
        Globals.magician.transform.localScale = Stand.transform.localScale;
        Globals.magician.CoverInMoonlight();

        // 相机就位
        UnityEngine.GameObject cam = Globals.getChildGameObject(MoonNightThief, "Camera");
        Globals.cameraFollowMagician.transform.parent = cam.transform.parent;
        Globals.cameraFollowMagician.transform.localPosition = cam.transform.localPosition;
        Globals.cameraFollowMagician.transform.localRotation = cam.transform.localRotation;
        Globals.cameraFollowMagician.transform.localScale = UnityEngine.Vector3.one;
        Globals.cameraFollowMagician.enabled = false;
        Globals.cameraFollowMagician.MiniMapPlane.SetActive(false);

        // 报纸报道
        // Newsreport.Show();
        papers = MoonNightThief.GetComponentsInChildren<UIMover>();
        Globals.transition.BlackIn(()=>NewsreportOut());
    }

    void NewsreportOut()
    {
        Sequence seq_action = new Sequence();
        foreach (UIMover paper in papers)
        {
            UIMover p = paper;
            seq_action.actions.Add(new SleepFor(paperMovingDuration));
            seq_action.actions.Add(new FunctionCall(() => p.BeginMove(paperMovingDuration)));
        }
        seq_action.actions.Add(new SleepFor(paperMovingDuration*3));
        seq_action.actions.Add(new FunctionCall(() => NewsreportOutEnd()));
        AddAction(seq_action);
    }

    void NewsreportOutEnd()
    {                
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.InitMyMaze)
        {
            Globals.asyncLoad.ToLoadSceneAsync("MyMaze");
        }
        else if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");
            //增加需要救助的穷人;
            Globals.AddPoorBuildingAchives(Globals.currentStealingTargetBuildingAchive);            
        }        
    }

    public void Leave()
    {
        UnityEngine.Debug.Log("back to city");
        canvasForStealingBegin.SetActive(false);
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public Guard m_lastNearest;
    public override void Update()
    {
        base.Update();
        if (Globals.magician.Stealing && Globals.magician.hypnosis.data.IsInUse())
        {
            float minDis = UnityEngine.Mathf.Infinity;
            Guard nearest = null;
            foreach (Guard guard in Globals.maze.guards)
            {
                float dis = UnityEngine.Vector3.Distance(Globals.magician.transform.position, guard.transform.position);
                if (dis < 7.0f && dis < minDis && guard.currentAction != guard.beenHypnosised)
                {
                    minDis = dis;
                    nearest = guard;
                }
            }
            if (nearest != null && m_lastNearest != nearest)
            {
                if (m_lastNearest != null)
                {
                    m_lastNearest.HideBtns();
                }
                nearest.ShowTrickBtns();
                m_lastNearest = nearest;
            }                                   
        }        
    }

    public override void ClickOnMap(UnityEngine.Vector2 finger_pos)
    {
        base.ClickOnMap(finger_pos);
        int mask = 1 << 10 | 1 << 27;
        Guard guard = Globals.FingerRayToObj<Guard>(
            Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

        mask = 1 << 14;
        Chest chest = Globals.FingerRayToObj<Chest>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

        if (!Globals.magician.Stealing && guard != null)
        {
            guard.GuardInfoBtnClicked();
        }
        else if (!Globals.magician.Stealing && chest != null)
        {
            chest.UpgradeBtnClicked();
        }
        else if (!Globals.PLAY_RECORDS)
        {
            UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(finger_pos);
            RayOnMap(ray);
            Globals.replay.RecordClick(ray);
        }               
    }

    public void RayOnMap(UnityEngine.Ray ray)
    {
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 9 | 1 << 21;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {            
            Pathfinding.Node node = Globals.maze.pathFinder.GetNearestWalkableNode(hitInfo.point);
            UnityEngine.Vector3 pos = Globals.GetPathNodePos(node);
            if (!Globals.magician.gameObject.activeSelf)
            {
                TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
                controller.landingMark.SetActive(true);
                controller.landingMark.transform.position = new UnityEngine.Vector3(pos.x, pos.y, controller.fogPlane.transform.position.z - 0.1f);
            }
            else if (Globals.magician.Stealing)
            {
                if (hitInfo.collider.gameObject.layer == 9)
                {
                    Globals.magician.GoTo(hitInfo.point);
                }
                else
                {
                    Globals.magician.ShotLight(hitInfo.collider.gameObject);
                }
            }
        }
    }
}
