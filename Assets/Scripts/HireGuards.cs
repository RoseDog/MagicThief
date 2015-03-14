public class HireGuards : UnityEngine.MonoBehaviour 
{
    public UnityEngine.UI.Button prevBtn;
    public UnityEngine.UI.Button nextBtn;
    UnityEngine.UI.GridLayoutGroup guardsLayout;
    public System.Collections.Generic.List<GuardData> guardsUnhired = new System.Collections.Generic.List<GuardData>();
    public System.Collections.Generic.List<UnityEngine.GameObject> guardHireBtns = new System.Collections.Generic.List<UnityEngine.GameObject>();

    int onePageCount = 3;

    public void Awake()
    {
        prevBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(transform.parent.gameObject, "Prev");
        prevBtn.onClick.AddListener(()=>Previous());
        prevBtn.gameObject.SetActive(false);
        nextBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(transform.parent.gameObject, "Next");
        nextBtn.onClick.AddListener(() => Next());
        nextBtn.gameObject.SetActive(false);

        guardsLayout = GetComponent<UnityEngine.UI.GridLayoutGroup>();
    }	

    public void UpdateGuardsInfo()
    {
        guardUnhiredUIAtLeftIdx = 0;
        guardsLayout = GetComponent<UnityEngine.UI.GridLayoutGroup>();

        foreach (UnityEngine.GameObject hireBtn in guardHireBtns)
        {
            DestroyObject(hireBtn);
        }
        guardHireBtns.Clear();        
        guardsUnhired.Clear();
        for (int mazeLevelIdx = 0; mazeLevelIdx < Globals.mazeLvDatas.Count; ++mazeLevelIdx)
        {
            MazeLvData mazeData = Globals.mazeLvDatas[mazeLevelIdx];
            for (int idx = 0; idx < mazeData.lockGuardsName.Count; ++idx)
            {
                GuardData guardInfo = Globals.GetGuardData(mazeData.lockGuardsName[idx]);
                guardInfo.locked = Globals.CurrentMazeLevel < mazeLevelIdx;
                guardInfo.unlockMazeLv = mazeLevelIdx;
                guardsUnhired.Add(guardInfo);                
            }
        }

        for (int idx = 0; idx < guardsUnhired.Count; ++idx)
        {
            if (idx >= guardUnhiredUIAtLeftIdx && idx < guardUnhiredUIAtLeftIdx + onePageCount)
            {
                CreateGuardInfoUI(guardsUnhired[idx], false);
            }
        }
    }

    UnityEngine.GameObject CreateGuardInfoUI(GuardData guardInfo, bool addToHead)
    {
        UnityEngine.GameObject guard_info_prefab = UnityEngine.Resources.Load("UI/guard_hireinfo") as UnityEngine.GameObject;
        UnityEngine.GameObject guard_info_object = UnityEngine.GameObject.Instantiate(guard_info_prefab) as UnityEngine.GameObject;
        guard_info_object.transform.parent = guardsLayout.transform;
        guard_info_object.transform.localScale = UnityEngine.Vector3.one;
        guard_info_object.transform.localPosition = UnityEngine.Vector3.zero;
        guard_info_object.name = guardInfo.name;
        UnityEngine.UI.Button hireBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(guard_info_object, "HireButton");
        UnityEngine.UI.Image LockBg = Globals.getChildGameObject<UnityEngine.UI.Image>(guard_info_object, "LockBg");
        LockBg.gameObject.SetActive(guardInfo.locked);
        hireBtn.gameObject.SetActive(!guardInfo.locked);
        if (!guardInfo.locked)
        {            
            if(!guardInfo.hired)
            {
                UnityEngine.UI.Text CashNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(guard_info_object, "CashNumber");
                CashNumber.text = guardInfo.price.ToString();
                CashNumber.gameObject.SetActive(!guardInfo.locked);
                hireBtn.onClick.AddListener(() => HireBtnClicked(guard_info_object, hireBtn, guardInfo));
            }
            else
            {
                Globals.languageTable.SetText(Globals.getChildGameObject<UnityEngine.UI.Text>(hireBtn.gameObject,"CashNumber"),"hired");
            }
            guard_info_object.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Summon(guardInfo));
        }
        else
        {
            UnityEngine.UI.Text LockMsg = Globals.getChildGameObject<UnityEngine.UI.Text>(guard_info_object, "LockMsg");
            Globals.languageTable.SetText(LockMsg, "upgrade_maze_to_unlock", new System.String[] { guardInfo.unlockMazeLv.ToString() });
        }

        UnityEngine.UI.Image iconImage = Globals.getChildGameObject<UnityEngine.UI.Image>(guard_info_object, "Icon");
        iconImage.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + guardInfo.name + "_hireInfoIcon");

        MultiLanguageUIText nameText = Globals.getChildGameObject<MultiLanguageUIText>(guard_info_object, "NameText");
        Globals.languageTable.SetText(nameText, guardInfo.name);

        MultiLanguageUIText Description = Globals.getChildGameObject<MultiLanguageUIText>(guard_info_object, "Description");
        Globals.languageTable.SetText(Description, guardInfo.name + "_desc");

        MultiLanguageUIText RoomConsume = Globals.getChildGameObject<MultiLanguageUIText>(guard_info_object, "RoomConsume");
        RoomConsume.text = guardInfo.roomConsume.ToString();

        if (addToHead)
        {
            guardHireBtns.Insert(0, guard_info_object);
        }
        else
        {
            guardHireBtns.Add(guard_info_object);
        }
        return guard_info_object;
    }

    void HireBtnClicked(UnityEngine.GameObject guard_info_object, UnityEngine.UI.Button hireBtn, GuardData guardInfo)
    {
        if (Globals.canvasForMagician.ChangeCash(-guardInfo.price))
        {
            hireBtn.onClick.RemoveAllListeners();
            guardInfo.hired = true;
            Globals.languageTable.SetText(hireBtn.GetComponentInChildren<UnityEngine.UI.Text>(), "hired");
        }
    }    

    void Summon(GuardData guardInfo)
    {
        if (guardInfo.hired)
        {
            if (Globals.canvasForMyMaze.ChangeMazeRoom(guardInfo.roomConsume))
            {
                Globals.canvasForMyMaze.enhanceDefenseUI.OnTouchUpOutside(null);
                Finger f = Globals.input.GetFingerByID(0);
                //UnityEngine.Vector3 screenPos = new UnityEngine.Vector3(f.nowPosition.x, f.nowPosition.y, 0);
                UnityEngine.Vector3 screenPos = new UnityEngine.Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0);
                Pathfinding.Node birthNode = Globals.maze.GetNodeFromScreenRay(screenPos);
                Guard guard = Globals.CreateGuard(guardInfo, birthNode);
                guard.data = guardInfo;
                guard.Choosen();
            }        
        }
        else
        {
            Globals.tipDisplay.Msg("not_hired_yet"); ;
        }        
    }

    int guardUnhiredUIAtLeftIdx;
    
    public void Previous()
    {
        --guardUnhiredUIAtLeftIdx;
        if (guardUnhiredUIAtLeftIdx < 0)
        {
            guardUnhiredUIAtLeftIdx = 0;
        }
        else
        {
            DestroyObject(guardHireBtns[guardHireBtns.Count - 1]);
            guardHireBtns.RemoveAt(guardHireBtns.Count - 1);
            UnityEngine.GameObject guard_info_object = CreateGuardInfoUI(guardsUnhired[guardUnhiredUIAtLeftIdx], true);
            guard_info_object.transform.SetAsFirstSibling();
        }        
    }

    public void Next()
    {
        ++guardUnhiredUIAtLeftIdx;
        if (guardUnhiredUIAtLeftIdx > guardsUnhired.Count - onePageCount)
        {
            guardUnhiredUIAtLeftIdx = guardsUnhired.Count - onePageCount;
        }
        else
        {
            DestroyObject(guardHireBtns[0]);
            guardHireBtns.RemoveAt(0);
            UnityEngine.GameObject guard_info_object = CreateGuardInfoUI(guardsUnhired[guardUnhiredUIAtLeftIdx + onePageCount-1], false);
        }             
    }
}
