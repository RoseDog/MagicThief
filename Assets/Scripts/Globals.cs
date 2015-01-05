public class Globals
{
    public static readonly System.String EAST = "E";
    public static readonly System.String SOUTH = "S";
    public static readonly System.String WEST = "W";
    public static readonly System.String NORTH = "N";
    public static readonly System.String[] DIRECTIONS = { EAST, SOUTH, WEST, NORTH };
    public static bool SHOW_MACE_GENERATING_PROCESS = false;
    public static bool SHOW_ROOMS = false;
    public static float CREATE_MAZE_TIME_STEP = 0.1f;
    public static float cameraMoveDuration = 1.2f;
    public static float uiMoveAndScaleDuration = 0.5f;
    public static SelectGuard selectGuard;
    public static CanvasForMagician canvasForMagician;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static CameraFollow cameraFollowMagician;
    public static MazeGenerate maze;    
    public static Joystick joystick;
    public static AsyncLoad asyncLoad;
    public static TipDisplayManager tipDisplay;
    public static Transition transition;
    public static LevelController LevelController;
    public static Magician magician;
    public static System.String iniFileName = "";    
    // 以后要存到服务器上的
    public enum TutorialLevel
    {
        FirstFalling = 0,
        Chest,
        Guard,
        MagicianBorn,
        InitMaze,
        Over
    }
    public static TutorialLevel TutorialLevelIdx = TutorialLevel.FirstFalling;
    public static float cashAmount;
    public static System.Collections.Generic.List<IniFile> unclickedBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> buildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> poorsBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> roseBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static IniFile currentStealingTargetBuildingAchive;

    public static System.String PosHolderKey = "PosHolder";
    public static System.String TargetBuildingDescriptionKey = "Description";
    public static System.Collections.Generic.List<System.String> AvatarAnimationEventNameCache = new System.Collections.Generic.List<System.String>();

    public static void AddNewTargetBuildingAchives(System.Collections.Generic.List<IniFile> newAchives)
    {
        // 添加到现有NewTargets列表中
        unclickedBuildingAchives.AddRange(newAchives);
    }

    public static void NewTargetBuildingClicked(System.String desc)
    {
        foreach (IniFile file in unclickedBuildingAchives)
        {
            if(file.get("Description") == desc)
            {
                unclickedBuildingAchives.Remove(file);
                buildingAchives.Add(file);
                return;
            }            
        }
        Assert(false);
    }

    public static void AddPoorBuildingAchives(IniFile achive)
    {
        Assert(unclickedBuildingAchives.Contains(achive) || buildingAchives.Contains(achive));
        Assert(!(unclickedBuildingAchives.Contains(achive) && buildingAchives.Contains(achive)));
        Assert(!(!unclickedBuildingAchives.Contains(achive) && !buildingAchives.Contains(achive)));
        Assert(!poorsBuildingAchives.Contains(achive));
        buildingAchives.Remove(achive);
        unclickedBuildingAchives.Remove(achive);
        poorsBuildingAchives.Add(achive);
    }

    public static void AddRoseBuilding(IniFile achive)
    {
        Assert(!unclickedBuildingAchives.Contains(achive));
        Assert(!buildingAchives.Contains(achive));
        Assert(poorsBuildingAchives.Contains(achive));
        Assert(!roseBuildingAchives.Contains(achive));
        poorsBuildingAchives.Remove(achive);
        roseBuildingAchives.Add(achive);
    }

    public static void Assert(bool boolean, System.String msg = "")
    {
        if (!boolean)
        {
            UnityEngine.Debug.LogError(UnityEngine.StackTraceUtility.ExtractStackTrace());
            UnityEngine.Debug.LogError(msg);
        }
    }

    public static Guard CreateGuard(System.String name, Pathfinding.Node birthNode)
    {
        UnityEngine.GameObject guard_prefab = UnityEngine.Resources.Load("Avatar/" + name) as UnityEngine.GameObject;
        UnityEngine.GameObject guardObject = UnityEngine.GameObject.Instantiate(guard_prefab) as UnityEngine.GameObject;
        guardObject.name = name;
        Guard guard = guardObject.GetComponent<Guard>();
        if (birthNode != null)
        {
            guardObject.transform.position = Globals.GetPathNodePos(birthNode);
            guard.birthNode = birthNode;
            guard.patrol.InitPatrolRoute();
        }
        LevelController.GuardCreated(guard);
        return guard;
    }

    public static T FingerRayToObj<T>(UnityEngine.Camera camera, int layer, Finger finger) 
        where T : UnityEngine.Component
    {
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << layer;
        UnityEngine.Ray ray = camera.ScreenPointToRay(finger.nowPosition);
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {
            return hitInfo.collider.gameObject.GetComponent<T>();
        }
        return null;
    }

    public static void ReadGlobalInfo()
    {
//         string path = Path.Combine(Application.persistentDataPath, "GlobalInfo.txt");
//         StreamReader sr = new StreamReader(path, false);
//         playerName = sr.ReadLine();
//         unlockedLevels = Convert.ToInt32(sr.ReadLine());
//         Globals.money = System.Convert.ToSingle(sr.ReadLine());
//         Globals.skillPoints = System.Convert.ToInt32(sr.ReadLine());
//         sr.Close();
    }

    public static void SaveGlobalInfo()
    {
//         string path = Path.Combine(Application.persistentDataPath, "GlobalInfo.txt");
//         StreamWriter sw = new StreamWriter(path, false);
//         sw.WriteLine(playerName);
//         sw.WriteLine(unlockedLevels.ToString());
//         sw.WriteLine(money.ToString());
//         sw.WriteLine(skillPoints.ToString());
//         sw.Flush();
//         sw.Close();
    }

    static public void DestroyGuard(Guard guard)
    {
        guard.patrol.DestroyRouteCubes();
        LevelController.GuardDestroyed(guard);        
        UnityEngine.Object.DestroyImmediate(guard.gameObject);
    }

    static public UnityEngine.GameObject getChildGameObject(UnityEngine.GameObject fromGameObject, System.String withName)
    {
        UnityEngine.Transform ts = fromGameObject.transform.FindChild(withName);
        if (ts)
        {
            return ts.gameObject;
        }

        return null;
    }

    static public T getChildGameObject<T>(UnityEngine.GameObject fromGameObject, System.String withName)
        where T : UnityEngine.Component
    {
        //Author: Isaac Dart, June-13.
        T[] ts = fromGameObject.GetComponentsInChildren<T>();
        foreach (T child in ts)
        {
            if (child.gameObject.name == withName)
                return child;
        }

        return null;
    }

    static public UnityEngine.Vector3 GetPathNodePos(Pathfinding.Node node)
    {
        return new UnityEngine.Vector3(node.position.x / 1000.0f,
            node.position.y / 1000.0f, node.position.z / 1000.0f);
    }

    static public void EnableAllInput(bool enabled)
    {
        if (canvasForMagician != null)
        {
            canvasForMagician.gameObject.SetActive(enabled);
        }

        if (joystick != null)
        {
            joystick.MannullyActive(enabled);
        }        
        input.enabled = enabled;
    }
    

    static public UnityEngine.Vector3 StringToVector3(System.String value)
    {
        string[] temp = value.Substring(1, value.Length - 2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
        return new UnityEngine.Vector3(x, y, z);    
    }

    public static bool Vector3AlmostEqual(UnityEngine.Vector3 v1, UnityEngine.Vector3 v2, float precision)
    {
        bool equal = true;
        if (UnityEngine.Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (UnityEngine.Mathf.Abs(v1.y - v2.y) > precision) equal = false;
        if (UnityEngine.Mathf.Abs(v1.z - v2.z) > precision) equal = false;
        return equal;
    }

    public static void ReadMazeIniFile(System.String mazeIniFileName)
    {
        IniFile ini = new IniFile(mazeIniFileName);
        UnityEngine.Random.seed = ini.get("randSeedCacheWhenEditLevel", 0);
        Globals.maze.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
        Globals.maze.Z_CELLS_COUNT = ini.get("Z_CELLS_COUNT", 0);
        Globals.maze.X_CELLS_COUNT = ini.get("X_CELLS_COUNT", 0);
        Globals.maze.CHANGE_DIRECTION_MODIFIER = ini.get("CHANGE_DIRECTION_MODIFIER", 0);
        Globals.maze.sparsenessModifier = ini.get("sparsenessModifier", 0);
        Globals.maze.deadEndRemovalModifier = ini.get("deadEndRemovalModifier", 0);
        Globals.maze.noOfRoomsToPlace = ini.get("noOfRoomsToPlace", 0);
        Globals.maze.minRoomXCellsCount = ini.get("minRoomXCellsCount", 0);
        Globals.maze.maxRoomXCellsCount = ini.get("maxRoomXCellsCount", 0);
        Globals.maze.minRoomZCellsCount = ini.get("minRoomZCellsCount", 0);
        Globals.maze.maxRoomZCellsCount = ini.get("maxRoomZCellsCount", 0);
        Globals.maze.GEMS_COUNT = ini.get("GEMS_COUNT", 0);
        Globals.maze.LevelTipText = ini.get("LevelTipText");        
    }

    public static void SaveMazeIniFile(System.String mazeIniFileName)
    {
        IniFile ini = new IniFile();
        ini.clear();
        Guard[] guards = Globals.maze.guards.ToArray();
        ini.set("GuardCount", guards.Length);
        foreach (Guard guard in guards)
        {
            ini.set(Globals.GetPathNodePos(guard.birthNode).ToString("F4"), guard.gameObject.name);
        }
        ini.set("randSeedCacheWhenEditLevel", Globals.maze.randSeedCacheWhenEditLevel);
        ini.set("Z_CELLS_COUNT", Globals.maze.Z_CELLS_COUNT);
        ini.set("X_CELLS_COUNT", Globals.maze.X_CELLS_COUNT);
        ini.set("CHANGE_DIRECTION_MODIFIER", Globals.maze.CHANGE_DIRECTION_MODIFIER);
        ini.set("sparsenessModifier", Globals.maze.sparsenessModifier);
        ini.set("deadEndRemovalModifier", Globals.maze.deadEndRemovalModifier);
        ini.set("noOfRoomsToPlace", Globals.maze.noOfRoomsToPlace);
        ini.set("minRoomXCellsCount", Globals.maze.minRoomXCellsCount);
        ini.set("maxRoomXCellsCount", Globals.maze.maxRoomXCellsCount);
        ini.set("minRoomZCellsCount", Globals.maze.minRoomZCellsCount);
        ini.set("maxRoomZCellsCount", Globals.maze.maxRoomZCellsCount);
        ini.set("GEMS_COUNT", Globals.maze.GEMS_COUNT);
        ini.set("LevelTipText", Globals.maze.LevelTipText);

        ini.save(mazeIniFileName);
    }    

    public static void UpdateUnclickedRedPointsText(UnityEngine.UI.Text redPointsText)
    {
        redPointsText.text = unclickedBuildingAchives.Count.ToString();
        if (Globals.unclickedBuildingAchives.Count == 0)
        {
            redPointsText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            redPointsText.transform.parent.gameObject.SetActive(true);
        }
    }

    public static System.String GetOppositeDir(System.String direction)
    {
        if (direction == Globals.EAST)
        {
            return Globals.WEST;
        }
        else if (direction == Globals.SOUTH)
        {
            return Globals.NORTH;
        }
        else if (direction == Globals.WEST)
        {
            return Globals.EAST;
        }
        else if (direction == Globals.NORTH)
        {
            return Globals.SOUTH;
        }

        return "";
    }
}
