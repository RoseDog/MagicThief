public class TargetBuilding : BuildingCouldDivedIn
{    
    public UnityEngine.UI.Text tip;
    public override void Awake()
    {        
        tip = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Tip");        
        base.Awake();
    }

    public override void Choosen()
    {
        base.Choosen();
        if (tip)
        {
            city.TargetClicked(gameObject.name);
        }        
    }

    float fallingDuration = 0.8f;
    public override void DivedIn()
    {
        base.DivedIn();        

        Globals.EnableAllInput(false);

        Globals.currentStealingTargetBuildingAchive = buildingAchive;
        Invoke("InToBuilding", fallingDuration + 0.5f);        
    }

    void InToBuilding()
    {
        // 这是FirstTarget，所以没有tip
        if (tip == null)
        {
            Globals.Assert(Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstTarget);
        }
        else
        {
            Globals.iniFileName = gameObject.name;
        }
        
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
}
