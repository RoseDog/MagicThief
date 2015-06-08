
public class TipDisplayManager : UnityEngine.MonoBehaviour
{
    public System.Collections.Generic.List<UnityEngine.RectTransform> msgList = new System.Collections.Generic.List<UnityEngine.RectTransform>();    
    UnityEngine.GameObject tipPrefab;
    // Use this for initialization
    void Start()
    {
        Globals.tipDisplay = this;
        tipPrefab = UnityEngine.Resources.Load("UI/Tip") as UnityEngine.GameObject;        
    }

    public void Msg(string msg, float height_ratio = 0.25f, UnityEngine.Transform parent = null)
    {
        Tip tip = (Instantiate(tipPrefab) as UnityEngine.GameObject).GetComponent<Tip>();
        UnityEngine.RectTransform tipTransform = tip.GetComponent<UnityEngine.RectTransform>();
        if (parent == null)
        {
            UnityEngine.Canvas[] canvases = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.Canvas>();
            UnityEngine.Canvas topPriorityCanvas = canvases[0];
            
            foreach (UnityEngine.Canvas canvas in canvases)
            {
                if (canvas.sortingOrder > topPriorityCanvas.sortingOrder)
                {
                    topPriorityCanvas = canvas;
                }
            }
            tipTransform.SetParent(topPriorityCanvas.transform);
        }
        else
        {
            tipTransform.SetParent(parent);
        }
        
        tipTransform.SetAsLastSibling();
        tipTransform.localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
                
        Globals.languageTable.SetText(tip.uiText, msg);
        msgList.Add(tipTransform);
        float tip_y_pos = UnityEngine.Screen.height * height_ratio;
        for (int idx = msgList.Count - 1; idx >= 0; --idx )
        {
            if (msgList[idx] != null)
            {
                msgList[idx].localPosition = new UnityEngine.Vector3(0.0f, tip_y_pos, 0.0f);
                tip_y_pos += msgList[idx].rect.height;
            }            
        }
     }
}