public class MagicianHomeLevel : LevelController 
{
    UnityEngine.GameObject CanvasForHome;
    UnityEngine.GameObject TipBuyHereAsHome;
    UnityEngine.GameObject btnCreateMaze;
    UnityEngine.GameObject TipToClickCreateMaze;
    UnityEngine.GameObject SelectGuardPanelTip;
    UnityEngine.UI.Button FingerImageToDragGuard;
    UnityEngine.GameObject thief_prefab;
    UnityEngine.Vector3 cameraOffsetOnThief = new UnityEngine.Vector3(0, 17, -8);
    TutorialThief currentThief;
    float cameraMoveDuration = 0.3f;
    float uiMoveAndScaleDuration = 0.3f;
    float dropPieceTimeGap = 0.03f;
    float fallingDuration = 0.03f;
    bool ShowDroppingProcess = false;
    int thiefIdx = 0;
    UnityEngine.Vector2 fingerImagePosCache;
    UnityEngine.GameObject GuardFullFillHisDutyTipPrefab;
    UnityEngine.GameObject GuardFullFillHisDutyTip;
    public override void Awake()
    {
        CanvasForHome = UnityEngine.GameObject.Find("CanvasForHome");
        TipBuyHereAsHome = Globals.getChildGameObject(CanvasForHome, "TipBuyHereAsHome");
        btnCreateMaze = Globals.getChildGameObject(CanvasForHome, "CreateMaze");
        TipToClickCreateMaze = Globals.getChildGameObject<UnityEngine.RectTransform>(CanvasForHome, "TipToClickCreateMaze").gameObject;
        TipToClickCreateMaze.SetActive(false);
        SelectGuardPanelTip = Globals.getChildGameObject<UnityEngine.RectTransform>(CanvasForHome, "SelectGuardPanelTip").gameObject;
        SelectGuardPanelTip.gameObject.SetActive(false);
        FingerImageToDragGuard = Globals.getChildGameObject<UnityEngine.UI.Button>(CanvasForHome, "FingerImageToDragGuard");
        FingerImageToDragGuard.gameObject.SetActive(false);
        fingerImagePosCache = FingerImageToDragGuard.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
        GuardFullFillHisDutyTipPrefab = UnityEngine.Resources.Load("UI/GuardFullFillHisDuty") as UnityEngine.GameObject;
        base.Awake();
    }
    public override void MazeFinished()
    {
        base.MazeFinished();
        thief_prefab = UnityEngine.Resources.Load("Avatar/Tutorial_Thief") as UnityEngine.GameObject;

        Globals.canvasForMagician.SetLifeVisible(false);
        HideUI(Globals.selectGuard.gameObject);
        Globals.selectGuard.EnableBtns(false);
        HideUI(btnCreateMaze);                
        Globals.canvasForMagician.tutorialText.gameObject.SetActive(false);
        Globals.map.SetRestrictToCamera(Globals.cameraForDefender);
        Globals.map.RegistGuardArrangeEvent();

        foreach(Cell cell in Globals.map.EveryCells)
        {
            UnityEngine.GameObject floor = cell.GetFloor();            
            if(cell.chest != null)
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
        foreach (Chest chest in chests)
        {
            chest.Visible(false);
        }
    }

    public void ChestFallingStart()
    {        
        StartCoroutine(ChestsFalling());        
        TipBuyHereAsHome.SetActive(false);
    }
    

    System.Collections.IEnumerator ChestsFalling()
    {        
        foreach(Chest chest in chests)
        {
            chest.Visible(true);
            if (ShowDroppingProcess)
            {
                chest.Falling(fallingDuration);
                yield return new UnityEngine.WaitForSeconds(fallingDuration);
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
        currentThief.gameObject.layer = 0;
        // 窃贼的目标
        currentThief.transform.position = Globals.map.entryOfMaze.GetFloorPos();
        // 绑定提示信息按钮的响应事件
        currentThief.BtnToShowMazeBtn.onClick.AddListener(() => ShowCreateMazeBtn());
        currentThief.RetryBtn.onClick.AddListener(() => Retry());
        currentThief.MorGuardBtn.onClick.AddListener(() => MoreGuardBtnClicker());
        // 相机聚焦到窃贼身上
        Globals.cameraForDefender.MoveToPoint(thief.transform.position, cameraOffsetOnThief, cameraMoveDuration);        
    }    

    public void ShowCreateMazeBtn()
    {
        currentThief.HideTip();

        ShowUI(btnCreateMaze);                
        Invoke("ShowTipToClickCreateMaze", uiMoveAndScaleDuration);        
    }

    public void ShowTipToClickCreateMaze()
    {
        TipToClickCreateMaze.SetActive(true);
        TipToClickCreateMaze.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new ScaleTo(TipToClickCreateMaze.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), uiMoveAndScaleDuration));
    }

    public void CreateMazeBtnClicked()
    {
        HideUI(btnCreateMaze.gameObject);
        TipToClickCreateMaze.SetActive(false);
        Globals.cameraForDefender.MoveToPoint(UnityEngine.Vector3.zero, 
            Globals.cameraForDefender.disOffset + new UnityEngine.Vector3(0.0f, 15.0f, -7.0f), 
            cameraMoveDuration);
        Invoke("StartDropPieces", cameraMoveDuration);        
    }

    void StartDropPieces()
    {
        StartCoroutine(MazePieces());
    }

    System.Collections.IEnumerator MazePieces()
    {
        foreach (Cell cell in Globals.map.EveryCells)
        {
            UnityEngine.MeshRenderer[] renderers = cell.GetComponentsInChildren<UnityEngine.MeshRenderer>();
            foreach (UnityEngine.MeshRenderer renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    UnityEngine.Vector3 dir = UnityEngine.Vector3.zero;
                    float directionRatio = UnityEngine.Random.Range(0.0f, 1.0f);
                    if(directionRatio < 0.33f)
                    {
                        dir.x = 10;
                    }
                    else if (directionRatio > 0.33f && directionRatio < 0.66f)
                    {
                        dir.y = 10;
                    }
                    else
                    {
                        dir.z = 10;
                    }

                    if(UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        dir *= -1.0f;
                    }
                    if (ShowDroppingProcess)
                    {
                        chests[0].AddAction(new MoveTo(renderer.transform, renderer.transform.localPosition, dropPieceTimeGap));
                        renderer.transform.localPosition += dir;
                    }                    
                    
                    renderer.enabled = true;
                    if(ShowDroppingProcess)
                    {
                        yield return new UnityEngine.WaitForSeconds(dropPieceTimeGap * 1.5f); 
                    }                    
                }                    
            }
        }

        if (ShowDroppingProcess)
        {
            yield return new UnityEngine.WaitForSeconds(dropPieceTimeGap * 1.5f);
        }

        ThiefAboutToMove();        
        Invoke("ShowSelectGuardPanelTip", uiMoveAndScaleDuration);
    }

