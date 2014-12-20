public class LevelEditor : LevelController 
{
    public override void BeforeGenerateMaze()
    {
        if (Globals.maze.IniFileNameForEditor == "")
        {
            Globals.maze.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
            UnityEngine.Random.seed = Globals.maze.randSeedCacheWhenEditLevel;
        }
        else
        {
            mazeIniFileName = Globals.maze.IniFileNameForEditor;
            base.BeforeGenerateMaze();
        }        
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();
    }

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
    }
}
