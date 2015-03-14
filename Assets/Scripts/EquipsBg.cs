public class EquipsBg : CustomEventTrigger
{
    UnityEngine.UI.Button TrickItemsTabBtn;
    UnityEngine.UI.Button OutFitsTabBtn;
    public UnityEngine.UI.GridLayoutGroup TrickItemsLayout;
    public System.Collections.Generic.List<TrickItem> trickItemsInPack = new System.Collections.Generic.List<TrickItem>();
    public System.Collections.Generic.List<UnityEngine.GameObject> trickSlots = new System.Collections.Generic.List<UnityEngine.GameObject>();

    UnityEngine.UI.GridLayoutGroup OutFitsLayout;    
    public override void Awake()
    {
        base.Awake();
        TrickItemsTabBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "TrickItemsTabBtn");
        OutFitsTabBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "OutFitsTabBtn");
        TrickItemsLayout = Globals.getChildGameObject<UnityEngine.UI.GridLayoutGroup>(gameObject, "TrickItemsLayout");
        OutFitsLayout = Globals.getChildGameObject<UnityEngine.UI.GridLayoutGroup>(gameObject, "OutFitsLayout");
        OutFitsLayout.gameObject.SetActive(false);

        TrickItemsTabBtn.onClick.AddListener(() => SwichPanel());
        OutFitsTabBtn.onClick.AddListener(() => SwichPanel());        
    }

    public void CreateTrickItemsInPack()
    {
        UnityEngine.GameObject itemSlotPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItemSlot");
        UnityEngine.GameObject itemPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>("UI/TrickItem");
        
        foreach (TrickData data in Globals.tricks)
        {
            UnityEngine.GameObject itemSlot = UnityEngine.GameObject.Instantiate(itemSlotPrefab) as UnityEngine.GameObject;
            itemSlot.transform.parent = TrickItemsLayout.transform;
            itemSlot.transform.localScale = UnityEngine.Vector3.one;
            if (!data.IsInUse())
            {
                TrickItem trickItem = (UnityEngine.GameObject.Instantiate(itemPrefab) as UnityEngine.GameObject).GetComponent<TrickItem>();                
                trickItem.rt.parent = itemSlot.transform;
                trickItem.rt.localScale = UnityEngine.Vector3.one;
                trickItem.rt.localPosition = UnityEngine.Vector3.zero;
                trickItem.name = data.nameKey;
                trickItem.GetComponent<UnityEngine.UI.Image>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/" + data.nameKey + "_icon_disabled");
                trickItem.trickData = data;
                trickItem.CheckIfUnlock();
                trickItem.CheckIfPurchased();
                
                trickItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Globals.canvasForMagician.OpenItemDescriptionUI(trickItem));
                trickItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Globals.magician.CastMagic(trickItem.trickData));                
                trickItem.slotInPack = itemSlot;
                trickItemsInPack.Add(trickItem);
            }
            trickSlots.Add(itemSlot);
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

    

    public void SwichPanel()
    {
        if (OutFitsLayout.gameObject.activeSelf)
        {
            OutFitsLayout.gameObject.SetActive(false);
            OutFitsTabBtn.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/UnselectTabBtn");

            TrickItemsLayout.gameObject.SetActive(true);
            TrickItemsTabBtn.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/SelectedTabBtn");
        }
        else
        {
            OutFitsLayout.gameObject.SetActive(true);
            OutFitsTabBtn.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/SelectedTabBtn");

            TrickItemsLayout.gameObject.SetActive(false);
            TrickItemsTabBtn.image.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("UI/UnselectTabBtn");

            Globals.canvasForMagician.TrickDescParent.gameObject.SetActive(false);
        }
    }

    public override void OnTouchUpOutside(Finger f)
    {
        if (!Globals.canvasForMagician.TrickDescParent.gameObject.activeSelf)
        {
            base.OnTouchUpOutside(f);
            foreach (UnityEngine.GameObject slot in trickSlots)
            {
                DestroyObject(slot);
            }
            trickSlots.Clear();
            trickItemsInPack.Clear();
            gameObject.SetActive(false);
            Globals.canvasForMagician.TrickDescParent.gameObject.SetActive(false);
            Globals.canvasForMagician.equipBtn.gameObject.SetActive(true);
            Globals.canvasForMagician.draggingItemFinger.gameObject.SetActive(false);
            TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
            if (controller != null)
            {
                Globals.maze.RegistChallengerEvent();
            }
            else
            {
                Globals.canvasForMagician.tricksInUsingPanel.gameObject.SetActive(false);
            }
        }        
    }
}
