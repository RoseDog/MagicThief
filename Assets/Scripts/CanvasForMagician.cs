
public class CanvasForMagician : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject CashNumerBg;
    public Number cashNumber;
    public LifeNumber lifeNumber;    
    public UnityEngine.GameObject LifeNumerBg;
    public LifeNumber PowerNumber;
    public UnityEngine.GameObject PowerNumerBg;
    public Number RoseNumber;
    public UnityEngine.GameObject RoseNumberBg;
    public UnityEngine.UI.Text RestartText;    
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
    public TrickSlot[] trickInUseSlots;
    public UIMover draggingItemFinger;
	void Awake () 
    {
        DontDestroyOnLoad(this);
        Globals.canvasForMagician = this;        
        CashNumerBg = Globals.getChildGameObject(gameObject, "CashNumerBg");
        cashNumber = CashNumerBg.GetComponentInChildren<Number>();

        LifeNumerBg = Globals.getChildGameObject(gameObject, "LifeNumerBg");
        lifeNumber = Globals.getChildGameObject<LifeNumber>(LifeNumerBg, "LifeNumber");

        PowerNumerBg = Globals.getChildGameObject(gameObject, "PowerNumerBg");
        PowerNumber = Globals.getChildGameObject<LifeNumber>(PowerNumerBg, "PowerNumber"); 

        RoseNumberBg = Globals.getChildGameObject(gameObject, "RoseNumberBg"); //UnityEngine.GameObject.Find("RoseNumberBg");
        RoseNumber = RoseNumberBg.GetComponentInChildren<Number>();                      

        RestartText = UnityEngine.GameObject.Find("RestartText").GetComponent<UnityEngine.UI.Text>();        

        equipBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "EquipBtn");
        equipBtn.onClick.AddListener(()=>OpenEquipUI());
        equipUnclickedCount = Globals.getChildGameObject<UnityEngine.UI.Text>(equipBtn.gameObject, "UnclickedCount");        

        clickEquipBtnPointer = Globals.getChildGameObject<UIMover>(equipBtn.gameObject, "ClickEquipBtnPointer");

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

        draggingItemFinger = Globals.getChildGameObject<UIMover>(gameObject, "DraggingItemFinger");        
        
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(mgrs_prefab);
        }
        InitUIStats();

        SelectedImage_prefab = UnityEngine.Resources.Load("UI/guardSelectedImage") as UnityEngine.GameObject;
        
        trickInUseSlots = Globals.canvasForMagician.tricksInUsingPanel.GetComponentsInChildren<TrickSlot>();
        transform.SetAsFirstSibling();
	}

    void Start()
    {
        //InitUIStats();        
        if (Globals.tricksInUse.Count == 0 && Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstTrick)
        {
            clickEquipBtnPointer.Jump();
        }
        else
        {
            clickEquipBtnPointer.gameObject.SetActive(false);
        }

        if (trickInUseSlots[0].data == null)
        {
            TrickUsingSlotData data = new TrickUsingSlotData();
            data.price = 0;
            trickInUseSlots[0].UpdateData(data);

            data = new TrickUsingSlotData();
            data.price = 8000;
            trickInUseSlots[1].UpdateData(data);

            data = new TrickUsingSlotData();
            data.price = 20000;
            trickInUseSlots[2].UpdateData(data);

            data = new TrickUsingSlotData();
            data.price = 50000;
            trickInUseSlots[3].UpdateData(data);
        }        
    }

    void InitUIStats()
    {
        RestartText.gameObject.SetActive(false);
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

    public void OpenEquipUI()
    {
        equipBtn.gameObject.SetActive(false);

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
        if (Globals.tricksInUse.Count == 0 && Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstTrick)
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
                draggingItemFinger.ForeverMoving(3.0f);
            }            
        }
        else
        {
            draggingItemFinger.gameObject.SetActive(false);
            clickEquipBtnPointer.gameObject.SetActive(false);
        }
    }

    public void TrickItemClicked(TrickItem item)
    {
        if (Globals.canvasForMagician.draggingTrickItem != null)
        {
            return;
        }
        TrickData data = item.trickData;
        if(Globals.magician.gameObject.activeSelf)
        {
            return;
        }
        TrickDescParent.gameObject.SetActive(true);
        Globals.languageTable.SetText(TrickName, data.nameKey);
        Globals.languageTable.SetText(TrickDesc, data.descriptionKey);
        Globals.languageTable.SetText(powerLabel, "power_cost", new System.String[]{data.powerCost.ToString()});
        Globals.languageTable.SetText(durationLabel, "duration", new System.String[]{data.duration.ToString()});
        Globals.languageTable.SetText(unlockLabel, "unlock_need_rose", new System.String[] { data.unlockRoseCount.ToString() });
                
        if (!data.IsLocked())
        {
            unlockLabel.gameObject.SetActive(false);
        }
        else
        {
            unlockLabel.gameObject.SetActive(true);
            unlockLabel.color = UnityEngine.Color.red;

            buyBtn.gameObject.SetActive(true);
            UnityEngine.UI.ColorBlock colors = buyBtn.colors;
            colors.normalColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1);
            buyBtn.colors = colors;
        }

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => BuyTrickItem(item));

        if (data.bought)
        {
            buyBtn.gameObject.SetActive(false);
        }
        else
        {
            if (data.price > 0)
            {
                cashCost.transform.parent.gameObject.SetActive(true);
                UnityEngine.UI.ColorBlock btnColors = buyBtn.colors;
                if (data.price <= Globals.cashAmount)
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
                cashCost.transform.parent.gameObject.SetActive(false);
            }
            buyBtn.gameObject.SetActive(true);            
        }   
    }    

    public void HideTricksPanel()
    {        
        TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
        if (controller != null)
        {
            foreach (TrickItem item in Globals.tricksInUse)
            {
                UnityEngine.UI.Button btn = item.GetComponent<UnityEngine.UI.Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => TrickItemClicked(item));
            }
        }
        
        tricksInUsingPanel.SetActive(false);
        equipBtn.gameObject.SetActive(false);
    }

    public void ShowTricksPanel()
    {
        equipBtn.gameObject.SetActive(true);
        tricksInUsingPanel.SetActive(true);        
    }

    public void BuyTrickItem(TrickItem item)
    {
        if(!item.trickData.IsLocked())
        {
            if (ChangeCash(-item.trickData.price))
            {
                item.Buy();
                buyBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            Globals.tipDisplay.Msg("not_enough_roses");
        }        
    }

    public bool ChangeCash(int delta)
    {       
        float cashTemp = Globals.cashAmount;
        cashTemp += delta;
        if (cashTemp < 0)
        {
            Globals.tipDisplay.Msg("not_enough_cash");
            return false;
        }
        else
        {
            Globals.cashAmount = cashTemp;
            Globals.canvasForMagician.cashNumber.SetNumber(Globals.cashAmount);
            MyMazeLevelController myLevel = Globals.LevelController as MyMazeLevelController;
            if (myLevel != null)
            {
                myLevel.PutCashInBox();
            }
            return true;
        }
    }
}
