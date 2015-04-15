
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
        if (Globals.self.roseCount >= unlockRoseCount)
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
    public int idx;
    public System.String statu = "-1";
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
    public System.String[] lockGuardsName;
    public int safeBoxCount;
    public bool playerEverClickGuards = false;
    public bool playerEverClickSafebox = false;
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

public class BuildingData
{
    public System.String posID = "";
    public System.String type = "None";
    public int unpickedRose = 0;
    public System.String targetName;
    public bool everClickedTarget = false;
    public bool isPvP;
    public int pveRandSeed;
}

public class ReplayData
{
    public System.DateTime date;    
    public System.String thief;
    public System.String guard;
    public float cashAmount;
    public float StealingCash;
    public IniFile ini = new IniFile();
    public bool everClicked = false;
    public bool isPvP;
}

public class PlayerInfo
{
    public System.String separator = "|";
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
    public System.String name;
    public TutorialLevel TutorialLevelIdx = TutorialLevel.Over;
    public float cashAmount = 80000.0f;
    public int roseCount = 10000;
    
    public int currentMazeRandSeedCache;    

    public int currentMazeLevel = 0;
    public System.Collections.Generic.List<SafeBoxData> safeBoxDatas = new System.Collections.Generic.List<SafeBoxData>();
    public System.Collections.Generic.List<System.String> tricksBought = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<TrickUsingSlotData> slotsDatas = new System.Collections.Generic.List<TrickUsingSlotData>();
    public System.Collections.Generic.List<System.String> guardsHired = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<BuildingData> buildingDatas = new System.Collections.Generic.List<BuildingData>();
    public System.Collections.Hashtable replays = new System.Collections.Hashtable();
    
    public BuildingData GetBuildingDataByID(System.String idString)
    {
        foreach(BuildingData building in buildingDatas)
        {
            if(building.posID == idString)
            {
                return building;
            }
        }
        Globals.Assert(false,"no building id:" + idString);
        return null;
    }
    
    public System.String summonedGuardsStr;

    public PlayerInfo enemy;
    public BuildingData stealingTarget;

    public PlayerInfo()
    {
        
    }
    int counter = 0;
    public void SyncWithServer()
    {
        TrickUsingSlotData data = new TrickUsingSlotData();
        data.price = 0;
        data.idx = 0;
        slotsDatas.Add(data);

        data = new TrickUsingSlotData();
        data.price = 8000;
        data.idx = 1;
        slotsDatas.Add(data);

        data = new TrickUsingSlotData();
        data.price = 20000;
        data.idx = 2;
        slotsDatas.Add(data);

        for (int idx = 1; idx < 5; ++idx)
        {
            BuildingData building = new BuildingData();
            building.posID = idx.ToString();
            buildingDatas.Add(building);
        }        

        enemy = new PlayerInfo();

        Globals.socket.IsReady = false;
        if (!Globals.socket.serverReplyActions.ContainsKey("sync"))
        {
            Globals.socket.serverReplyActions.Add("sync", (reply) => SyncBack(reply));
            Globals.socket.serverReplyActions.Add("download_bought_tricks", (reply) => DownloadedBoughtTricks(reply));
            Globals.socket.serverReplyActions.Add("download_guards_hired", (reply) => DownloadedGuardsHired(reply));
            Globals.socket.serverReplyActions.Add("download_guards_summoned", (reply) => DownloadedGuardsSummoned(reply));
            Globals.socket.serverReplyActions.Add("download_safe_boxes", (reply) => DownloadedSafeBoxes(reply));
            Globals.socket.serverReplyActions.Add("download_slots_statu", (reply) => DownloadedSlotsStatu(reply));
            Globals.socket.serverReplyActions.Add("replay", (reply) => DownloadedOneReplay(reply));
            Globals.socket.serverReplyActions.Add("building", (reply) => DownloadedOneBuilding(reply));
            Globals.socket.serverReplyActions.Add("rose_grow", (reply) => RoseGrow(reply));
            Globals.socket.serverReplyActions.Add("roseBuildingEnd", (reply) => RoseBuildingEnd(reply));            
            Globals.socket.serverReplyActions.Add("new_target", (reply) => NewTarget(reply));
            Globals.socket.serverReplyActions.Add("new_poor", (reply) => NewPoor(reply));
            Globals.socket.serverReplyActions.Add("new_rosebuilding", (reply) => NewRoseBuilding(reply));
        }
        // 这个顺序根本没有保证啊...日..
        Globals.socket.Send("sync" + separator + name);
        Globals.socket.Send("download_buildings");
        Globals.socket.Send("download_bought_tricks");
        Globals.socket.Send("download_guards_hired");
        Globals.socket.Send("download_guards_summoned");
        Globals.socket.Send("download_safe_boxes");                
        Globals.socket.Send("download_slots_statu");
        Globals.socket.Send("download_replays");
    }

