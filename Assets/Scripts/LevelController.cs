public class LevelController : Actor
{
    public int randomSeed;
    protected System.String mazeIniFileName = "";    
    public override void Awake()
    {
        base.Awake();
        Globals.LevelController = this;        
    }

    public virtual void BeforeGenerateMaze()
    {
        Globals.ReadMazeIniFile(mazeIniFileName);        
    }

    public virtual void MazeFinished()
    {        
        // 创建相机，不允许跟随
        if (Globals.cameraFollowMagician == null)
        {
            UnityEngine.GameObject camera_follow_prefab = UnityEngine.Resources.Load("CameraFollowMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject camera_follow = UnityEngine.GameObject.Instantiate(camera_follow_prefab) as UnityEngine.GameObject;
        }
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);

        if (mazeIniFileName != "")
        {
            UnityEngine.Debug.Log(mazeIniFileName);
            IniFile ini = new IniFile(mazeIniFileName);
            // 关卡守卫               
            int guard_count = ini.get("GuardCount", 0);
            System.String[] keys = ini.keys();
            for (int i = 1; i <= guard_count; ++i)
            {
                UnityEngine.Vector3 pos = Globals.StringToVector3(keys[i]);
                Pathfinding.Node birthNode = Globals.maze.pathFinder.GetSingleNode(pos, false);
                if (!birthNode.walkable)
                {
                    throw new System.InvalidOperationException("guard read from file should on walkable node");
                }
                Guard guard = Globals.CreateGuard(ini.get(keys[i]), birthNode);
                guard.patrol.DestroyRouteCubes();
                guard.patrol.Excute();
            }
        }
        
        
        // 魔术师
        if (Globals.canvasForMagician == null)
        {
            UnityEngine.GameObject canvas_prefab = UnityEngine.Resources.Load("CanvasForMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject canvas = UnityEngine.GameObject.Instantiate(canvas_prefab) as UnityEngine.GameObject;
        }        
	}

    bool levelPassed = false;
    public virtual void MagicianGotCash(float value)
    {        
        if (levelPassed)
        {
            return;
        }

        Gem[] gems = UnityEngine.GameObject.FindObjectsOfType<Gem>();        
        if (gems.Length == 0)
        {            
            foreach(Chest chest in Globals.maze.chests)
            {
                if (chest.goldLast > 0)
                {
                    return;
                }
            }
            levelPassed = true;
            Invoke("LevelPassed", 0.5f);
        }
    }

    public virtual void LevelPassed()
    {
        // 这个是防止LevelPassed重复调用的，
        levelPassed = false;
    }

    public virtual void AfterMagicianFalling()
    {

    }

    public virtual void AfterMagicianSuccessedEscaped()
    {

    }

    public virtual void MagicianLifeOver()
    {
        Globals.magician.lifeOver.Excute();
    }

    public virtual void GuardCreated(Guard guard)
    {        
    }

    public virtual void GuardDestroyed(Guard guard)
    {        
    }    

    public virtual void GuardDropped(Guard guard)
    {

    }

    public virtual void GoldAllLost(Chest chest)
    {
        StopAllGuards();
    }

    void StopAllGuards()
    {
        foreach (Guard guard in Globals.maze.guards)
        {
            guard.StopAttacking();
        }
    }
}
