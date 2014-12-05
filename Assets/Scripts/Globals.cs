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
    public static CanvasForMagician canvasForMagician;
    public static CanvasForLoading canvasForLoading;
    public static CanvasForLogin canvasForLogin;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static CameraFollow cameraFollowMagician;
    public static CameraForDefender cameraForDefender;
    public static MapGenerate map;    
    public static Joystick joystick;
    public static AsyncLoad asyncLoad;
    public static TipDisplayManager tipDisplay;
    public static Transition transition;
    public static LevelController pveLevelController;
    public static Magician magician;

    public static Guard CreateGuard(System.String name, Pathfinding.Node birthNode)
    {
        UnityEngine.GameObject guard_prefab = UnityEngine.Resources.Load("Avatar/" + name) as UnityEngine.GameObject;
        UnityEngine.GameObject guardObject = UnityEngine.GameObject.Instantiate(guard_prefab) as UnityEngine.GameObject;
        guardObject.name = name;
        Guard guard = guardObject.GetComponent<Guard>();
        guardObject.transform.position = Globals.GetPathNodePos(birthNode);
        guard.birthNode = birthNode;
        guard.patrol.InitPatrolRoute();
        return guard;
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

        if (canvasForLoading != null)
        {
            canvasForLoading.gameObject.SetActive(enabled);
        }

        if (canvasForLogin != null)
        {
            canvasForLogin.gameObject.SetActive(enabled);
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
}
