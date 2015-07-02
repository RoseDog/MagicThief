
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
    public bool clickOnGuardToCast = false;

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
    public PlayerInfo owner;
}

public class BuildingData
{
    public System.String posID = "";
    public System.String type = "None";
    public int unpickedRose = 0;
    public System.String targetName;
    public bool everClickedTarget = false;
    public bool isPvP;
    public int levelRandSeed;
    public int PvELevelIdx;
}

public class ReplayData
{
    public System.DateTime date;        
    public float StealingCash;
    public IniFile ini = new IniFile();
    public bool everClicked = false;
    public PlayerInfo thief = new PlayerInfo();
    public PlayerInfo guard = new PlayerInfo();
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
    public TutorialLevel TutorialLevelIdx = TutorialLevel.GetGem;
    public float cashAmount = 80000.0f;
    public int roseCount = 10000;
    
    public int currentMazeRandSeedCache;    
    public int currentMazeLevel = 0;

    public int pveProgress;

    public System.Collections.Generic.List<SafeBoxData> safeBoxDatas = new System.Collections.Generic.List<SafeBoxData>();
    public System.Collections.Generic.List<System.String> tricksBought = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<TrickUsingSlotData> slotsDatas = new System.Collections.Generic.List<TrickUsingSlotData>();
    public System.Collections.Generic.List<System.String> guardsHired = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<BuildingData> buildingDatas = new System.Collections.Generic.List<BuildingData>();
    public System.Collections.Hashtable defReplays = new System.Collections.Hashtable();
    public System.Collections.Hashtable atkReplays = new System.Collections.Hashtable();
    
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


    public BuildingData stealingTarget;
    public bool isBot = false;

    public PlayerInfo()
    {
        
    }
    int counter = 0;
    public void SyncWithServer()
    {
        defReplays.Clear();
        atkReplays.Clear();

        slotsDatas.Clear();
        TrickUsingSlotData data = new TrickUsingSlotData();
        data.price = 0;
        data.idx = 0;
        slotsDatas.Add(data);

        data = new TrickUsingSlotData();
        data.price = 500;
        data.idx = 1;
        slotsDatas.Add(data);

        data = new TrickUsingSlotData();
        data.price = 32000;
        data.idx = 2;
        slotsDatas.Add(data);

        buildingDatas.Clear();
        for (int idx = 1; idx < 5; ++idx)
        {
            BuildingData building = new BuildingData();
            building.posID = idx.ToString();
            buildingDatas.Add(building);
        }        

        Globals.playersOnRank.Clear();

        Globals.socket.SetReady(false);
        if (!Globals.socket.serverReplyActions.ContainsKey("self_stealing_info"))
        {
            Globals.socket.serverReplyActions.Add("self_stealing_info", (reply) => SelfStealingInfo(reply));            
            
            Globals.socket.serverReplyActions.Add("buildings_ready", (reply) => BuildingsOver(reply));
            Globals.socket.serverReplyActions.Add("replays_ready", (reply) => ReplaysOver(reply));            

            Globals.socket.serverReplyActions.Add("download_one_rank", (reply) => OneRank(reply));            
            Globals.socket.serverReplyActions.Add("download_ranks_over", (reply) => PlayersRankOver(reply));
            Globals.socket.serverReplyActions.Add("download_one_other_replay", (reply) => OneOtherReplay(reply));
            Globals.socket.serverReplyActions.Add("download_other_replays_over", (reply) => OtherReplaysOver(reply));
                        
            Globals.socket.serverReplyActions.Add("replay", (reply) => OneDefReplay(reply));
            Globals.socket.serverReplyActions.Add("atk_replay", (reply) => OneAtkReplay(reply));
            
            Globals.socket.serverReplyActions.Add("building", (reply) => OneBuilding(reply));
            Globals.socket.serverReplyActions.Add("rose_grow", (reply) => RoseGrow(reply));
            Globals.socket.serverReplyActions.Add("roseBuildingEnd", (reply) => RoseBuildingEnd(reply));            
            Globals.socket.serverReplyActions.Add("new_target", (reply) => NewTarget(reply));
            Globals.socket.serverReplyActions.Add("new_poor", (reply) => NewPoor(reply));
            Globals.socket.serverReplyActions.Add("new_rosebuilding", (reply) => NewRoseBuilding(reply));
        }
        // 这个顺序根本没有保证�?..�?.
        Globals.socket.Send("self_stealing_info" + separator + name);        
    }

