public class TricksBg : CustomEventTrigger
{
    UnityEngine.UI.Button TrickItemsTabBtn;
    public UnityEngine.UI.GridLayoutGroup TrickItemsLayout;
    public System.Collections.Generic.List<UnityEngine.GameObject> trickSlots = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public UIMover ClickHypnosisPointer;
    public override void Awake()
    {
        base.Awake();
        TrickItemsTabBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TrickItemsTabBtn");
        TrickItemsLayout = Globals.getChildGameObject<UnityEngine.UI.GridLayoutGroup>(gameObject, "TrickItemsLayout");        

        ClickHypnosisPointer.transform.parent.gameObject.SetActive(false);
    }

    public void CreateTrickItemsInPack()
    {
        UnityEngine.GameObject itemSlotPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItemSlot");
        UnityEngine.GameObject itemPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItem");
        ClickHypnosisPointer.transform.parent.gameObject.SetActive(false);
        foreach (TrickData data in Globals.self.tricks)
        {
            UnityEngine.GameObject itemSlot = UnityEngine.GameObject.Instantiate(itemSlotPrefab) as UnityEngine.GameObject;
            itemSlot.transform.SetParent(TrickItemsLayout.transform);
            itemSlot.transform.localScale = UnityEngine.Vector3.one;

            UnityEngine.UI.Text Unlock_label = Globals.getChildGameObject<UnityEngine.UI.Text>(itemSlot.gameObject, "Unlock_label");
            Unlock_label.gameObject.SetActive(false);
            UnityEngine.UI.Text Inventory = Globals.getChildGameObject<UnityEngine.UI.Text>(itemSlot.gameObject, "inventory");
            Inventory.gameObject.SetActive(false);
            UnityEngine.UI.Text Not_Learned = Globals.getChildGameObject<UnityEngine.UI.Text>(itemSlot.gameObject, "Not_Learned");
            Not_Learned.gameObject.SetActive(false);

            if (!data.IsInUse())
            {
                TrickItem trickItem = (UnityEngine.GameObject.Instantiate(itemPrefab) as UnityEngine.GameObject).GetComponent<TrickItem>();
                trickItem.trickData = data;

                trickItem.rt.parent = itemSlot.transform;
                trickItem.rt.localScale = UnityEngine.Vector3.one;
                trickItem.rt.localPosition = UnityEngine.Vector3.zero;
                trickItem.name = data.nameKey;                
                trickItem.CheckIfUnlock();
                trickItem.slotInPack = itemSlot;

                if (!data.IsLocked())
                {
                    Inventory.gameObject.SetActive(true);
                    Globals.languageTable.SetText(Inventory, "inventory", new System.String[] { data.inventory.ToString() });
                    trickItem.GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + data.nameKey + "_icon");
                }
                else
                {
                    trickItem.GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + data.nameKey + "_icon_disabled");

                    if (trickItem.LockImage == null)
                    {
                        Not_Learned.gameObject.SetActive(true);
                    }
                    else
                    {
                        Unlock_label.gameObject.SetActive(true);
                        Globals.languageTable.SetText(Unlock_label, "need_rose", new System.String[] { data.unlockRoseCount.ToString() });
                    }
                }
            }            
            trickSlots.Add(itemSlot);

//             // 如果在教程阶段，还没有购买催眠
//             if (data.nameKey == "dove" && Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.UnlockNewTrick)
//             {
//                 ClickHypnosisPointer.transform.parent.gameObject.SetActive(true);
//                 ClickHypnosisPointer.BlinkForever();
//             }            
        }
    }

    public int GetEmptyItemSlotIdx()
    {
        for (int idx = 0; idx < trickSlots.Count; ++idx)
        {
            if (trickSlots[idx].transform.childCount == 0)
            {
                return idx;
            }
        }
        return -1;
    }

    public UnityEngine.GameObject GetEmptyItemSlot()
    {
        int slotIdx = GetEmptyItemSlotIdx();
        return trickSlots[slotIdx];        
    }    

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        
    }

    public void Close()
    {
        Globals.canvasForMagician.itemHighLightFrame.transform.parent = transform;
        Globals.canvasForMagician.itemHighLightFrame.SetActive(false);

        foreach (UnityEngine.GameObject slot in trickSlots)
        {
            DestroyObject(slot);
        }
        trickSlots.Clear();
        gameObject.SetActive(false);
        Globals.canvasForMagician.charSelect.gameObject.SetActive(false);
        Globals.canvasForMagician.CheckIfNeedDraggingItemFinger();
        StealingLevelController controller = (Globals.LevelController as StealingLevelController);
        if (controller != null)
        {
            Globals.maze.RegistChallengerEvent();
        }
    }
}
