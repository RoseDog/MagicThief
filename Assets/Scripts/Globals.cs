public class TrickData
{
    public System.String nameKey;
    public System.String descriptionKey;
    public int duration;
    public int powerCost;
    public int unlockRoseCount;
    public int slotIdxInUsingPanel = -1;
    public int price;
    public bool bought = false;

    public bool IsLocked()
    {
        if (Globals.roseCount >= unlockRoseCount)
        {
            return false;
        }
        return true;
    }

    public bool IsInUse()
    {
        if (slotIdxInUsingPanel == -1)
        {
            return false;
        }
        return true;
    }

    public void Use(int idx)
    {
        slotIdxInUsingPanel = idx;
    }

    public void Unuse()
    {
        slotIdxInUsingPanel = -1;
    }

    public void CopyTo(TrickData data)
    {
        data.nameKey = nameKey;
        data.duration = duration;
        data.powerCost = powerCost;
    }
}

public class TrickUsingSlotData
{
    public int price;
    public bool bought = false;
}

public class GuardData
{
    public System.String name;
    public int price;
    public bool locked;
    public bool hired;
    public int unlockMazeLv;
    public int roomConsume;

    public int magicianOutVisionTime = 250;
    public int atkCd = 120;
    public int attackValue = 40;
    public float atkShortestDistance = 1.5f;
    public float rushAtShortestDistance = 1.0f;
    public int doveOutVisionTime = 100;
    public float attackSpeed = 1.0f;

    public GuardData()
    {
        
    }
}

public class MazeLvData
{
    public int roseRequire;
    public float price;
    public int roomSupport;
    public System.Collections.Generic.List<System.String> lockGuardsName;
    public bool playerEverClickGuards = false;
    public bool playerEverClickSafebox = false;
    public int safeBoxCount;
}

public class SafeBoxLvData
{
    public float price;
    public float capacity;
    public SafeBoxLvData(float p, float c)
    {
        price = p;
        capacity = c;
    }
}

