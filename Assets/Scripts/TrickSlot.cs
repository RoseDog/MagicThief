public class TrickSlot : CustomEventTrigger
{
    UnityEngine.RectTransform draggingDownImage;
    public TrickUsingSlotData data;
    public UnityEngine.UI.Text powerCost;
    public UnityEngine.GameObject lockImage;
    public int index;
    public UnityEngine.UI.Text cashCost;
    public TrickItem trickItem;
    public override void Awake()
    {
        base.Awake();
        btn.onClick.AddListener(() => TrickSlotClicked());
        index = System.Convert.ToInt32(gameObject.name);
        cashCost = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "CashCost");
        
        powerCost = GetComponentInChildren<UnityEngine.UI.Text>();
        powerCost.text = "";
        lockImage = Globals.getChildGameObject(gameObject, "lock");
    }

    public void UpdateData(TrickUsingSlotData d, PlayerInfo player)
    {
        data = d;
        cashCost.text = data.price.ToString();
        if (trickItem != null)
        {
            DestroyObject(trickItem.gameObject);
            trickItem = null;
        }
        if(data.statu != "-1")
        {
            Buy();
            TrickData trickData = player.GetTrickBySlotIdx(d.idx);
            if (trickData != null)
            {
                // Slot里的魔术道具
                UnityEngine.GameObject itemPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItem");                
                trickItem = (UnityEngine.GameObject.Instantiate(itemPrefab) as UnityEngine.GameObject).GetComponent<TrickItem>();
                trickItem.trickData = trickData;                
                trickItem.name = trickData.nameKey;
                trickItem.GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + trickData.nameKey + "_icon");
                trickItem.PutItemInUse(this);
                trickItem.CheckIfUnlock();
            }
        }
        else
        {
            cashCost.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(true);
        }

        if (trickItem != null)
        {
            powerCost.text = trickItem.trickData.powerCost.ToString();
        }
        else
        {
            powerCost.text = "";
        }        
    }

    public void TrickSlotClicked()
    {
        if ((Globals.stealingController == null || !Globals.stealingController.IsMagicianInStealingAction()) 
            && Globals.playingReplay == null)
        {
            // 如果是锁住的，询问是否购买
            if (data.statu == "-1")
            {
                MsgBox msgbox = Globals.MessageBox("",() => ClickToBuyTrickSlot(),true);
                Globals.languageTable.SetText(msgbox.msgBoxText, "sure_to_buy_slot", new System.String[] { data.price.ToString() });
            }
            // 否则打开装备界面
            else if (!Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
            {
                Globals.canvasForMagician.OpenTricksUI(Globals.canvasForMagician.portrait);
                Globals.canvasForMagician.SetTrickDescriptionVisible(false);
            }            
        }        
    }

    public void ClickToBuyTrickSlot()
    {
        Globals.canvasForMagician.TryMoreTricksTipHand.transform.parent.gameObject.SetActive(false);
        if (Globals.canvasForMagician.ChangeCash(-data.price))
        {
            Globals.self.TrickSlotBought(data);
            Buy();
        }
    }

    public void Buy()
    {        
        cashCost.gameObject.SetActive(false);
        lockImage.gameObject.SetActive(false);
    }    

    public bool CheckIfPointerEnter(UnityEngine.Vector2 position, UnityEngine.Camera cam)
    {
        if (lockImage.activeSelf)
        {
            return false;
        }
        UnityEngine.RectTransform rt = GetComponent<UnityEngine.RectTransform>();
        if (UnityEngine.RectTransformUtility.RectangleContainsScreenPoint(rt, position, cam))
        {
            Globals.Assert(Globals.canvasForMagician.draggingDownSlot == null);
            draggingDownImage = (UnityEngine.GameObject.Instantiate(Globals.canvasForMagician.SelectedImage_prefab) as UnityEngine.GameObject).GetComponent<UnityEngine.RectTransform>();
            draggingDownImage.sizeDelta = GetComponent<UnityEngine.RectTransform>().sizeDelta;
            draggingDownImage.parent = transform;
            draggingDownImage.anchoredPosition = UnityEngine.Vector3.zero;
            draggingDownImage.localScale = UnityEngine.Vector3.one;
            Globals.canvasForMagician.draggingDownSlot = this;
            return true;
        }
        return false;
    }

    public void CheckPointerExit(UnityEngine.Vector2 position, UnityEngine.Camera cam)
    {
        if (lockImage.activeSelf)
        {
            return ;
        }

        Globals.Assert(Globals.canvasForMagician.draggingDownSlot == this);
        UnityEngine.RectTransform rt = GetComponent<UnityEngine.RectTransform>();
        if (!UnityEngine.RectTransformUtility.RectangleContainsScreenPoint(rt, position, cam))
        {
            powerCost.text = "";
            PointerExit();
        }        
    }

    public void PointerExit()
    {
        DestroyObject(draggingDownImage.gameObject);
        Globals.canvasForMagician.draggingDownSlot = null;        
    }

    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData d)
    {
        base.OnPointerEnter(d);
        if(!Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
        {
            Globals.canvasForMagician.cast_tip.gameObject.SetActive(true);            
            MultiLanguageUIText tip = Globals.canvasForMagician.cast_tip.GetComponentInChildren<MultiLanguageUIText>();
            TrickData trick = Globals.self.GetTrickBySlotIdx(data.idx);
            if (trick != null)
            {                
                if (!trick.useShortcut)
                {
                    Globals.languageTable.SetText(tip, trick.shortDescriptionKey);
                }
                else
                {
                    System.String shortcut_str = "";
                    if (data.idx == 0)
                    {
                        shortcut_str = "Q";
                    }
                    if (data.idx == 1)
                    {
                        shortcut_str = "W";
                    }
                    if (data.idx == 2)
                    {
                        shortcut_str = "E";
                    }
                    tip.text = Globals.languageTable.GetText(trick.shortDescriptionKey) + " " + Globals.languageTable.GetText("shortcut",
                        new System.String[] { shortcut_str });
                }
            }
            else if (data.statu == "-1")
            {
                Globals.languageTable.SetText(tip, "unlock_to_bring_more_item");
            }
            else if (data.statu == "0")
            {
                Globals.languageTable.SetText(tip, "plz_bring_more_item");
            }
        }
    }

    public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData d)
    {
        base.OnPointerExit(d);
        if (!Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
        {
            Globals.canvasForMagician.cast_tip.gameObject.SetActive(false);
        }
    }
}
