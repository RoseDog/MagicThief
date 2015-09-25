public class RoseIntroUI : CustomEventTrigger
{
    MultiLanguageUIText rose_desc;
    MultiLanguageUIText total_capacity;
    MultiLanguageUIText capacity;
    MultiLanguageUIText cash_averaged;
    public override void Awake()
    {
        base.Awake();
        rose_desc = Globals.getChildGameObject<MultiLanguageUIText>(gameObject, "rose_desc");
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

	public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        gameObject.SetActive(false);
    }
}
