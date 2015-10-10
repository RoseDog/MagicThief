
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public UnityEngine.UI.Text NameText;
    public UnityEngine.GameObject CashNumerBg;
    public UnityEngine.GameObject money_full;
    public LifeNumber cashNumber;
    public CashIntroUI cashIntroUI;
    public RoseIntroUI roseIntroUI;
    public UnityEngine.UI.Text Char_Name;
    public LifeNumber lifeNumber;    
    public UnityEngine.GameObject LifeNumerBg;
    public LifeNumber PowerNumber;
    public UnityEngine.GameObject PowerNumerBg;
    public Number RoseNumber;
    public UnityEngine.GameObject RoseNumberBg;    
    public UnityEngine.UI.Button TrickBtn;
    public UnityEngine.UI.Text trickUnclickedCount;
    public UnityEngine.UI.Button portrait;    
    public MultiLanguageUIText Speed;
    public MultiLanguageUIText UnlockingDuration;
    public MultiLanguageUIText TotalWeight;
    public TricksBg tricksBg;
    public CharacterSelect charSelect;
    public UIMover clickTrickBtnPointer;
    public UnityEngine.GameObject SelectedImage_prefab;
    public TrickItem draggingTrickItem;
    public bool draggingFlashGrenade = false;
    public TrickSlot draggingDownSlot;
    public UnityEngine.UI.Text TrickDesc;
    public UnityEngine.UI.Text powerLabel;
    public UnityEngine.UI.Text dropOddsLabel;
    public UnityEngine.UI.Text weightLabel;
    public UnityEngine.UI.Button buyAndLearnTrickBtn;
    public UnityEngine.UI.Text inventory_on_description;
    public UnityEngine.UI.Text trickCashCost;
    public UnityEngine.GameObject tricksInUsingPanel;
    public UIMover TryMoreTricksTipHand;
    public TrickSlot[] trickInUseSlots;
    public UIMover draggingItemFinger;
    public UIMover dropFlashFinger;
    public UnityEngine.RectTransform cast_tip;
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;        
        CashNumerBg = Globals.getChildGameObject(gameObject, "CashNumerBg");
        CashNumerBg.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>OpenCashIntroUI());
        cashNumber = CashNumerBg.GetComponentInChildren<LifeNumber>();
        money_full = Globals.getChildGameObject(CashNumerBg, "money_full");


        //LifeNumerBg = Globals.getChildGameObject(gameObject, "LifeNumerBg");
        lifeNumber = Globals.getChildGameObject<LifeNumber>(gameObject, "LifeNumber");

        //PowerNumerBg = Globals.getChildGameObject(gameObject, "PowerNumerBg");
        PowerNumber = Globals.getChildGameObject<LifeNumber>(gameObject, "PowerNumber"); 

        RoseNumberBg = Globals.getChildGameObject(gameObject, "RoseNumberBg"); //UnityEngine.GameObject.Find("RoseNumberBg");
        RoseNumberBg.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OpenRoseIntroUI());
        RoseNumber = RoseNumberBg.GetComponentInChildren<Number>();
        
        TrickBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TrickBtn");
        TrickBtn.onClick.AddListener(() => OpenTricksUI(TrickBtn));        
        trickUnclickedCount = Globals.getChildGameObject<UnityEngine.UI.Text>(TrickBtn.gameObject, "UnclickedCount");
        TrickBtn.gameObject.SetActive(false);

        tricksBg = Globals.getChildGameObject<TricksBg>(gameObject, "TricksBg");


        TrickDesc = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Desc");                

        tricksInUsingPanel = Globals.getChildGameObject(gameObject, "TricksInUsingPanel");
        TryMoreTricksTipHand = Globals.getChildGameObject<UIMover>(tricksInUsingPanel, "TryMoreTricksTipHand");
        TryMoreTricksTipHand.transform.parent.gameObject.SetActive(false);


        draggingItemFinger = Globals.getChildGameObject<UIMover>(gameObject, "DraggingItemFinger");
        dropFlashFinger = Globals.getChildGameObject<UIMover>(gameObject, "DropFlashFinger");
                
        InitUIStats();

        SelectedImage_prefab = UnityEngine.Resources.Load("UI/guardSelectedImage") as UnityEngine.GameObject;
        
        trickInUseSlots = tricksInUsingPanel.GetComponentsInChildren<TrickSlot>();
        transform.SetAsFirstSibling();

        clickTrickBtnPointer = Globals.getChildGameObject<UIMover>(tricksInUsingPanel.gameObject, "ClickTrickBtnPointer");
        

        itemHighLightFrame = Globals.getChildGameObject<UnityEngine.RectTransform>(tricksInUsingPanel, "highLightFrame").gameObject;
        itemHighLightFrame.SetActive(false);

        cast_tip.gameObject.SetActive(false);
        charSelect.gameObject.SetActive(false);
        roseIntroUI.gameObject.SetActive(false);
        cashIntroUI.gameObject.SetActive(false);

        portrait.onClick.AddListener(() => OpenTricksUI(portrait));
        
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
        NameText.text = Globals.self.name;
        if (!Globals.self.IsAnyTricksInUse() && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.FirstTrick)
        {
            clickTrickBtnPointer.Jump();
        }
        else
        {
            clickTrickBtnPointer.gameObject.SetActive(false);
        }

        // 玫瑰
        RoseNumber.SetNumber(Globals.self.roseCount);

        // 金钱
        UpdateCash();

        cashIntroUI.gameObject.SetActive(false);
        roseIntroUI.gameObject.SetActive(false);
    }

    public void UpdateTrickInUseSlots(PlayerInfo player, Magician magician)
    {
        // Slot的解锁状况
        for (int idx = 0; idx < trickInUseSlots.Length; ++idx)
        {
            TrickSlot slot = trickInUseSlots[idx];
            slot.UpdateData(player.slotsDatas[idx], player);         
        }    
    }

    public void RegisterTrickCastingFunc(PlayerInfo player, Magician magician)
    {
        for (int idx = 0; idx < trickInUseSlots.Length; ++idx)
        {
            TrickSlot slot = trickInUseSlots[idx];            
            if (magician != null && slot.trickItem != null && Globals.playingReplay == null)
            {
                slot.trickItem.itemButton.onClick.AddListener(() => magician.CastMagic(slot.trickItem.trickData));
            }
        }    
    }

    

    void InitUIStats()
    {        
        trickUnclickedCount.gameObject.SetActive(false);
        tricksBg.gameObject.SetActive(false);
        draggingItemFinger.gameObject.SetActive(false);
        dropFlashFinger.gameObject.SetActive(false);
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
            cashNumber.numberText.color = new UnityEngine.Color(1.0f, 151 / 255.0f, 71 / 255.0f);
        }

        cashNumber.UpdateCurrentLife(current.ToString("F0"), amount);
    }

    public void OpenCashIntroUI()
    {
        if (Globals.stealingController != null && !Globals.stealingController.magician.Stealing)
        {
            cashIntroUI.Open();
        }
    }

    public void OpenRoseIntroUI()
    {
        if (Globals.stealingController != null && !Globals.stealingController.magician.Stealing)
        {
            roseIntroUI.Open();
        }
    }

    public void OpenTricksUI(UnityEngine.UI.Button btn)
    {
        if (!tricksBg.gameObject.activeSelf && !(Globals.stealingController != null && Globals.stealingController.magician.Stealing) 
            && Globals.playingReplay == null)
        {
            itemHighLightFrame.SetActive(false);
            TryMoreTricksTipHand.transform.parent.gameObject.SetActive(false);
            charSelect.gameObject.SetActive(true);
            charSelect.UpdateData();
            tricksBg.gameObject.SetActive(true);
            tricksBg.CreateTrickItemsInPack();
            cast_tip.gameObject.SetActive(false);

            if(btn == portrait)
            {
                SetTrickDescriptionVisible(false);
            }


            StealingLevelController controller = (Globals.LevelController as StealingLevelController);
            if (controller != null)
            {
                Globals.maze.UnRegistChallengerEvent();
                controller.landingMark.SetActive(false);
            }

            CheckIfNeedDraggingItemFinger();
        }        
    }

    public void CheckIfNeedDraggingItemFinger()
    {
        TrickData dove = Globals.self.GetTrickByName("dove");
        if (!dove.IsInUse() && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.UnlockNewTrick && Globals.stealingController != null)
        {            
            // 如果催眠已经购买，出现拖动的提示手指
            if (tricksBg.gameObject.activeSelf)
            {
                if (!dove.IsLocked())
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
            clickTrickBtnPointer.gameObject.SetActive(false);
            draggingItemFinger.gameObject.SetActive(false);
        }

        TrickData trick = Globals.self.GetTrickByName("flashGrenade");
        if (!draggingFlashGrenade && !tricksBg.gameObject.activeSelf && trick.IsInUse() && Globals.stealingController != null && !Globals.stealingController.magician.Stealing)
        {
            dropFlashFinger.gameObject.SetActive(true);
            dropFlashFinger.transform.parent = trickInUseSlots[trick.slotIdxInUsingPanel].rectTransform;
            dropFlashFinger.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector2.zero;
            dropFlashFinger.originPosition = UnityEngine.Vector2.zero;
            dropFlashFinger.to = dropFlashFinger.originPosition - new UnityEngine.Vector3(0, -160, 0);
            dropFlashFinger.RecoverPos();
            dropFlashFinger.ClearAllActions();
            dropFlashFinger.GetComponent<UnityEngine.CanvasGroup>().interactable = false;
            dropFlashFinger.GetComponent<UnityEngine.CanvasGroup>().blocksRaycasts = false;
            dropFlashFinger.ForeverMoving(50);
        }
        else
        {
            dropFlashFinger.gameObject.SetActive(false);
        }
    }

    public void SetTrickDescriptionVisible(bool visible)
    {
        TrickDesc.gameObject.SetActive(visible);
        powerLabel.gameObject.SetActive(visible);
        inventory_on_description.gameObject.SetActive(visible);
        dropOddsLabel.gameObject.SetActive(visible);
        weightLabel.gameObject.SetActive(visible);
        //durationLabel.transform.parent.gameObject.SetActive(visible);
        buyAndLearnTrickBtn.gameObject.SetActive(visible);
    }

    public void UpdateCharacter(PlayerInfo player)
    {
        Char_Name.text = player.selectedMagician.name;
        portrait.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("Misc/" + player.selectedMagician.name + "_portrait");
        lifeNumber.UpdateText(player.selectedMagician.GetLifeAmount().ToString("F1"), player.selectedMagician.GetLifeAmount());
        PowerNumber.UpdateText(player.selectedMagician.GetPowerAmount().ToString("F1"), player.selectedMagician.GetPowerAmount());
        Globals.languageTable.SetText(Speed, "Speed", new System.String[] { player.selectedMagician.GetNormalSpeed().ToString("F1") });
        Globals.languageTable.SetText(UnlockingDuration, "Unlocking Duration", new System.String[] { player.selectedMagician.GetUnlockSafeTime().ToString("F1") });
        Globals.languageTable.SetText(TotalWeight, "TricksTotalWeight", new System.String[] { player.GetTrickTotalWeight().ToString("F1") });        
    }

    public void UpdateItemDescriptionUI(TrickItem item)
    {
        if (Globals.canvasForMagician.draggingTrickItem != null || draggingFlashGrenade)
        {
            return;
        }
  
        TrickData data = item.trickData;
        // 如果魔术师已经降下，不打开界面        
        if (Globals.stealingController != null && Globals.stealingController.magician.gameObject.activeSelf)
        {
            return;
        }
        // 如果装备界面还没打开，首先打开装备界面
        if (!tricksBg.gameObject.activeSelf)
        {
            OpenTricksUI(null);
        }
        SetTrickDescriptionVisible(true);
        TrickDesc.text = Globals.languageTable.GetText(data.nameKey) + ":\n" + Globals.languageTable.GetText(data.descriptionKey);                
        Globals.languageTable.SetText(dropOddsLabel, "drop_odds", new System.String[] { (data.dropOdds * 100).ToString("F0") });
        Globals.languageTable.SetText(weightLabel, "weight", new System.String[] { data.weight.ToString() });        
        Globals.languageTable.SetText(powerLabel, "power_cost", new System.String[]{data.powerCost.ToString()});
        Globals.languageTable.SetText(inventory_on_description, "inventory", new System.String[] { data.inventory.ToString() });
        //Globals.languageTable.SetText(durationLabel, "duration", new System.String[] { (data.duration * UnityEngine.Time.fixedDeltaTime).ToString("F1") });
        buyAndLearnTrickBtn.gameObject.SetActive(true);
        if (data.IsLocked())
        {
            UnityEngine.UI.ColorBlock colors = buyAndLearnTrickBtn.colors;
            colors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
            buyAndLearnTrickBtn.colors = colors;
        }

        buyAndLearnTrickBtn.onClick.RemoveAllListeners();
        UnityEngine.UI.ColorBlock btnColors = buyAndLearnTrickBtn.colors;
        if (!data.learned && data.learnPrice > 0)
        {
            buyAndLearnTrickBtn.onClick.AddListener(() => LearnTrickItem(item));
            Globals.languageTable.SetText(trickCashCost, "learn", new System.String[] { data.learnPrice.ToString() });

            if (data.learnPrice <= Globals.self.cashAmount)
            {
                btnColors.normalColor = UnityEngine.Color.white;
                trickCashCost.color = UnityEngine.Color.yellow;
            }
            else
            {
                btnColors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
                trickCashCost.color = UnityEngine.Color.red;
            }

            buyAndLearnTrickBtn.colors = btnColors;
        }
        else
        {
            buyAndLearnTrickBtn.onClick.AddListener(() => BuyTrickItem(item));
            Globals.languageTable.SetText(trickCashCost, "buy", new System.String[] { data.buyPrice.ToString() });

            btnColors.normalColor = UnityEngine.Color.white;
            trickCashCost.color = UnityEngine.Color.yellow;            
        }

        itemHighLightFrame.SetActive(true);
        itemHighLightFrame.transform.parent = item.rt.parent.transform;
        itemHighLightFrame.transform.localScale = UnityEngine.Vector3.one;
        itemHighLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
        itemHighLightFrame.transform.SetAsFirstSibling();
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
        if(Globals.self.TutorialLevelIdx >= PlayerInfo.TutorialLevel.Sneaking)
        {
            tricksInUsingPanel.SetActive(true);
        }
        else
        {
            Globals.canvasForMagician.HideTricksPanel();
        }
    }

    public void LearnTrickItem(TrickItem item)
    {
        if(!item.trickData.IsLocked())
        {
            if (!item.trickData.learned)
            {
                if (ChangeCash(-item.trickData.learnPrice))
                {
                    Globals.self.LearnTrick(item.trickData);
                    item.Learn();
                    UnityEngine.UI.Text Not_Learned = Globals.getChildGameObject<UnityEngine.UI.Text>(item.slotInPack.gameObject, "Not_Learned");
                    Not_Learned.gameObject.SetActive(false);

                    UnityEngine.UI.Text Inventory = Globals.getChildGameObject<UnityEngine.UI.Text>(item.slotInPack.gameObject, "inventory");
                    Inventory.gameObject.SetActive(true);
                    Globals.languageTable.SetText(Inventory, "inventory", new System.String[] { item.trickData.inventory.ToString() });
                    Globals.languageTable.SetText(trickCashCost, "buy", new System.String[] { item.trickData.buyPrice.ToString()});
                    CheckIfNeedDraggingItemFinger();
                    buyAndLearnTrickBtn.onClick.AddListener(() => BuyTrickItem(item));
                }                
            }
        }
        else
        {
            Globals.tipDisplay.Msg(Globals.languageTable.GetText("unlock_need_rose", new System.String[] { item.trickData.unlockRoseCount.ToString() }));
        }        
    }

    public void BuyTrickItem(TrickItem item)
    {
        if(!item.trickData.IsLocked())
        {
            if (ChangeCash(-item.trickData.buyPrice))
            {
                Globals.self.AddTrickItem(item.trickData);

                UnityEngine.UI.Text Inventory = Globals.getChildGameObject<UnityEngine.UI.Text>(item.slotInPack.gameObject, "inventory");
                Inventory.gameObject.SetActive(true);
                Globals.languageTable.SetText(Inventory, "inventory", new System.String[] { item.trickData.inventory.ToString() });
                Globals.languageTable.SetText(inventory_on_description, "inventory", new System.String[] { item.trickData.inventory.ToString() });

                tricksBg.ClickHypnosisPointer.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            Globals.tipDisplay.Msg(Globals.languageTable.GetText("unlock_need_rose", new System.String[] { item.trickData.unlockRoseCount.ToString() }));
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
                // orange
                cashNumber.numberText.color = new UnityEngine.Color(1.0f,151/255.0f,71/255.0f);
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
    public UnityEngine.GameObject itemHighLightFrame;
    public void TrickUsingHighlightOn(TrickData data)
    {        
        for (int idx = 0; idx < trickInUseSlots.Length; ++idx)
        {
            TrickSlot slot = trickInUseSlots[idx];
            if (slot.data.statu == data.nameKey)
            {
                itemHighLightFrame.SetActive(true);
                itemHighLightFrame.transform.parent = slot.transform;
                itemHighLightFrame.transform.localScale = UnityEngine.Vector3.one;
                itemHighLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
                itemHighLightFrame.transform.SetAsFirstSibling();
                break;
            }
        }                
    }

    public void TrickUsingHighlightOff()
    {
        if (itemHighLightFrame != null)
        {
            itemHighLightFrame.SetActive(false);            
        }        
    }

    public void CheckBuyTrickAndSlotTip()
    {
        if (Globals.playingReplay == null && Globals.guardPlayer.isBot && Globals.self.pveProgress <= 6)
        {
            // 易容，鸽子没购买，第一个槽没解锁
            if (Globals.self.tricks[1].IsLocked() || Globals.self.tricks[2].IsLocked() || Globals.self.slotsDatas[1].statu == "-1")
            {
                TryMoreTricksTipHand.ClearAllActions();
                TryMoreTricksTipHand.transform.parent.gameObject.SetActive(true);
                TryMoreTricksTipHand.BlinkForever();
            }            
        }
    }

    public void OnDestroy()
    {
        UnityEngine.Debug.Log("Canvas for mage Destroyed");
    }
}
