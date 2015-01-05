public class LevelEditor : LevelController 
{
    public override void BeforeGenerateMaze()
    {
        // 如果没有iniFile，生成新的迷宫        
        if (!System.IO.File.Exists(UnityEngine.Application.dataPath + "/Resources/" + Globals.maze.IniFileNameForEditor + ".txt"))
        {
            Globals.maze.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
            UnityEngine.Random.seed = Globals.maze.randSeedCacheWhenEditLevel;
        }
        // 如果有，就读文件
        else
        {
            Globals.iniFileName = Globals.maze.IniFileNameForEditor;
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
