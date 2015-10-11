public class MyMazeLevelController : LevelController 
{            
    UnityEngine.GameObject thief_prefab;
    public TutorialThief currentThief;    
    UnityEngine.Vector2 piecesDropDuration = new UnityEngine.Vector2(25, 40);
    int chestFallingDuration = 30;
    float piecesFarDis = 800;
    bool ShowDroppingProcess = true;
    int thiefIdx = 0;

    UnityEngine.GameObject GuardFullFillHisDutyTipPrefab;
    UnityEngine.GameObject GuardFullFillHisDutyTip;
    
    public override void Awake()
    {
        Globals.myMazeController = this;
        base.Awake();        
        UnityEngine.GameObject CanvasForMyMaze = UnityEngine.GameObject.Find("CanvasForMyMaze");
        mainCanvas = CanvasForMyMaze.GetComponent<UnityEngine.Canvas>();
        
        // 在TutorialLevel.InitMaze这个阶段，最开始“完整的迷宫”会闪一帧，      
        FindObjectOfType<CameraFollow>().GetComponent<UnityEngine.Camera>().enabled = false;

        GuardFullFillHisDutyTipPrefab = UnityEngine.Resources.Load("UI/GuardFullFillHisDuty") as UnityEngine.GameObject;
        fogPlane.SetActive(false);
        Globals.guardPlayer = null;
    }

    public override void Start()
    {
        base.Start();
        Globals.cameraFollowMagician.audioSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/Bluesy_Vibes");
        Globals.cameraFollowMagician.audioSource.Play();
        Globals.cameraFollowMagician.audioSource.volume = 1.25f;
    }

    public override void BeforeGenerateMaze()
    {
        Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().enabled = false;
        if (Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze)
        {
            Globals.self.currentMazeRandSeedCache = (int)System.DateTime.Now.Ticks;
            if(Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.InitMyMaze)
            {
                Globals.self.UpgradeMaze();
            }            
        }

        if (Globals.self.currentMazeLevel == 0)
        {
            Globals.iniFileName = "MyMaze_1";
        }
        else
        {
            Globals.iniFileName = "MyMaze_" + Globals.self.currentMazeLevel.ToString();
        }
        
        Globals.ReadMazeIniFile(Globals.iniFileName, Globals.self.currentMazeRandSeedCache);
        base.BeforeGenerateMaze();
    }

    public override IniFile GetGuardsIniFile()
    {
        IniFile ini = new IniFile();
        ini.loadFromText(Globals.self.summonedGuardsStr);
        return ini;
    }

    public override void MazeFinished()
    {        
        base.MazeFinished();        
        
        if (Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze)
        {
            Globals.transition.Stop();
            Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze = false;
        }

        if (Globals.maze.guards.Count != 0)
        {
            foreach(Guard guard in Globals.maze.guards)
            {
                Globals.canvasForMyMaze.ChangeMazeRoom(guard.data.roomConsume);
            }        
        }
        else
        {
            Globals.canvasForMyMaze.roomConsumed = 0;
            Globals.canvasForMyMaze.ChangeMazeRoom(0);
        }
        Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().enabled = true;
        Globals.cameraFollowMagician.Reset();
        Globals.canvasForMyMaze.CheckRoomFullUses();
        Globals.canvasForMyMaze.UpdateIncomeIntro();
        
        Globals.EnableAllInput(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(true);
        Globals.canvasForMagician.SetLifeVisible(false);
        Globals.canvasForMagician.SetPowerVisible(false);
        Globals.canvasForMagician.HideTricksPanel();
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();
        for (int idx = 0; idx < Globals.self.guardsHired.Count; ++idx)
        {
            System.String guardname = Globals.self.guardsHired[idx];
            Globals.GetGuardData(guardname).hired = true;
        }

        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {
            // 上次教程未完的时候放下来的守卫
            Globals.maze.ClearGuards();

            thief_prefab = UnityEngine.Resources.Load("Avatar/Tutorial_Thief") as UnityEngine.GameObject;
            foreach (Cell cell in Globals.maze.EveryCells)
            {
                if (cell.chest != null)
                {
                    cell.HideEverythingExceptFloor();
                }
                else
                {
                    cell.gameObject.SetActive(false);                    
                }
            }
           
            Globals.canvasForMyMaze.gameObject.SetActive(true);
            Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(false);
            Globals.canvasForMyMaze.TipBuyHereAsHome.BeginMove(Globals.uiMoveAndScaleDuration);
        }
        else
        {
            SyncWithChestData(Globals.self);
            StartDropPieces();
        }        
    }

    public override void MoneyFull(bool full)
    {
        base.MoneyFull(full);
        Globals.canvasForMyMaze.enhanceDefenseUI.add_box_tip_pointer.transform.parent.gameObject.SetActive(full);
        if(full)
        {
            Globals.canvasForMyMaze.enhanceDefenseUI.add_box_tip_pointer.ClearAllActions();
            Globals.canvasForMyMaze.enhanceDefenseUI.add_box_tip_pointer.BlinkForever();
        }        
    }

    public void ChestFallingStart()
    {        
        AddAction(new Sequence(
            new SleepFor(10), new FunctionCall(() => AddSafeBox()),
            new SleepFor(chestFallingDuration + 20), new FunctionCall(() => AddSafeBox()),
            new SleepFor(chestFallingDuration + 20), new FunctionCall(() => ChestsFallingEnd())));
        Globals.canvasForMyMaze.TipBuyHereAsHome.Goback(Globals.uiMoveAndScaleDuration);
    }
    
    void ChestsFallingEnd()
    {
        CreateNextThief();
        currentThief.ShowTipToShowMazeBtn();
    }

    void CreateNextThief()
    {        
        // 出现窃贼
        UnityEngine.GameObject thief = UnityEngine.GameObject.Instantiate(thief_prefab) as UnityEngine.GameObject;
        currentThief = thief.GetComponent<TutorialThief>();
        
        // 窃贼的位置
        currentThief.transform.position = Globals.maze.chests[thiefIdx].locate.room.doors[0].GetFloorPos();
        // 绑定提示信息按钮的响应事件
        currentThief.BtnToShowMazeBtn.onClick.AddListener(() => Globals.canvasForMyMaze.ShowCreateMazeBtn());
        currentThief.RetryBtn.onClick.AddListener(() => Retry());
        currentThief.MorGuardBtn.onClick.AddListener(() => MoreGuardBtnClicker());
        // 相机聚焦到窃贼身上        
        Globals.cameraFollowMagician.MoveToPoint(thief.transform.position, Globals.cameraMoveDuration);
    }    

    public void StartDropPieces()
    {
        Globals.canvasForMyMaze.gameObject.SetActive(false);
        Globals.canvasForMagician.gameObject.SetActive(false);
        StartCoroutine(MazePieces());
    }

    System.Collections.IEnumerator MazePieces()
    {
        System.Collections.Generic.List<Cocos2dAction> action_list = new System.Collections.Generic.List<Cocos2dAction>();
        foreach (Cell cell in Globals.maze.EveryCells)
        {
            cell.gameObject.SetActive(true);
            if (cell.GetFloor() == null)
            {
                continue;
            }
            
            UnityEngine.SpriteRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.SpriteRenderer>(true);
            foreach (UnityEngine.SpriteRenderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    if (ShowDroppingProcess)
                    {                        
                        UnityEngine.Vector3 dir = UnityEngine.Vector3.zero;
                        double directionRatio = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (directionRatio < 0.5f)
                        {
                            dir.x = UnityEngine.Random.Range(piecesFarDis/3, piecesFarDis);
                        }
                        else
                        {
                            dir.y = UnityEngine.Random.Range(piecesFarDis / 3, piecesFarDis);
                        }

                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                        {
                            dir *= -1.0f;
                        }

                        int droppingDuration = (int)UnityEngine.Random.Range(piecesDropDuration.x, piecesDropDuration.y);
                        int waitingDuration = (int)UnityEngine.Random.Range(piecesDropDuration.x, piecesDropDuration.y);
                        action_list.Add(new Sequence(
                            new SleepFor(waitingDuration),
                            new MoveTo(renderer.transform, renderer.transform.localPosition, droppingDuration)));
                        renderer.transform.localPosition += dir;
                    }                    
                    
                    //renderer.enabled = true;
                    if(ShowDroppingProcess)
                    {
                        //yield return new UnityEngine.WaitForSeconds(0.1f * 1.5f); 
                    }                    
                }                    
            }
        }
       
        if(Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {
            AddAction(new Cocos2dParallel(action_list.ToArray()));
            if (ShowDroppingProcess)
            {
                yield return new UnityEngine.WaitForSeconds(3.5f);
            }

            ThiefAboutToMove();
            SleepThenCallFunction(Globals.uiMoveAndScaleDuration, () => Globals.canvasForMyMaze.ShowSelectGuardPanelTip());
        }
        else
        {            
            AddAction(new Sequence(new Cocos2dParallel(action_list.ToArray()), 
                new SleepFor(30),
                new FunctionCall(() => Globals.canvasForMyMaze.TutorialEnd())));
        }
    }

    public void ThiefAboutToMove()
    {
        UnityEngine.Debug.Log("ThiefAboutToMove");
        currentThief.OutStealing();
        currentThief.transform.position = Globals.maze.chests[thiefIdx].locate.room.doors[0].GetFloorPos();
        currentThief.AimAtTargetChest(Globals.maze.chests[thiefIdx]);
    }

    Guard lastGuard;

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();        
    }

    public override void GuardDestroyed(Guard guard)
    {        
        Globals.canvasForMyMaze.enhanceDefenseUI.Open();
        Globals.canvasForMyMaze.ChangeMazeRoom(-guard.data.roomConsume);        
        base.GuardDestroyed(guard);
        Globals.canvasForMyMaze.CheckRoomFullUses();
        Globals.maze.guards.Remove(guard);
        Globals.self.UploadGuards();
        Globals.canvasForMyMaze.UpdateIncomeIntro();
    }

    public override void GuardChoosen(Guard guard)
    {
        base.GuardChoosen(guard);
        Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(false);
    }

    public override void GuardDropped(Guard guard)
    {        
		Globals.Assert (Globals.maze.draggingGuard == null);
        lastGuard = guard;

        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over && currentThief.currentAction != currentThief.lifeOver)
        {
            SyncWithChestData(Globals.self);
            Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, Globals.cameraMoveDuration);
            currentThief.InStealing();
        }
        else
        {
            Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(true);
            Globals.canvasForMyMaze.CheckRoomFullUses();
            Globals.canvasForMyMaze.UpdateIncomeIntro();
        }
        base.GuardDropped(guard);
    }

    public override void OneChestGoldAllLost(Chest chest)
    {
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position,Globals.cameraMoveDuration);
        currentThief.ShowTipRetry();
        currentThief.moving.canMove = false;
        currentThief.transform.localEulerAngles = UnityEngine.Vector3.zero;
        StopAllGuards();
        base.OneChestGoldAllLost(chest);
    }

    public void Retry()
    {
        foreach(Guard guard in Globals.maze.guards)
        {
            if (lastGuard != guard)
            {
                guard.BeginPatrol();
            }           
        }
        Globals.canvasForMyMaze.ChangeMazeRoom(-lastGuard.data.roomConsume);
        DestroyObject(lastGuard.gameObject);
        
        
        Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(true);
        currentThief.targetChest.ResetGold();
        currentThief.HideTip();
        currentThief.currentAction = null;
        ThiefAboutToMove();
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position,Globals.cameraMoveDuration);
    }

    public override void MagicianLifeOver()
    {
		++thiefIdx;
		UnityEngine.Debug.Log ("GuardTakeThiefDown:" + thiefIdx.ToString());
        
        if (thiefIdx != 2)
        {
            GuardFullFillHisDutyTip = UnityEngine.GameObject.Instantiate(GuardFullFillHisDutyTipPrefab) as UnityEngine.GameObject;
            GuardFullFillHisDutyTip.transform.position = currentThief.transform.position;
            UnityEngine.UI.Button btn = GuardFullFillHisDutyTip.GetComponentInChildren<UnityEngine.UI.Button>();
            btn.onClick.AddListener(() => NextThief());
			Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position,
			                                         30);
        }
        else
        {
            Globals.canvasForMyMaze.Invoke("ShowTutorialEndMsgBox", 1.0f);
        }
        currentThief.Invoke("Vanish", 3.5f);
        base.MagicianLifeOver();
    }    

    public void NextThief()
    {
        DestroyObject(GuardFullFillHisDutyTip);
        CreateNextThief();
        currentThief.ShowTipAnotherThief();
    }

    public void MoreGuardBtnClicker()
    {
        currentThief.HideTip();
        ThiefAboutToMove();

        Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(true);
    }

    public UnityEngine.Vector3 AddSafeBox()
    {        
        SafeBoxData data = Globals.self.AddSafeBox();
        UnityEngine.Vector3 falling_pos = UnityEngine.Vector3.zero;
        foreach (Chest chest in Globals.maze.chests)
        {
            if(!chest.IsVisible())
            {                
                chest.SyncWithData(data);
                falling_pos = chest.transform.position;
                Globals.cameraFollowMagician.MoveToPoint(falling_pos, Globals.cameraMoveDuration);
                chest.Falling(chestFallingDuration);
                break;
            }            
        }
        
        PutCashInBox(Globals.self);
        return falling_pos;
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        AstarPath.CalculatePaths(AstarPath.threadInfos[0]);
        // 拖动的guard要在雾上面
        if (Globals.maze.choosenGuard != null)
        {
            Globals.maze.choosenGuard.spriteRenderer.sortingOrder = 10000;
        }
// 
//         foreach( Guard guard in Globals.maze.guards )
//         {
//             // 狗和蜘蛛放在迷雾上面
//             if ((guard.eye != null && guard.eye.gameObject.layer == 27) || guard.eye == null)
//             {
//                 guard.spriteRenderer.sortingOrder = 4999;
//                 UnityEngine.Vector3 pos = guard.transform.position;
//                 if (fogPlane.activeSelf)
//                 {
//                     guard.heightOriginCache = -0.6f;
//                 }
//                 else
//                 {
//                     guard.heightOriginCache = 0.0f;
//                 }
// 
//                 guard.transform.position = new UnityEngine.Vector3(pos.x, pos.y, (float)guard.heightOriginCache);
//             }
//         }
    }
}
