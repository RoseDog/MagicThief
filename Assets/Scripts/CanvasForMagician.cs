
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject CashNumerBg;
    public LifeNumber cashNumber;
    CashIntroUI cashIntroUI;
    public LifeNumber lifeNumber;    
    public UnityEngine.GameObject LifeNumerBg;
    public LifeNumber PowerNumber;
    public UnityEngine.GameObject PowerNumerBg;
    public Number RoseNumber;
    public UnityEngine.GameObject RoseNumberBg;    
    public UnityEngine.UI.Button equipBtn;
    public UnityEngine.UI.Text equipUnclickedCount;
    public EquipsBg equips;    
    public UIMover clickEquipBtnPointer;
    public UnityEngine.GameObject SelectedImage_prefab;
    public TrickItem draggingTrickItem;
    public TrickSlot draggingDownSlot;
    public TrickDesc TrickDescParent;
    public UnityEngine.UI.Text TrickName;
    public UnityEngine.UI.Text TrickDesc;
    public UnityEngine.UI.Text powerLabel;
    public UnityEngine.UI.Text durationLabel;
    public UnityEngine.UI.Text unlockLabel;
    public UnityEngine.UI.Button buyBtn;
    public UnityEngine.UI.Text cashCost;
    public UnityEngine.GameObject tricksInUsingPanel;
    public UnityEngine.GameObject tricksInUseTip;
    public TrickSlot[] trickInUseSlots;
    public UIMover draggingItemFinger;
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;        
        CashNumerBg = Globals.getChildGameObject(gameObject, "CashNumerBg");
        CashNumerBg.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>OpenCashIntroUI());
        cashNumber = CashNumerBg.GetComponentInChildren<LifeNumber>();

        cashIntroUI = Globals.getChildGameObject<CashIntroUI>(gameObject, "CashIntroUI");


        LifeNumerBg = Globals.getChildGameObject(gameObject, "LifeNumerBg");
        lifeNumber = Globals.getChildGameObject<LifeNumber>(LifeNumerBg, "LifeNumber");

        PowerNumerBg = Globals.getChildGameObject(gameObject, "PowerNumerBg");
        PowerNumber = Globals.getChildGameObject<LifeNumber>(PowerNumerBg, "PowerNumber"); 

        RoseNumberBg = Globals.getChildGameObject(gameObject, "RoseNumberBg"); //UnityEngine.GameObject.Find("RoseNumberBg");
        RoseNumber = RoseNumberBg.GetComponentInChildren<Number>();                              

        equipBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "EquipBtn");
        equipBtn.onClick.AddListener(()=>OpenEquipUI());        
        equipUnclickedCount = Globals.getChildGameObject<UnityEngine.UI.Text>(equipBtn.gameObject, "UnclickedCount");        

        clickEquipBtnPointer = Globals.getChildGameObject<UIMover>(equipBtn.gameObject, "ClickEquipBtnPointer");
        equipBtn.gameObject.SetActive(false);

        equips = Globals.getChildGameObject<EquipsBg>(gameObject, "Equipments");

        TrickDescParent = Globals.getChildGameObject<TrickDesc>(gameObject, "TrickDescBg");
        TrickName = Globals.getChildGameObject<UnityEngine.UI.Text>(TrickDescParent.gameObject, "Name");
        TrickDesc = Globals.getChildGameObject<UnityEngine.UI.Text>(TrickDescParent.gameObject, "Desc");

        UnityEngine.GameObject power = Globals.getChildGameObject(TrickDescParent.gameObject, "PowerCost");
        powerLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(power, "label");

        UnityEngine.GameObject duration = Globals.getChildGameObject(TrickDescParent.gameObject, "Duration");
        durationLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(duration, "label");

        UnityEngine.GameObject unlock = Globals.getChildGameObject(TrickDescParent.gameObject, "Unlock");
        unlockLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(unlock, "label");

        buyBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(TrickDescParent.gameObject, "BuyBtn");        
        cashCost = Globals.getChildGameObject<UnityEngine.UI.Text>(TrickDescParent.gameObject, "CashCost");                        

        tricksInUsingPanel = Globals.getChildGameObject(gameObject, "TricksInUsingPanel");
        tricksInUseTip = Globals.getChildGameObject(tricksInUsingPanel, "tip"); 

        draggingItemFinger = Globals.getChildGameObject<UIMover>(gameObject, "DraggingItemFinger");        
                
        InitUIStats();

        SelectedImage_prefab = UnityEngine.Resources.Load("UI/guardSelectedImage") as UnityEngine.GameObject;
        
        trickInUseSlots = tricksInUsingPanel.GetComponentsInChildren<TrickSlot>();
        transform.SetAsFirstSibling();        
	}

    public void Start()
    {
        if(!Globals.socket.FromLogin && !Globals.socket.IsReady)
        {
            return;
        }        

        if (!Globals.self.IsAnyTricksInUse() && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.FirstTrick)
        {
            clickEquipBtnPointer.Jump();
        }
        else
        {
            clickEquipBtnPointer.gameObject.SetActive(false);
        }
        // Slot的解锁状况
        for (int idx = 0; idx < trickInUseSlots.Length; ++idx)
        {
            TrickSlot slot = trickInUseSlots[idx];
            slot.UpdateData(Globals.self.slotsDatas[idx]);
        }                

        // 玫瑰
        RoseNumber.SetNumber(Globals.self.roseCount);

        // 金钱
        UpdateCash();

        cashIntroUI.gameObject.SetActive(false);        
    }

    void InitUIStats()
    {        
        equipUnclickedCount.gameObject.SetActive(false);
        equips.gameObject.SetActive(false);
        draggingItemFinger.gameObject.SetActive(false);
        TrickDescParent.gameObject.SetActive(false);
    }

    public void SetLifeVisible(bool Visible)
    {
        LifeNumerBg.SetActive(Visible);
        PowerNumerBg.SetActive(Visible);
    }

    public void SetCashVisible(bool Visible)
    {
        CashNumerBg.SetActive(Visible);
    }

    public void UpdateCash()
    {
        cashNumber.UpdateCurrentLife(Globals.self.cashAmount, Globals.AccumulateSafeboxCapacity(Globals.self));
    }

    public void OpenCashIntroUI()
    {
        if(!Globals.magician.Stealing)
        {
            cashIntroUI.Open();
        }
    }

    public void OpenEquipUI()
    {        
        equips.gameObject.SetActive(true);
        equips.CreateTrickItemsInPack();
        
        TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
        if (controller != null)
        {
            Globals.maze.UnRegistChallengerEvent();
            controller.landingMark.SetActive(false);
        }      

        CheckIfNeedDraggingItemFinger();
    }

    public void CheckIfNeedDraggingItemFinger()
    {
        if (!Globals.self.IsAnyTricksInUse() && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.FirstTrick)
        {
            clickEquipBtnPointer.gameObject.SetActive(true);
            clickEquipBtnPointer.ClearAllActions();
            clickEquipBtnPointer.Jump();
            if (equips.gameObject.activeSelf)
            {
                draggingItemFinger.gameObject.SetActive(true);
                draggingItemFinger.RecoverPos();
                draggingItemFinger.ClearAllActions();
                draggingItemFinger.GetComponent<UnityEngine.CanvasGroup>().interactable = false;
                draggingItemFinger.GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = false;
                draggingItemFinger.ForeverMoving(100);
            }            
        }
        else
        {
            draggingItemFinger.gameObject.SetActive(false);
            clickEquipBtnPointer.gameObject.SetActive(false);
        }
    }

    public void OpenItemDescriptionUI(TrickItem item)
    {        
        if (Globals.canvasForMagician.draggingTrickItem != null)
        {
            return;
        }
        // 如果是闪光弹，不打开界面。闪光弹是在战斗开始前使用的
        TrickData data = item.trickData;
        if (data.nameKey == "flash_grenade" && data.IsInUse())
        {
            return;
        }
        // 如果魔术师已经降下，不打开界面        
        if(Globals.magician.gameObject.activeSelf)
        {
            return;
        }
        // 如果装备界面还没打开，首先打开装备界面
        if (!equips.gameObject.activeSelf)
        {
            OpenEquipUI();
            return;
        }
        
        TrickDescParent.gameObject.SetActive(true);
        Globals.languageTable.SetText(TrickName, data.nameKey);
        Globals.languageTable.SetText(TrickDesc, data.descriptionKey);
        Globals.languageTable.SetText(powerLabel, "power_cost", new System.String[]{data.powerCost.ToString()});
        Globals.languageTable.SetText(durationLabel, "duration", new System.String[]{data.duration.ToString()});
                        
        if (!data.IsLocked())
        {
            Globals.languageTable.SetText(unlockLabel, "already_unlocked");
        }
        else
        {
            Globals.languageTable.SetText(unlockLabel, "unlock_need_rose", new System.String[] { data.unlockRoseCount.ToString() });
            unlockLabel.color = UnityEngine.Color.red;

            buyBtn.gameObject.SetActive(true);
            UnityEngine.UI.ColorBlock colors = buyBtn.colors;
            colors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
            buyBtn.colors = colors;
        }

        buyBtn.onClick.RemoveAllListeners();        

        if (data.bought)
        {
            Globals.languageTable.SetText(cashCost, "already_bought");
        }
        else
        {
            buyBtn.onClick.AddListener(() => BuyTrickItem(item));
            if (data.price > 0)
            {
                cashCost.transform.parent.gameObject.SetActive(true);
                UnityEngine.UI.ColorBlock btnColors = buyBtn.colors;
                if (data.price <= Globals.self.cashAmount)
                {
                    btnColors.normalColor = UnityEngine.Color.white;
                    cashCost.color = UnityEngine.Color.white;
                }
                else
                {
                    btnColors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
                    cashCost.color = UnityEngine.Color.red;
                }
                cashCost.text = data.price.ToString();
                buyBtn.colors = btnColors;
            }
            else
            {
                Globals.languageTable.SetText(cashCost, "already_bought");
            }            
        }   
    }    

    public void HideTricksPanel()
    {        
//         TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
//         if (controller != null)
//         {
//             foreach (TrickItem item in Globals.tricksInUse)
//             {
//                 UnityEngine.UI.Button btn = item.GetComponent<UnityEngine.UI.Button>();
//                 btn.onClick.RemoveAllListeners();
//                 btn.onClick.AddListener(() => OpenItemDescriptionUI(item));        
//             }
//         }
        
        tricksInUsingPanel.SetActive(false);
    }

    public void ShowTricksPanel()
    {
        tricksInUsingPanel.SetActive(true);
        tricksInUseTip.SetActive(true);
    }

    public void BuyTrickItem(TrickItem item)
    {
        if(!item.trickData.IsLocked())
        {
            if (ChangeCash(-item.trickData.price))
            {
                Globals.self.BuyTrick(item.trickData.nameKey);
                item.Buy();
                Globals.languageTable.SetText(cashCost, "already_bought");
            }
        }
        else
        {
            Globals.tipDisplay.Msg("not_enough_roses");
        }        
    }

    public bool ChangeCash(float delta)
    {       
        float cashTemp = Globals.self.cashAmount;
        cashTemp += delta;
        if (cashTemp < 0)
        {
            Globals.tipDisplay.Msg("not_enough_cash");
            return false;
        }
        else
        {            
            float cashMax = Globals.AccumulateSafeboxCapacity(Globals.self);
            if (cashTemp > cashMax)
            {
                cashTemp = cashMax;
            }            
            Globals.self.ChangeCashAmount(cashTemp);
            UpdateCash();
            MyMazeLevelController myLevel = Globals.LevelController as MyMazeLevelController;
            if (myLevel != null)
            {
                myLevel.PutCashInBox(Globals.self);
            }
            return true;
        }
    }
}
