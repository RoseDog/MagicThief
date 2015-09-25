public class CharacterSelect : UnityEngine.MonoBehaviour 
{
    public MultiLanguageUIText Desc;    
    public MultiLanguageUIText StrengthBase;
    public MultiLanguageUIText AgilityBase;
    public MultiLanguageUIText WisdomBase;
    public UnityEngine.UI.InputField StrengthInputField;
    public UnityEngine.UI.InputField AgilityInputField;
    public UnityEngine.UI.InputField WisdomInputField;
    public MultiLanguageUIText StrengthGrowth;
    public MultiLanguageUIText AgilityGrowth;
    public MultiLanguageUIText WisdomGrowth;
    public MultiLanguageUIText TotalHpGrow;
    public MultiLanguageUIText TotalSpeedGrow;
    public MultiLanguageUIText TotalPowerGrow;
    public LifeNumber RoseNumber;
    public UnityEngine.UI.Button prev;
    public UnityEngine.UI.Button next;
	public void Awake()
    {
        StrengthInputField.onEndEdit.AddListener((str) => onStrengthEndEdit(str));
        AgilityInputField.onEndEdit.AddListener((str) => onAgilityEndEdit(str));
        WisdomInputField.onEndEdit.AddListener((str) => onWisdomEndEdit(str));
        StrengthInputField.onValidateInput = OnValidateInput;
        AgilityInputField.onValidateInput = OnValidateInput;
        WisdomInputField.onValidateInput = OnValidateInput;
    }

    public void PreviousBtn()
    {
        int mage_idx = Globals.self.selectedMagician.idx - 1;
        if (mage_idx >= 0 && Globals.self.selectedMagician != Globals.self.magicians[mage_idx])
        {
            Globals.self.SelectMagician(Globals.self.magicians[mage_idx]);
            UpdateData();
        }
    }

    public void NextBtn()
    {
        int mage_idx = Globals.self.selectedMagician.idx + 1;
        if (mage_idx < Globals.self.magicians.Count && Globals.self.selectedMagician != Globals.self.magicians[mage_idx])
        {
            Globals.self.SelectMagician(Globals.self.magicians[mage_idx]);
            UpdateData();
        }
    }

    public void UpdateData()
    {
        Globals.languageTable.SetText(StrengthBase, "strength", new System.String[] { Globals.self.selectedMagician.strengthBase.ToString("F0"), Globals.self.selectedMagician.GetStrengthDelta().ToString("F1") });
        Globals.languageTable.SetText(AgilityBase, "agility", new System.String[] { Globals.self.selectedMagician.agilityBase.ToString("F0"),
        Globals.self.selectedMagician.GetAgilityDelta().ToString("F1")});
        Globals.languageTable.SetText(WisdomBase, "wisdom", new System.String[] { Globals.self.selectedMagician.wisdomBase.ToString("F0"),
        Globals.self.selectedMagician.GetWisdomDelta().ToString("F1")});        

        StrengthInputField.text = "+" + Globals.self.selectedMagician.strengthAllot.ToString();
        AgilityInputField.text = "+" + Globals.self.selectedMagician.agilityAllot.ToString();
        WisdomInputField.text = "+" + Globals.self.selectedMagician.wisdomAllot.ToString();

        StrengthGrowth.text = Globals.self.selectedMagician.strengthGrowth.ToString("F1");
        AgilityGrowth.text = Globals.self.selectedMagician.agilityGrowth.ToString("F1");
        WisdomGrowth.text = Globals.self.selectedMagician.wisdomGrowth.ToString("F1");

        UpdateCharacterData();

        Globals.languageTable.SetText(Desc, Globals.self.selectedMagician.desc);
        RoseNumber.UpdateCurrentLife(Globals.self.roseLast.ToString(), Globals.self.roseCount, false);               
    }

    public void UpdateCharacterData()
    {
        TotalHpGrow.text = "HP + " + Globals.self.selectedMagician.GetLifeDelta();
        TotalPowerGrow.text = "MP + " + Globals.self.selectedMagician.GetPowerDelta();
        Globals.languageTable.SetText(TotalSpeedGrow, "speed_total_grow", new System.String[] { Globals.self.selectedMagician.GetNormalSpeedDelta().ToString("F1"), Globals.self.selectedMagician.GetUnlockSafeTimeDelta().ToString("F1") });
        Globals.canvasForMagician.UpdateCharacter(Globals.self);
    }

    char OnValidateInput(string text, int charIndex, char addedChar)
    {
        if(addedChar >= '0' && addedChar <= '9')
        {
            return addedChar;
        }
        return '\0';
    }

    void onStrengthEndEdit(System.String str)
    {        
        AllotRose(str, ref Globals.self.selectedMagician.strengthAllot,StrengthInputField);
    }
    void onAgilityEndEdit(System.String str)
    {
        AllotRose(str, ref Globals.self.selectedMagician.agilityAllot, AgilityInputField);
    }
    void onWisdomEndEdit(System.String str)
    {
        AllotRose(str, ref Globals.self.selectedMagician.wisdomAllot, WisdomInputField);
    }

    void AllotRose(System.String str, ref int property, UnityEngine.UI.InputField inputField)
    {
        int allot = System.Convert.ToInt32(str);
        allot = UnityEngine.Mathf.Clamp(allot, 0, Globals.self.roseLast + property);
        
        if (property != allot)
        {
            Globals.self.roseLast -= allot;
            Globals.self.roseLast += property;
            property = allot;            
            RoseNumber.UpdateCurrentLife(Globals.self.roseLast.ToString(), Globals.self.roseCount);
            UpdateCharacterData();
            Globals.self.UploadMagicianProperties();

            Globals.languageTable.SetText(StrengthBase, "strength", new System.String[] { Globals.self.selectedMagician.strengthBase.ToString("F0"), Globals.self.selectedMagician.GetStrengthDelta().ToString("F1") });
            Globals.languageTable.SetText(AgilityBase, "agility", new System.String[] { Globals.self.selectedMagician.agilityBase.ToString("F0"),
        Globals.self.selectedMagician.GetAgilityDelta().ToString("F1")});
            Globals.languageTable.SetText(WisdomBase, "wisdom", new System.String[] { Globals.self.selectedMagician.wisdomBase.ToString("F0"),
        Globals.self.selectedMagician.GetWisdomDelta().ToString("F1")});        
        }

        inputField.text = "+" + allot.ToString();
    }
}
