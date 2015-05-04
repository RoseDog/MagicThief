public class TargetBuilding : BuildingCouldDivedIn
{    
    public UnityEngine.UI.Text tip;
    UnityEngine.RectTransform introBtn;
    UnityEngine.Vector3 introBtnScaleCache;
    UnityEngine.RectTransform intro;
    UnityEngine.Vector3 introScaleCache;
    public override void Awake()
    {        
        tip = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Tip");

        introBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "introBtn");
        if (introBtn != null)
        {
            introBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TargetIntro());
            introBtnScaleCache = introBtn.transform.localScale;
            introBtn.gameObject.SetActive(false);

            intro = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "intro");
            introScaleCache = intro.transform.localScale;
            intro.localScale = UnityEngine.Vector3.zero;            
        }
        
        
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        if (data != null)
        {
            Globals.languageTable.SetText(tip, data.targetName);
            city.eventsWindow.AddEvent(this);
        }        
    }

    public void TargetIntro()
    {
        AddAction(new Sequence(
            new ScaleTo(intro.transform, introScaleCache * 1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(intro.transform, introScaleCache, Globals.uiMoveAndScaleDuration / 4)));
        if(data.isPvP)
        {
            Globals.languageTable.SetText(intro.GetComponentInChildren<UnityEngine.UI.Text>(), "pvp_intro");
        }
        else
        {
            Globals.languageTable.SetText(intro.GetComponentInChildren<UnityEngine.UI.Text>(), "pve_intro");
        }
        
    }

    public override void Choosen()
    {
        base.Choosen();
        if (intro != null)
        {
            introBtn.gameObject.SetActive(true);
            introBtn.transform.localScale = UnityEngine.Vector3.zero;
            AddAction(new Sequence(
                new ScaleTo(introBtn.transform, introBtnScaleCache * 1.2f, Globals.uiMoveAndScaleDuration / 3),
                new ScaleTo(introBtn.transform, introBtnScaleCache, Globals.uiMoveAndScaleDuration / 4)));
            if (tip)
            {
                city.TargetClicked(data.targetName);
            }    
        }
            
    }

    public override void Unchoose()
    {
        if (intro != null)
        {
            AddAction(new ScaleTo(intro.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));

            AddAction(new ScaleTo(introBtn.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        }        
        base.Unchoose();
    }

    public override void DivedIn()
    {
        base.DivedIn();        

        Globals.EnableAllInput(false);

        Globals.currentStealingTargetBuildingData = data;
        Invoke("InToBuilding", 0.5f);        
    }

    void InToBuilding()
    {
        Globals.thiefPlayer = Globals.self;
        Globals.self.DownloadTarget(data);
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
}
