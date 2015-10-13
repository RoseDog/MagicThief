public class EnhanceDefenseUI : Actor
{
    public UnityEngine.UI.Button mazeTab;
    public UnityEngine.UI.Button guardsTab;
    public UnityEngine.UI.Button safeboxTab;
    public MazeInfo mazeInfo;        
    HireGuards hireGuards;
    AddSafeBoxUI addSafeBox;
    UnityEngine.UI.Text RedPointsOnGuardsTab;
    UnityEngine.UI.Text RedPointsOnSafeboxTab;
    public UnityEngine.GameObject SummonGuardTip;
    public UIMover MazeTabPointer;
    public UIMover GuardsTabPointer;
    public UIMover add_box_tip_pointer;
    public override void Awake()
    {
        base.Awake();
        mazeTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "MazeTab");
        guardsTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "GuardsTab");        
        safeboxTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "SafeBoxTab");        

        mazeInfo = Globals.getChildGameObject<MazeInfo>(gameObject, "MazeInfo");        
        hireGuards = Globals.getChildGameObject<HireGuards>(gameObject, "HireGuards");        
        addSafeBox = Globals.getChildGameObject<AddSafeBoxUI>(gameObject, "AddSafeBox");
        

        safeboxTab.onClick.AddListener(() => SwichPanel(safeboxTab));
        mazeTab.onClick.AddListener(() => SwichPanel(mazeTab));
        guardsTab.onClick.AddListener(() => SwichPanel(guardsTab));

        //RedPointsOnGuardsTab = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnGuardsTab").GetComponentInChildren<UnityEngine.UI.Text>();
        //RedPointsOnSafeboxTab = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnSafeboxTab").GetComponentInChildren<UnityEngine.UI.Text>();

        
        SummonGuardTip.gameObject.SetActive(false);

        MazeTabPointer = Globals.getChildGameObject<UIMover>(gameObject, "MazeTabPointer");
        MazeTabPointer.transform.parent.gameObject.SetActive(false);

        GuardsTabPointer = Globals.getChildGameObject<UIMover>(gameObject, "GuardsTabPointer");
        GuardsTabPointer.transform.parent.gameObject.SetActive(false);

        add_box_tip_pointer = Globals.getChildGameObject<UIMover>(gameObject, "add_box_tip_pointer");
        add_box_tip_pointer.transform.parent.gameObject.SetActive(false);
    }

    public void Start()
    {
        mazeInfo.gameObject.SetActive(false);
        hireGuards.gameObject.SetActive(false);
        addSafeBox.gameObject.SetActive(false);        
    }

    public void Open()
    {
        gameObject.SetActive(true);

        mazeTab.image.enabled = false;
        guardsTab.image.enabled = false;
        safeboxTab.image.enabled = false;
        
        mazeInfo.UpdateMazeInfo();
        hireGuards.UpdateGuardsInfo();
        addSafeBox.UpdateInfo();        

        MazeLvData mazeData = Globals.mazeLvDatas[Globals.self.currentMazeLevel];
//         if (!mazeData.playerEverClickGuards)
//         {
//             RedPointsOnGuardsTab.transform.parent.gameObject.SetActive(true);
//             RedPointsOnGuardsTab.text = mazeData.lockGuardsName.Length.ToString();
//         }
//         else
//         {
//             RedPointsOnGuardsTab.transform.parent.gameObject.SetActive(false);
//         }
// 
//         if (!mazeData.playerEverClickSafebox)
//         {
//             RedPointsOnSafeboxTab.transform.parent.gameObject.SetActive(true);
//             RedPointsOnSafeboxTab.text = mazeData.lockGuardsName.Length.ToString();
//         }
//         else
//         {
//             RedPointsOnSafeboxTab.transform.parent.gameObject.SetActive(false);
//         }
        CheckPointerJumpInTutorial();
    }

    public void CheckPointerJumpInTutorial()
    {
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.InitMyMaze)
        {
            if (Globals.self.currentMazeLevel == 0)
            {
                MazeTabPointer.transform.parent.gameObject.SetActive(true);
                MazeTabPointer.Jump();
            }

            if (Globals.self.currentMazeLevel == 1)
            {
                GuardsTabPointer.transform.parent.gameObject.SetActive(true);
                GuardsTabPointer.Jump();
            }
        }   
    }

    public void SwichPanel(UnityEngine.UI.Button tabBtn)
    {        
        mazeInfo.gameObject.SetActive(false);
        hireGuards.gameObject.SetActive(false);
        addSafeBox.gameObject.SetActive(false);

        mazeTab.image.enabled = false;
        guardsTab.image.enabled = false;
        safeboxTab.image.enabled = false;

        tabBtn.image.enabled = true;

        MazeTabPointer.transform.parent.gameObject.SetActive(false);
        GuardsTabPointer.transform.parent.gameObject.SetActive(false);
        add_box_tip_pointer.transform.parent.gameObject.SetActive(false);
        SummonGuardTip.gameObject.SetActive(false);

        if (tabBtn == mazeTab)
        {
            mazeInfo.gameObject.SetActive(true);
            mazeInfo.UpdateMazeInfo();
        }
        else if (tabBtn == guardsTab)
        {
            hireGuards.gameObject.SetActive(true);
            Globals.mazeLvDatas[Globals.self.currentMazeLevel].playerEverClickGuards = true;         
        }
        else if (tabBtn == safeboxTab)
        {
            addSafeBox.gameObject.SetActive(true);
            addSafeBox.UpdateInfo();
            Globals.mazeLvDatas[Globals.self.currentMazeLevel].playerEverClickSafebox = true;            
        }


        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.InitMyMaze)
        {
            if (Globals.self.currentMazeLevel == 1)
            {
                if (tabBtn == guardsTab)
                {
                    GuardsTabPointer.transform.parent.gameObject.SetActive(false);
                    SummonGuardTip.gameObject.SetActive(true);
                    SummonGuardTip.GetComponentInChildren<UIMover>().Jump();
                }
                else
                {
                    GuardsTabPointer.transform.parent.gameObject.SetActive(true);
                    GuardsTabPointer.Jump();
                    SummonGuardTip.gameObject.SetActive(false);
                }  
            }
        }
    }
}
