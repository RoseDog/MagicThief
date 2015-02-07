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
        CheckIfPurchased();
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
                    PutItemBackInPack(itemInDraggingDownSlot, slotInPack);
                }
                trickData.Use(Globals.canvasForMagician.draggingDownSlot.index);
                Globals.tricksInUse.Add(this);
                Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
                rt.pivot = new UnityEngine.Vector2(0.0f, 0.0f);
                rt.position = Globals.canvasForMagician.draggingDownSlot.transform.position;
                rt.parent = Globals.canvasForMagician.draggingDownSlot.transform;
                Globals.canvasForMagician.equips.trickItemsInPack.Remove(this);
                Globals.canvasForMagician.draggingDownSlot.powerCost.text = trickData.powerCost.ToString();
                Globals.canvasForMagician.draggingDownSlot.PointerExit();
            }
            else if (Globals.canvasForMagician.equips.gameObject.activeSelf)
            {
                if (slotInPack == null)
                {
                    int slotIdx = Globals.canvasForMagician.equips.GetEmptyItemSlotIdx();
                    slotInPack = Globals.canvasForMagician.equips.trickSlots[slotIdx];
                    Globals.canvasForMagician.equips.trickItemsInPack.Insert(slotIdx, this);
                }
                PutItemBackInPack(this, slotInPack);                
            }
        }        
    }

    void PutItemBackInPack(TrickItem item, UnityEngine.GameObject slotInPack)
    {
        item.trickData.Unuse();
        Globals.tricksInUse.Remove(this);
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        item.rt.pivot = new UnityEngine.Vector2(0.5f, 0.5f);
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

    public void CheckIfPurchased()
    {
        UnityEngine.UI.ColorBlock colors = itemButton.colors;
        if (!trickData.bought)
        {
            colors.normalColor = new UnityEngine.Color(0.2f,0.2f,0.2f,1);
        }
        else
        {
            colors.normalColor = UnityEngine.Color.white;
        }
        itemButton.colors = colors;
    }

    public void Buy()
    {
        trickData.bought = true;
        UnityEngine.UI.ColorBlock colors = itemButton.colors;
        colors.normalColor = UnityEngine.Color.white;
        itemButton.colors = colors;
    }
}
