public class TargetBuilding : BuildingCouldDivedIn
{    
    public UnityEngine.UI.Text tip;
    UnityEngine.RectTransform intro;
    UnityEngine.Vector3 introScaleCache;
    public override void Awake()
    {        
        tip = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Tip");
        intro = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "intro");
        if (intro != null)
        {
            introScaleCache = intro.transform.localScale;
            intro.transform.localScale = UnityEngine.Vector3.zero;
        }
        
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        if (data != null)
        {
            if (data.isPvP)
            {
                tip.text = data.targetName;
                
            }
            else
            {
                Globals.languageTable.SetText(tip, data.targetName);                
            }
            tip.text = "<color=red>Lv." + data.maze_lv.ToString() + "</color> " + tip.text;
        }
        
        //spriteSheet.AddAnim("idle", 5, 0.2f, false);
        //spriteSheet.Play("idle");
    }  

    public override void Choosen()
    {
        base.Choosen();
        if (intro != null)
        {
            AddAction(new Sequence(
            new ScaleTo(intro.transform, introScaleCache * 1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(intro.transform, introScaleCache, Globals.uiMoveAndScaleDuration / 4)));
            if (data.isPvP)
            {
                Globals.languageTable.SetText(intro.GetComponentInChildren<UnityEngine.UI.Text>(), "pvp_intro", new System.String[] { data.maze_lv.ToString() });
            }
            else
            {
                Globals.languageTable.SetText(intro.GetComponentInChildren<UnityEngine.UI.Text>(), "pve_intro", new System.String[] { data.maze_lv.ToString() });
            }            
        
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
        }        
        base.Unchoose();
    }

    public override void DivedIn()
    {
        base.DivedIn();        

        Globals.EnableAllInput(false);

        Globals.currentStealingTargetBuildingData = data;
        Globals.thiefPlayer = Globals.self;
        Globals.self.DownloadTarget(data);
        Globals.asyncLoad.ToLoadSceneAsync("StealingLevel");
    }    
}
