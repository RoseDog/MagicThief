public class RoseBuilding : Building 
{
    UnityEngine.RectTransform peopleGivesYouRose;
    UnityEngine.RectTransform roseBtn;
    UnityEngine.GameObject rosefly_prefab;
    UnityEngine.GameObject roseIcon_prefab;
    UnityEngine.GameObject RosePickedTip_prefab;
    UnityEngine.Vector3 tipScaleCache;
    public MultiLanguageUIText rose_total;
    public MultiLanguageUIText roseTimeLastText;
    public UnityEngine.UI.Text timeLastText;
    public override void Awake()
    {
        base.Awake();
        peopleGivesYouRose = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "PeopleGivesYouRose");
        tipScaleCache = peopleGivesYouRose.transform.localScale;
        peopleGivesYouRose.localScale = UnityEngine.Vector3.zero;
        roseBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "Rose");
        roseBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => RoseClicked());
        roseBtn.GetComponent<UnityEngine.UI.Image>().enabled = false;

        rosefly_prefab = UnityEngine.Resources.Load("Props/RoseFly") as UnityEngine.GameObject;
        roseIcon_prefab = UnityEngine.Resources.Load("Props/RoseIcon") as UnityEngine.GameObject;
        RosePickedTip_prefab = UnityEngine.Resources.Load("Props/RosePickedTip") as UnityEngine.GameObject;
    }

    public override void Start()
    {
        base.Start();
        Globals.asyncLoad.AddBuildingRoseTimeUpdate(data);
        for (int idx = 0; idx < data.unpickedRose; ++idx)
        {
            RoseGrow();
        }
        int roses = (int)(data.roseGrowTotalDuration / Globals.self.roseGrowCycle) + 1;
        Globals.languageTable.SetText(rose_total, "rose_total", new System.String[] { roses.ToString() });
    }

    public override void Choosen()
    {
        if (peopleGivesYouRose.localScale.x < UnityEngine.Mathf.Epsilon)
        {
            AddAction(new Sequence(
            new ScaleTo(peopleGivesYouRose.transform, tipScaleCache*1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(peopleGivesYouRose.transform, tipScaleCache, Globals.uiMoveAndScaleDuration / 4)));
        }        
        base.Choosen();
    }

    public override void Unchoose()
    {
        AddAction(new ScaleTo(peopleGivesYouRose.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        base.Unchoose();
    }
    
    public void RoseClicked()
    {
        UnityEngine.UI.Image[] rose_icons = roseBtn.gameObject.GetComponentsInChildren<UnityEngine.UI.Image>(true);
        foreach(UnityEngine.UI.Image icon in rose_icons)
        {
            if (roseBtn.GetComponent<UnityEngine.UI.Image>() != icon)
            {
                UnityEngine.GameObject roseFly = UnityEngine.GameObject.Instantiate(rosefly_prefab) as UnityEngine.GameObject;
                roseFly.transform.position = icon.transform.position;
                FlyToScreenNumber number = roseFly.GetComponent<FlyToScreenNumber>();
                number.speed = number.speed * UnityEngine.Random.Range(0.5f, 1.3f);
                number.numberDelta = 1;
                number.ToRoseNumber();

                DestroyObject(icon.gameObject);
            }            
        }

        int delta_before_change = Globals.self.GetPowerDelta();
        Globals.self.ChangeRoseCount(data.unpickedRose, data);
        int delta_after_change = Globals.self.GetPowerDelta();

        UnityEngine.GameObject RosePickedTip = UnityEngine.GameObject.Instantiate(RosePickedTip_prefab) as UnityEngine.GameObject;

        RosePickedTip.GetComponent<UnityEngine.RectTransform>().anchoredPosition = transform.position;        
        UIMover mover = RosePickedTip.GetComponent<UIMover>();

        
        Globals.city.AddAction(new Sequence(new EaseOut(mover.transform, mover.to + transform.position, 60), 
            new FunctionCall(() => city.DestroyRosePickTip(RosePickedTip))));

        Globals.languageTable.SetText(RosePickedTip.GetComponentInChildren<MultiLanguageUIText>(), "rose_pick_tip",
            new System.String[] { data.unpickedRose.ToString(), (delta_after_change - delta_before_change).ToString() });
        data.unpickedRose = 0;

        Globals.magician.ResetLifeAndPower(Globals.self);

        Globals.canvasForMagician.PowerNumber.UpdateText(Globals.magician.PowerCurrent, Globals.magician.PowerAmount + Globals.self.GetPowerDelta());
    }

    

    public void RoseGrow()
    {
        UnityEngine.GameObject roseIcon = UnityEngine.GameObject.Instantiate(roseIcon_prefab) as UnityEngine.GameObject;
        roseIcon.GetComponent<UnityEngine.RectTransform>().parent = roseBtn.transform;
        roseIcon.GetComponent<UnityEngine.RectTransform>().anchoredPosition =
            new UnityEngine.Vector2(UnityEngine.Random.Range(-3.0f, 3.0f),UnityEngine.Random.Range(-2.5f, 1.0f));
        roseIcon.GetComponent<UnityEngine.RectTransform>().localScale = UnityEngine.Vector3.one * 0.5f;        
    }

    public void FixedUpdate()
    {
        if (data.roseGrowLastDuration > 1.0f)
        {
            System.String str = GetBuildingTimeLastStr(data.roseGrowLastDuration);
            
            timeLastText.text = str;
            Globals.languageTable.SetText(roseTimeLastText, "roseTimeLastText", new System.String[] { str });            
        }
        else
        {
            Globals.asyncLoad.RemoveBuildingRoseTimeUpdate(data);
            timeLastText.text = "";
            roseTimeLastText.text = "";
        }
    }
}
