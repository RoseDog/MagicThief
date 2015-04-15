public class TrickItem : UnityEngine.MonoBehaviour,
    UnityEngine.EventSystems.IBeginDragHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IEndDragHandler 
{
    public TrickData trickData;
    public UnityEngine.RectTransform rt;
    public UnityEngine.GameObject slotInPack;
    public UnityEngine.GameObject LockImage;
    public UnityEngine.UI.Button itemButton;
    public void Awake()
    {
        rt = GetComponent<UnityEngine.RectTransform>();
        itemButton = GetComponent<UnityEngine.UI.Button>();
        LockImage = Globals.getChildGameObject(gameObject, "Lock");        
    }

    public void Start()
    {
        itemButton.onClick.AddListener(() => Globals.canvasForMagician.OpenItemDescriptionUI(this));
        itemButton.onClick.AddListener(() => Globals.magician.CastMagic(trickData));
    }

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        if(!trickData.IsLocked() && trickData.bought && Globals.canvasForMagician.equips.gameObject.activeSelf)
        {
            rt.parent = Globals.canvasForMagician.transform;
            rt.pivot = new UnityEngine.Vector2(0.5f, 0.5f);
            Globals.canvasForMagician.draggingTrickItem = this;
        }        
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        if (Globals.canvasForMagician.draggingTrickItem == this)
        {
            UnityEngine.Vector3 globalMousePos;
            if (UnityEngine.RectTransformUtility.ScreenPointToWorldPointInRectangle(
                Globals.canvasForMagician.GetComponent<UnityEngine.RectTransform>(),
                data.position, data.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos;

                if (Globals.canvasForMagician.draggingDownSlot != null)
                {
                    Globals.canvasForMagician.draggingDownSlot.CheckPointerExit(globalMousePos, data.pressEventCamera);
                }

                foreach (TrickSlot slot in Globals.canvasForMagician.trickInUseSlots)
                {
                    if (slot != Globals.canvasForMagician.draggingDownSlot && slot.CheckIfPointerEnter(globalMousePos, data.pressEventCamera))
                    {
                        break;
                    }
                }
            }
        }        
    }

    public void OnEndDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        if (Globals.canvasForMagician.draggingTrickItem == this)
        {
            Globals.canvasForMagician.draggingTrickItem = null;
            if (Globals.canvasForMagician.draggingDownSlot != null)
            {
                TrickItem itemInDraggingDownSlot = Globals.canvasForMagician.draggingDownSlot.GetComponentInChildren<TrickItem>();
                if (itemInDraggingDownSlot != null)
                {
                    PutItemBackInPack(itemInDraggingDownSlot);
                }
                PutItemInUse(Globals.canvasForMagician.draggingDownSlot);
                Globals.self.AddUsingTrick(trickData.nameKey, 
                    Globals.canvasForMagician.draggingDownSlot.data.idx);
                Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();                                                
                Globals.canvasForMagician.draggingDownSlot.PointerExit();
                Globals.canvasForMagician.equips.trickItemsInPack.Remove(this);
            }
            else if (Globals.canvasForMagician.equips.gameObject.activeSelf)
            {                
                Globals.canvasForMagician.equips.trickItemsInPack.Insert(Globals.GetTrickIdx(trickData.nameKey), this);
                PutItemBackInPack(this);                
            }
        }        
    }

    public void PutItemInUse(TrickSlot slot)
    {
        trickData.Use(slot.index);
        rt.parent = slot.transform;
        rt.anchoredPosition = UnityEngine.Vector3.zero;
        rt.localScale = UnityEngine.Vector3.one;
        slot.powerCost.text = trickData.powerCost.ToString();
    }

    void PutItemBackInPack(TrickItem item)
    {
        UnityEngine.GameObject slotInPack =
            Globals.canvasForMagician.equips.trickSlots[Globals.GetTrickIdx(item.trickData.nameKey)];

        if (item.trickData.IsInUse())
        {
            Globals.self.RemoveUsingTrick(item.trickData.nameKey, item.trickData.slotIdxInUsingPanel);
            item.trickData.Unuse();
        }
                
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        item.rt.parent = slotInPack.transform;
        item.rt.anchoredPosition = UnityEngine.Vector3.zero;
    }

    public void CheckIfUnlock()
    {
        if (!trickData.IsLocked())
        {
            DestroyObject(LockImage);            
        }                        
    }

    public void Buy()
    {
        trickData.bought = true;
//         UnityEngine.UI.ColorBlock colors = itemButton.colors;
//         colors.normalColor = UnityEngine.Color.white;
//         itemButton.colors = colors;

        GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + trickData.nameKey + "_icon");
    }
}
