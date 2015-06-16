public class MyMazeLevelController : LevelController 
{            
    UnityEngine.GameObject thief_prefab;
    public TutorialThief currentThief;    
    UnityEngine.Vector2 piecesDropDuration = new UnityEngine.Vector2(100, 200);
    int chestFallingDuration = 50;
    float piecesFarDis = 20;
    bool ShowDroppingProcess = false;
    int thiefIdx = 0;

    UnityEngine.GameObject GuardFullFillHisDutyTipPrefab;
    UnityEngine.GameObject GuardFullFillHisDutyTip;
    
    public override void Awake()
    {        
        base.Awake();        
        UnityEngine.GameObject CanvasForMyMaze = UnityEngine.GameObject.Find("CanvasForMyMaze");
        mainCanvas = CanvasForMyMaze.GetComponent<UnityEngine.Canvas>();
        
        // 在TutorialLevel.InitMaze这个阶段，最开始“完整的迷宫”会闪一帧，      
        FindObjectOfType<CameraFollow>().camera.enabled = false;

        GuardFullFillHisDutyTipPrefab = UnityEngine.Resources.Load("UI/GuardFullFillHisDuty") as UnityEngine.GameObject;
        fogPlane.SetActive(false);
    }

    public override void BeforeGenerateMaze()
    {
        if (Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze)
        {
            Globals.self.currentMazeRandSeedCache = (int)System.DateTime.Now.Ticks;
            Globals.self.UpgradeMaze();
            Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze = false;
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
        Globals.cameraFollowMagician.camera.enabled = true;
        base.MazeFinished();
        
        Globals.canvasForMyMaze.ShowUnclickedGuardsRedPointsOnEnhanceDefBtn();

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

        Globals.canvasForMyMaze.CheckRoomFullUses();
        
        
        Globals.EnableAllInput(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(true);
        Globals.canvasForMagician.SetLifeVisible(false);
        Globals.canvasForMagician.SetPowerVisible(false);
        Globals.canvasForMagician.HideTricksPanel();
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();

        if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
        {
            // 上次教程未完的时候放下来的守卫
            Globals.maze.ClearGuards();

            thief_prefab = UnityEngine.Resources.Load("Avatar/Tutorial_Thief") as UnityEngine.GameObject;
            //Globals.selectGuard.EnableBtns(false);            
            foreach (Cell cell in Globals.maze.EveryCells)
            {
                if ((cell.chest != null) || cell == Globals.maze.entryOfMaze)
                {
                    cell.HideEverythingExceptFloor();
                }
                else
                {
                    UnityEngine.SpriteRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.SpriteRenderer>();
                    foreach (UnityEngine.SpriteRenderer renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }
            }            

            Globals.canvasForMyMaze.TipBuyHereAsHome.BeginMove(Globals.uiMoveAndScaleDuration);
        }
        else
        {
            SyncWithChestData(Globals.self);
            
            Globals.Assert(Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over);
            Globals.canvasForMyMaze.TutorialEnd();
        }
        
        for (int idx = 0; idx < Globals.self.guardsHired.Count; ++idx)
        {
            System.String guardname = Globals.self.guardsHired[idx];
            Globals.GetGuardData(guardname).hired = true;
        }
    }

    public override void MoneyFull(bool full)
    {
        base.MoneyFull(full);
        Globals.canvasForMyMaze.add_box_tip_pointer.transform.parent.gameObject.SetActive(full);
        if(full)
        {
            Globals.canvasForMyMaze.add_box_tip_pointer.ClearAllActions();
            Globals.canvasForMyMaze.add_box_tip_pointer.Blink();
        }        
    }

    public void ChestFallingStart()
    {        
        AddAction(new Sequence(
            new SleepFor(10), new FunctionCall(() => AddSafeBox()),
            new SleepFor(chestFallingDuration + 30), new FunctionCall(() => AddSafeBox()),
            new SleepFor(chestFallingDuration + 30), new FunctionCall(() => ChestsFallingEnd())));
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
        StartCoroutine(MazePieces());
    }

    System.Collections.IEnumerator MazePieces()
    {
        System.Collections.Generic.List<Cocos2dAction> action_list = new System.Collections.Generic.List<Cocos2dAction>();
        foreach (Cell cell in Globals.maze.EveryCells)
        {
            if (cell.GetFloor() == null)
            {
                continue;
            }
            UnityEngine.SpriteRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.SpriteRenderer>();
            foreach (UnityEngine.SpriteRenderer renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    if (ShowDroppingProcess)
                    {                        
                        UnityEngine.Vector3 dir = UnityEngine.Vector3.zero;
                        double directionRatio = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (directionRatio < 0.5f)
                        {
                            dir.x = piecesFarDis;
                        }
                        else
                        {
                            dir.y = piecesFarDis;
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
                    
                    renderer.enabled = true;
                    if(ShowDroppingProcess)
                    {
                        //yield return new UnityEngine.WaitForSeconds(dropPieceTimeGap * 1.5f); 
                    }                    
                }                    
            }
        }

        Globals.maze.chests[0].AddAction(new Cocos2dParallel(action_list.ToArray()));

        if (ShowDroppingProcess)
        {
            yield return new UnityEngine.WaitForSeconds(7.0f);
        }

        ThiefAboutToMove();
        SleepThenCallFunction(Globals.uiMoveAndScaleDuration, ()=>Globals.canvasForMyMaze.ShowSelectGuardPanelTip());        
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
        Globals.canvasForMyMaze.CheckRoomFullUses();
    }

    public override void GuardDestroyed(Guard guard)
    {        
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
        Globals.canvasForMyMaze.ChangeMazeRoom(-guard.data.roomConsume);        
        base.GuardDestroyed(guard);
        Globals.canvasForMyMaze.CheckRoomFullUses();
        Globals.maze.guards.Remove(guard);
        Globals.self.UploadGuards();        
    }

    public override void GuardChoosen(Guard guard)
    {
        base.GuardChoosen(guard);
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(false);
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
            Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
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
        
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
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
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
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

    public override void Update()
    {
        base.Update();
        // 拖动的guard要在雾上面
        if (Globals.maze.choosenGuard != null)
        {
            Globals.maze.choosenGuard.spriteRenderer.sortingOrder = 10;
        }

        foreach( Guard guard in Globals.maze.guards )
        {
            // 狗和蜘蛛放在迷雾上面
            if ((guard.eye != null && guard.eye.gameObject.layer == 27) || guard.eye == null)
            {
                guard.spriteRenderer.sortingOrder = 10;
                UnityEngine.Vector3 pos = guard.transform.position;
                guard.heightOriginCache = -0.6f;
                guard.transform.position = new UnityEngine.Vector3(pos.x, pos.y, -0.6f);
            }
        }
    }
}
