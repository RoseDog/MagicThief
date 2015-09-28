public class StealingLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 500.0f, 0.0f);
    protected float fallingDuration = 0.4f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealing;
    public LevelTip LevelTip;
    public UnityEngine.UI.Button LeaveBtn;
    UnityEngine.UI.Button DiveInBtn;
    UnityEngine.UI.Button ReplaySpeedBtn;
    UnityEngine.UI.Text ReplaySpeedText;
    public Number StealingCash;
    public UnityEngine.UI.Text RestartText;    
    public System.Collections.Generic.List<UnityEngine.GameObject> coinsOnFloor = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public int paperMovingDuration = 25;    

    UnityEngine.GameObject mark_prefab;
    public UnityEngine.GameObject landingMark;

    UnityEngine.GameObject movingmark_prefab; 

    public System.Collections.Generic.List<UnityEngine.GameObject> unstolenGems = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public System.Collections.Generic.List<Chest> unstolenChests = new System.Collections.Generic.List<Chest>();

    UnityEngine.GameObject HypnosisMouseHoverSpriter_prefab;
    UnityEngine.GameObject ShotGunMouseHoverSpriter_prefab;
    bool bRandomGuards = false;

    public System.Collections.Generic.List<System.String> pickedItems = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<System.String> itemsConsumed = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<System.String> itemsDropingWhenEscape = new System.Collections.Generic.List<System.String>();
    
    public override void Awake()
    {
        base.Awake();
        Globals.stealingController = this;
        countDownSeconds = restartInSeconds;

        canvasForStealing = UnityEngine.GameObject.Find("CanvasForStealing") as UnityEngine.GameObject;
        mainCanvas = canvasForStealing.GetComponent<UnityEngine.Canvas>();
        LevelTip = Globals.getChildGameObject<LevelTip>(canvasForStealing, "LevelTip");
        LevelTip.gameObject.SetActive(false);
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealing, "LeaveBtn");        
        LeaveBtn.gameObject.SetActive(false);
        DiveInBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealing, "DiveInBtn");
        DiveInBtn.gameObject.SetActive(false);

        ReplaySpeedBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealing, "ReplaySpeedBtn");
        ReplaySpeedText = Globals.getChildGameObject<UnityEngine.UI.Text>(ReplaySpeedBtn.gameObject, "ReplaySpeedText");
        ReplaySpeedBtn.gameObject.SetActive(false);
        
        

        StealingCash = Globals.getChildGameObject<Number>(canvasForStealing, "StealingCash");
        StealingCash.gameObject.SetActive(false);
        RestartText = Globals.getChildGameObject<UnityEngine.UI.Text>(canvasForStealing, "RestartText");
        RestartText.gameObject.SetActive(false);
                
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);


        mark_prefab = UnityEngine.Resources.Load("Misc/LandingPositionMark") as UnityEngine.GameObject;        
        landingMark = UnityEngine.GameObject.Instantiate(mark_prefab) as UnityEngine.GameObject;

        movingmark_prefab = UnityEngine.Resources.Load("Avatar/moving_mark") as UnityEngine.GameObject;
        
        
        
        landingMark.SetActive(false);

        fogTex = new UnityEngine.Texture2D(256, 256, UnityEngine.TextureFormat.ARGB32, false);        
        Globals.Assert(fogPlane != null, "no FogPlane");

        HypnosisMouseHoverSpriter_prefab = UnityEngine.Resources.Load("Avatar/HypnosisMouseHoverSpriter") as UnityEngine.GameObject;
        ShotGunMouseHoverSpriter_prefab = UnityEngine.Resources.Load("Avatar/ShotGunMouseHoverSpriter") as UnityEngine.GameObject;
    }

    public override void Start()
    {
        base.Start();        
    }

    public override void BeforeGenerateMaze()
    {        
        if (Globals.playingReplay == null)
        {
            ReplaySpeedBtn.gameObject.SetActive(false);
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

            ReplaySpeedBtn.gameObject.SetActive(true);
            ReplaySpeedBtn.onClick.AddListener(() => ReplaySpeed());
            Globals.languageTable.SetText(ReplaySpeedText, "replay_speed", new System.String[] { Globals.replaySystem.playSpeed.ToString() });
        }

        Globals.guardPlayer = new PlayerInfo();
        Globals.guardPlayer.isBot = true;
        Globals.iniFileName = "Test";
        SafeBoxData data = new SafeBoxData();
        Globals.guardPlayer.safeBoxDatas.Add(data);
        data = new SafeBoxData();
        Globals.guardPlayer.safeBoxDatas.Add(data);
        data = new SafeBoxData();
        Globals.guardPlayer.safeBoxDatas.Add(data);

        Globals.guardPlayer.currentMazeRandSeedCache = -1;
        Globals.guardPlayer.currentMazeLevel = 5;
        Globals.thiefPlayer = Globals.guardPlayer;
        Globals.ReadMazeIniFile(Globals.iniFileName, Globals.guardPlayer.currentMazeRandSeedCache);
//         bool is_rand_bot = false;
//         int seed = 0;
//         if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
//         {
//             Globals.iniFileName = "Tutorial_Level_" + Globals.self.TutorialLevelIdx.ToString();
//             seed = -1;            
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
//                     is_rand_bot = true;
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
//             Globals.guardPlayer.cashAmount = Globals.maze.CASH;
//             if (is_rand_bot)
//             {
//                 Globals.guardPlayer.cashAmount /= 2;
//             }            
//         }
        
        randSeedCache = UnityEngine.Random.seed;

        if (magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/" + Globals.thiefPlayer.selectedMagician.name) as UnityEngine.GameObject;
            magician = UnityEngine.GameObject.Instantiate(magician_prefab).GetComponent<Magician>();
            magician.gameObject.SetActive(false);
            if (Globals.playingReplay != null || Globals.iniFileName == "Test")
            {
                Globals.canvasForMagician.UpdateTrickInUseSlots(Globals.thiefPlayer, magician);
                Globals.canvasForMagician.UpdateCharacter(Globals.thiefPlayer);
            }
        }

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

                    int roomConsumed = 0;
                    while (roomConsumed < mazeData.roomSupport)
                    {
                        int rand_guard_idx = UnityEngine.Random.Range(0, mazeData.lockGuardsName.Length);
                        System.String rand_guard_name = mazeData.lockGuardsName[rand_guard_idx];
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
       
        magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
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
            Globals.languageTable.SetText(RestartText, "click_guard_to_show_info", new System.String[] { Globals.guardPlayer.currentMazeLevel.ToString() });
        }
        else
        {
            Globals.languageTable.SetText(RestartText, "other_player_maze_name",new System.String[] { Globals.guardPlayer.name, Globals.guardPlayer.currentMazeLevel.ToString() });            
        }               

        if (Globals.playingReplay != null)
        {
            Globals.canvasForMagician.SetCashVisible(false);
            Globals.canvasForMagician.SetRoseVisible(false);
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
            magician.gameObject.SetActive(false);
        }
        // 没有守卫，而且是教程中。不需要潜入按钮，直接开始
        else if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {
            // 主角降下          
            magician.transform.position += new UnityEngine.Vector3(0, 0, -0.6f);
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
        Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/David Julyan - Flirting");
        Globals.cameraFollowMagician.audioSource.PlayDelayed(1.0f);
        Globals.cameraFollowMagician.audioSource.volume = 1.4f;        
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
            TrickData flash = Globals.thiefPlayer.GetTrickByName("flash_grenade");
            if (flash.IsInUse())
            {
                Globals.tipDisplay.Msg("cant_bring_flash_to_stealing");
            }
            else
            {                
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
        magician.gameObject.SetActive(true);        

        Globals.EnableAllInput(false);

        if (landingMark.activeSelf)
        {
            magician.transform.position = new UnityEngine.Vector3(
                landingMark.transform.position.x,
                landingMark.transform.position.y,
                landingMark.transform.position.z - 0.01f);
        }

        Globals.cameraFollowMagician.MoveToPoint(magician.transform.position, 30);

        // 主角降下     
        magician.falling.from = magician.transform.position + posOnSky;
        magician.falling.to = magician.transform.position;
        magician.falling.Excute();        
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
            LeaveBtn.gameObject.SetActive(false);
        }
                
        if (Globals.maze.LevelTipText != "")
        {
            LevelTip.gameObject.SetActive(true);
            LevelTip.Show(Globals.maze.LevelTipText);        
        }
        
        StealingCash.gameObject.SetActive(true);
        
        //Globals.canvasForMagician.tricksInUseTip.SetActive(false);
        // 魔术师出场   
        magician.InStealing();        

        foreach(Chest chest in Globals.maze.chests)
        {
            chest.spriteRenderer.GetComponent<UnityEngine.BoxCollider>().enabled = false;
        }

        Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/尋問");
        Globals.cameraFollowMagician.audioSource.PlayDelayed(1.0f);
        Globals.cameraFollowMagician.audioSource.volume = 1.0f;        
    }

    

    public override void PerfectStealing()
    {
        base.PerfectStealing();

        magician.victory.Excute();

        if (Globals.playingReplay != null)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }        
    }

    public override void AfterStealingSuccessedEscaped()
    {
        if (Globals.playingReplay == null)
        {
            // 成功了，无论如何要消耗道具
            itemsConsumed = Globals.self.ConsumeItem();
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                Globals.canvasForMagician.ChangeCash(StealingCash.numberAmont);
                Globals.self.AdvanceTutorial();
            }
            // 不在播放录像，而且的确潜入开始了的，上传这次潜入
            else
            {
                foreach(System.String itemname in pickedItems)
                {
                    Globals.self.AddTrickItem(Globals.self.GetTrickByName(itemname));
                }
                
                if (!Globals.guardPlayer.isBot)
                {
                    foreach (System.String itemname in pickedItems)
                    {
                        Globals.guardPlayer.RemoveDroppedItem(itemname);
                    }
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
                Globals.transition.BlackOut(() => EndingUI());
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

    public void AfterMagicianLifeOverEscaped()
    {
        if (Globals.playingReplay == null)
        {
            StealingCash.SetToZero();
            StealingCash.gameObject.SetActive(false);
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                InvokeRepeating("RestartCount", 4.0f, 1.0f);
            }
            else
            {
                // 失败了，不在教程中，要消耗道具
                itemsConsumed = Globals.self.ConsumeItem();
                itemsDropingWhenEscape = Globals.self.DropItems();
                Globals.self.StealingOver(0, 0, false);
                Globals.canvasForMagician.CheckBuyTrickAndSlotTip();
            }        
        }
        else
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");
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

    void Restart()
    {
        StealingCash.SetToZero();
        Globals.guardPlayer.safeBoxDatas.Clear();
        magician.gameObject.SetActive(false);
        Globals.maze.ClearMaze();
        Start();
        Globals.maze.Start();
    }

    UnityEngine.UI.Text sentences_ui;
    public void EndingUI()
    {
        magician.gameObject.SetActive(false);
        Globals.maze.ClearGuards();
        UnityEngine.Debug.Log("EndingUI");
        
        
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
        
        sentences_ui = Globals.getChildGameObject<UnityEngine.UI.Text>(MoonNightThief, "Sentences");
        sentences_ui.text = "";

        Globals.cameraFollowMagician.audioSource.Stop();
        if (!magician.IsLifeOver())
        {
            Globals.cameraFollowMagician.audioSource.PlayOneShot(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/Shop"));
            Globals.transition.BlackIn(() => GetPaid());
        }
        else
        {
            FiddleGem fiddle_gem = Globals.getChildGameObject<FiddleGem>(MoonNightThief, "fiddle_gem");
            fiddle_gem.gameObject.SetActive(false);
            Globals.cameraFollowMagician.audioSource.volume = 0.5f;                    
            Globals.cameraFollowMagician.audioSource.PlayOneShot(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/23195__kaponja__2trump"));
            Globals.transition.BlackIn(() => LoseRose());
        }

        // consumed
        ending_str = "";
        ending_str += Globals.languageTable.GetText("item_consumed_label");
        ending_str += "\n";
        foreach (System.String item in itemsConsumed)
        {
            ending_str += Globals.languageTable.GetText("item_consumed", new System.String[] { Globals.languageTable.GetText(item), "1" });
            ending_str += "\n";
        }
    }

    System.String ending_str;
    System.String[] strs;
    int sentence_idx;
    void GetPaid()
    {
        if (pickedItems.Count != 0)
        {
            ending_str += Globals.languageTable.GetText("item_picked_label");
            ending_str += "\n";
            // picked
            foreach (TrickData data in Globals.self.tricks)
            {
                int picked_count = 0;
                foreach (System.String itemname in pickedItems)
                {
                    if (data.nameKey == itemname)
                    {
                        ++picked_count;
                    }
                }
                if (picked_count > 0)
                {
                    ending_str += Globals.languageTable.GetText("item_picked", new System.String[] { Globals.languageTable.GetText(data.nameKey), picked_count.ToString() });
                    ending_str += "\n";
                }
            }
        }
        

        ending_str += Globals.languageTable.GetText("get_paid",new System.String[] { StealingCash.numberAmont.ToString("F0") });

        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over && !Globals.guardPlayer.isBot && bIsPerfectStealing)
        {
            ending_str += Globals.languageTable.GetText("perfect_stealing", new System.String[] { perfect_stealing_bonus.ToString(), rose_bonus.ToString() });            
            Globals.self.PickRose(rose_bonus, null);
            Globals.canvasForMagician.RoseNumber.Add(rose_bonus);
        }
        else
        {
            ending_str += Globals.languageTable.GetText("then_you_leave", new System.String[] {});
        }

        strs = ending_str.Split('\n');
        sentence_idx = 0;
        sentences_ui.text = "";
        StartCoroutine(Sentence(strs[sentence_idx]));
    }

    void LoseRose()
    {
        if (itemsDropingWhenEscape.Count != 0)
        {
            ending_str += Globals.languageTable.GetText("item_dropped_label");
            ending_str += "\n";
            foreach (System.String itemname in itemsDropingWhenEscape)
            {
                ending_str += Globals.languageTable.GetText("item_dropped", new System.String[] { Globals.languageTable.GetText(itemname), "1" });
                ending_str += "\n";
            }
        }        

        int lose_rose = 0;
        if(Globals.guardPlayer.isBot)
        {
            int lv_idx = System.Convert.ToInt32(Globals.iniFileName.Split('_')[1]);
            if (lv_idx >= 0 && lv_idx<=6)
            {
                lose_rose = 1;
            }
            else if (lv_idx >= 7 && lv_idx <= 12)
            {
                lose_rose = 3;
            }
            else
            {
                lose_rose = 5;
            }
        }
        else
        {
            lose_rose = Globals.guardPlayer.currentMazeLevel;
        }

        ending_str += Globals.languageTable.GetText("lose_rose", new System.String[] { lose_rose.ToString("F0") });

        Globals.self.ChangeRose(-lose_rose);
        Globals.canvasForMagician.RoseNumber.Add(-lose_rose);

        strs = ending_str.Split('\n');
        sentence_idx = 0;
        sentences_ui.text = "";
        StartCoroutine(Sentence(strs[sentence_idx]));
    }

    System.Collections.IEnumerator Sentence(System.String sentence)
    {
        sentences_ui.GetComponent<UnityEngine.AudioSource>().Play();
        // 有颜色的文本，代表奖励，直接一次性加上去
        if(sentence.Contains("<"))
        {
            sentences_ui.text += sentence;
            yield return new UnityEngine.WaitForSeconds(0.35f);
        }
        else
        {
            for (int idx = 0; idx < sentence.Length; ++idx)
            {
                sentences_ui.text += sentence[idx];
                yield return new UnityEngine.WaitForSeconds(0.06f);
            }
        }

        sentences_ui.GetComponent<UnityEngine.AudioSource>().Stop();
        yield return new UnityEngine.WaitForSeconds(1.0f);
        sentences_ui.text += "\n";
        ++sentence_idx;
        if (sentence_idx < strs.Length)
        {
            StartCoroutine(Sentence(strs[sentence_idx]));
        }
        else
        {
            yield return new UnityEngine.WaitForSeconds(1.0f);
            NewsreportOutEnd();
        }
    }

    void NewsreportOutEnd()
    {
        magician.transform.parent = null;
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
            magician.OutStealing();
            Globals.asyncLoad.ToLoadSceneAsync("City");            
        }
        else
        {
            Globals.replaySystem.RecordMagicianTryEscape();

            if (!magician.IsLifeOver())
            {
                magician.tryEscape.Excute();
            }
        }
    }    

    public void StopReplay()
    {
        Clear();
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public void ReplaySpeed()
    {
        Globals.replaySystem.playSpeed *= 2;
        // 最大4倍
        if (Globals.replaySystem.playSpeed > 4)
        {
            Globals.replaySystem.playSpeed = 1;
        }

        if (Globals.replaySystem.playSpeed == 1)
        {
            UnityEngine.Application.targetFrameRate = 30;
        }
        else
        {
            UnityEngine.Application.targetFrameRate = 120;
        }

        Globals.languageTable.SetText(ReplaySpeedText, "replay_speed", new System.String[] { Globals.replaySystem.playSpeed.ToString() });
    }

    public override void Update()
    {
        FrameFunc();        
        if(Globals.playingReplay!=null)
        {
            if (Globals.replaySystem.playSpeed == 2)
            {
                FrameFunc();
            }
            else if (Globals.replaySystem.playSpeed == 4)
            {
                FrameFunc();
                FrameFunc();
            }
        }        
    }

    public Guard hoveredGuard;
    MouseHoverSpriter hoveringSpriter;
    public override void FrameFunc()
    {
        Globals.replaySystem.FrameFunc();
        base.FrameFunc();        
        AstarPath.CalculatePaths(AstarPath.threadInfos[0]);        

        if (magician !=null && magician.Stealing && Globals.playingReplay == null)            
        {
            UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(UnityEngine.Input.mousePosition);
            UnityEngine.RaycastHit hitInfo;
            int layermask = 1 << 13;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask) 
                && magician.currentAction != magician.hitted
                && magician.currentAction != magician.catchByNet)
            {
                Guard hovered = hitInfo.collider.gameObject.GetComponent<Guard>();
    
                if (magician.hypnosis.data.IsInUse() && hovered.beenHypnosised)
                {
                    if (hoveringSpriter == null)
                    {
                        hoveringSpriter = (UnityEngine.GameObject.Instantiate(HypnosisMouseHoverSpriter_prefab) as UnityEngine.GameObject).GetComponent<MouseHoverSpriter>();
                    }
                    
                    hoveredGuard = hovered;
                    hoveringSpriter.transform.position = new UnityEngine.Vector3(ray.origin.x, ray.origin.y, 0);
                }
                else if (magician.shot.data.IsInUse() && hovered as Machine != null)
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

        UnityEngine.RenderTexture.active = fogCam_2.targetTexture;
        UnityEngine.Rect rectReadPicture = new UnityEngine.Rect(0,0,256,256);
        // Read pixels
        fogTex.ReadPixels(rectReadPicture, 0, 0);
        UnityEngine.RenderTexture.active = null; // added to avoid errors 

        foreach(Guard guard in Globals.maze.guards)
        {            
            UnityEngine.Vector3 view_pos = fogCam.WorldToViewportPoint(guard.transform.position);
            int x = (int)((view_pos.x) * 256.0f);
            int y = (int)((view_pos.y) * 256.0f);
            UnityEngine.Color32 color = fogTex.GetPixel(x, y);
            if (color.a > 50)
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
        
        if (magician.Stealing && hoveredGuard != null)
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
            magician.Shot(guard.gameObject);
        }
        else if (magician.currentAction != magician.hypnosis)
        {
            magician.hypnosis.Cast(guard);
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
        System.String content = "RayOnMap：";
        content += ray.origin.ToString("F3") + " " + ray.direction.ToString();

        Globals.record("testReplay", content);
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 9;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {            
            Pathfinding.Node node = Globals.maze.pathFinder.GetNearestWalkableNode(hitInfo.point);
            UnityEngine.Vector3 pos = Globals.GetPathNodePos(node);

            if (!magician.gameObject.activeSelf && !IsInvoking("RestartCount"))
            {                
                landingMark.SetActive(true);
                landingMark.transform.position = new UnityEngine.Vector3(pos.x, pos.y, fogPlane.transform.position.z - 0.1f);
            }
            else if (magician.Stealing)
            {
                UnityEngine.GameObject moving_mark = UnityEngine.GameObject.Instantiate(movingmark_prefab) as UnityEngine.GameObject;
                moving_mark.transform.position = hitInfo.point;
                if (magician.currentAction == magician.flyUp)
                {
                    magician.flyUp.destination = hitInfo.point;
                }
                else
                {
                    if (Globals.maze.pathFinder.IsPositionWalkable(hitInfo.point))
                    {
                        magician.GoTo(hitInfo.point);
                    }
                    else
                    {
                        magician.GoTo(Globals.GetPathNodePos(Globals.maze.pathFinder.GetNearestWalkableNode(hitInfo.point)));
                    }
                }                   
            }
            
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Clear();
    }

    public void Clear()
    {        
        Globals.thiefPlayer = null;
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            Globals.guardPlayer = null;
        }               

        UnityEngine.Application.targetFrameRate = 30;
        Globals.replaySystem.ResetData();
        Globals.canvasForMagician.TrickUsingHighlightOff();
    }
}
