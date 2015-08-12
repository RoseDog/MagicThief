public class RoseIntroUI : CustomEventTrigger
{
    MultiLanguageUIText rose_desc;
    MultiLanguageUIText power_delta;
    MultiLanguageUIText rose_add_power;
    MultiLanguageUIText total_capacity;
    MultiLanguageUIText capacity;
    MultiLanguageUIText cash_averaged;
    public override void Awake()
    {
        base.Awake();
        rose_desc = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "rose_desc");
        power_delta = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "power_delta");
        rose_add_power = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "rose_add_power");        
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Globals.languageTable.SetText(rose_add_power, "rose_add_power", new System.String[] { Globals.self.roseAddPowerRate.ToString("F0") });
        Globals.languageTable.SetText(power_delta, "power_delta", new System.String[] { Globals.self.GetPowerDelta().ToString() });
    }

	public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
    }
}