    public void SelfStealingInfo(System.String[] reply)
    {
        StealingInfo(reply[0]);

        if(this == Globals.self)
        {
            if (TutorialLevelIdx != TutorialLevel.Over)
            {
                Globals.guardPlayer = new PlayerInfo();
                Globals.guardPlayer.isBot = true;
                Globals.thiefPlayer = Globals.self;
            }

            if (Globals.socket.IsFromLogin())
            {                
                if (TutorialLevelIdx == TutorialLevel.GetGem || 
                    TutorialLevelIdx == TutorialLevel.GetAroundGuard || 
                    TutorialLevelIdx == TutorialLevel.FirstTrick)
                {
                    Globals.asyncLoad._ToLoadingScene("StealingLevel");
                }
                else if (TutorialLevelIdx == TutorialLevel.FirstTarget || TutorialLevelIdx == TutorialLevel.Over)
                {
                    Globals.asyncLoad._ToLoadingScene("City");
                }
                else if (TutorialLevelIdx == TutorialLevel.InitMyMaze)
                {
                    Globals.asyncLoad._ToLoadingScene("MyMaze");
                }                
            }
            else
            {
                Globals.socket.StartCoroutine("WaitingForSyncRead");
            }

            Globals.socket.Send("download_buildings");
        }        
    }

    public void StealingInfo(System.String info)
    {
        System.String[] reply = info.Split('&');

        TutorialLevelIdx = (TutorialLevel)System.Enum.Parse(typeof(TutorialLevel), reply[0]);
        cashAmount = float.Parse(reply[1]);
        roseCount = int.Parse(reply[2]);
        currentMazeLevel = int.Parse(reply[3]);
        currentMazeRandSeedCache = (int)long.Parse(reply[4]);

        System.String[] temp;
        temp = reply[5].Split(',');
        for (int idx = 0; idx < slotsDatas.Count; ++idx)
        {
            TrickUsingSlotData slot = slotsDatas[idx];
            // 由于字符串拼接问题，temp[0]是空字符�?
            slot.statu = temp[idx+1];
        }

        temp = reply[6].Split(',');
        for (int idx = 1; idx < temp.Length; ++idx)
        {            
            SafeBoxData data = new SafeBoxData();
            data.owner = this;
            safeBoxDatas.Add(data);
            data.Lv = System.Convert.ToInt32(temp[idx]);
        }

        temp = reply[7].Split(',');
        for (int idx = 1; idx < temp.Length; ++idx)
        {
            guardsHired.Add(temp[idx]);
        }

        summonedGuardsStr = reply[8];

        temp = reply[9].Split(',');
        for (int idx = 1; idx < temp.Length; ++idx)
        {
            tricksBought.Add(temp[idx]);
        }

        isBot = System.Convert.ToBoolean(reply[10]);

        name = reply[11];

        pveProgress = System.Convert.ToInt32(reply[12]);
    }

    void BuildingsOver(System.String[] reply)
    {
        Globals.socket.Send("download_replays");
    }
   
    void ReplaysOver(System.String[] reply)
    {
        Globals.socket.Send("download_ranks");
    }
    
    void OneRank(System.String[] reply)
    {
        Globals.playerDownloading = new PlayerInfo();
        Globals.playerDownloading.name = reply[0];
        Globals.playerDownloading.roseCount = System.Convert.ToInt32(reply[1]);
        Globals.playersOnRank.Add(Globals.playerDownloading);
    }

    void PlayersRankOver(System.String[] reply)
    {
        Globals.socket.SetReady(true);
    }

    public void DownloadOtherReplays(PlayerInfo other)
    {
        Globals.playerDownloading = other;
        Globals.socket.Send("download_other_replays" + separator + other.name);
        Globals.socket.OpenWaitingUI();
    }

    void OneOtherReplay(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        Globals.playerDownloading.atkReplays.Add(replay.date, replay);        
    }

    void OtherReplaysOver(System.String[] reply)
    {
        (Globals.LevelController as City).ranksWindow.viewRankPlayer.Open(Globals.playerDownloading);
        Globals.socket.CloseWaitingUI();
    }    

