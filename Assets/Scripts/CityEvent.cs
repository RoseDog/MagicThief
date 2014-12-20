public class CityEvent : Actor 
{
    public UnityEngine.RectTransform rectTransform;
    public UnityEngine.UI.Text uiText;
    public UnityEngine.UI.Text newText;

    public override void Awake()
    {        
        base.Awake();
        rectTransform = GetComponent<UnityEngine.RectTransform>();
        uiText = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Text");
        newText = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "New");
    }

    public void Clicked()
    {
        //newText.enabled = false;
    }
}
