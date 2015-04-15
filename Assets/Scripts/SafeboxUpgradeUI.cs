public class SafeboxUpgradeUI : CustomEventTrigger
{
    UnityEngine.UI.Text Name;
    UnityEngine.UI.Text Desc;
    UnityEngine.UI.Text LvNumber;
    UnityEngine.UI.Text CashInBoxNumber;
    UnityEngine.UI.Text CapacityNumber;
    UnityEngine.UI.Text AddNumber;
    UnityEngine.UI.Button UpgradeBtn;
    UnityEngine.UI.Text PriceNumber;
    UnityEngine.GameObject LockBg;
    UnityEngine.UI.Text LockMsg;
    public override void Awake()
    {
        base.Awake();
        Name = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Name");
        Desc = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Desc");
        LvNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LvNumber");        
        CashInBoxNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashInBoxNumber");
        CapacityNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CapacityNumber");
        AddNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "AddNumber");
        UpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "UpgradeBtn");
        PriceNumber = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "PriceNumber");
        LockBg = Globals.getChildGameObject(gameObject, "LockBg");
        LockMsg = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "LockMsg");
    }

    public void SetSafebox(SafeBoxData data)
    {
        transform.parent.parent = Globals.LevelController.mainCanvas.transform;
        transform.parent.SetAsLastSibling();
        (transform.parent as UnityEngine.RectTransform).anchoredPosition = UnityEngine.Vector2.zero;

        transform.parent.localScale = UnityEngine.Vector3.one;

        LvNumber.text = data.Lv.ToString();

        CashInBoxNumber.text = data.cashInBox.ToString();
        CapacityNumber.text = Globals.safeBoxLvDatas[data.Lv].capacity.ToString();
        AddNumber.text = "+"+(Globals.safeBoxLvDatas[data.Lv + 1].capacity - Globals.safeBoxLvDatas[data.Lv].capacity).ToString();

        if (data.Lv < Globals.self.currentMazeLevel-1)
        {
            LockBg.gameObject.SetActive(false);
            UpgradeBtn.gameObject.SetActive(true);            
            PriceNumber.text = Globals.safeBoxLvDatas[data.Lv].price.ToString();
            UpgradeBtn.onClick.AddListener(() => UpgradeBtnClicked(data));
        }
        else
        {
            LockBg.gameObject.SetActive(true);
            Globals.languageTable.SetText(LockMsg,
                    "upgrade_maze_to_upgrade_safe_box", new System.String[] { (Globals.self.currentMazeLevel + 1).ToString() });
            UpgradeBtn.gameObject.SetActive(false);
        }
    }

    void UpgradeBtnClicked(SafeBoxData data)
    {
        if (Globals.canvasForMagician.ChangeCash(-Globals.safeBoxLvDatas[data.Lv].price))
        {
            Globals.canvasForMyMaze.enhanceDefenseUI.OnTouchUpOutside(null);
            Globals.self.UpgradeSafebox(data);
            (Globals.LevelController as MyMazeLevelController).PutCashInBox(Globals.self);
            SetSafebox(data);
        }
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        DestroyObject(transform.parent.gameObject);
    }
}