    public void SyncBack(System.String[] reply)
    {
        TutorialLevelIdx = (TutorialLevel)System.Enum.Parse(typeof(TutorialLevel), reply[0]);
        cashAmount = float.Parse(reply[1]);
        roseCount = int.Parse(reply[2]);
        currentMazeLevel = int.Parse(reply[3]);
        currentMazeRandSeedCache = (int)long.Parse(reply[4]);
    }

    public void DownloadedGuardsHired(System.String[] reply)
    {
        guardsHired.AddRange(reply);
    }

    public void DownloadedGuardsSummoned(System.String[] reply)
    {
        summonedGuardsStr = reply[0];
    }

    public void DownloadedBoughtTricks(System.String[] reply)
    {
        tricksBought.AddRange(reply);
    }

    public void DownloadedOneBuilding(System.String[] reply)
    {
        // poker_face
        // cat_eye_lady
        // cash_eye
        BuildingData building = GetBuildingDataByID(reply[0]);
        building.type = reply[1];
        building.unpickedRose = System.Convert.ToInt32(reply[2]);        
        building.everClickedTarget = System.Convert.ToBoolean(reply[3]);
        building.targetName = reply[4];
        building.isPvP = System.Convert.ToBoolean(reply[5]);
        building.pveRandSeed = System.Convert.ToInt32(reply[6]);
    }

