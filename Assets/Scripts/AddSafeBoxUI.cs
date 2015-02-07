﻿public class AddSafeBoxUI : UnityEngine.MonoBehaviour 
{
    UnityEngine.UI.Button AddBtn;    
    UnityEngine.UI.Text AddPrice;
    UnityEngine.GameObject Lock;
    UnityEngine.UI.Text LockMsg;
    
    public void Awake()
    {                
        AddBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "AddBtn");
        AddBtn.onClick.AddListener(() => ClickToAddSafeBox());
        AddPrice = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashNumber");       
        Lock = Globals.getChildGameObject(gameObject, "LockBg");
        LockMsg = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LockMsg");        
    }

    public void ClickToAddSafeBox()
    {
        if (Globals.canvasForMagician.ChangeCash(-Globals.buySafeBoxPrice))
        {
            Globals.canvasForMyMaze.enhanceDefenseUI.OnTouchUpOutside(null);
            (Globals.LevelController as MyMazeLevelController).AddSafeBox();
        }
    }

	public void UpdateInfo()
    {
        MazeLvData mazeData = Globals.mazeLvDatas[Globals.CurrentMazeLevel];
        if (Globals.CurrentMazeLevel != 0)
        {
            Globals.Assert(Globals.maze.noOfRoomsToPlace == mazeData.safeBoxCount);
        }        
        if (Globals.safeBoxDatas.Count >= mazeData.safeBoxCount)
        {
            if (Globals.CurrentMazeLevel == Globals.mazeLvDatas.Count - 1)
            {
                AddBtn.gameObject.SetActive(false);
                Lock.gameObject.SetActive(true);                
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg, "no_more_safebox");
            }
            else
            {
                AddBtn.gameObject.SetActive(false);
                Lock.gameObject.SetActive(true);
                LockMsg.gameObject.SetActive(true);
                Globals.languageTable.SetText(LockMsg,
                    "upgrade_maze_to_add_safe_box", new System.String[] { (Globals.CurrentMazeLevel + 1).ToString() });
            }            
        }
        else if (Globals.safeBoxDatas.Count < mazeData.safeBoxCount)
        {            
            AddBtn.gameObject.SetActive(true);
            AddPrice.text = Globals.buySafeBoxPrice.ToString();
            if (Globals.cashAmount < Globals.buySafeBoxPrice)
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
}
