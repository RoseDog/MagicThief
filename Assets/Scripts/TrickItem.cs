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
        itemButton.onClick.AddListener(() => Globals.canvasForMagician.UpdateItemDescriptionUI(this));        
    }

    
    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        if(trickData.inventory == 0)
        {
            Globals.tipDisplay.Msg("inventory zero");
            return;
        }

        if(!trickData.IsLocked() && trickData.learned && Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
        {
            UnityEngine.UI.Text Inventory = Globals.getChildGameObject<UnityEngine.UI.Text>(transform.parent.gameObject, "inventory");
            if (Inventory)
            {
                Inventory.gameObject.SetActive(false);
            }
            
            rt.parent = Globals.canvasForMagician.transform;
            rt.pivot = new UnityEngine.Vector2(0.5f, 0.5f);
            Globals.canvasForMagician.draggingTrickItem = this;
        }

        if (Globals.LevelController as StealingLevelController != null && 
            trickData.nameKey == "flash_grenade" && !Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
        {
            Globals.canvasForMagician.draggingFlashGrenade = true;
            Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
            UnityEngine.Cursor.SetCursor(UnityEngine.Resources.Load("UI/flash_grenade_cursor") as UnityEngine.Texture2D, new UnityEngine.Vector2(0,0), UnityEngine.CursorMode.ForceSoftware);                        
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
        if (Globals.canvasForMagician.draggingFlashGrenade)
        {
            bool cast = true;
            // 如果在slot的范围内，不释放
            foreach (TrickSlot slot in Globals.canvasForMagician.trickInUseSlots)
            {
                UnityEngine.RectTransform rt = GetComponent<UnityEngine.RectTransform>();
                if (UnityEngine.RectTransformUtility.RectangleContainsScreenPoint(rt, data.position, data.pressEventCamera))
                {
                    cast = false;
                }
            }
            
            if (cast)
            {
                
                if (Globals.stealingController != null && trickData.nameKey == "flash_grenade")
                {
                    if (Globals.stealingController.magician.ChangePower(-trickData.powerCost))
                    {
                        UnityEngine.Vector3 finger_pos = UnityEngine.Camera.main.ScreenToWorldPoint(data.position);
                        finger_pos.z = Globals.stealingController.landingMark.transform.position.z;
                        Globals.replaySystem.RecordFlash(finger_pos);
                        Globals.stealingController.magician.CastFlash(finger_pos);
                        Globals.stealingController.LeaveBtn.gameObject.SetActive(false);
                    }
                }
            }

            UnityEngine.Cursor.SetCursor(null, new UnityEngine.Vector2(0, 0), UnityEngine.CursorMode.Auto);
            Globals.canvasForMagician.draggingFlashGrenade = false;
            Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        }

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
                
                Globals.self.UsingTrick(trickData,
                    Globals.canvasForMagician.draggingDownSlot.data.idx);
                PutItemInUse(Globals.canvasForMagician.draggingDownSlot);                
                Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();                                                
                Globals.canvasForMagician.draggingDownSlot.PointerExit();
                Globals.canvasForMagician.tricksBg.trickItemsInPack.Remove(this);
            }
            else if (Globals.canvasForMagician.tricksBg.gameObject.activeSelf)
            {
                Globals.canvasForMagician.tricksBg.trickItemsInPack.Insert(Globals.self.GetTrickIdx(trickData.nameKey), this);
                PutItemBackInPack(this);                
            }
        }        
    }

    public void PutItemInUse(TrickSlot slot)
    {
        rt.SetParent(slot.transform);
        rt.SetAsFirstSibling();
        Globals.canvasForMagician.itemHighLightFrame.transform.SetAsFirstSibling();
        rt.anchoredPosition = UnityEngine.Vector3.zero;
        rt.localScale = UnityEngine.Vector3.one;
        slot.powerCost.text = trickData.powerCost.ToString();
        slot.trickItem = this;
        Globals.canvasForMagician.UpdateCharacter(Globals.self);
    }

    public void PutItemBackInPack(TrickItem item)
    {
        UnityEngine.GameObject slotInPack =
            Globals.canvasForMagician.tricksBg.trickSlots[Globals.self.GetTrickIdx(item.trickData.nameKey)];

        if (item.trickData.IsInUse())
        {
            Globals.self.RemoveUsingTrick(item.trickData);
        }
                
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        Globals.canvasForMagician.UpdateCharacter(Globals.self);
        item.rt.parent = slotInPack.transform;
        item.rt.anchoredPosition = UnityEngine.Vector3.zero;

        UnityEngine.UI.Text Inventory = Globals.getChildGameObject<UnityEngine.UI.Text>(slotInPack.gameObject, "inventory");
        Inventory.gameObject.SetActive(true);
        Globals.languageTable.SetText(Inventory, "inventory", new System.String[] { trickData.inventory.ToString() });        
    }

    public void CheckIfUnlock()
    {
        if (!trickData.IsLocked())
        {
            DestroyObject(LockImage);
            LockImage = null;
        }                        
    }

    public void Learn()
    {        
//         UnityEngine.UI.ColorBlock colors = itemButton.colors;
//         colors.normalColor = UnityEngine.Color.white;
//         itemButton.colors = colors;

        GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + trickData.nameKey + "_icon");
    }
}
