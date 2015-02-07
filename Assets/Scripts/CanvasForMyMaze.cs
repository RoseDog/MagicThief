public class CanvasForMyMaze : Actor
{
    public MyMazeLevelController myMaze;
    UnityEngine.GameObject TipToClickCreateMaze;
    UnityEngine.GameObject SelectGuardPanelTip;
    public UnityEngine.UI.Button FingerImageToDragGuard;
    UnityEngine.Vector2 fingerImagePosCache;

    public EnhanceDefenseUI enhanceDefenseUI;
    UIMover ExitMazeBtn;
    
    public UnityEngine.UI.Text unclickedTargetCount;
    public UIMover TipBuyHereAsHome;
    public UIMover btnEnhanceDef;

    public UnityEngine.UI.Text RedPointsOnEnchanceDefBtn;    
    void Awake()
    {
        Globals.canvasForMyMaze = this;
        TipBuyHereAsHome = Globals.getChildGameObject<UIMover>(gameObject, "TipBuyHereAsHome");
        btnEnhanceDef = Globals.getChildGameObject<UIMover>(gameObject, "EnhanceDefenseBtn");
        btnEnhanceDef.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ShowEnhanceMazeUI());
        enhanceDefenseUI = Globals.getChildGameObject<EnhanceDefenseUI>(gameObject, "EnhanceDefenseUI");
        
//         TipToClickCreateMaze = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "TipToClickCreateMaze").gameObject;
//         TipToClickCreateMaze.SetActive(false);
//         SelectGuardPanelTip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "SelectGuardPanelTip").gameObject;
//         UnityEngine.UI.Button howToUseBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(SelectGuardPanelTip.gameObject, "HowToUse");
//         howToUseBtn.onClick.AddListener(() => HowToUseGuardBtnClicked());
//         SelectGuardPanelTip.gameObject.SetActive(false);
//         FingerImageToDragGuard = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "FingerImageToDragGuard");
//         FingerImageToDragGuard.gameObject.SetActive(false);
//        fingerImagePosCache = FingerImageToDragGuard.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
        ExitMazeBtn = Globals.getChildGameObject<UIMover>(gameObject, "ExitMazeBtn");
        ExitMazeBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ExitHomeMaze());
        
        
        unclickedTargetCount = UnityEngine.GameObject.Find("UnclickedCount").GetComponent<UnityEngine.UI.Text>();

        RedPointsOnEnchanceDefBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnEnchanceDefBtn").GetComponentInChildren<UnityEngine.UI.Text>();        
    }
	// Use this for initialization
	void Start () 
    {
        myMaze = (Globals.LevelController as MyMazeLevelController);

        UnityEngine.UI.Button chestDownBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(TipBuyHereAsHome.gameObject, "ChestDown");
        chestDownBtn.onClick.AddListener(() => myMaze.ChestFallingStart());

        enhanceDefenseUI.gameObject.SetActive(false);
	}
	
    public void ShowCreateMazeBtn()
    {
        myMaze.currentThief.HideTip();

        btnEnhanceDef.BeginMove(Globals.uiMoveAndScaleDuration);
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

    public void ShowEnhanceMazeUI()
    {
        btnEnhanceDef.gameObject.SetActive(false);
        enhanceDefenseUI.Open();
    }

    public void CreateMazeBtnClicked()
    {
        btnEnhanceDef.Goback(Globals.uiMoveAndScaleDuration);
        TipToClickCreateMaze.SetActive(false);
        Globals.cameraFollowMagician.MoveToPoint(UnityEngine.Vector3.zero,
            Globals.cameraFollowMagician.disOffset + new UnityEngine.Vector3(0.0f, 15.0f, -7.0f),
            Globals.cameraMoveDuration);
        myMaze.Invoke("StartDropPieces", Globals.cameraMoveDuration);
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
        if (SelectGuardPanelTip.activeSelf)
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

    void ShowTutorialEndMsgBox()
    {
        Globals.MessageBox(Globals.languageTable.GetText("property_is_safe_now"), () => TutorialEnd());
        if (!UnityEngine.Application.isMobilePlatform)
        {
            Globals.SaveMazeIniFile(Globals.iniFileName);
        }

        ++Globals.TutorialLevelIdx;
    }

    public void TutorialEnd()
    {
        //Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
        btnEnhanceDef.BeginMove(Globals.uiMoveAndScaleDuration);        
        ExitMazeBtn.BeginMove(Globals.uiMoveAndScaleDuration);
        //进入MyMaze，HomeExit上面的红点根据new targets来更新        
        Globals.UpdateUnclickedRedPointsText(unclickedTargetCount);
    }

    public void ExitHomeMaze()
    {
        Globals.asyncLoad.ToLoadSceneAsync("City");

        //Globals.selectGuard.mover.Goback(Globals.uiMoveAndScaleDuration);
        btnEnhanceDef.Goback(Globals.uiMoveAndScaleDuration);
        ExitMazeBtn.Goback(Globals.uiMoveAndScaleDuration);
    }

    public void ShowUnclickedGuardsRedPointsOnEnhanceDefBtn()
    {
        MazeLvData mazeData = Globals.mazeLvDatas[Globals.CurrentMazeLevel];
        if (!mazeData.playerEverClickGuards)
        {
            RedPointsOnEnchanceDefBtn.transform.parent.gameObject.SetActive(true);
            RedPointsOnEnchanceDefBtn.text = mazeData.guards.Count.ToString();            
        }
        else
        {
            RedPointsOnEnchanceDefBtn.transform.parent.gameObject.SetActive(false);
        }
    }
}
