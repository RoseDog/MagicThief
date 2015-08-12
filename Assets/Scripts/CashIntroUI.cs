public class CashIntroUI : CustomEventTrigger
{
    MultiLanguageUIText cash;
    MultiLanguageUIText safebox_count;
    MultiLanguageUIText total_capacity;
    MultiLanguageUIText capacity;
    MultiLanguageUIText cash_averaged;
    public override void Awake()
    {
        base.Awake();
        cash = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "cash");
        safebox_count = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "safebox_count");
        total_capacity = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "total_capacity");
        capacity = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "capacity");
        cash_averaged = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "cash_averaged");
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Globals.languageTable.SetText(cash, "you_have_some_cash", new System.String[]{Globals.self.cashAmount.ToString()});
        Globals.languageTable.SetText(safebox_count, "you_have_some_safeboxes", new System.String[] { Globals.self.safeBoxDatas.Count.ToString() });
        System.String str = "=";
        foreach(SafeBoxData data in Globals.self.safeBoxDatas)
        {
            str += Globals.safeBoxLvDatas[data.Lv].capacity.ToString();
            if(Globals.self.safeBoxDatas[Globals.self.safeBoxDatas.Count-1] != data)
            {
                str += "+";
            }
        }
        capacity.text = Globals.AccumulateSafeboxCapacity(Globals.self).ToString() + str;
        Globals.languageTable.SetText(cash_averaged, "cash_average_put_in_box");
    }

	public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
    }
}
