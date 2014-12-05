public class LevelController : UnityEngine.MonoBehaviour
{
    public int randomSeed;
    public virtual void Awake()
    {
        Globals.pveLevelController = this;
    }

    public virtual void Start()
    {

    }

    public virtual void MazeFinished()
    {
        IniFile ini = new IniFile(UnityEngine.Application.loadedLevelName);
        
        int guard_count = ini.get("GuardCount",0);
        System.String[] keys = ini.keys();
        for (int i = 1; i <= guard_count; ++i )
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

        // 魔术师出场
        UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
        UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;

        Cell magician_birth = Globals.map.entryOfMaze;
        magician.name = "Mage_Girl";
        magician.transform.position = magician_birth.GetFloorPos();        
	}

    public virtual void GotGem(int value)
    {

    }

    public virtual void MagicianLifeOver()
    {
        Globals.magician.lifeOver.Excute();
    }
}
