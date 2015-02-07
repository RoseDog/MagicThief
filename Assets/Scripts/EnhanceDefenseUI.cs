public class EnhanceDefenseUI : CustomEventTrigger
{
    UnityEngine.UI.Button mazeTab;
    public UnityEngine.UI.Button guardsTab;
    public UnityEngine.UI.Button safeboxTab;
    public MazeInfo mazeInfo;        
    HireGuards hireGuards;
    AddSafeBoxUI addSafeBox;
    UnityEngine.UI.Text RedPointsOnGuardsTab;
    UnityEngine.UI.Text RedPointsOnSafeboxTab;
    public override void Awake()
    {
        base.Awake();
        mazeTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "MazeTab");
        guardsTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "GuardsTab");
        safeboxTab = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "SafeBoxTab");

        mazeInfo = Globals.getChildGameObject<MazeInfo>(gameObject, "MazeInfo");        
        hireGuards = Globals.getChildGameObject<HireGuards>(gameObject, "HireGuardsLayout");        
        addSafeBox = Globals.getChildGameObject<AddSafeBoxUI>(gameObject, "AddSafeBox");
        

        safeboxTab.onClick.AddListener(() => SwichPanel(safeboxTab));
        mazeTab.onClick.AddListener(() => SwichPanel(mazeTab));
        guardsTab.onClick.AddListener(() => SwichPanel(guardsTab));

        RedPointsOnGuardsTab = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnGuardsTab").GetComponentInChildren<UnityEngine.UI.Text>();
        RedPointsOnSafeboxTab = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "RedPointsOnSafeboxTab").GetComponentInChildren<UnityEngine.UI.Text>();
    }

    public void Start()
    {
        hireGuards.gameObject.SetActive(false);
        addSafeBox.gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        mazeInfo.UpdateMazeInfo();
        hireGuards.UpdateGuardsInfo();
        addSafeBox.UpdateInfo();

        MazeLvData mazeData = Globals.mazeLvDatas[Globals.CurrentMazeLevel];
        if (!mazeData.playerEverClickGuards)
        {
            RedPointsOnGuardsTab.transform.parent.gameObject.SetActive(true);
            RedPointsOnGuardsTab.text = mazeData.guards.Count.ToString();
        }
        else
        {
            RedPointsOnGuardsTab.transform.parent.gameObject.SetActive(false);
        }

        if (!mazeData.playerEverClickSafebox)
        {
            RedPointsOnSafeboxTab.transform.parent.gameObject.SetActive(true);
            RedPointsOnSafeboxTab.text = mazeData.guards.Count.ToString();
        }
        else
        {
            RedPointsOnSafeboxTab.transform.parent.gameObject.SetActive(false);
        }
    }    

    public void SwichPanel(UnityEngine.UI.Button tabBtn)
    {
        mazeTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/UnselectTabBtn");
        guardsTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/UnselectTabBtn");
        safeboxTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/UnselectTabBtn");
        
        mazeInfo.gameObject.SetActive(false);
        hireGuards.gameObject.SetActive(false);
        addSafeBox.gameObject.SetActive(false);

        if (tabBtn == mazeTab)
        {
            mazeInfo.gameObject.SetActive(true);
            mazeTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/SelectedTabBtn");
        }
        else if (tabBtn == guardsTab)
        {
            hireGuards.gameObject.SetActive(true);

            Globals.mazeLvDatas[Globals.CurrentMazeLevel].playerEverClickGuards = true;
            Globals.canvasForMyMaze.RedPointsOnEnchanceDefBtn.transform.parent.gameObject.SetActive(false);
            guardsTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/SelectedTabBtn");
        }
        else if (tabBtn == safeboxTab)
        {
            addSafeBox.gameObject.SetActive(true);

            Globals.mazeLvDatas[Globals.CurrentMazeLevel].playerEverClickSafebox = true;            
            safeboxTab.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/SelectedTabBtn");
        }     

        hireGuards.prevBtn.gameObject.SetActive(hireGuards.gameObject.activeSelf);
        hireGuards.nextBtn.gameObject.SetActive(hireGuards.gameObject.activeSelf);
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);        
        gameObject.SetActive(false);
        Globals.canvasForMyMaze.btnEnhanceDef.gameObject.SetActive(true);
    }
}