    public void RoseGrow(System.String[] reply)
    {
        BuildingData building = GetBuildingDataByID(reply[0]);
        building.unpickedRose += 1;
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            city.RoseGrow(building);
        }
    }

    public void RoseBuildingEnd(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.type = "None";
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            city.UpdateBuilding(city.GetBuilding(data));
        }
    }

    public void NewTarget(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.type = "Target";
        data.everClickedTarget = false;
        data.targetName = reply[1];
        data.isPvP = System.Convert.ToBoolean(reply[2]);
        data.pveRandSeed = System.Convert.ToInt32(reply[3]);
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            city.UpdateBuilding(city.GetBuilding(data));
        }
    }

    public void NewPoor(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.type = "Poor";
        data.targetName = "";
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            city.UpdateBuilding(city.GetBuilding(data));
        }
    }

    public void NewRoseBuilding(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.type = "Rose";
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            city.UpdateBuilding(city.GetBuilding(data));
        }
    }

    public void TurnPoorToRose(BuildingData data)
    {
        Globals.socket.Send("poor_turn_to_rose" + separator + data.posID);
    }    

    public void DownloadedSlotsStatu(System.String[] reply)
    {                
        for(int idx = 0; idx < slotsDatas.Count; ++idx)
        {
            TrickUsingSlotData slot = slotsDatas[idx];
            slot.statu = reply[idx];                 
        }        
    }

    public void DownloadedSafeBoxes(System.String[] reply)
    {
        for (int idx = 0; idx < reply.Length;++idx )
        {
            SafeBoxData data = new SafeBoxData();
            safeBoxDatas.Add(data);
            data.Lv = System.Convert.ToInt32(reply[idx]);
        }
    }
    
    public void UpgradeMaze()
    {
        Globals.socket.Send("upgrade_maze" + separator + currentMazeLevel.ToString() + separator + currentMazeRandSeedCache.ToString());
    }

    public void BuyTrick(System.String trickname)
    {
        Globals.Assert(!tricksBought.Contains(trickname));
        tricksBought.Add(trickname);
        Globals.socket.Send("trick_bought" + separator + trickname);        
    }

    public void AddUsingTrick(System.String trickname, int slotIdx)
    {
        slotsDatas[slotIdx].statu = "trickname";
        Globals.socket.Send("using_trick" + separator + trickname + separator + slotIdx.ToString());        
    }

    public void RemoveUsingTrick(System.String trickname, int slotIdx)
    {        
        slotsDatas[slotIdx].statu = "0";
        Globals.socket.Send("unuse_trick" + separator + trickname);        
    }

    public void TrickSlotBought(TrickUsingSlotData data)
    {
        data.statu = "0";
        Globals.socket.Send("slot_bought" + separator + data.idx.ToString());        
    }

    public bool IsAnyTricksInUse()
    {
        foreach(TrickUsingSlotData data in slotsDatas)
        {
            if (data.statu != "0" && data.statu != "-1")
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeCashAmount(float cashTemp)
    {
        cashAmount = cashTemp;
        Globals.socket.Send("cash" + separator + cashAmount.ToString());
    }

    public void ChangeRoseCount(int rose_delta, BuildingData building)
    {
        roseCount += rose_delta;
        Globals.socket.Send("pick_rose" + separator + rose_delta.ToString() + separator + building.posID);
    }

    public SafeBoxData AddSafeBox()
    {
        SafeBoxData data = new SafeBoxData();
        safeBoxDatas.Add(data);
        Globals.socket.Send("add_safebox");
        return data;
    }

    public void UpgradeSafebox(SafeBoxData boxdata)
    {
        ++boxdata.Lv;
        int idx=0;
        for(; idx<safeBoxDatas.Count;++idx)
        {
            SafeBoxData data = safeBoxDatas[idx];
            if(data == boxdata)
            {
                break;
            }
        }
        Globals.socket.Send("upgrade_safebox" + separator + idx.ToString());
    }

    public void AdvanceTutorial()
    {
        ++TutorialLevelIdx;
    }

    public void HireGuard(GuardData guard)
    {
        guard.hired = true;
        Globals.socket.Send("hire_guard" + separator + guard.name);
    }

    public void UploadGuards()
    {
        IniFile ini = new IniFile();
        ini.clear();
        Guard[] guards = Globals.maze.guards.ToArray();
        ini.set("GuardCount", guards.Length);
        foreach (Guard guard in guards)
        {
            ini.set(Globals.GetPathNodePos(guard.birthNode).ToString("F4"), guard.gameObject.name);
        }
        summonedGuardsStr = ini.toString();
        Globals.socket.Send("upload_summoned_guards" + separator + summonedGuardsStr);
    }

    public void BuildingClicked(BuildingData building)
    {
        building.everClickedTarget = true;
        Globals.socket.Send("click_target" + separator + building.posID);
    }

    public void DownloadTarget(BuildingData data)
    {
        stealingTarget = data;        
        enemy.name = stealingTarget.targetName;
        if (data.isPvP)
        {
            Globals.socket.serverReplyActions.Add("sync_enemy", (reply) => enemy.SyncBack(reply));
            Globals.socket.serverReplyActions.Add("download_guards_summoned_enemy", (reply) => enemy.DownloadedGuardsSummoned(reply));
            Globals.socket.serverReplyActions.Add("download_safe_boxes_enemy", (reply) => enemy.DownloadedSafeBoxes(reply));
            Globals.socket.serverReplyActions.Add("download_target", (reply) => enemy.DownloadedTargetReady(reply));
            Globals.socket.Send("download_target" + separator + enemy.name);
            Globals.socket.IsReady = false;            
        }
        else if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            enemy.currentMazeRandSeedCache = data.pveRandSeed;
            Globals.iniFileName = Globals.self.enemy.name;
        }
    }

    public void DownloadedTargetReady(System.String[] reply)
    {
        Globals.iniFileName = "MyMaze_" + Globals.self.enemy.currentMazeLevel.ToString();
        Globals.socket.serverReplyActions.Remove("sync_enemy");
        Globals.socket.serverReplyActions.Remove("download_guards_summoned_enemy");
        Globals.socket.serverReplyActions.Remove("download_safe_boxes_enemy");
        Globals.socket.serverReplyActions.Remove("download_target");
        Globals.socket.IsReady = true;
    }

    public void StealingOver(float StealingCash)
    {        
        // 不在播放录像，而且的确潜入开始了的，上传这次潜入
        if (Globals.replay_key == "" && Globals.replay.mage_falling_down_frame_no != -1)
        {
            ReplayData replay = new ReplayData();
            replay.date = System.DateTime.Now;            
            replay.thief = Globals.self.name;
            replay.guard = Globals.self.enemy.name;
            replay.cashAmount = Globals.self.enemy.cashAmount;
            replay.StealingCash = StealingCash;
            replay.ini = Globals.replay.Pack();
            replay.everClicked = false;
            replay.isPvP = stealingTarget.isPvP;
            
            Globals.socket.Send(
                "stealing_over" + separator +
                replay.date + separator +
                replay.thief + separator +
                replay.guard + separator +
                replay.cashAmount.ToString("F0") + separator +
                replay.StealingCash.ToString("F0") + separator +
                replay.ini.toString() + separator +
                replay.everClicked.ToString() + separator +
                replay.isPvP.ToString() + separator +
                stealingTarget.posID);

            replays.Add(replay.date.ToString(), replay);
            Globals.canvasForMagician.ChangeCash(StealingCash);            
        }        
        Globals.transition.BlackOut(() => (Globals.LevelController as TutorialLevelController).Newsreport());
    }

    public void DownloadedOneReplay(System.String[] reply)
    {        
        ReplayData replay = new ReplayData();
        replay.date = System.Convert.ToDateTime(reply[0]);
        replay.thief = reply[1];
        replay.guard = reply[2];
        replay.cashAmount = System.Convert.ToSingle(reply[3]);
        replay.StealingCash = System.Convert.ToSingle(reply[4]);
        replay.ini.loadFromText(reply[5]);
        replay.everClicked = System.Convert.ToBoolean(reply[6]);
        replay.isPvP = System.Convert.ToBoolean(reply[7]);

        replays.Add(replay.date.ToString(), replay);
    }

    public void Replay()
    {
        ReplayData rep = replays[Globals.replay_key] as ReplayData;
        Globals.self.enemy.cashAmount = rep.cashAmount;
        Globals.replay.Unpack(rep.ini);
    }

    public void ReplayClicked(ReplayData replay)
    {
        replay.everClicked = true;
        Globals.socket.Send("click_replay" + separator + replay.date.ToString());
    }    

    public void DontNeedReply(System.String[] reply)
    {

    }
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
    public static CanvasForLogin canvasForLogin;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static MagicThiefCamera cameraFollowMagician;
    public static MazeGenerate maze;    
    public static AsyncLoad asyncLoad;
    public static TipDisplayManager tipDisplay;
    public static Transition transition;
    public static LevelController LevelController;
    public static Magician magician;
    public static ClientSocket socket;
    public static System.String iniFileName = "poker_face";
    public static float FLOOR_HEIGHT = 0.1f;
    public static System.Collections.Generic.List<System.String> AvatarAnimationEventNameCache = new System.Collections.Generic.List<System.String>();
    public static Replay replay;
    public static System.String replay_key = "";
    public static bool DEBUG_REPLAY = true;

    public static PlayerInfo self = new PlayerInfo();    

    public static System.Collections.Generic.List<TrickData> tricks = new System.Collections.Generic.List<TrickData>();
    public static System.Collections.Generic.List<MazeLvData> mazeLvDatas = new System.Collections.Generic.List<MazeLvData>();
    public static System.Collections.Generic.List<GuardData> guardDatas = new System.Collections.Generic.List<GuardData>();    
    public static int buySafeBoxPrice = 3000;
    public static SafeBoxLvData[] safeBoxLvDatas = new SafeBoxLvData[] { 
        new SafeBoxLvData(2000, 10000), 
        new SafeBoxLvData(5000, 15000), 
        new SafeBoxLvData(8000, 25000) };

    
    public static BuildingData currentStealingTargetBuildingData;
    

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

    public static IniFile ReadMazeIniFile(System.String mazeIniFileName, int seed = -1)
    {
        IniFile ini = new IniFile(mazeIniFileName);
        return ReadMazeIni(ini, seed);
    }

    public static IniFile ReadMazeIni(IniFile ini, int seed = -1)
    {
        if (seed == -1)
        {
            UnityEngine.Random.seed = ini.get("randSeedCacheWhenEditLevel", 0);
        }
        else
        {
            UnityEngine.Random.seed = seed;
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

    public static IniFile SaveMazeIniFile(System.String mazeIniFileName, int randSeedCache, bool saveGuards)
    {
        IniFile ini = new IniFile();
        ini.clear();
        if (saveGuards)
        {
            Guard[] guards = Globals.maze.guards.ToArray();
            ini.set("GuardCount", guards.Length);
            foreach (Guard guard in guards)
            {
                ini.set(Globals.GetPathNodePos(guard.birthNode).ToString("F4"), guard.gameObject.name);
            }
        }
        
        ini.set("randSeedCacheWhenEditLevel", randSeedCache);
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

        if (mazeIniFileName != "")
        {
            ini.save(mazeIniFileName);
        }        
        return ini;
    }    

    public static void UpdateUnclickedRedPointsText(UnityEngine.UI.Text redPointsText)
    {
        int unclicked_count = 0;
        foreach(BuildingData b in self.buildingDatas)
        {
            if (b.type == "Target" && !b.everClickedTarget)
            {
                ++unclicked_count;
            }            
        }

        foreach (System.Collections.DictionaryEntry entry in self.replays)
        {
            ReplayData replay = entry.Value as ReplayData;
            if (!replay.everClicked)
            {
                ++unclicked_count;
            }            
        }
        
        if (unclicked_count == 0)
        {
            redPointsText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            redPointsText.transform.parent.gameObject.SetActive(true);
            redPointsText.text = unclicked_count.ToString();
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
        MsgBox msgbox = (UnityEngine.GameObject.Instantiate(msgbox_prefab) as UnityEngine.GameObject).GetComponentInChildren<MsgBox>();
        msgbox.Msg(text, yesAction, bNeedCancel);
    }

    public static float AccumulateCashInBox(PlayerInfo player)
    {
        float amount = 0;
        foreach (SafeBoxData data in player.safeBoxDatas)
        {
            amount += data.cashInBox;
        }
        return amount;
    }

    public static float AccumulateSafeboxCapacity(PlayerInfo player)
    {
        float capacity = 0;
        foreach (SafeBoxData data in player.safeBoxDatas)
        {
            capacity += Globals.safeBoxLvDatas[data.Lv].capacity;
        }
        return capacity;
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
            content = " frame:" + (UnityEngine.Time.frameCount -  Globals.replay.frameBeginNo).ToString() + content;

            System.String filename = file;
            if (replay_key == "")
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

    public static TrickData GetTrickByName(System.String trickname)
    {
        foreach(TrickData data in tricks)
        {
            if(trickname == data.nameKey)
            {
                return data;
            }
        }
        return null;
    }

    public static int GetTrickIdx(System.String trickname)
    {
        for (int idx = 0; idx < tricks.Count; ++idx)
        {
            TrickData data = tricks[idx];
            if (trickname == data.nameKey)
            {
                return idx;
            }
        }
        return -1;
    }

    public static byte[] ConvertVector3ToByteArray(UnityEngine.Vector3 v)
    {
        var floatArray = new float[] { v.x, v.y, v.z };

        // create a byte array and copy the floats into it...
        var byteArray = new byte[floatArray.Length * 4];
        System.Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);

        return byteArray;
    }

    public static System.Collections.Generic.List<UnityEngine.Vector3> ConvertByteArrayToVector3List(byte[] bytes)
    {
        System.Collections.Generic.List<UnityEngine.Vector3> vectors = 
            new System.Collections.Generic.List<UnityEngine.Vector3>();        
        int length = bytes.Length / 4;
        for (int i = 0; i < length/3;)
        {
            UnityEngine.Vector3 vec = UnityEngine.Vector3.zero;
            vec.x = System.BitConverter.ToSingle(bytes, i * 4);
            i++;
            vec.y = System.BitConverter.ToSingle(bytes, i * 4);
            i++;
            vec.z = System.BitConverter.ToSingle(bytes, i * 4);
            i++;
            vectors.Add(vec);
        }        
        return vectors;
    }
}