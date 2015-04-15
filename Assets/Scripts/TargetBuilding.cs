public class TargetBuilding : BuildingCouldDivedIn
{    
    public UnityEngine.UI.Text tip;
    public override void Awake()
    {        
        tip = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Tip");        
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

    public override void Choosen()
    {
        base.Choosen();
        if (tip)
        {
            city.TargetClicked(data.targetName);
        }        
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
        Globals.self.DownloadTarget(data);
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
}
