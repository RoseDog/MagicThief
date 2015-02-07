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
        btn.onClick.AddListener(() => ClickToBuyTrickSlot());
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
        if (data.bought || index == 0)
        {
            Buy();
        }
    }

    public void ClickToBuyTrickSlot()
    {
        if (!data.bought && Globals.canvasForMagician.ChangeCash(-data.price))
        {
            Buy();
        }
    }

    public void Buy()
    {
        data.bought = true;
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
