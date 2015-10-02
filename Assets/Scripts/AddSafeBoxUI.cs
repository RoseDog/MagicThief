public class AddSafeBoxUI : CustomEventTrigger
{
    UnityEngine.UI.Button AddBtn;    
    UnityEngine.UI.Text AddPrice;
    UnityEngine.GameObject Lock;
    UnityEngine.UI.Text LockMsg;
    MultiLanguageUIText capacity;
    public override void Awake()
    {
        base.Awake();
        AddBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "AddBtn");
        AddBtn.onClick.AddListener(() => ClickToAddSafeBox());
        AddPrice = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashNumber");       
        Lock = Globals.getChildGameObject(gameObject, "LockBg");
        LockMsg = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LockMsg");
        capacity = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "capacity");
    }

    public void ClickToAddSafeBox()
    {
        UnityEngine.Debug.Log("ClickToAddSafeBox");
        if (Globals.canvasForMagician.ChangeCash(-Globals.buySafeBoxPrice))
        {
            OnTouchUpOutside(null);
            UnityEngine.Vector3 falling_pos = (Globals.LevelController as MyMazeLevelController).AddSafeBox();
            Globals.cameraFollowMagician.MoveToPoint(falling_pos, 30);
        }
    }

	public void UpdateInfo()
    {
        MazeLvData mazeData = Globals.mazeLvDatas[Globals.self.currentMazeLevel];
        if (Globals.self.currentMazeLevel != 0)
        {
            Globals.Assert(Globals.maze.noOfRoomsToPlace == mazeData.safeBoxCount);
        }
        Globals.languageTable.SetText(capacity,"capacity",
            new System.String[] { Globals.safeBoxLvDatas[0].capacity.ToString() });
        if (Globals.self.safeBoxDatas.Count >= mazeData.safeBoxCount)
        {
            if (Globals.self.currentMazeLevel == Globals.mazeLvDatas.Count - 1)
            {
                AddBtn.gameObject.SetActive(false);
                capacity.gameObject.SetActive(false);
                Lock.gameObject.SetActive(true);                
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg, "no_more_safebox");
            }
            else
            {
                int maze_lv_add_box_needed = 0;
                for (int i = Globals.self.currentMazeLevel; i < Globals.mazeLvDatas.Count;++i )
                {
                    MazeLvData data = Globals.mazeLvDatas[i];
                    if (data.safeBoxCount > Globals.self.safeBoxDatas.Count)
                    {
                        maze_lv_add_box_needed = i;
                        break;
                    }
                }
                AddBtn.gameObject.SetActive(false);
                Lock.gameObject.SetActive(true);
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg,
                    "upgrade_maze_to_add_safe_box", new System.String[] { maze_lv_add_box_needed.ToString() });
            }            
        }
        else if (Globals.self.safeBoxDatas.Count < mazeData.safeBoxCount)
        {            
            AddBtn.gameObject.SetActive(true);
            Globals.languageTable.SetText(AddPrice, "buy", new System.String[] { Globals.buySafeBoxPrice.ToString() });
            if (Globals.self.cashAmount < Globals.buySafeBoxPrice)
            {
                AddPrice.color = UnityEngine.Color.red;
            }
            else
            {
                AddPrice.color = UnityEngine.Color.white;
            }
            Lock.gameObject.SetActive(false);
        }        
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
        Globals.canvasForMyMaze.enhanceDefenseUI.CheckPointerJumpInTutorial();
        Globals.canvasForMyMaze.enhanceDefenseUI.safeboxTab.image.enabled = false;
    }
}
