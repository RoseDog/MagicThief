public class LevelEditor : LevelController 
{
    public override void BeforeGenerateMaze()
    {
        Globals.map.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
        UnityEngine.Random.seed = Globals.map.randSeedCacheWhenEditLevel;
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.selectGuard.gameObject.SetActive(true);
        Globals.canvasForMagician.tutorialText.gameObject.SetActive(false);
        Globals.map.SetRestrictToCamera(Globals.cameraForDefender);
        Globals.map.RegistGuardArrangeEvent();
    }
}
