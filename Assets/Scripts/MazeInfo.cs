public class MazeInfo : CustomEventTrigger
{
    public UnityEngine.UI.Image current_MazeIcon;
    public UnityEngine.UI.Text current_lv;
    public UnityEngine.UI.Text current_Room;
    public UnityEngine.UI.Image next_MazeIcon;
    public UnityEngine.UI.Text next_lv;
    public UnityEngine.UI.Text next_Room;
    
    public UnityEngine.UI.Button TestUpgradeBtn;
    public UnityEngine.UI.Button UpgradeBtn;
    public UnityEngine.GameObject Lock;
    public UnityEngine.UI.Text UpgradePrice;

    public UnityEngine.UI.Text LockMsg;

    public bool isUpgradingMaze = false;    

    public override void Awake()
    {
        base.Awake();
        TestUpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TestUpgradeBtn");
        TestUpgradeBtn.onClick.AddListener(() => UpgradeMaze());
        //TestUpgradeBtn.gameObject.SetActive(false);

        UpgradeBtn.onClick.AddListener(() => ClickToUpgradeMaze());                    
    }
    
    public void ClickToUpgradeMaze()
    {
        if (Globals.canvasForMagician.ChangeCash(-Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].price))
        {
            Globals.self.currentMazeLevel++;
            UpgradeMaze();            
        }
    }

    public void UpgradeMaze()
    {        
        if (Globals.self.currentMazeLevel == 1)
        {
            Globals.Assert(Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.InitMyMaze);
            Globals.canvasForMyMaze.MazeZeroToOne();
            Globals.canvasForMyMaze.ChangeMazeRoom(0);
        }
        else
        {
            isUpgradingMaze = true;            
            Globals.maze.UnRegisterGuardArrangeEvent();            
            Globals.maze.ClearMaze();
            Globals.self.summonedGuardsStr = "";
            Globals.self.UploadGuards();
            Globals.maze.Start();            
        }
        Globals.canvasForMyMaze.enhanceDefenseUI.gameObject.SetActive(false);
    }
    

    public void UpdateMazeInfo()
    {                
        // 0级迷宫代表还没有通过创建迷宫的教程。没有迷宫的状态
        
        current_lv.text = Globals.self.currentMazeLevel.ToString();
        current_Room.text = Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport.ToString();
        if (Globals.self.currentMazeLevel == 0)
        {
            current_MazeIcon.sprite = null;
        }
        else
        {
            current_MazeIcon.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/maze_" + Globals.self.currentMazeLevel.ToString());
        }
        
        
        if (Globals.self.currentMazeLevel == Globals.mazeLvDatas.Count - 1)
        {
            UpgradeBtn.gameObject.SetActive(false);
            Lock.gameObject.SetActive(true);
            LockMsg.gameObject.SetActive(true);
            next_MazeIcon.gameObject.SetActive(true);
            Globals.languageTable.SetText(LockMsg,"top_maze");
        }
        else
        {
            next_MazeIcon.gameObject.SetActive(true);
            next_lv.text = (Globals.self.currentMazeLevel + 1).ToString();
            next_Room.text = Globals.mazeLvDatas[Globals.self.currentMazeLevel+1].roomSupport.ToString();
            next_MazeIcon.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/maze_" + (Globals.self.currentMazeLevel + 1).ToString());
            if (Globals.self.roseCount >= Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].roseRequire)
            {
                UpgradeBtn.gameObject.SetActive(true);
                UpgradePrice.text = Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].price.ToString();
                if (Globals.self.cashAmount < Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].price)
                {
                    UpgradePrice.color = UnityEngine.Color.red;
                }
                else
                {
                    UpgradePrice.color = UnityEngine.Color.white;
                }
                Lock.gameObject.SetActive(false);
            }
            else
            {
                UpgradeBtn.gameObject.SetActive(false);
                Lock.gameObject.SetActive(true);
                
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg,
                    "reach_rose_count_will_unlock", new System.String[] { Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].roseRequire.ToString() });
            }
        }
        
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
        Globals.canvasForMyMaze.enhanceDefenseUI.CheckPointerJumpInTutorial();
        Globals.canvasForMyMaze.enhanceDefenseUI.mazeTab.image.enabled = false;
    }
}