public class SafeBoxData
{
    public int Lv;
    public float cashInBox;
    public bool unlocked = false;
}

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
    public static int cameraMoveDuration = 10;
    public static int uiMoveAndScaleDuration = 20;
    public static MultiLanguageTable languageTable;
    public static SelectGuard selectGuard;
    public static CanvasForMagician canvasForMagician;
    public static CanvasForMyMaze canvasForMyMaze;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static MagicThiefCamera cameraFollowMagician;
    public static MazeGenerate maze;    
    public static AsyncLoad asyncLoad;
    public static TipDisplayManager tipDisplay;
    public static Transition transition;
    public static LevelController LevelController;
    public static Magician magician;
    public static System.String iniFileName = "";
    public static float FLOOR_HEIGHT = 0.1f;
    public static System.Collections.Generic.List<System.String> AvatarAnimationEventNameCache = new System.Collections.Generic.List<System.String>();
    public static Replay replay;
    public static bool PLAY_RECORDS = false;
    public static bool DEBUG_REPLAY = false;
    // 以后要存到服务器上的
    public enum TutorialLevel
    {
        GetGem = 0,
        GetAroundGuard,
        FirstTrick,
        FirstTarget,
        InitMyMaze,
        Over
    }
    public static TutorialLevel TutorialLevelIdx = TutorialLevel.Over;
    public static float cashAmount = 80000.0f;
    public static int roseCount = 10000;
    

    public static System.String PosHolderKey = "PosHolder";
    public static System.String TargetBuildingDescriptionKey = "Description";
    public static System.Collections.Generic.List<IniFile> unclickedBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> buildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> poorsBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static System.Collections.Generic.List<IniFile> roseBuildingAchives = new System.Collections.Generic.List<IniFile>();
    public static IniFile currentStealingTargetBuildingAchive;    
    
    public static System.Collections.Generic.List<TrickData> tricks = new System.Collections.Generic.List<TrickData>();
    public static System.Collections.Generic.List<TrickItem> tricksInUse = new System.Collections.Generic.List<TrickItem>();

    public static int CurrentMazeLevel = 0;
    public static System.Collections.Generic.List<MazeLvData> mazeLvDatas = new System.Collections.Generic.List<MazeLvData>();

    public static int buySafeBoxPrice = 3000;
    public static SafeBoxLvData[] safeBoxLvDatas = new SafeBoxLvData[] { 
        new SafeBoxLvData(2000, 10000), 
        new SafeBoxLvData(5000, 15000), 
        new SafeBoxLvData(8000, 25000) };
    public static System.Collections.Generic.List<SafeBoxData> safeBoxDatas = new System.Collections.Generic.List<SafeBoxData>();
    public static System.Collections.Generic.List<GuardData> guardDatas = new System.Collections.Generic.List<GuardData>();    

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

    public static Guard CreateGuard(GuardData data, Pathfinding.Node birthNode)
    {
        UnityEngine.GameObject guard_prefab = UnityEngine.Resources.Load("Avatar/" + data.name) as UnityEngine.GameObject;
        UnityEngine.GameObject guardObject = UnityEngine.GameObject.Instantiate(guard_prefab) as UnityEngine.GameObject;
        Guard guard = guardObject.GetComponent<Guard>();
        guard.data = data;
        if (birthNode != null)
        {
            guardObject.transform.position = Globals.GetPathNodePos(birthNode);
            guard.birthNode = birthNode;
            if (guard.patrol != null)
            {
                guard.patrol.InitPatrolRoute();
            }            
        }
        LevelController.GuardCreated(guard);
        return guard;
    }

    public static T FingerRayToObj<T>(UnityEngine.Camera camera, int layermask, UnityEngine.Vector2 finger_pos) 
        where T : UnityEngine.Component
    {
        UnityEngine.RaycastHit hitInfo;
        UnityEngine.Ray ray = camera.ScreenPointToRay(finger_pos);
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {
            T ret = hitInfo.collider.gameObject.GetComponent<T>();
            if (ret == null)
            {
                ret = hitInfo.collider.gameObject.GetComponentInParent<T>();
            }
            return ret;
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
        if (guard.patrol != null)
        {
            guard.patrol.DestroyRouteCubes();
        }        
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
        T[] ts = fromGameObject.GetComponentsInChildren<T>(true);
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

    public static IniFile ReadMazeIniFile(System.String mazeIniFileName, bool readSeed)
    {
        IniFile ini = new IniFile(mazeIniFileName);
        
        if (readSeed)
        {
            UnityEngine.Random.seed = ini.get("randSeedCacheWhenEditLevel", 0);
            Globals.maze.randSeedCacheWhenEditLevel = ini.get("randSeedCacheWhenEditLevel", 0);
        }
        else
        {
            Globals.maze.randSeedCacheWhenEditLevel = UnityEngine.Random.seed;
            UnityEngine.Random.seed = Globals.maze.randSeedCacheWhenEditLevel;
        }
        
        Globals.maze.Y_CELLS_COUNT = ini.get("Z_CELLS_COUNT", 0);
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
        Globals.maze.LAMPS_COUNT = ini.get("LAMPS_COUNT", 0);
        Globals.maze.LevelTipText = ini.get("LevelTipText");

        return ini;
    }

    public static IniFile SaveMazeIniFile(System.String mazeIniFileName)
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
        ini.set("Z_CELLS_COUNT", Globals.maze.Y_CELLS_COUNT);
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
        ini.set("LAMPS_COUNT", Globals.maze.LAMPS_COUNT);        
        ini.set("LevelTipText", Globals.maze.LevelTipText);

        ini.save(mazeIniFileName);
        return ini;
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

    public static void AddEventTrigger(UnityEngine.EventSystems.EventTrigger eventTrigger,
        UnityEngine.Events.UnityAction<UnityEngine.EventSystems.PointerEventData> action, 
        UnityEngine.EventSystems.EventTriggerType triggerType)
    {
        // Create a nee TriggerEvent and add a listener
        UnityEngine.EventSystems.EventTrigger.TriggerEvent trigger = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
        UnityEngine.EventSystems.PointerEventData eventData =
            new UnityEngine.EventSystems.PointerEventData(UnityEngine.GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>());
        trigger.AddListener((data) => action(eventData)); // you can capture and pass the event data to the listener
        // Create and initialise EventTrigger.Entry using the created TriggerEvent
        UnityEngine.EventSystems.EventTrigger.Entry entry =
            new UnityEngine.EventSystems.EventTrigger.Entry() { callback = trigger, eventID = triggerType };
        // Add the EventTrigger.Entry to delegates list on the EventTrigger
        eventTrigger.delegates.Add(entry);
    }

    public static void MessageBox(System.String text, UnityEngine.Events.UnityAction yesAction = null, bool bNeedCancel = false)
    {
        UnityEngine.GameObject msgbox_prefab = UnityEngine.Resources.Load("UI/MsgBox") as UnityEngine.GameObject;
        MsgBox msgbox = (UnityEngine.GameObject.Instantiate(msgbox_prefab) as UnityEngine.GameObject).GetComponent<MsgBox>();
        msgbox.Msg(text, yesAction, bNeedCancel);
    }

    public static float AccumulateCashInBox()
    {
        float amount = 0;
        foreach (SafeBoxData data in safeBoxDatas)
        {
            amount += data.cashInBox;
        }
        return amount;
    }

    public static float AccumulateSafeboxCapacity()
    {
        float capacity = 0;
        foreach (SafeBoxData data in safeBoxDatas)
        {
            capacity += Globals.safeBoxLvDatas[data.Lv].capacity;
        }
        return capacity;
    }

    public static SafeBoxData AddSafeBox()
    {
        SafeBoxData data = new SafeBoxData();
        Globals.safeBoxDatas.Add(data);
        return data;
    }

    public static System.String StripCloneString(System.String s)
    {
        int idx = s.IndexOf("(");
        if(idx == -1)
        {
            return s;
        }
        return s.Substring(0, idx);
    }

    public static float Angle(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
    {
        float angle = UnityEngine.Vector3.Angle(a.normalized, b);
        int sign = UnityEngine.Vector3.Cross(a, b).z > 0 ? -1 : 1;
        if (sign == -1)
        {
            angle = 360 - angle;
        }
        return angle;
    }


    public static void record(System.String file, System.String content)
    {
        if (DEBUG_REPLAY)
        {
            content = " rand seed:" + UnityEngine.Random.seed.ToString() + " " + content;
            content = " frame:" + UnityEngine.Time.frameCount + content;

            System.String filename = file;
            if (!PLAY_RECORDS)
            {
                filename += "_pvp";
            }
            else
            {
                filename += "_reply";
            }

            System.IO.StreamWriter stream = new System.IO.StreamWriter(UnityEngine.Application.dataPath + "/Resources/" + filename + ".txt",
                true, System.Text.Encoding.UTF8);
            stream.WriteLine(content);
            stream.Close();
        }        
    }

    public static float Floor(float value)
    {
        int temp = (int)(value * 10);
        float ret = temp / 10.0f;
        return ret;
    }

    public static float Floor2(float value)
    {
        int temp = (int)(value * 100);
        float ret = temp / 100.0f;
        return ret;
    }

    public static GuardData GetGuardData(System.String name)
    {
        foreach(GuardData data in guardDatas)
        {
            if (data.name == name)
            {
                return data;
            }
        }
        Globals.Assert(false,"no guard named:" + name);
        return null;
    }
}