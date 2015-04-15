public class MazeInfo : UnityEngine.MonoBehaviour 
{
    UnityEngine.UI.Image MazeIcon;
    UnityEngine.UI.Text MazeLv;
    public UnityEngine.UI.Button TestUpgradeBtn;
    UnityEngine.UI.Button UpgradeBtn;
    UnityEngine.GameObject Lock;
    UnityEngine.UI.Text UpgradePrice;
    UnityEngine.UI.RawImage RoseIcon;
    UnityEngine.UI.Text LockMsg;
    MultiLanguageUIText Room;
    public bool isUpgradingMaze = false;    

    public void Awake()
    {
        MazeIcon = Globals.getChildGameObject<UnityEngine.UI.Image>(gameObject, "Icon");
        MazeLv = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LvNumber");

        TestUpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TestUpgradeBtn");
        TestUpgradeBtn.onClick.AddListener(() => UpgradeMaze());
        //TestUpgradeBtn.gameObject.SetActive(false);

        UpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "UpgradeBtn");
        UpgradeBtn.onClick.AddListener(() => ClickToUpgradeMaze());
        Lock = Globals.getChildGameObject(gameObject, "LockBg");        
        UpgradePrice = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashNumber");
        RoseIcon = Globals.getChildGameObject<UnityEngine.UI.RawImage>(gameObject, "RoseIcon");
        LockMsg = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LockMsg");
        Room = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "Room");
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
            Globals.canvasForMyMaze.InitMazeBtnClicked();
        }
        else
        {
            isUpgradingMaze = true;             
            Globals.maze.UnRegisterGuardArrangeEvent();
            Globals.maze.ClearMaze();
            Globals.maze.Start();
        }
        Globals.canvasForMyMaze.enhanceDefenseUI.OnTouchUpOutside(null);
    }
    

    public void UpdateMazeInfo()
    {                
        // 0级迷宫代表还没有通过创建迷宫的教程。没有迷宫的状态
        
        MazeLv.text = Globals.self.currentMazeLevel.ToString();
        if (Globals.self.currentMazeLevel == Globals.mazeLvDatas.Count - 1)
        {
            UpgradeBtn.gameObject.SetActive(false);
            Lock.gameObject.SetActive(true);
            RoseIcon.gameObject.SetActive(false);
            LockMsg.gameObject.SetActive(true);
            Room.gameObject.SetActive(false);
            Globals.languageTable.SetText(LockMsg,"top_maze");
            MazeIcon.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/maze_" + (Globals.self.currentMazeLevel-1).ToString());
        }
        else
        {
            MazeIcon.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/maze_" + Globals.self.currentMazeLevel.ToString());
            Room.text = Globals.languageTable.GetText("room") + ":" + Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport
                + " + " + (Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].roomSupport - Globals.mazeLvDatas[Globals.self.currentMazeLevel].roomSupport).ToString();

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
                RoseIcon.gameObject.SetActive(true);
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg,
                    "reach_rose_count_will_unlock", new System.String[] { Globals.mazeLvDatas[Globals.self.currentMazeLevel + 1].roseRequire.ToString() });
            }
        }
        
    }
}
