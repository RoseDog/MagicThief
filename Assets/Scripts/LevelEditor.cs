public class LevelEditor : LevelController 
{
    public override void BeforeGenerateMaze()
    {
        if (Globals.map.IniFileNameForEditor == "")
        {
            Globals.map.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
            UnityEngine.Random.seed = Globals.map.randSeedCacheWhenEditLevel;
        }
        else
        {
            mapIniFileName = Globals.map.IniFileNameForEditor;
            base.BeforeGenerateMaze();
        }        
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.selectGuard.gameObject.SetActive(true);
        Globals.map.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.map.RegistGuardArrangeEvent();
    }

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
    }
}
