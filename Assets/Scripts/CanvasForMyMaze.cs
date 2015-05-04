public class CanvasForMyMaze : Actor
{
    public MyMazeLevelController myMaze;
    UnityEngine.GameObject TipToClickCreateMaze;
    UnityEngine.UI.Text tip;
    public UnityEngine.UI.Button FingerImageToDragGuard;
    UnityEngine.Vector2 fingerImagePosCache;

    public EnhanceDefenseUI enhanceDefenseUI;    
    UIMover ExitMazeBtn;
    
    public UnityEngine.UI.Text unclickedCityEventCount;
    public UIMover TipBuyHereAsHome;
    public UIMover btnEnhanceDef;

    public UnityEngine.UI.Text RedPointsOnEnchanceDefBtn;

    public UnityEngine.GameObject MazeRoomNumerBg;
    public LifeNumber MazeRoomNumber;
    public int roomConsumed;
    public UnityEngine.GameObject room_not_full_used;
    public override void Awake()
    {
        base.Awake();
        Globals.canvasForMyMaze = this;
        TipBuyHereAsHome = Globals.getChildGameObject<UIMover>(gameObject, "TipBuyHereAsHome");
        btnEnhanceDef = Globals.getChildGameObject<UIMover>(gameObject, "EnhanceDefenseBtn");
        btnEnhanceDef.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ShowEnhanceMazeUI());
        enhanceDefenseUI = Globals.getChildGameObject<EnhanceDefenseUI>(gameObject, "EnhanceDefenseUI");        
        
        TipToClickCreateMaze = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "TipToClickCreateMaze").gameObject;        
        tip = Globals.getChildGameObject(TipToClickCreateMaze, "Tip").GetComponent<UnityEngine.UI.Text>();        
        TipToClickCreateMaze.SetActive(false);
