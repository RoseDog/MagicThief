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
    public static float cameraMoveDuration = 0.3f;
    public static float uiMoveAndScaleDuration = 0.5f;
    public static SelectGuard selectGuard;
    public static CanvasForMagician canvasForMagician;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static CameraFollow cameraFollowMagician;
    public static MapGenerate map;    
    public static Joystick joystick;
    public static AsyncLoad asyncLoad;
    public static TipDisplayManager tipDisplay;
    public static Transition transition;
    public static LevelController LevelController;
    public static Magician magician;
    public static int LevelIdx = 0;

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
        UnityEngine.Object.DestroyImmediate(guard.canvasForCommandBtns.gameObject);
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

    public static void ReadIniFile(System.String mapIniFileName)
    {
        IniFile ini = new IniFile(mapIniFileName);
        UnityEngine.Random.seed = ini.get("randSeedCacheWhenEditLevel", 0);
        Globals.map.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
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
        Globals.map.LevelTipText = ini.get("LevelTipText");        
    }
}
