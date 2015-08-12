
public class TipDisplayManager : UnityEngine.MonoBehaviour
{
    public System.Collections.Generic.List<UnityEngine.RectTransform> msgList = new System.Collections.Generic.List<UnityEngine.RectTransform>();    
    UnityEngine.GameObject tipPrefab;
    // Use this for initialization
    void Awake()
    {
        Globals.tipDisplay = this;
        tipPrefab = UnityEngine.Resources.Load("UI/Tip") as UnityEngine.GameObject;
    }
    void OnLevelWasLoaded(int scene_id)
    {
        if (msgList.Count != 0)
        {
            foreach(UnityEngine.RectTransform rt in msgList)
            {
                if (rt != null)
                {
                    DestroyObject(rt.gameObject);
                }                
            }
            msgList.Clear();
        }
    }

    public void Msg(string msg, float height_ratio = 0.25f, UnityEngine.Transform parent = null)
    {
        Tip tip = (Instantiate(tipPrefab) as UnityEngine.GameObject).GetComponentInChildren<Tip>();
        UnityEngine.RectTransform tipTransform = tip.GetComponent<UnityEngine.RectTransform>();
           
        tipTransform.parent.SetAsLastSibling();
               
        Globals.languageTable.SetText(tip.uiText, msg);
        msgList.Add(tipTransform);
        float tip_y_pos = UnityEngine.Screen.height * height_ratio;
        for (int idx = msgList.Count - 1; idx >= 0; --idx )
        {
            if (msgList[idx] != null)
            {
                msgList[idx].localPosition = new UnityEngine.Vector3(0, tip_y_pos, 0.0f);
                tip_y_pos += msgList[idx].rect.height;
            }            
        }
     }
}