public class NoneBuilding : Building  
{
    public UnityEngine.UI.Text bornNewTargetTimeLastText;
    public override void Start()
    {
        base.Start();
        if (data != null)
        {
            Globals.asyncLoad.AddBuildingNewTargetTimeUpdate(data);
        }        
    }

    public void FixedUpdate()
    {
        if (data != null && data.bornNewTargetLastDuration > 1.0f)
        {
            System.String str = GetBuildingTimeLastStr(data.bornNewTargetLastDuration);
            bornNewTargetTimeLastText.text = str;            
        }
        else
        {
            Globals.asyncLoad.RemoveBuildingNewTargetTimeUpdate(data);
            bornNewTargetTimeLastText.text = "";
        }
    }
}
