public class MultiLanguageUIText : UnityEngine.UI.Text 
{
    protected override void Awake()
    {
        base.Awake();        
    }

    protected override void Start()
    {
        base.Start();
        if (Globals.languageTable)
        {
            Globals.languageTable.SetText(this, text);
        }        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Globals.languageTable)
        {
            Globals.languageTable.SetText(this, text);
        }
    }
}