    public void ThiefAboutToMove()
    {        
        currentThief.transform.position = Globals.map.entryOfMaze.GetFloorPos();
        currentThief.AimAtTargetChest(chests[thiefIdx]);        
        ShowUI(Globals.selectGuard.gameObject);        
    }

    public void ShowUI(UnityEngine.GameObject ui)
    {
        ui.SetActive(true);
        UnityEngine.RectTransform uiTransform = ui.transform as UnityEngine.RectTransform;
        UnityEngine.Vector3 to = uiTransform.anchoredPosition;
        if (uiTransform.pivot.x < UnityEngine.Mathf.Epsilon)
        {
            to = new UnityEngine.Vector3(UnityEngine.Mathf.Abs(to.x) - uiTransform.sizeDelta.x, to.y, to.z);
        }
        else
        {
            to = new UnityEngine.Vector3(to.x, UnityEngine.Mathf.Abs(to.y) - uiTransform.sizeDelta.y, to.z);
        }
        AddAction(new MoveTo(uiTransform, to, uiMoveAndScaleDuration, true));
    }

    public void HideUI(UnityEngine.GameObject ui)
    {
        ui.SetActive(true);
        UnityEngine.RectTransform uiTransform = ui.transform as UnityEngine.RectTransform;
        UnityEngine.Vector3 to = uiTransform.anchoredPosition;
        if (uiTransform.pivot.x < UnityEngine.Mathf.Epsilon)
        {
            to = new UnityEngine.Vector3(-uiTransform.sizeDelta.x - to.x, to.y, to.z);
        }
        else
        {
            to = new UnityEngine.Vector3(to.x, -uiTransform.sizeDelta.y - to.y, to.z);
        }

        AddAction(new MoveTo(uiTransform, to, uiMoveAndScaleDuration, true));
    }


