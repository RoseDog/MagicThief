
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

    public void Msg(string msg)
    {
        Tip tip = (Instantiate(tipPrefab) as UnityEngine.GameObject).GetComponent<Tip>();
        UnityEngine.RectTransform tipTransform = tip.GetComponent<UnityEngine.RectTransform>();
        tipTransform.SetParent(Globals.LevelController.mainCanvas.transform);        
        tipTransform.SetAsLastSibling();
        tipTransform.localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
        
        // left
        tipTransform.position = new UnityEngine.Vector3(0.0f, UnityEngine.Screen.height*0.3f, 0.0f);
        // right
        //tipTransform.sizeDelta = new UnityEngine.Vector2(0.0f, tipTransform.sizeDelta.y);

        Globals.languageTable.SetText(tip.uiText, msg);
        msgList.Add(tipTransform);
        float tip_y_pos = UnityEngine.Screen.height * 0.5f;
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