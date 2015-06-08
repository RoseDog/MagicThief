
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject CashNumerBg;
    public UnityEngine.GameObject money_full;
    public LifeNumber cashNumber;
    CashIntroUI cashIntroUI;
    RoseIntroUI roseIntroUI;
    public LifeNumber lifeNumber;    
    public UnityEngine.GameObject LifeNumerBg;
    public LifeNumber PowerNumber;
    public UnityEngine.GameObject PowerNumerBg;
    public Number RoseNumber;
    public UnityEngine.GameObject RoseNumberBg;    
    public UnityEngine.UI.Button TrickBtn;
    public UnityEngine.UI.Text trickUnclickedCount;
    public TricksBg tricksBg;    
    public UIMover clickTrickBtnPointer;
    public UnityEngine.GameObject SelectedImage_prefab;
    public TrickItem draggingTrickItem;
    public TrickSlot draggingDownSlot;
    public UnityEngine.UI.Text TrickName;
    public UnityEngine.UI.Text TrickDesc;
    public UnityEngine.UI.Text powerLabel;
    public UnityEngine.UI.Text durationLabel;
    public UnityEngine.UI.Text unlockLabel;
    public UnityEngine.UI.Button buyTrickBtn;
    public UnityEngine.UI.Text trickCashCost;
    public UnityEngine.GameObject tricksInUsingPanel;
    public UIMover TryMoreTricksTipHand;
    public TrickSlot[] trickInUseSlots;
    public UIMover draggingItemFinger;
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;        
        CashNumerBg = Globals.getChildGameObject(gameObject, "CashNumerBg");
        CashNumerBg.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>OpenCashIntroUI());
        cashNumber = CashNumerBg.GetComponentInChildren<LifeNumber>();
        money_full = Globals.getChildGameObject(CashNumerBg, "money_full");

        cashIntroUI = Globals.getChildGameObject<CashIntroUI>(gameObject, "CashIntroUI");
        roseIntroUI = Globals.getChildGameObject<RoseIntroUI>(gameObject, "RoseIntroUI");

        //LifeNumerBg = Globals.getChildGameObject(gameObject, "LifeNumerBg");
        lifeNumber = Globals.getChildGameObject<LifeNumber>(gameObject, "LifeNumber");

        //PowerNumerBg = Globals.getChildGameObject(gameObject, "PowerNumerBg");
        PowerNumber = Globals.getChildGameObject<LifeNumber>(gameObject, "PowerNumber"); 

        RoseNumberBg = Globals.getChildGameObject(gameObject, "RoseNumberBg"); //UnityEngine.GameObject.Find("RoseNumberBg");
        RoseNumberBg.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OpenRoseIntroUI());
        RoseNumber = RoseNumberBg.GetComponentInChildren<Number>();
        
        TrickBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TrickBtn");
        TrickBtn.onClick.AddListener(()=>OpenTricksUI());        
        trickUnclickedCount = Globals.getChildGameObject<UnityEngine.UI.Text>(TrickBtn.gameObject, "UnclickedCount");
        TrickBtn.gameObject.SetActive(false);

        tricksBg = Globals.getChildGameObject<TricksBg>(gameObject, "TricksBg");


        TrickName = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Name");
        TrickDesc = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Desc");

        UnityEngine.GameObject power = Globals.getChildGameObject(tricksBg.gameObject, "PowerCost");
        powerLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(power, "label");

        UnityEngine.GameObject duration = Globals.getChildGameObject(tricksBg.gameObject, "Duration");
        durationLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(duration, "label");

        UnityEngine.GameObject unlock = Globals.getChildGameObject(tricksBg.gameObject, "Unlock");
        unlockLabel = Globals.getChildGameObject<UnityEngine.UI.Text>(unlock, "label");

        buyTrickBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(tricksBg.gameObject, "BuyBtn");        
        trickCashCost = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashCost");                        

        tricksInUsingPanel = Globals.getChildGameObject(gameObject, "TricksInUsingPanel");
        TryMoreTricksTipHand = Globals.getChildGameObject<UIMover>(tricksInUsingPanel, "TryMoreTricksTipHand");
        TryMoreTricksTipHand.transform.parent.gameObject.SetActive(false);


        draggingItemFinger = Globals.getChildGameObject<UIMover>(gameObject, "DraggingItemFinger");        
                
        InitUIStats();

        SelectedImage_prefab = UnityEngine.Resources.Load("UI/guardSelectedImage") as UnityEngine.GameObject;
        
        trickInUseSlots = tricksInUsingPanel.GetComponentsInChildren<TrickSlot>();
        transform.SetAsFirstSibling();

        clickTrickBtnPointer = Globals.getChildGameObject<UIMover>(tricksInUsingPanel.gameObject, "ClickTrickBtnPointer");
        

        highLightFrame = Globals.getChildGameObject<UnityEngine.RectTransform>(tricksInUsingPanel, "highLightFrame").gameObject;
        highLightFrame.SetActive(false);
	}
    bool initialized = false;
    public void Start()
    {
        if (!Globals.socket.IsFromLogin() && !Globals.socket.IsReady())
        {
            return;
        }
        // 如果Globals.socket.fromLoginScene = false, start可能会执行两次。
        if (initialized)
        {
            return;
        }
        initialized = true;
        if (!Globals.self.IsAnyTricksInUse() && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.FirstTrick)
        {
            clickTrickBtnPointer.Jump();
        }
        else
        {
            clickTrickBtnPointer.gameObject.SetActive(false);
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
        roseIntroUI.gameObject.SetActive(false);
    }

    void InitUIStats()
    {        
        trickUnclickedCount.gameObject.SetActive(false);
        tricksBg.gameObject.SetActive(false);
        draggingItemFinger.gameObject.SetActive(false);
    }

    public void SetLifeVisible(bool Visible)
    {
        //LifeNumerBg.SetActive(Visible);
        lifeNumber.gameObject.SetActive(Visible);
    }

    public void SetPowerVisible(bool Visible)
    {
        //PowerNumerBg.SetActive(Visible);
        PowerNumber.gameObject.SetActive(Visible);
    }

    public void SetCashVisible(bool Visible)
    {
        CashNumerBg.SetActive(Visible);
    }

    public void SetRoseVisible(bool Visible)
    {
        RoseNumberBg.SetActive(Visible);
    }

    public void UpdateCash()
    {
        float current = Globals.self.cashAmount;
        float amount = Globals.AccumulateSafeboxCapacity(Globals.self);
        if (amount - current < 0.1f)
        {
            money_full.SetActive(true);
            Globals.LevelController.MoneyFull(true);
            cashNumber.numberText.color = UnityEngine.Color.red;
        }
        else
        {
            money_full.SetActive(false);
            Globals.LevelController.MoneyFull(false);
            cashNumber.numberText.color = UnityEngine.Color.white;
        }

        cashNumber.UpdateCurrentLife(current, amount);
    }

    public void OpenCashIntroUI()
    {
        if(!Globals.magician.Stealing)
        {
            cashIntroUI.Open();
        }
    }

    public void OpenRoseIntroUI()
    {
        if (!Globals.magician.Stealing)
        {
            roseIntroUI.Open();
        }
    }

    public void OpenTricksUI()
    {
        TryMoreTricksTipHand.transform.parent.gameObject.SetActive(false);
        tricksBg.gameObject.SetActive(true);
        tricksBg.CreateTrickItemsInPack();
        
        StealingLevelController controller = (Globals.LevelController as StealingLevelController);
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
            // 如果催眠已经购买，出现拖动的提示手指
            if (tricksBg.gameObject.activeSelf)
            {
                if (Globals.tricks[0].bought)
                {
                    draggingItemFinger.gameObject.SetActive(true);
                    draggingItemFinger.RecoverPos();
                    draggingItemFinger.ClearAllActions();
                    draggingItemFinger.GetComponent<UnityEngine.CanvasGroup>().interactable = false;
                    draggingItemFinger.GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = false;
                    draggingItemFinger.ForeverMoving(100);
                }
                clickTrickBtnPointer.gameObject.SetActive(false);
            }
            else
            {
                draggingItemFinger.gameObject.SetActive(false);
                clickTrickBtnPointer.gameObject.SetActive(true);
                clickTrickBtnPointer.ClearAllActions();
                clickTrickBtnPointer.Jump();
            }
        }
        else
        {
            draggingItemFinger.gameObject.SetActive(false);            
        }
    }

    public void SetTrickDescriptionVisible(bool visible)
    {
        TrickName.gameObject.SetActive(visible);
        TrickDesc.gameObject.SetActive(visible);
        powerLabel.transform.parent.gameObject.SetActive(visible);
        durationLabel.transform.parent.gameObject.SetActive(visible);
        unlockLabel.transform.parent.gameObject.SetActive(visible);
        buyTrickBtn.gameObject.SetActive(visible);
    }

    public void UpdateItemDescriptionUI(TrickItem item)
    {
        if (Globals.canvasForMagician.draggingTrickItem != null)
        {
            return;
        }
        // 如果是闪光弹，不打开界面。闪光弹是在战斗开始前使用的
        TrickData data = item.trickData;
        City city = Globals.LevelController as City;
        if (city == null && data.nameKey == "flash_grenade" && data.IsInUse())
        {
            return;
        }
        // 如果魔术师已经降下，不打开界面        
        if(Globals.magician.gameObject.activeSelf)
        {
            return;
        }
        // 如果装备界面还没打开，首先打开装备界面
        if (!tricksBg.gameObject.activeSelf)
        {
            OpenTricksUI();
        }
        SetTrickDescriptionVisible(true);
        Globals.languageTable.SetText(TrickName, data.nameKey);
        Globals.languageTable.SetText(TrickDesc, data.descriptionKey);
        Globals.languageTable.SetText(powerLabel, "power_cost", new System.String[]{data.powerCost.ToString()});
        Globals.languageTable.SetText(durationLabel, "duration", new System.String[] { (data.duration * UnityEngine.Time.fixedDeltaTime).ToString("F1") });
        if (!data.IsLocked())
        {
            Globals.languageTable.SetText(unlockLabel, "already_unlocked");
        }
        else
        {
            Globals.languageTable.SetText(unlockLabel, "unlock_need_rose", new System.String[] { data.unlockRoseCount.ToString() });
            unlockLabel.color = UnityEngine.Color.red;

            buyTrickBtn.gameObject.SetActive(true);
            UnityEngine.UI.ColorBlock colors = buyTrickBtn.colors;
            colors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
            buyTrickBtn.colors = colors;
        }

        buyTrickBtn.onClick.RemoveAllListeners();        

        if (data.bought)
        {
            Globals.languageTable.SetText(trickCashCost, "already_bought");
            trickCashCost.color = UnityEngine.Color.green;
            buyTrickBtn.interactable = false;
        }
        else
        {
            buyTrickBtn.interactable = true;
            buyTrickBtn.onClick.AddListener(() => BuyTrickItem(item));
            if (data.price > 0)
            {
                trickCashCost.transform.parent.gameObject.SetActive(true);
                UnityEngine.UI.ColorBlock btnColors = buyTrickBtn.colors;
                if (data.price <= Globals.self.cashAmount)
                {
                    btnColors.normalColor = UnityEngine.Color.white;
                    trickCashCost.color = UnityEngine.Color.yellow;
                }
                else
                {
                    btnColors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
                    trickCashCost.color = UnityEngine.Color.red;
                }
                trickCashCost.text = data.price.ToString();
                buyTrickBtn.colors = btnColors;
            }
            else
            {
                Globals.languageTable.SetText(trickCashCost, "already_bought");
                trickCashCost.color = UnityEngine.Color.green;
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
        if(Globals.playingReplay == null && Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.GetAroundGuard
            && Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.GetGem)
        {
            tricksInUsingPanel.SetActive(true);
        }
        else
        {
            Globals.canvasForMagician.HideTricksPanel();
        }
    }

    public void BuyTrickItem(TrickItem item)
    {
        if(!item.trickData.IsLocked())
        {
            if (ChangeCash(-item.trickData.price))
            {
                Globals.self.BuyTrick(item.trickData.nameKey);
                item.Buy();
                Globals.languageTable.SetText(trickCashCost, "already_bought");
                buyTrickBtn.interactable = false;
                tricksBg.ClickHypnosisPointer.transform.parent.gameObject.SetActive(false);
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
                money_full.SetActive(true);
                Globals.LevelController.MoneyFull(true);
                cashNumber.numberText.color = UnityEngine.Color.red;
            }
            else
            {
                money_full.SetActive(false);
                Globals.LevelController.MoneyFull(false);
                cashNumber.numberText.color = UnityEngine.Color.white;
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
    public UnityEngine.GameObject highLightFrame;
    public void TrickUsingHighlightOn(TrickData data)
    {        
        for (int idx = 0; idx < trickInUseSlots.Length; ++idx)
        {
            TrickSlot slot = trickInUseSlots[idx];
            if (slot.data.statu == data.nameKey)
            {
                highLightFrame.SetActive(true);
                highLightFrame.transform.parent = slot.transform;
                highLightFrame.transform.localScale = UnityEngine.Vector3.one;
                highLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
                highLightFrame.transform.SetAsFirstSibling();
                break;
            }
        }                
    }

    public void TrickUsingHighlightOff()
    {
        if (highLightFrame != null)
        {
            highLightFrame.SetActive(false);            
        }        
    }

    public void CheckBuyTrickAndSlotTip()
    {
        if (Globals.playingReplay == null && Globals.guardPlayer.isBot && Globals.self.pveProgress <= 3)
        {
            // 易容，鸽子没购买，第一个槽没解锁
            if (!Globals.tricks[1].bought || !Globals.tricks[2].bought || Globals.self.slotsDatas[1].statu == "-1")
            {
                TryMoreTricksTipHand.ClearAllActions();
                TryMoreTricksTipHand.transform.parent.gameObject.SetActive(true);                
                TryMoreTricksTipHand.Blink();                
            }            
        }
    }

    public void OnDestroy()
    {
        UnityEngine.Debug.Log("Canvas for mage Destroyed");
    }
}