//         SelectGuardPanelTip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "SelectGuardPanelTip").gameObject;
//         UnityEngine.UI.Button howToUseBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(SelectGuardPanelTip.gameObject, "HowToUse");
//         howToUseBtn.onClick.AddListener(() => HowToUseGuardBtnClicked());
//         SelectGuardPanelTip.gameObject.SetActive(false);
//         FingerImageToDragGuard = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "FingerImageToDragGuard");
//         FingerImageToDragGuard.gameObject.SetActive(false);
//        fingerImagePosCache = FingerImageToDragGuard.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
        ExitMazeBtn = Globals.getChildGameObject<UIMover>(gameObject, "ExitMazeBtn");
        ExitMazeBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => GoToCity());

        unclickedCityEventCount = Globals.getChildGameObject<UnityEngine.UI.Text>(ExitMazeBtn.gameObject, "UnclickedCount");

        RedPointsOnEnchanceDefBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnEnchanceDefBtn").GetComponentInChildren<UnityEngine.UI.Text>();

        MazeRoomNumerBg = Globals.getChildGameObject(btnEnhanceDef.gameObject, "MazeRoomNumerBg");
        MazeRoomNumber = Globals.getChildGameObject<LifeNumber>(MazeRoomNumerBg, "MazeRoomNumber");

        room_not_full_used = Globals.getChildGameObject(gameObject, "room_not_full_used");
        room_not_full_used.gameObject.SetActive(false);
    }
	// Use this for initialization
	public override void Start () 
    {
        base.Start();
        myMaze = (Globals.LevelController as MyMazeLevelController);

        UnityEngine.UI.Button chestDownBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(TipBuyHereAsHome.gameObject, "ChestDown");
        chestDownBtn.onClick.AddListener(() => myMaze.ChestFallingStart());

        enhanceDefenseUI.gameObject.SetActive(false);
	}
	
    public void ShowCreateMazeBtn()
    {
        myMaze.currentThief.HideTip();

        btnEnhanceDef.BeginMove(Globals.uiMoveAndScaleDuration);
        SleepThenCallFunction(Globals.uiMoveAndScaleDuration, ()=>ShowTipToClickCreateMaze());
    }

    public void ShowTipToClickCreateMaze()
    {        
        TipToClickCreateMaze.SetActive(true);
        TipToClickCreateMaze.transform.localScale = UnityEngine.Vector3.zero;        
        TipToClickCreateMaze.GetComponent<UnityEngine.CanvasGroup>().ignoreParentGroups = true;
        TipToClickCreateMaze.GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = false;
        TipToClickCreateMaze.GetComponent<UnityEngine.CanvasGroup>().interactable = false;
        tip.GetComponent<UnityEngine.CanvasGroup>().ignoreParentGroups = true;
        tip.GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = false;
        tip.GetComponent<UnityEngine.CanvasGroup>().interactable = false;
        AddAction(new Sequence(
            new ScaleTo(TipToClickCreateMaze.transform, new UnityEngine.Vector3(1.2f, 1.2f, 1.2f), Globals.uiMoveAndScaleDuration / 2),
            new ScaleTo(TipToClickCreateMaze.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration / 4)));
    }

    public void ShowEnhanceMazeUI()
    {
        btnEnhanceDef.gameObject.SetActive(false);
        enhanceDefenseUI.Open();
        room_not_full_used.SetActive(false);
    }

    public void InitMazeBtnClicked()
    {
        btnEnhanceDef.Goback(Globals.uiMoveAndScaleDuration);
        TipToClickCreateMaze.SetActive(false);
//         Globals.cameraFollowMagician.MoveToPoint(UnityEngine.Vector3.zero,
//             Globals.cameraMoveDuration);
        myMaze.StartDropPieces();
    }

    public void ShowSelectGuardPanelTip()
    {
        ShowCreateMazeBtn();
        Globals.languageTable.SetText(tip, "place_guard_in_maze_to_protect_your_property");        
    }

    void ShowTutorialEndMsgBox()
    {
        Globals.MessageBox("property_is_safe_now", () => TutorialEnd());
        Globals.self.AdvanceTutorial();
    }

    public void TutorialEnd()
    {
        //Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
        DestroyObject(Globals.canvasForMyMaze.TipToClickCreateMaze);
        btnEnhanceDef.gameObject.SetActive(true);
        btnEnhanceDef.BeginMove(Globals.uiMoveAndScaleDuration);        
        ExitMazeBtn.BeginMove(Globals.uiMoveAndScaleDuration);
        //进入MyMaze，HomeExit上面的红点根据new targets来更新        
        Globals.UpdateUnclickedRedPointsText(unclickedCityEventCount);
    }

    public void GoToCity()
    {
        Globals.asyncLoad.ToLoadSceneAsync("City");

        //Globals.selectGuard.mover.Goback(Globals.uiMoveAndScaleDuration);
        btnEnhanceDef.Goback(Globals.uiMoveAndScaleDuration);
        ExitMazeBtn.Goback(Globals.uiMoveAndScaleDuration);
        Globals.cameraFollowMagician.CloseMinimap();
        room_not_full_used.gameObject.SetActive(false);
    }

    public void ShowUnclickedGuardsRedPointsOnEnhanceDefBtn()
    {
        MazeLvData mazeData = Globals.mazeLvDatas[Globals.self.currentMazeLevel];
        if (!mazeData.playerEverClickGuards)
        {
            RedPointsOnEnchanceDefBtn.transform.parent.gameObject.SetActive(true);
            RedPointsOnEnchanceDefBtn.text = mazeData.lockGuardsName.Length.ToString();            
        }
        else
        {
            RedPointsOnEnchanceDefBtn.transform.parent.gameObject.SetActive(false);
        }
    }

    public bool ChangeMazeRoom(int delta)
    {
        int roomTemp = roomConsumed;
        roomTemp += delta;
        if (roomTemp > Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport)
        {
            Globals.tipDisplay.Msg("not_enough_room");
            return false;
        }
        else
        {
            roomConsumed = roomTemp;
            MazeRoomNumber.UpdateCurrentLife(roomTemp, Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport);            
            return true;
        }
    }

    public void CheckRoomFullUses()
    {
        int roomSupport = Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport;
        if (roomConsumed == roomSupport || roomSupport == 0)
        {
            room_not_full_used.SetActive(false);
        }
        else
        {
            room_not_full_used.SetActive(true);
        }
    }
}