    public void OneBuilding(System.String[] reply)
    {
        BuildingData building = GetBuildingDataByID(reply[0]);
        building.type = reply[1];
        building.unpickedRose = System.Convert.ToInt32(reply[2]);        
        building.everClickedTarget = System.Convert.ToBoolean(reply[3]);
        building.targetName = reply[4];
        building.isPvP = System.Convert.ToBoolean(reply[5]);
        building.levelRandSeed = System.Convert.ToInt32(reply[6]);
        building.PvELevelIdx = System.Convert.ToInt32(reply[7]);        
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
        data.levelRandSeed = System.Convert.ToInt32(reply[3]);
        data.PvELevelIdx = System.Convert.ToInt32(reply[4]);
        pveProgress = System.Convert.ToInt32(reply[5]);
        City city = Globals.LevelController as City;
        if (city != null && city.buildings.Count != 0)
        {
            Building building = city.UpdateBuilding(city.GetBuilding(data));            
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
        data.unpickedRose = 0;
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
        slotsDatas[slotIdx].statu = trickname;
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
        if(TutorialLevelIdx != TutorialLevel.InitMyMaze)
        {
            Globals.socket.Send("cash" + separator + cashAmount.ToString());
        }        
    }

    public void ChangeRoseCount(int rose_delta, BuildingData building)
    {
        roseCount += rose_delta;
        if (building != null)
        {
            Globals.socket.Send("pick_rose" + separator + rose_delta.ToString() + separator + building.posID);
        }
        else
        {
            Globals.socket.Send("pick_rose" + separator + rose_delta.ToString() + separator + "-1");
        }
        
    }

    public int GetRoseAddPowerRate()
    {
        return 2;
    }

    public int GetPowerDelta()
    {
        return roseCount / GetRoseAddPowerRate();
    }

    public SafeBoxData AddSafeBox()
    {
        if (TutorialLevelIdx == TutorialLevel.Over)
        {
            SafeBoxData data = new SafeBoxData();
            data.owner = this;
            safeBoxDatas.Add(data);
            Globals.socket.Send("add_safebox");
            return data;
        }
        else
        {            
            for (int idx = 0; idx < safeBoxDatas.Count; ++idx)
            {
                SafeBoxData data = safeBoxDatas[idx];
                if (!data.unlocked)
                {
                    return data;
                }
            }
        }
        return null;
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
        Globals.socket.Send("advance_tutorial" + separator + TutorialLevelIdx.ToString());
        if(Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            UploadGuards();
            UpgradeMaze();
            ChangeCashAmount(cashAmount);
        }        
    }

    public void HireGuard(GuardData guard)
    {
        if (!guard.hired)
        {
            guard.hired = true;
            Globals.socket.Send("hire_guard" + separator + guard.name);
        }        
    }

    public void UploadGuards()
    {
        if(Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            PackSummonedGuardsStr();
            Globals.socket.Send("upload_summoned_guards" + separator + summonedGuardsStr);
        }        
    }

    public void PackSummonedGuardsStr()
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
    }

    public void BuildingClicked(BuildingData building)
    {
        building.everClickedTarget = true;
        Globals.socket.Send("click_target" + separator + building.posID);
    }

    public void DownloadTarget(BuildingData data)
    {
        if (Globals.self.TutorialLevelIdx == PlayerInfo.TutorialLevel.Over)
        {
            stealingTarget = data;
            Globals.guardPlayer = new PlayerInfo();
            Globals.guardPlayer.name = stealingTarget.targetName;
            Globals.guardPlayer.isBot = !data.isPvP;
            Globals.guardPlayer.currentMazeRandSeedCache = stealingTarget.levelRandSeed;

            if (Globals.guardPlayer.isBot)
            {
                Globals.iniFileName = "pve_" + data.PvELevelIdx.ToString();
            }

            DownloadOtherPlayer(Globals.guardPlayer, data);
        }
    }

    public void DownloadOtherPlayer(PlayerInfo player, BuildingData data)
    {
        Globals.playerDownloading = player;        
        Globals.socket.serverReplyActions.Add("download_target", (reply) => player.TargetReady(reply));
        System.String buildingID = "-1";
        if (data != null)
        {
            buildingID = data.posID;
        }

        Globals.socket.Send("download_target" + separator + player.name + separator + player.isBot.ToString() + separator + buildingID);
        
        Globals.socket.SetReady(false);
    }

    public void TargetReady(System.String[] reply)
    {
        Globals.playerDownloading.StealingInfo(reply[0]);        
        Globals.socket.serverReplyActions.Remove("download_target");
        Globals.socket.SetReady(true);
    }

