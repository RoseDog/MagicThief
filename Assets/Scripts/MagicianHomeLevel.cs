public class MagicianHomeLevel : LevelController 
{
    UnityEngine.GameObject CanvasForHome;
    UIMover TipBuyHereAsHome;
    UIMover btnCreateMaze;
    UnityEngine.GameObject TipToClickCreateMaze;
    UnityEngine.GameObject SelectGuardPanelTip;
    UnityEngine.UI.Button FingerImageToDragGuard;
    UIMover ExitHomeMazeBtn;
    UnityEngine.GameObject thief_prefab;
    UnityEngine.Vector3 cameraOffsetOnThief = new UnityEngine.Vector3(0, 17, -8);
    TutorialThief currentThief;    
    UnityEngine.Vector2 piecesDropDuration = new UnityEngine.Vector2(1, 2);
    float chestFallingDuration = 0.3f;
    float piecesFarDis = 20;
    bool ShowDroppingProcess = true;
    int thiefIdx = 0;
    UnityEngine.Vector2 fingerImagePosCache;
    UnityEngine.GameObject GuardFullFillHisDutyTipPrefab;
    UnityEngine.GameObject GuardFullFillHisDutyTip;
    public UnityEngine.UI.Text unclickedTargetCount;
    public override void Awake()
    {        
        mazeIniFileName = "MagicianHome";        
        CanvasForHome = UnityEngine.GameObject.Find("CanvasForHome");
        TipBuyHereAsHome = Globals.getChildGameObject<UIMover>(CanvasForHome, "TipBuyHereAsHome");
        UnityEngine.UI.Button chestDownBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(TipBuyHereAsHome.gameObject, "ChestDown");
        chestDownBtn.onClick.AddListener(()=>ChestFallingStart());
        btnCreateMaze = Globals.getChildGameObject<UIMover>(CanvasForHome, "CreateMaze");
        TipToClickCreateMaze = Globals.getChildGameObject<UnityEngine.RectTransform>(CanvasForHome, "TipToClickCreateMaze").gameObject;
        TipToClickCreateMaze.SetActive(false);
        SelectGuardPanelTip = Globals.getChildGameObject<UnityEngine.RectTransform>(CanvasForHome, "SelectGuardPanelTip").gameObject;
        SelectGuardPanelTip.gameObject.SetActive(false);
        FingerImageToDragGuard = Globals.getChildGameObject<UnityEngine.UI.Button>(CanvasForHome, "FingerImageToDragGuard");
        FingerImageToDragGuard.gameObject.SetActive(false);
        ExitHomeMazeBtn = Globals.getChildGameObject<UIMover>(CanvasForHome, "ExitHomeMaze");
        ExitHomeMazeBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ExitHomeMaze());
        fingerImagePosCache = FingerImageToDragGuard.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
        GuardFullFillHisDutyTipPrefab = UnityEngine.Resources.Load("UI/GuardFullFillHisDuty") as UnityEngine.GameObject;
        unclickedTargetCount = UnityEngine.GameObject.Find("UnclickedCount").GetComponent<UnityEngine.UI.Text>();
        base.Awake();
    }
    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.EnableAllInput(true);
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            // 上次教程未完的时候放下来的守卫
            Globals.maze.ClearGuards();

            thief_prefab = UnityEngine.Resources.Load("Avatar/Tutorial_Thief") as UnityEngine.GameObject;
            Globals.selectGuard.EnableBtns(false);            
            foreach (Cell cell in Globals.maze.EveryCells)
            {
                UnityEngine.GameObject floor = cell.GetFloor();
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
            foreach (Chest chest in Globals.maze.chests)
            {
                chest.Visible(false);
            }

            TipBuyHereAsHome.BeginMove(Globals.uiMoveAndScaleDuration);
        }
        else
        {
            Globals.Assert(Globals.TutorialLevelIdx == Globals.TutorialLevel.Over);
            TutorialEnd();
        }
    }

    public void ChestFallingStart()
    {        
        StartCoroutine(ChestsFalling());
        TipBuyHereAsHome.Goback(Globals.uiMoveAndScaleDuration);
    }
    

    System.Collections.IEnumerator ChestsFalling()
    {
        foreach (Chest chest in Globals.maze.chests)
        {
            chest.Visible(true);
            if (ShowDroppingProcess)
            {
                chest.Falling(chestFallingDuration);
                yield return new UnityEngine.WaitForSeconds(chestFallingDuration);
            }                        
        }

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
        currentThief.BtnToShowMazeBtn.onClick.AddListener(() => ShowCreateMazeBtn());
        currentThief.RetryBtn.onClick.AddListener(() => Retry());
        currentThief.MorGuardBtn.onClick.AddListener(() => MoreGuardBtnClicker());
        // 相机聚焦到窃贼身上
        Globals.cameraFollowMagician.MoveToPoint(thief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);        
    }    

    public void ShowCreateMazeBtn()
    {
        currentThief.HideTip();

        btnCreateMaze.BeginMove(Globals.uiMoveAndScaleDuration);
        Invoke("ShowTipToClickCreateMaze", Globals.uiMoveAndScaleDuration);        
    }

    public void ShowTipToClickCreateMaze()
    {
        TipToClickCreateMaze.SetActive(true);
        TipToClickCreateMaze.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new Sequence(
            new ScaleTo(TipToClickCreateMaze.transform, new UnityEngine.Vector3(1.2f, 1.2f, 1.2f), Globals.uiMoveAndScaleDuration / 2),
            new ScaleTo(TipToClickCreateMaze.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration / 4)));
    }

    public void CreateMazeBtnClicked()
    {
        btnCreateMaze.Goback(Globals.uiMoveAndScaleDuration);
        TipToClickCreateMaze.SetActive(false);
        Globals.cameraFollowMagician.MoveToPoint(UnityEngine.Vector3.zero,
            Globals.cameraFollowMagician.disOffset + new UnityEngine.Vector3(0.0f, 15.0f, -7.0f),
            Globals.cameraMoveDuration);
        Invoke("StartDropPieces", Globals.cameraMoveDuration);        
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
        Invoke("ShowSelectGuardPanelTip", Globals.uiMoveAndScaleDuration);
    }

    public void ThiefAboutToMove()
    {        
        currentThief.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        currentThief.AimAtTargetChest(Globals.maze.chests[thiefIdx]);
        Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
    }

    public void ShowSelectGuardPanelTip()
    {
        SelectGuardPanelTip.SetActive(true);
        SelectGuardPanelTip.transform.localScale = UnityEngine.Vector3.zero;

        AddAction(new Sequence(
            new ScaleTo(SelectGuardPanelTip.transform, new UnityEngine.Vector3(1.2f, 1.2f, 1.2f), Globals.uiMoveAndScaleDuration / 2),
            new ScaleTo(SelectGuardPanelTip.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration / 4)));
    }

    public void HowToUseGuardBtnClicked()
    {
        if (SelectGuardPanelTip.active)
        {
            AddAction(new ScaleTo(SelectGuardPanelTip.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration));
            Globals.selectGuard.EnableBtns(true);
        }                
        FingerImageToDragGuard.gameObject.SetActive(true);        
        InvokeRepeating("FingerDraggingAnimation", 0, 2.5f);
    }

    void FingerDraggingAnimation()
    {
        FingerImageToDragGuard.GetComponent<UnityEngine.RectTransform>().anchoredPosition = fingerImagePosCache;
        FingerImageToDragGuard.GetComponent<Actor>().ClearAllActions();
        FingerImageToDragGuard.GetComponent<Actor>().AddAction(new MoveTo(FingerImageToDragGuard.transform, new UnityEngine.Vector2(0.0f, 300.0f), 2.0f, true));        
    }

    Guard lastGuard;

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
        CancelInvoke("FingerDraggingAnimation");
        FingerImageToDragGuard.gameObject.SetActive(false);
    }

    public override void GuardDestroyed(Guard guard)
    {
        if(Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            InvokeRepeating("FingerDraggingAnimation", 0, 2.5f);
            FingerImageToDragGuard.gameObject.SetActive(true);
        }        
        base.GuardDestroyed(guard);
    }    

    public override void GuardDropped(Guard guard)
    {        
        lastGuard = guard;

        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
            Globals.selectGuard.mover.Goback(Globals.uiMoveAndScaleDuration);
            currentThief.InStealing();
        }                        
        base.GuardDropped(guard);
    }

    public override void GoldAllLost(Chest chest)
    {
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
        currentThief.ShowTipRetry();
        currentThief.moving.canMove = false;
        currentThief.transform.localEulerAngles = UnityEngine.Vector3.zero;        
        base.GoldAllLost(chest);
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

        currentThief.targetChest.ResetGold();
        currentThief.HideTip();
        ThiefAboutToMove();
        Globals.cameraFollowMagician.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, Globals.cameraMoveDuration);
        HowToUseGuardBtnClicked();
    }

    public void GuardTakeThiefDown()
    {
        ++thiefIdx;
        if (thiefIdx != 1)
        {
            GuardFullFillHisDutyTip = UnityEngine.GameObject.Instantiate(GuardFullFillHisDutyTipPrefab) as UnityEngine.GameObject;
            GuardFullFillHisDutyTip.transform.position = currentThief.transform.position;
            UnityEngine.UI.Button btn = GuardFullFillHisDutyTip.GetComponentInChildren<UnityEngine.UI.Button>();            
            btn.onClick.AddListener(() => NextThief());                       
        }
        else
        {
            Invoke("ShowTutorialEndMsgBox", 1.0f);
        }
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
        HowToUseGuardBtnClicked();
    }

    void ShowTutorialEndMsgBox()
    {
        Globals.canvasForMagician.MessageBox("你的财产暂时安全了", () => TutorialEnd());
        Globals.SaveMazeIniFile(mazeIniFileName);
        ++Globals.TutorialLevelIdx;

        System.Collections.Generic.List<IniFile> newAchives = new System.Collections.Generic.List<IniFile>() { 
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition1\n" + Globals.TargetBuildingDescriptionKey + "=猫眼三姐妹"), 
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition2\n" + Globals.TargetBuildingDescriptionKey + "=扑克脸"), 
                    IniFile.ReadIniText(Globals.PosHolderKey + "=BuildingPosition3\n" + Globals.TargetBuildingDescriptionKey + "=现金眼" )};
        Globals.AddNewTargetBuildingAchives(newAchives);
    }

    void TutorialEnd()
    {
        Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
        btnCreateMaze.BeginMove(Globals.uiMoveAndScaleDuration);
        ExitHomeMazeBtn.BeginMove(Globals.uiMoveAndScaleDuration);
        //进入MyMaze HomeExit上面的红点根据new targets来更新        
        Globals.UpdateUnclickedRedPointsText(unclickedTargetCount);        
    }

    public void ExitHomeMaze()
    {
        Globals.asyncLoad.ToLoadSceneAsync("City");

        Globals.selectGuard.mover.Goback(Globals.uiMoveAndScaleDuration);
        btnCreateMaze.Goback(Globals.uiMoveAndScaleDuration);
        ExitHomeMazeBtn.Goback(Globals.uiMoveAndScaleDuration);
    }
}
