public class StealingLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 500.0f, 0.0f);
    protected float fallingDuration = 0.4f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealing;
    public LevelTip LevelTip;
    UnityEngine.UI.Button LeaveBtn;
    UnityEngine.UI.Button DiveInBtn;
    public Number StealingCash;
    public UnityEngine.UI.Text RestartText;    
    public System.Collections.Generic.List<UnityEngine.GameObject> coinsOnFloor = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public int paperMovingDuration = 40;    

    UnityEngine.GameObject mark_prefab;
    public UnityEngine.GameObject landingMark;
    

    public System.Collections.Generic.List<UnityEngine.GameObject> unstolenGems = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public System.Collections.Generic.List<Chest> unstolenChests = new System.Collections.Generic.List<Chest>();

    UnityEngine.GameObject HypnosisMouseHoverSpriter_prefab;
    UnityEngine.GameObject ShotGunMouseHoverSpriter_prefab;
    bool bRandomGuards = false;
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
        DiveInBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealing, "DiveInBtn");
        DiveInBtn.gameObject.SetActive(false);
        

        StealingCash = Globals.getChildGameObject<Number>(canvasForStealing, "StealingCash");
        StealingCash.gameObject.SetActive(false);
        RestartText = Globals.getChildGameObject<UnityEngine.UI.Text>(canvasForStealing, "RestartText");
        RestartText.gameObject.SetActive(false);
                
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);


        mark_prefab = UnityEngine.Resources.Load("Misc/LandingPositionMark") as UnityEngine.GameObject;        
        landingMark = UnityEngine.GameObject.Instantiate(mark_prefab) as UnityEngine.GameObject;
        
        landingMark.SetActive(false);
        
        fogTex = new UnityEngine.Texture2D(512, 512, UnityEngine.TextureFormat.ARGB32, false);        
        Globals.Assert(fogPlane != null, "no FogPlane");

        HypnosisMouseHoverSpriter_prefab = UnityEngine.Resources.Load("Avatar/HypnosisMouseHoverSpriter") as UnityEngine.GameObject;
        ShotGunMouseHoverSpriter_prefab = UnityEngine.Resources.Load("Avatar/ShotGunMouseHoverSpriter") as UnityEngine.GameObject;      
    }    

    public override void BeforeGenerateMaze()
    {
        if (Globals.playingReplay == null)
        {
            DiveInBtn.onClick.AddListener(() => DiveInBtnClicked());
            LeaveBtn.onClick.AddListener(() => LeaveBtnClicked());

            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/replay.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_pvp.txt");
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");
        }
        else
        {
            System.IO.File.Delete(UnityEngine.Application.dataPath + "/Resources/testReplay_reply.txt");
            Globals.replaySystem.Unpack(Globals.playingReplay.ini);
            LeaveBtn.onClick.AddListener(() => StopReplay());
            DiveInBtn.gameObject.SetActive(false);
        }

        Globals.replaySystem.frameBeginNo = UnityEngine.Time.frameCount;

        Globals.guardPlayer = new PlayerInfo();
        Globals.guardPlayer.isBot = true;
        Globals.iniFileName = "Test";
        SafeBoxData data = new SafeBoxData();
        Globals.guardPlayer.safeBoxDatas.Add(data);

        Globals.guardPlayer.currentMazeRandSeedCache = -1;
        Globals.thiefPlayer = Globals.guardPlayer;
        Globals.ReadMazeIniFile(Globals.iniFileName, Globals.guardPlayer.currentMazeRandSeedCache);
//         int seed = 0;
//         if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
//         {
//             Globals.iniFileName = "Tutorial_Level_" + Globals.self.TutorialLevelIdx.ToString();
//             seed = -1;
//             
//         }
//         else
//         {
//             if(!Globals.guardPlayer.isBot)
//             {
//                 seed = Globals.guardPlayer.currentMazeRandSeedCache;
//                 Globals.iniFileName = "MyMaze_" + Globals.guardPlayer.currentMazeLevel.ToString();
//             }
//             else
//             {
//                 if(Globals.playingReplay == null)
//                 {
//                     Globals.replaySystem.RecordPvEFileName(Globals.iniFileName);
//                 }
//                 else
//                 {
//                     Globals.iniFileName = Globals.replaySystem.pveFile;
//                 }
//                 UnityEngine.TextAsset textAssets = UnityEngine.Resources.Load(Globals.iniFileName) as UnityEngine.TextAsset;
//                 if (textAssets != null && textAssets.text.Length != 0)
//                 {
//                     seed = -1;
//                 }
//                 else
//                 {
//                     // 寻找到最近有配置的地图
//                     int lv_idx = System.Convert.ToInt32(Globals.iniFileName.Split('_')[1]);
//                     while (UnityEngine.Resources.Load(Globals.iniFileName) == null)
//                     {
//                         Globals.iniFileName = "pve_" + lv_idx.ToString();
//                         --lv_idx;
//                     }
//                     bRandomGuards = true;
//                     seed = Globals.guardPlayer.currentMazeRandSeedCache;
//                 }
//             }
//         }
// 
//         IniFile ini = Globals.ReadMazeIniFile(Globals.iniFileName, seed);
//         if (Globals.guardPlayer.isBot)
//         {
//             Globals.guardPlayer.cashAmount = ini.get("CASH", 0);
//         }
//         
//         randSeedCache = UnityEngine.Random.seed;            
           
        
        base.BeforeGenerateMaze();
    }

    public override IniFile GetGuardsIniFile()
    {
        if (!Globals.guardPlayer.isBot)
        {
            IniFile ini = new IniFile();
            ini.loadFromText(Globals.guardPlayer.summonedGuardsStr);
            return ini;
        }
        else if(!bRandomGuards)
        {
            return base.GetGuardsIniFile();
        }
        return null;        
    }

    public override void MazeFinished()
    {
        base.MazeFinished();

        if( Globals.iniFileName != "Test")
        {
//             // 如果是PvE 随机金钱和守卫
            if (Globals.guardPlayer.isBot)
            {
                // 迷宫数据
                if(bRandomGuards)
                {
                    MazeLvData mazeData = Globals.mazeLvDatas[Globals.guardPlayer.currentMazeLevel];
                    System.Collections.Generic.List<System.String> unlockedGuardNames = new System.Collections.Generic.List<System.String>();
                    unlockedGuardNames.Add("joker");                    
                    if (Globals.guardPlayer.currentMazeLevel >= 2)
                    {
                        unlockedGuardNames.Add("dog");
                    }
                    if (Globals.guardPlayer.currentMazeLevel >= 5)
                    {
                        unlockedGuardNames.Add("Spider");
                    }

                    int roomConsumed = 0;
                    while (roomConsumed < mazeData.roomSupport * 0.5)
                    {
                        int rand_guard_idx = UnityEngine.Random.Range(0, unlockedGuardNames.Count);
                        System.String rand_guard_name = unlockedGuardNames[rand_guard_idx];
                        GuardData guard = Globals.GetGuardData(rand_guard_name);
                        Globals.maze.PlaceRandGuard(guard);
                        roomConsumed += guard.roomConsume;
                    }
                }

                Globals.guardPlayer.safeBoxDatas.Clear();
                for (int idx = 0; idx < Globals.maze.noOfRoomsToPlace; ++idx)
                {
                    SafeBoxData data = new SafeBoxData();
                    Globals.guardPlayer.safeBoxDatas.Add(data);
                    data.Lv = Globals.safeBoxLvDatas.Length - 1;
                }

                Globals.maze.PlaceGemsAtBoarder();
            }
        }

        SyncWithChestData(Globals.guardPlayer);
        
        unstolenChests.Clear();
        foreach(Chest chest in Globals.maze.chests)
        {
            if (chest.IsVisible())
            {
                unstolenChests.Add(chest);
            }            
        }

        unstolenGems.Clear();
        foreach (UnityEngine.GameObject gem in Globals.maze.gemHolders)
        {
            unstolenGems.Add(gem);
        }

        foreach(Guard guard in Globals.maze.guards)
        {
            guard.FindGuardedChest();
        }
       
        Globals.magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        Globals.maze.RegistChallengerEvent();

        Globals.cameraFollowMagician.Reset();

        Globals.canvasForMagician.gameObject.SetActive(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(false);

        Globals.EnableAllInput(true);
        canvasForStealing.SetActive(true);
        Globals.canvasForMagician.ShowTricksPanel();
        Globals.canvasForMagician.SetLifeVisible(true);
        Globals.canvasForMagician.SetPowerVisible(true);
        RestartText.gameObject.SetActive(true);
        if (Globals.guardPlayer.isBot)
        {
            Globals.languageTable.SetText(RestartText, "click_guard_to_show_info");
        }
        else
        {
            Globals.languageTable.SetText(RestartText, "other_player_maze_name",new System.String[] { Globals.guardPlayer.name });            
        }
        
        Globals.magician.ResetLifeAndPower(Globals.thiefPlayer);

        if (Globals.playingReplay != null)
        {
            Globals.canvasForMagician.SetCashVisible(false);
            Globals.canvasForMagician.SetRoseVisible(false);
            Globals.canvasForMagician.HideTricksPanel();
        }
        else
        {
            DiveInBtn.gameObject.SetActive(true);
            Globals.canvasForMagician.SetCashVisible(true);
            Globals.canvasForMagician.SetRoseVisible(false);
            //Globals.cameraFollowMagician.disOffset = camOffsetInSpy;
//             if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.GetAroundGuard
//                 && Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.GetGem)
//             {                
//                 Globals.canvasForMagician.tricksInUseTip.SetActive(true);
//             }            
        }

        // 有守卫，要点了潜入才能开始
        if (Globals.maze.guards.Count != 0)
        {
            Globals.magician.gameObject.SetActive(false);
        }
        // 没有守卫，而且是教程中。不需要潜入按钮，直接开始
        else if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {
            // 主角降下          
            Globals.magician.transform.position += new UnityEngine.Vector3(0, 0, -0.6f);
            MagicianFallingDown();
        }

        // 不是教学关卡，可以在潜入开始前离开
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            LeaveBtn.gameObject.SetActive(true);
        }
        else
        {
            LeaveBtn.gameObject.SetActive(false);
        }        

        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/FiveFloorGoodbye");
        Globals.cameraFollowMagician.audioSource.Play();
        Globals.cameraFollowMagician.audioSource.volume = 0.25f;        
    }

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitTricksUI();
    }

    public override void OneChestGoldAllLost(Chest chest)
    {
        base.OneChestGoldAllLost(chest);
        unstolenChests.Remove(chest);
    }

    public void DiveInBtnClicked()
    {
        if (landingMark.activeSelf)
        {
            TrickData flash = Globals.GetTrickByName("flash_grenade");
            if (flash.IsInUse())
            {
                Globals.tipDisplay.Msg("cant_bring_flash_to_stealing");
            }
            else
            {
                Globals.magician.transform.position = new UnityEngine.Vector3(
                landingMark.transform.position.x,
                landingMark.transform.position.y,
                landingMark.transform.position.z - 0.01f);
                MagicianFallingDown();
            }            
        }
        else
        {
            Globals.tipDisplay.Msg("no_landmark_yet");
        }
    }

    public void MagicianFallingDown()
    {                        
        Globals.replaySystem.RecordMageFallingDown();
        canvasForStealing.gameObject.SetActive(false);
        DiveInBtn.gameObject.SetActive(false);
        Globals.magician.gameObject.SetActive(true);        

        Globals.cameraFollowMagician.MoveToPoint(Globals.magician.transform.position, 30);
        Globals.EnableAllInput(false);

        // 主角降下     
        Globals.magician.falling.from = Globals.magician.transform.position + posOnSky;
        Globals.magician.falling.to = Globals.magician.transform.position;
        Globals.magician.falling.duration = fallingDuration;
        Globals.magician.falling.Excute();        
    }

    public override void AfterMagicianFalling()
    {
        Globals.EnableAllInput(true);
        OperateMagician();
        canvasForStealing.gameObject.SetActive(true);
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.GetGem)
        {
            Globals.languageTable.SetText(RestartText, "operate_guide_info");
        }
        else
        {
            RestartText.gameObject.SetActive(false);
        }
        
        if (landingMark.activeSelf)
        {            
            landingMark.SetActive(false);
        }
        
        base.AfterMagicianFalling();
    }    

    public void OperateMagician()
    {
        if(Globals.playingReplay == null)
        {
            if (!Globals.guardPlayer.isBot)
            {
                LeaveBtn.gameObject.SetActive(true);
            }
            else
            {
                LeaveBtn.gameObject.SetActive(false);
            }
        }
        
        
        if (Globals.maze.LevelTipText != "")
        {
            LevelTip.gameObject.SetActive(true);
            LevelTip.Show(Globals.maze.LevelTipText);        
        }
        
        StealingCash.gameObject.SetActive(true);
        
        //Globals.canvasForMagician.tricksInUseTip.SetActive(false);
        // 魔术师出场   
        Globals.magician.InStealing();                
    }

    public override void MagicianLifeOver()
    {
        // 抛洒钱币。暂时不要
//         float forceFactor = 2.0f;
//         float rotateForceFactor = 115.0f;
// 
//         UnityEngine.GameObject dropPrefab = UnityEngine.Resources.Load("Props/DroppedCoin") as UnityEngine.GameObject;
//         for (int i = 0; i < 15; ++i)
//         {
//             UnityEngine.GameObject coin = UnityEngine.GameObject.Instantiate(dropPrefab,
//                 Globals.magician.transform.position + new UnityEngine.Vector3(0, Globals.magician.controller.bounds.size.y * 0.5f, 0),
//                 Globals.magician.transform.rotation) as UnityEngine.GameObject;
// 
//             UnityEngine.Vector3 randForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
//                 UnityEngine.Random.Range(1.0f, 4.0f),
//                 UnityEngine.Random.Range(-1.0f, 1.0f));
//             randForce *= forceFactor;
// 
//             UnityEngine.Vector3 randRotForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
//                                                 UnityEngine.Random.Range(-1.0f, 1.0f),
//                                                 UnityEngine.Random.Range(-1.0f, 1.0f));
//             randRotForce *= rotateForceFactor;
//             coin.GetComponent<UnityEngine.Rigidbody>().velocity = randForce;
//             coin.GetComponent<UnityEngine.Rigidbody>().angularVelocity = randRotForce;
//             coinsOnFloor.Add(coin);
//         }


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
        canvasForStealing.gameObject.SetActive(true);
        RestartText.gameObject.SetActive(true);
        Globals.canvasForMagician.SetLifeVisible(false);
        Globals.canvasForMagician.SetPowerVisible(false);
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

            Globals.transition.BlackOut(() => Restart());
        }
    }

    public override void PerfectStealing()
    {
        base.PerfectStealing();

        Globals.magician.victory.Excute();

        if (Globals.playingReplay != null)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }        
    }

    public override void AfterStealingSuccessedEscaped()
    {
        if (Globals.playingReplay == null)
        {
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                Globals.canvasForMagician.ChangeCash(StealingCash.numberAmont);
                Globals.self.AdvanceTutorial();
            }
            // 不在播放录像，而且的确潜入开始了的，上传这次潜入
            else
            {
                if (!Globals.guardPlayer.isBot)
                {
                    Globals.self.StealingOver(StealingCash.numberAmont, perfect_stealing_bonus, true);
                }
                else
                {
                    Globals.self.StealingOver(StealingCash.numberAmont, 0, true);
                }
            }
            // 金钱
            Globals.canvasForMagician.UpdateCash();

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
                Globals.transition.BlackOut(() => Restart());
            }
        }        
        base.AfterStealingSuccessedEscaped();
    }

    void Restart()
    {
        StealingCash.SetToZero();
        Globals.guardPlayer.safeBoxDatas.Clear();
        Globals.magician.gameObject.SetActive(false);
        Globals.maze.ClearMaze();
        Start();
        Globals.maze.Start();
    }

    public void PvPEscaped()
    {
        if (Globals.playingReplay == null && Globals.replaySystem.mage_falling_down_frame_no != -1)
        {
            Globals.self.StealingOver(StealingCash.numberAmont, 0, false);
        }
        else
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");
        }        
    }

    public void AfterMagicianLifeOverEscaped()
    {
        if(Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");

            Globals.canvasForMagician.CheckBuyTrickAndSlotTip();            
        }        
    }

    UIMover[] papers;
    public void Newsreport()
    {
        Globals.magician.gameObject.SetActive(false);
        Globals.maze.ClearGuards();
        UnityEngine.Debug.Log("Newsreport");
        
        
        // 禁止输入        
        Globals.EnableAllInput(false);

        UnityEngine.GameObject MoonNightThief_prefab = UnityEngine.Resources.Load("MoonNightThief") as UnityEngine.GameObject;
        UnityEngine.GameObject MoonNightThief = UnityEngine.GameObject.Instantiate(MoonNightThief_prefab) as UnityEngine.GameObject;
        // 远离迷宫
        MoonNightThief.transform.position = new UnityEngine.Vector3(1000, 0, 1000);


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

        if (!Globals.magician.IsLifeOver())
        {
            Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/Shop");
            Globals.cameraFollowMagician.audioSource.Play();
        }
        else
        {
            UnityEngine.GameObject fiddle_gem = Globals.getChildGameObject(MoonNightThief, "fiddle_gem");
            fiddle_gem.SetActive(false);
            Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/23195__kaponja__2trump");
            Globals.cameraFollowMagician.audioSource.Play();
        }
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
        for(int idx = 0; idx < 3; ++idx)
        {
            UIMover paper = papers[idx];            
            seq_action.actions.Add(new SleepFor(paperMovingDuration));
            seq_action.actions.Add(new FunctionCall(() => paper.BeginMove(paperMovingDuration)));
            Globals.languageTable.SetText(
                    paper.GetComponentInChildren<UnityEngine.UI.Text>(), news_prefix + paper.gameObject.name,
                    new System.String[] { StealingCash.numberAmont.ToString("F0") });
        }

        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over && !Globals.guardPlayer.isBot && bIsPerfectStealing)
        {
            UIMover paper = papers[3];            
            seq_action.actions.Add(new SleepFor(paperMovingDuration));
            seq_action.actions.Add(new FunctionCall(() => paper.BeginMove(paperMovingDuration)));
            Globals.languageTable.SetText(
                    paper.GetComponentInChildren<UnityEngine.UI.Text>(), "perfect_stealing",
                    new System.String[] { perfect_stealing_bonus.ToString(), rose_bonus.ToString() });
            Globals.self.ChangeRoseCount(rose_bonus, null);
            Globals.canvasForMagician.RoseNumber.Add(rose_bonus);
        }

        seq_action.actions.Add(new SleepFor(paperMovingDuration*6));
        seq_action.actions.Add(new FunctionCall(() => NewsreportOutEnd()));
        AddAction(seq_action);
    }

    void NewsreportOutEnd()
    {
        Globals.magician.transform.parent = null;
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
        if (Globals.replaySystem.mage_falling_down_frame_no == -1)
        {
            //Globals.canvasForMagician.tricksInUseTip.SetActive(false);
            Globals.magician.OutStealing();
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }
        else
        {
            Globals.replaySystem.RecordMagicianTryEscape();

            if (!Globals.magician.IsLifeOver())
            {
                Globals.magician.tryEscape.Excute();
            }
        }
    }    

    public void StopReplay()
    {
        Clear();
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public Guard hoveredGuard;
    MouseHoverSpriter hoveringSpriter;
    public override void Update()
    {
        base.Update();
        if (Globals.magician.Stealing && Globals.playingReplay == null)            
        {
            UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(UnityEngine.Input.mousePosition);
            UnityEngine.RaycastHit hitInfo;
            int layermask = 1 << 13;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask) 
                && Globals.magician.currentAction != Globals.magician.hitted
                && Globals.magician.currentAction != Globals.magician.catchByNet)
            {
                Guard hovered = hitInfo.collider.gameObject.GetComponent<Guard>();
    
                if (Globals.magician.hypnosis.data.IsInUse() && hovered.beenHypnosised)
                {
                    if (hoveringSpriter == null)
                    {
                        hoveringSpriter = (UnityEngine.GameObject.Instantiate(HypnosisMouseHoverSpriter_prefab) as UnityEngine.GameObject).GetComponent<MouseHoverSpriter>();
                    }
                    
                    hoveredGuard = hovered;
                    hoveringSpriter.transform.position = new UnityEngine.Vector3(ray.origin.x, ray.origin.y, 0);
                }
                else if (Globals.magician.shot.data.IsInUse() && hovered as Machine != null)
                {
                    if (hoveringSpriter == null)
                    {
                        hoveringSpriter = (UnityEngine.GameObject.Instantiate(ShotGunMouseHoverSpriter_prefab) as UnityEngine.GameObject).GetComponent<MouseHoverSpriter>();
                    }
                    
                    hoveredGuard = hovered;
                    hoveringSpriter.transform.position = new UnityEngine.Vector3(ray.origin.x, ray.origin.y, 0);
                }
                else
                {
                    hoveredGuard = null;
                }
            }
            else
            {
                if (hoveringSpriter != null)
                {
                    DestroyObject(hoveringSpriter.gameObject);
                    hoveringSpriter = null;
                }                
                hoveredGuard = null;
            }
        }                        

        UnityEngine.RenderTexture.active = fogCam.targetTexture;
        UnityEngine.Rect rectReadPicture = new UnityEngine.Rect(0,0,512,512);
        // Read pixels
        fogTex.ReadPixels(rectReadPicture, 0, 0);
        UnityEngine.RenderTexture.active = null; // added to avoid errors 

        foreach(Guard guard in Globals.maze.guards)
        {            
            UnityEngine.Vector3 view_pos = fogCam.WorldToViewportPoint(guard.transform.position);
            int x = (int)((view_pos.x) * 512.0f);
            int y = (int)((view_pos.y) * 512.0f);
            UnityEngine.Color32 color = fogTex.GetPixel(x, y);
            if (color.r > 128)
            {
                guard.SetInFog(false);
            }
            else
            {
                guard.SetInFog(true);
            }
        }
    }

    public override void ClickOnMap(UnityEngine.Vector2 finger_pos)
    {
        base.ClickOnMap(finger_pos);
        
        if (Globals.magician.Stealing && hoveredGuard != null)
        {
            Globals.replaySystem.RecordClickOnGuard(hoveredGuard.idx);
            ClickOnHoveredGuard(hoveredGuard);
        }
        else
        {
            int mask = 1 << 13;
            Guard guard = Globals.FingerRayToObj<Guard>(
                Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

            mask = 1 << 14;
            Chest chest = Globals.FingerRayToObj<Chest>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

            if (guard != null && !guard.inFog)
            {
                guard.GuardInfoBtnClicked();
            }
            else if (chest != null)
            {
                chest.UpgradeBtnClicked();
            }
        }        
    }

    public void ClickOnHoveredGuard(Guard guard)
    {
        if (guard as Machine != null)
        {
            Globals.magician.Shot(guard.gameObject);
        }
        else if (Globals.magician.currentAction != Globals.magician.hypnosis)
        {
            Globals.magician.hypnosis.Cast(guard);
        }
    }

    public override void RightClickOnMap(UnityEngine.Vector2 pos)
    {
        base.RightClickOnMap(pos);
        if (Globals.playingReplay == null)
        {
            UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(pos);
            RayOnMap(ray);
            Globals.replaySystem.RecordRightClickToMove(ray);
        }
    }

    public void RayOnMap(UnityEngine.Ray ray)
    {
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 9;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {            
            Pathfinding.Node node = Globals.maze.pathFinder.GetNearestWalkableNode(hitInfo.point);
            UnityEngine.Vector3 pos = Globals.GetPathNodePos(node);

            if (!Globals.magician.gameObject.activeSelf && !IsInvoking("RestartCount"))
            {                
                landingMark.SetActive(true);
                landingMark.transform.position = new UnityEngine.Vector3(pos.x, pos.y, fogPlane.transform.position.z - 0.1f);
            }
            else if (Globals.magician.Stealing && hoveredGuard == null)
            {
                if (Globals.magician.currentAction == Globals.magician.flyUp)
                {
                    Globals.magician.flyUp.destination = hitInfo.point;
                }
                else
                {
                    if (Globals.maze.pathFinder.IsPositionWalkable(hitInfo.point))
                    {
                        Globals.magician.GoTo(hitInfo.point);
                    }
                    else
                    {
                        Globals.magician.GoTo(Globals.GetPathNodePos(Globals.maze.pathFinder.GetNearestWalkableNode(hitInfo.point)));
                    }
                }                   
            }
            
        }
    }

    public void OnDestroy()
    {
        Clear();
    }

    public void Clear()
    {
        Globals.replaySystem.ResetData();        
        Globals.maze.ClearMaze();
        Globals.canvasForMagician.TrickUsingHighlightOff();
        // 这样不会再增加数字了，也不会调用到Globals.LevelController.MagicianGotCash(numberDelta);
        if (StealingCash != null)
        {
            StealingCash.gameObject.SetActive(false);
        }        
        Globals.magician.ClearAllActions();
        if (Globals.magician.currentAction != null)
        {
            Globals.magician.currentAction.Stop();
        }
        if (Globals.thiefPlayer != null)
        {
            Globals.magician.ResetLifeAndPower(Globals.thiefPlayer);
        }        
        Globals.thiefPlayer = null;
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            Globals.guardPlayer = null;
        }
    }
}