    public void StealingOver(float StealingCash, int PerfectStealingBonus, bool bIsPerfectStealing)
    {
        ReplayData replay = new ReplayData();
        replay.date = System.DateTime.Now;
        replay.StealingCash = StealingCash;
        replay.ini = Globals.replaySystem.Pack();
        replay.everClicked = false;

        Globals.socket.Send(
            "stealing_over" + separator +
            replay.date + separator +
            replay.StealingCash.ToString("F0") + separator +
            replay.ini.toString() + separator +
            replay.everClicked.ToString() + separator +
            bIsPerfectStealing.ToString() + separator +
            Globals.guardPlayer.cashAmount.ToString() + separator +
            stealingTarget.posID);

        Globals.canvasForMagician.ChangeCash(StealingCash + PerfectStealingBonus);
        Globals.transition.BlackOut(() => (Globals.LevelController as StealingLevelController).Newsreport());
    }

    public void OneAtkReplay(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        atkReplays.Add(replay.date.ToString(), replay);
    }

    public void OneDefReplay(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        defReplays.Add(replay.date.ToString(), replay);
    }

    ReplayData UnpackOneReplayData(System.String[] reply)
    {
        ReplayData replay = new ReplayData();
        replay.date = System.Convert.ToDateTime(reply[0]);                
        replay.StealingCash = System.Convert.ToSingle(reply[1]);
        replay.ini.loadFromText(reply[2]);
        replay.everClicked = System.Convert.ToBoolean(reply[3]);
        replay.thief.StealingInfo(reply[4]);
        replay.guard.StealingInfo(reply[5]);
        return replay;
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
    public static System.String iniFileName = "MyMaze_1";
    public static float FLOOR_HEIGHT = 0.1f;
    public static System.Collections.Generic.List<System.String> AvatarAnimationEventNameCache = new System.Collections.Generic.List<System.String>();
    public static Replay replaySystem;
    public static ReplayData playingReplay;
    public static bool DEBUG_REPLAY = false;
    public static UnityEngine.Sprite[] buildingSprites = null;

    public static PlayerInfo self = new PlayerInfo();
    public static System.Collections.Generic.List<PlayerInfo> playersOnRank = new System.Collections.Generic.List<PlayerInfo>();
    public static PlayerInfo playerDownloading;
    public static PlayerInfo visitPlayer;
    public static PlayerInfo thiefPlayer;
    public static PlayerInfo guardPlayer;

    public static System.Collections.Generic.List<TrickData> tricks = new System.Collections.Generic.List<TrickData>();
    public static System.Collections.Generic.List<MazeLvData> mazeLvDatas = new System.Collections.Generic.List<MazeLvData>();
    public static System.Collections.Generic.List<GuardData> guardDatas = new System.Collections.Generic.List<GuardData>();    
    public static int buySafeBoxPrice;
    public static SafeBoxLvData[] safeBoxLvDatas = null;

    
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
        Globals.maze.CASH = ini.get("CASH",0);
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
        ini.set("CASH", Globals.maze.CASH);

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

        foreach (System.Collections.DictionaryEntry entry in self.defReplays)
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
        eventTrigger.triggers.Add(entry);
    }

    public static MsgBox MessageBox(System.String text, UnityEngine.Events.UnityAction yesAction = null, bool bNeedCancel = false)
    {
        UnityEngine.GameObject msgbox_prefab = UnityEngine.Resources.Load("UI/MsgBox") as UnityEngine.GameObject;
        MsgBox msgbox = (UnityEngine.GameObject.Instantiate(msgbox_prefab) as UnityEngine.GameObject).GetComponentInChildren<MsgBox>();
        msgbox.Msg(text, yesAction, bNeedCancel);
        return msgbox;
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
            content = " frame:" + (UnityEngine.Time.frameCount -  Globals.replaySystem.frameBeginNo).ToString() + content;

            System.String filename = file;
            if (playingReplay == null)
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

    public static byte[] ConvertVector3ToByteArray(System.Collections.Generic.List<UnityEngine.Vector3> poses)
    {
        var floatArray = new float[3 * poses.Count];

        int i = 0;
        foreach(UnityEngine.Vector3 pos in poses)
        {
            floatArray[i++] = pos.x;
            floatArray[i++] = pos.y;
            floatArray[i++] = pos.z;
        }

        // create a byte array and copy the floats into it...
        var byteArray = new byte[3 * 4 * poses.Count];
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

    public static double Clamp(double value, double min, double max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
}

