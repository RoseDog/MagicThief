public class TutorialLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 5.0f, 0.0f);
    protected float fallingDuration = 0.4f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealing;
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

        canvasForStealing = UnityEngine.GameObject.Find("CanvasForStealing") as UnityEngine.GameObject;
        mainCanvas = canvasForStealing.GetComponent<UnityEngine.Canvas>();
        LevelTip = Globals.getChildGameObject<LevelTip>(canvasForStealing, "LevelTip");
        LevelTip.gameObject.SetActive(false);
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealing, "LeaveBtn");        
        LeaveBtn.gameObject.SetActive(false);
        StealingCash = Globals.getChildGameObject<Number>(canvasForStealing, "StealingCash");
        StealingCash.gameObject.SetActive(false);
        RestartText = Globals.getChildGameObject<UnityEngine.UI.Text>(canvasForStealing, "RestartText");
        RestartText.gameObject.SetActive(false);
                
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
        if (Globals.replay_key == "")
        {
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                Globals.iniFileName = "Tutorial_Level_" + Globals.self.TutorialLevelIdx.ToString();
            }
            Globals.ReadMazeIniFile(Globals.iniFileName, Globals.self.enemy.currentMazeRandSeedCache);
            
            randSeedCache = UnityEngine.Random.seed;            
            
            LeaveBtn.onClick.AddListener(() => LeaveBtnClicked());
            
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/replay.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_pvp.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");
        }
        else
        {
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");            
            Globals.self.Replay();
            LeaveBtn.onClick.AddListener(() => StopReplay());
        }      

        Globals.replay.frameBeginNo = UnityEngine.Time.frameCount;
        
        base.BeforeGenerateMaze();
    }

    public override IniFile GetGuardsIniFile()
    {
        if (Globals.replay_key != "")
        {
            return (Globals.self.replays[Globals.replay_key] as ReplayData).ini;
        }
        else if (Globals.self.stealingTarget.isPvP)
        {
            IniFile ini = new IniFile();
            ini.loadFromText(Globals.self.enemy.summonedGuardsStr);
            return ini;
        }        
        return base.GetGuardsIniFile();
    }

    public override void MazeFinished()
    {
        base.MazeFinished();

        // 如果是PvE 随机金钱和守卫
        if ((Globals.replay_key != "" && !(Globals.self.replays[Globals.replay_key] as ReplayData).isPvP)||
            !Globals.self.stealingTarget.isPvP)
        {
            // 找出是几级迷宫
            MazeLvData mazeData = null;
            System.Collections.Generic.List<System.String> unlockedGuardNames = new System.Collections.Generic.List<System.String>();
            for (int idx = 0; idx < Globals.mazeLvDatas.Count; ++idx)
            {
                unlockedGuardNames.AddRange(Globals.mazeLvDatas[idx].lockGuardsName);
                // 如果房间数量和箱子数量相等
                if (Globals.mazeLvDatas[idx].safeBoxCount == Globals.maze.noOfRoomsToPlace)
                {
                    Globals.self.enemy.currentMazeLevel = idx;
                    mazeData = Globals.mazeLvDatas[idx];
                    break;
                }
            }

            // 录像会同步箱子和金钱，不用生成
            if (Globals.replay_key == "")
            {
                Globals.self.enemy.cashAmount = 0;
                // 金钱满额
                for (int idx = 0; idx < Globals.maze.noOfRoomsToPlace; ++idx)
                {
                    SafeBoxData data = new SafeBoxData();
                    Globals.self.enemy.safeBoxDatas.Add(data);
                    data.Lv = Globals.self.enemy.currentMazeLevel - 1;
                    Globals.self.enemy.cashAmount += Globals.safeBoxLvDatas[data.Lv].capacity;
                }
            }
            
            if(Globals.maze.guards.Count == 0)
            {
                int roomConsumed = 0;
                while (roomConsumed < mazeData.roomSupport)
                {
                    int rand_guard_idx = UnityEngine.Random.Range(0, unlockedGuardNames.Count);
                    System.String rand_guard_name = unlockedGuardNames[rand_guard_idx];
                    GuardData guard = Globals.GetGuardData(rand_guard_name);
                    Globals.maze.PlaceRandGuard(guard);
                    roomConsumed += guard.roomConsume * 3;
                }
                Globals.maze.PlaceGemsAtBoarder();            
            }
        }

        Globals.replay.RecordSafeboxes(Globals.self.enemy);
        SyncWithChestData(Globals.self.enemy);
        
        unstolenGems.Clear();
        foreach(UnityEngine.GameObject gem in Globals.maze.gemHolders)
        {
            unstolenGems.Add(gem);
        }
       
        Globals.magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        Globals.maze.RegistChallengerEvent();

        Globals.cameraFollowMagician.Reset();

        Globals.canvasForMagician.gameObject.SetActive(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(false);

        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.GetGem)
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
            if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.GetAroundGuard)
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
            // 没有守卫，而且是教程中。不需要潜入按钮，直接开始
            else if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
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

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitTricksUI();
        guard.FindGuardedGem();
    }

    public void MagicianFallingDown()
    {
        Globals.replay.RecordMageFallingDown();
        canvasForStealing.gameObject.SetActive(false);
        Globals.magician.gameObject.SetActive(true);        
        if(landingMark.activeSelf)
        {
            Globals.magician.transform.position = new UnityEngine.Vector3(
                landingMark.transform.position.x, 
                landingMark.transform.position.y,
                landingMark.transform.position.z-0.01f);
        }
        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.GetGem)
        {
//            UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
//            markBtn.interactable = false;
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
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.GetGem)
        {
            // 妈比这个名字到底咋个取
            SleepThenCallFunction(80, () => TutorialOneMageGirlFallingOver());
        }
        else 
        {
            Globals.EnableAllInput(true);            
            OperateMagician();
        }
        canvasForStealing.gameObject.SetActive(true);
       
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
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.GetGem)
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
        base.MagicianLifeOver();
        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
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
        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {            
            Globals.self.AdvanceTutorial();
        }
        else
        {
            Globals.self.StealingOver(StealingCash.numberAmont);
        }

        Globals.magician.victory.Excute();
    }

    public override void AfterMagicianSuccessedEscaped()
    {
        if(Globals.replay_key == "")
        {
            if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.InitMyMaze)
            {
                Globals.transition.BlackOut(() => Newsreport());
            }
            else if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.FirstTarget)
            {
                Globals.asyncLoad.ToLoadSceneAsync("City");
            }
            else if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                Globals.magician.gameObject.SetActive(false);
                Globals.maze.ClearMaze();
                Start();
                Globals.maze.Start();
            }
        }        
        base.AfterMagicianSuccessedEscaped();
    }

    public override void AfterMagicianLifeOverEscaped()
    {
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            Leave();            
        }
        base.AfterMagicianLifeOverEscaped();
    }

    UIMover[] papers;
    public void Newsreport()
    {
        Globals.maze.ClearGuards();
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
        System.String news_prefix = "";
        if (!Globals.magician.IsLifeOver())
        {
            news_prefix = "news_success_";
        }
        else
        {
            news_prefix = "news_failure_";
        }
        foreach (UIMover paper in papers)
        {
            UIMover p = paper;
            seq_action.actions.Add(new SleepFor(paperMovingDuration));
            seq_action.actions.Add(new FunctionCall(() => p.BeginMove(paperMovingDuration)));
            Globals.languageTable.SetText(
                    p.GetComponentInChildren<UnityEngine.UI.Text>(), news_prefix + p.gameObject.name,
                    new System.String[] { StealingCash.numberAmont.ToString("F0") });
        }
        seq_action.actions.Add(new SleepFor(paperMovingDuration*3));
        seq_action.actions.Add(new FunctionCall(() => NewsreportOutEnd()));
        AddAction(seq_action);
    }

    void NewsreportOutEnd()
    {
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.InitMyMaze)
        {
            Globals.asyncLoad.ToLoadSceneAsync("MyMaze");
        }
        else if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }        
    }

    public void LeaveBtnClicked()
    {
        // 尚未潜入的时候离开
        if (Globals.replay.mage_falling_down_frame_no == -1)
        {
            Globals.magician.OutStealing();
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }
        else
        {
            if (!Globals.maze.IsAnyGuardSpotMagician())
            {                
                Globals.replay.RecordMagicianEscape();
                Leave();
            }
            else
            {
                Globals.tipDisplay.Msg("spotted_cant_escape");
            }
        }
    }

    public void Leave()
    {        
        if(!Globals.magician.IsLifeOver())
        {
            Globals.magician.OutStealing();
            Globals.magician.escape.Excute();
        }        
        if(Globals.replay_key == "")
        {
            Globals.self.StealingOver(StealingCash.numberAmont);            
        }
        else
        {
            StopReplay();
        }
    }

    public void StopReplay()
    {
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }    

    public Guard m_lastNearest;
    public override void Update()
    {
        base.Update();
        if (Globals.magician.Stealing && 
            (Globals.magician.hypnosis.data.IsInUse() || Globals.replay_key !="") && 
            Globals.magician.currentAction != Globals.magician.hypnosis)
        {
            float minDis = UnityEngine.Mathf.Infinity;
            Guard nearest = null;
            foreach (Guard guard in Globals.maze.guards)
            {
                if(guard.beenHypnosised != null)
                {
                    float dis = UnityEngine.Vector3.Distance(Globals.magician.transform.position, guard.transform.position);
                    if (dis < 7.0f && dis < minDis && guard.currentAction != guard.beenHypnosised)
                    {
                        minDis = dis;
                        nearest = guard;
                    }
                }                
            }
            if (m_lastNearest != nearest)
            {
                if (m_lastNearest != null)
                {
                    m_lastNearest.HideBtns();
                }
                if (nearest != null)
                {
                    nearest.ShowTrickBtns();
                }                
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
        else if (Globals.replay_key == "")
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

    public void OnDestroy()
    {        
        Globals.replay.ResetData();
        Globals.replay_key = "";
        Globals.self.stealingTarget = null;
        Globals.self.enemy = new PlayerInfo();
    }
}