    public void ShowSelectGuardPanelTip()
    {
        SelectGuardPanelTip.SetActive(true);
        SelectGuardPanelTip.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new ScaleTo(SelectGuardPanelTip.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), uiMoveAndScaleDuration));
    }

    public void HowToUseGuardBtnClicked()
    {
        if (SelectGuardPanelTip.active)
        {
            AddAction(new ScaleTo(SelectGuardPanelTip.transform, UnityEngine.Vector3.zero, uiMoveAndScaleDuration));
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
    System.Collections.Generic.List<Guard> guardsOnMap = new System.Collections.Generic.List<Guard>();    

    public override void GuardDropped(Guard guard)
    {
        guardsOnMap.Add(guard);
        lastGuard = guard;

        CancelInvoke("FingerDraggingAnimation"); 
        Globals.cameraForDefender.MoveToPoint(currentThief.transform.position,cameraOffsetOnThief,cameraMoveDuration);
        FingerImageToDragGuard.gameObject.SetActive(false);
        HideUI(Globals.selectGuard.gameObject);
        currentThief.moving.canMove = true;
        currentThief.gameObject.layer = 11;
        base.GuardDropped(guard);
    }

    public override void GoldAllLost(Chest chest)
    {
        Globals.cameraForDefender.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, cameraMoveDuration);
        currentThief.ShowTipRetry();
        currentThief.moving.canMove = false;
        currentThief.transform.localEulerAngles = UnityEngine.Vector3.zero;
        StopAllGuards();
        base.GoldAllLost(chest);
    }

    public void Retry()
    {
        foreach(Guard guard in guardsOnMap)
        {
            if (lastGuard == guard)
            {
                guardsOnMap.Remove(guard);
            }
            else
            {
                guard.BeginPatrol();                
            }
        }
        DestroyObject(lastGuard.gameObject);

        currentThief.targetChest.ResetGold();
        currentThief.HideTip();
        ThiefAboutToMove();
        Globals.cameraForDefender.MoveToPoint(currentThief.transform.position, cameraOffsetOnThief, cameraMoveDuration);
        HowToUseGuardBtnClicked();
    }

    public void GuardTakeThiefDown()
    {
        ++thiefIdx;
        if (thiefIdx != 3)
        {
            GuardFullFillHisDutyTip = UnityEngine.GameObject.Instantiate(GuardFullFillHisDutyTipPrefab) as UnityEngine.GameObject;
            GuardFullFillHisDutyTip.transform.position = currentThief.transform.position;
            UnityEngine.UI.Button btn = GuardFullFillHisDutyTip.GetComponentInChildren<UnityEngine.UI.Button>();            
            btn.onClick.AddListener(() => NextThief());                       
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

    void StopAllGuards()
    {
        foreach (Guard guard in guardsOnMap)
        {
            guard.StopAttacking();
        }
    }
}
