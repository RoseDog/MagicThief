public class TrickSlot : UnityEngine.MonoBehaviour    
{
    UnityEngine.RectTransform draggingDownImage;
    public TrickUsingSlotData data;
    UnityEngine.UI.Button btn;
    public UnityEngine.UI.Text powerCost;
    public UnityEngine.GameObject lockImage;
    public int index;
    public MultiLanguageUIText cashCost;
    public void Awake()
    {
        btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => TrickSlotClicked());
        index = System.Convert.ToInt32(gameObject.name);
        cashCost = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "CashCost");
        
        powerCost = GetComponentInChildren<UnityEngine.UI.Text>();
        powerCost.text = "";
        lockImage = Globals.getChildGameObject(gameObject, "lock");
    }

    public void UpdateData(TrickUsingSlotData d)
    {
        data = d;
        cashCost.text = data.price.ToString();
        if(data.statu != "-1")
        {
            Buy();
            if (data.statu != "0")
            {
                // Slot里的魔术道具
                UnityEngine.GameObject itemPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItem");
                TrickData trickData = Globals.GetTrickByName(data.statu);
                TrickItem trickItem = (UnityEngine.GameObject.Instantiate(itemPrefab) as UnityEngine.GameObject).GetComponent<TrickItem>();
                trickItem.trickData = trickData;                
                trickItem.name = trickData.nameKey;
                trickItem.Buy();
                trickItem.PutItemInUse(this);
                trickItem.CheckIfUnlock();
            }            
        }        
    }

    public void TrickSlotClicked()
    {
        if(!Globals.magician.Stealing)
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
                Globals.canvasForMagician.OpenTricksUI();
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
}
