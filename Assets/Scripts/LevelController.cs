public class LevelController : Actor
{
    public int randomSeed;
    public Chest[] chests;
    public override void Awake()
    {
        base.Awake();
        Globals.LevelController = this;
    }

    public virtual void BeforeGenerateMaze()
    {
        IniFile ini = new IniFile(UnityEngine.Application.loadedLevelName);
        UnityEngine.Random.seed = ini.get("randSeedCacheWhenEditLevel", 0);
        Globals.map.Z_CELLS_COUNT = ini.get("Z_CELLS_COUNT", 0);
        Globals.map.X_CELLS_COUNT = ini.get("X_CELLS_COUNT", 0);
        Globals.map.CHANGE_DIRECTION_MODIFIER = ini.get("CHANGE_DIRECTION_MODIFIER", 0);
        Globals.map.sparsenessModifier = ini.get("sparsenessModifier", 0);
        Globals.map.deadEndRemovalModifier = ini.get("deadEndRemovalModifier", 0);
        Globals.map.noOfRoomsToPlace = ini.get("noOfRoomsToPlace", 0);
        Globals.map.minRoomXCellsCount = ini.get("minRoomXCellsCount", 0);
        Globals.map.maxRoomXCellsCount = ini.get("maxRoomXCellsCount", 0);
        Globals.map.minRoomZCellsCount = ini.get("minRoomZCellsCount", 0);
        Globals.map.maxRoomZCellsCount = ini.get("maxRoomZCellsCount", 0);
        Globals.map.GEMS_COUNT = ini.get("GEMS_COUNT", 0);
    }

    public virtual void MazeFinished()
    {
        IniFile ini = new IniFile(UnityEngine.Application.loadedLevelName);
        
        int guard_count = ini.get("GuardCount",0);
        System.String[] keys = ini.keys();
        for (int i = 1; i <= guard_count; ++i)
        {
            UnityEngine.Vector3 pos = Globals.StringToVector3(keys[i]);
            Pathfinding.Node birthNode = Globals.map.pathFinder.GetSingleNode(pos, false);
            if (!birthNode.walkable)
            {
                throw new System.InvalidOperationException("guard read from file should on walkable node");
            }
            Guard guard = Globals.CreateGuard(ini.get(keys[i]), birthNode);
            guard.patrol.DestroyRouteCubes();
            guard.patrol.Excute();
        }

        if (Globals.canvasForMagician == null)
        {
            UnityEngine.GameObject canvas_prefab = UnityEngine.Resources.Load("CanvasForMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject canvas = UnityEngine.GameObject.Instantiate(canvas_prefab) as UnityEngine.GameObject;
        }

        chests = UnityEngine.GameObject.FindObjectsOfType<Chest>();
	}

    bool levelPassed = false;
    public virtual void GotGem(float value)
    {
        if (levelPassed)
        {
            return;
        }

        Gem[] gems = UnityEngine.GameObject.FindObjectsOfType<Gem>();        
        if (gems.Length == 0)
        {            
            foreach(Chest chest in chests)
            {
                if (chest.goldAmount > 0)
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

    }

    public virtual void MagicianLifeOver()
    {
        Globals.magician.lifeOver.Excute();
    }

    public virtual void GuardDropped(Guard guard)
    {

    }

    public virtual void GoldAllLost(Chest chest)
    {

    }
}
