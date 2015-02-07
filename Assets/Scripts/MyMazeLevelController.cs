public class MyMazeLevelController : LevelController 
{            
    UnityEngine.GameObject thief_prefab;
    UnityEngine.Vector3 cameraOffsetOnThief = new UnityEngine.Vector3(0, 17, -8);
    public TutorialThief currentThief;    
    UnityEngine.Vector2 piecesDropDuration = new UnityEngine.Vector2(1, 2);
    float chestFallingDuration = 0.3f;
    float piecesFarDis = 20;
    bool ShowDroppingProcess = true;
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
    }

    public override void BeforeGenerateMaze()
    {
        if(Globals.CurrentMazeLevel != 0)
        {
            Globals.ReadMazeIniFile(Globals.iniFileName, true);
        }
        
        if (Globals.canvasForMyMaze.enhanceDefenseUI.mazeInfo.isUpgradingMaze)
        {
            Globals.maze.randSeedCacheWhenEditLevel = (int)System.DateTime.Now.Ticks;
            UnityEngine.Random.seed = Globals.maze.randSeedCacheWhenEditLevel;            
        }
        base.BeforeGenerateMaze();
    }    

    public override void MazeFinished()
    {
        Globals.cameraFollowMagician.camera.enabled = true;
        base.MazeFinished();
        Globals.canvasForMyMaze.ShowUnclickedGuardsRedPointsOnEnhanceDefBtn();        
        
        cameraOffsetOnThief = Globals.cameraFollowMagician.disOffset * 0.5f;
        Globals.EnableAllInput(true);
        Globals.canvasForMagician.RoseNumberBg.SetActive(true);
        Globals.canvasForMagician.SetLifeVisible(false);
        Globals.canvasForMagician.HideTricksPanel();
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();

        if (Globals.safeBoxDatas.Count == 0)
        {
            AddSafeBox();
            AddSafeBox();
        }

        for(int idx=0; idx < Globals.safeBoxDatas.Count;++idx)
        {
            Globals.maze.chests[idx].data = Globals.safeBoxDatas[idx];
            Globals.maze.chests[idx].Visible(true);
        }        
        
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            // 上次教程未完的时候放下来的守卫
            Globals.maze.ClearGuards();

            thief_prefab = UnityEngine.Resources.Load("Avatar/Tutorial_Thief") as UnityEngine.GameObject;
            //Globals.selectGuard.EnableBtns(false);            
            foreach (Cell cell in Globals.maze.EveryCells)
            {                
                if (cell.chest != null || cell == Globals.maze.entryOfMaze)
                {
                    cell.HideEverythingExceptFloor();
                }
                else
                {
                    UnityEngine.MeshRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.MeshRenderer>();
                    foreach (UnityEngine.MeshRenderer renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }
            }            

            Globals.canvasForMyMaze.TipBuyHereAsHome.BeginMove(Globals.uiMoveAndScaleDuration);
        }
        else
        {
            Globals.Assert(Globals.TutorialLevelIdx == Globals.TutorialLevel.Over);
            Globals.canvasForMyMaze.TutorialEnd();
            // 如果没有守卫，那么应该是升级了。买已购买的守卫放在chest周围
            if(Globals.maze.guards.Count == 0)
            {                
                int room_idx = 0;
                for (int mazeLevelIdx = 0; mazeLevelIdx < Globals.mazeLvDatas.Count; ++mazeLevelIdx)
                {
                    MazeLvData mazeData = Globals.mazeLvDatas[mazeLevelIdx];
                    for (int idx = 0; idx < mazeData.guards.Count; ++idx)
                    {
                        GuardHireInfo guardInfo = mazeData.guards[idx];

                        if (guardInfo.hired)
                        {
                            MazeGenerate.Room room = Globals.maze.rooms[room_idx];

                            Guard guard = null;
                            if (guardInfo.name == "lamp")
                            {
                                guard = Globals.CreateGuard(guardInfo.name, Globals.maze.pathFinder.GetNearestUnwalkableNode(room.GetRandomRoomPosition()));
                            }
                            else
                            {
                                guard = Globals.CreateGuard(guardInfo.name, Globals.maze.pathFinder.GetNearestWalkableNode(room.GetRandomRoomPosition()));
                            }
                            guard.BeginPatrol();
                            room_idx++;
                            room_idx = room_idx % Globals.maze.rooms.Count;
                        }
                    }
                }
            }
        }
    }

    public void ChestFallingStart()
    {        
        StartCoroutine(ChestsFalling());
        Globals.canvasForMyMaze.TipBuyHereAsHome.Goback(Globals.uiMoveAndScaleDuration);
    }
    

    System.Collections.IEnumerator ChestsFalling()
    {
        foreach (Chest chest in Globals.maze.chests)
        {
            chest.Visible(true);
            if (ShowDroppingProcess)
            {
                chest.Falling(chestFallingDuration);
				yield return new UnityEngine.WaitForSeconds(chestFallingDuration + 0.5f);
			}                        
        }

		yield return new UnityEngine.WaitForSeconds(0.5f);

        CreateNextThief();
        currentThief.ShowTipToShowMazeBtn();
    }

    void CreateNextThief()
    {
        // 出现窃贼
        UnityEngine.GameObject thief = UnityEngine.GameObject.Instantiate(thief_prefab) as UnityEngine.GameObject;
        currentThief = thief.GetComponent<TutorialThief>();
        
        // 窃贼的目标
        currentThief.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        // 绑定提示信息按钮的响应事件
        currentThief.BtnToShowMazeBtn.onClick.AddListener(() => Globals.canvasForMyMaze.ShowCreateMazeBtn());
        currentThief.RetryBtn.onClick.AddListener(() => Retry());
        currentThief.MorGuardBtn.onClick.AddListener(() => MoreGuardBtnClicker());
        // 相机聚焦到窃贼身上        
        Globals.cameraFollowMagician.MoveToPoint(thief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
    }    

    

    void StartDropPieces()
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
            UnityEngine.MeshRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.MeshRenderer>();
            foreach (UnityEngine.MeshRenderer renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    if (ShowDroppingProcess)
                    {                        
                        UnityEngine.Vector3 dir = UnityEngine.Vector3.zero;
                        float directionRatio = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (directionRatio < 0.33f)
                        {
                            dir.x = piecesFarDis;
                        }
                        else if (directionRatio > 0.33f && directionRatio < 0.66f)
                        {
                            dir.y = piecesFarDis;
                        }
                        else
                        {
                            dir.z = piecesFarDis;
                        }

                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                        {
                            dir *= -1.0f;
                        }

                        float droppingDuration = UnityEngine.Random.Range(piecesDropDuration.x, piecesDropDuration.y);
                        float waitingDuration = UnityEngine.Random.Range(piecesDropDuration.x, piecesDropDuration.y);
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
            yield return new UnityEngine.WaitForSeconds(piecesDropDuration.y * 2);
        }

        ThiefAboutToMove();
        Globals.canvasForMyMaze.Invoke("ShowSelectGuardPanelTip", Globals.uiMoveAndScaleDuration);
    }

    public void ThiefAboutToMove()
    {        
        currentThief.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        currentThief.AimAtTargetChest(Globals.maze.chests[thiefIdx]);
        Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
    }

    Guard lastGuard;

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
        CancelInvoke("FingerDraggingAnimation");
        //Globals.canvasForMyMaze.FingerImageToDragGuard.gameObject.SetActive(false);
    }

    public override void GuardDestroyed(Guard guard)
    {
        if(Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
			if(!IsInvoking("FingerDraggingAnimation"))
			{
				InvokeRepeating("FingerDraggingAnimation", 0, 2.5f);
                //Globals.canvasForMyMaze.FingerImageToDragGuard.gameObject.SetActive(true);
			}
        }
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
        base.GuardDestroyed(guard);
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

        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
            Globals.selectGuard.mover.Goback(Globals.uiMoveAndScaleDuration);
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
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
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
        DestroyObject(lastGuard.gameObject);
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
        currentThief.targetChest.ResetGold();
        currentThief.HideTip();
        ThiefAboutToMove();
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
        Globals.canvasForMyMaze.HowToUseGuardBtnClicked();
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
			                                         Globals.cameraFollowMagician.disOffset,
			                                         0.7f);
        }
        else
        {
            Globals.canvasForMyMaze.Invoke("ShowTutorialEndMsgBox", 1.0f);
        }
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
        Globals.canvasForMyMaze.HowToUseGuardBtnClicked();
    }

    public void AddSafeBox()
    {
        SafeBoxData data = new SafeBoxData();
        Globals.safeBoxDatas.Add(data);
        foreach (Chest chest in Globals.maze.chests)
        {
            if(!chest.IsVisible())
            {
                chest.data = data;
                chest.Visible(true);
                chest.Falling(chestFallingDuration);
                break;
            }            
        }
        
        PutCashInBox();        
    }

    public void PutCashInBox()
    {
        // 给safebox分配金钱
        float cash = Globals.cashAmount;
        int box_count = Globals.safeBoxDatas.Count;
        while (box_count > 0)
        {
            float average_cash = cash / box_count;
            SafeBoxData box_data = Globals.safeBoxDatas[box_count - 1];
            float cash_limit = Globals.safeBoxLvDatas[box_data.Lv].capacity;
            float cash_put_in = 0;
            if (cash_limit >= average_cash)
            {
                cash_put_in = average_cash;
            }
            else
            {
                cash_put_in = cash_limit;
            }
            cash -= cash_put_in;
            box_data.cashInBox = (int)cash_put_in;
            --box_count;
        }

        Globals.cashAmount = Globals.AccumulateCashInBox();
        Globals.canvasForMagician.cashNumber.SetNumber(Globals.cashAmount);
    }
}
