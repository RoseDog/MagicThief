
public class TrickData
{
    public System.String nameKey;
    public System.String descriptionKey;
    public int duration;
    public int powerCost;
    public int unlockRoseCount;
    public int slotIdxInUsingPanel = -1;
    public int buyPrice;
    public bool clickOnGuardToCast = false;
    public bool clickButtonToCast = false;
    public int inventory;
    public float dropOdds;
    public UnityEngine.Vector2 castRange;
    public float weight = 0.5f;

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
        data.clickOnGuardToCast = clickOnGuardToCast;
        data.clickButtonToCast = clickButtonToCast;
    }

    public int CalcTrickDurationBasedOnDistance(float distance, bool bad_trick)
    {
        int retVal = (int)(duration * UnityEngine.Mathf.Clamp01(1 - (distance - castRange.x) / (castRange.y - castRange.x)));
        
        if (retVal < 30)
        {
            retVal = 30;
        }
        else if(bad_trick)
        {
            retVal = 60;
        }
        return retVal;
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
    public float moveSpeed = 1.0f;
    public int income;

    public GuardData()
    {

    }
}

public class MagicianData
{
    public System.String name;
    public System.String desc;
    public int unlockSafeDuration;
    public float sneakingFactor;
    public float runningFactor;
    public float LifeConsumePerWeight;

    public float strengthBase;
    public float agilityBase;
    public float wisdomBase;
    
    public float strengthGrowth;
    public float agilityGrowth;
    public float wisdomGrowth;
    public int idx;
    public float stepSoundLimit;
    public PlayerInfo owner;
    public int roseRequire;
    public MagicianData()
    {
        
    }

    public float GetLifeAmount()
    {
        return (strengthBase + owner.strengthAllot * strengthGrowth) * owner.GetDataBasedOnRoseCount().LifeDelta;
    }

    public float GetPowerAmount()
    {
        return (wisdomBase + owner.wisdomAllot * wisdomGrowth) * owner.GetDataBasedOnRoseCount().PowerDelta; ;
    }

    public float GetSneakingSpeed()
    {
        return GetSpeed() * sneakingFactor;
    }

    public float GetRunningSpeed()
    {
        return GetSpeed() * runningFactor;
    }

    public float GetNormalSpeed()
    {
        return GetSpeed();
    }

    public float GetUnlockSafeTime()
    {
        return UnityEngine.Time.fixedDeltaTime * GetUnlockSafeDuration();
    }

    public float GetSpeed()
    {
        return (agilityBase + owner.agilityAllot * agilityGrowth) * owner.GetDataBasedOnRoseCount().SpeedDelta;
    }

    public int GetUnlockSafeDuration()
    {
        // 这里还需要再仔细调整
        return (int)(unlockSafeDuration - GetUnlockSafeDurationDelta()) + 30;
    }

    public float GetUnlockSafeDurationDelta()
    {
        return unlockSafeDuration * UnityEngine.Mathf.Clamp01(((agilityBase + owner.agilityAllot) / 300.0f));
    }

    public float GetUnlockSafeTimeDelta()
    {
        return UnityEngine.Time.fixedDeltaTime * GetUnlockSafeDurationDelta();
    }

    public float GetLifeConsume()
    {
        return LifeConsumePerWeight;        
    }

    public bool IsLocked()
    {
        return roseRequire > owner.roseCount;
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
    public float roseGrowLastDuration;
    public float roseGrowTotalDuration;
    public float roseGrowCycle;
    public float bornNewTargetLastDuration;
    public int maze_lv;
}

public class ReplayData
{
    public System.DateTime date;        
    public float StealingCashInSafebox;
    public float PickedCash;
    public IniFile ini = new IniFile();
    public bool everClicked = false;
    public int reward_rose_count;
    public bool rewardAccepted = false;
    public PlayerInfo thief = new PlayerInfo();
    public PlayerInfo guard = new PlayerInfo();
}

public class CloudData
{
    public int idx;
    public bool locked;
    public float price;
}

public class DataBasedOnRoseCount
{
    public int levelIdxMin;
    public int levelIdxMax;
    public float LifeDelta;
    public float SpeedDelta;
    public float PowerDelta;
}

public class PlayerInfo
{
    public System.String separator = "|";
    
    public enum TutorialLevel
    {
        GetChest = 0,
        Sneaking,
        FirstTrick,
        NewEnemy,
        UnlockNewTrick,
        InitMyMaze,
        Over
    }
    public System.String name;
    public TutorialLevel TutorialLevelIdx = TutorialLevel.GetChest;
    public float cashAmount = 80000.0f;
    public int roseCount = 80;
    public int roseLast;
    public int punishRoseCount;
    
    public int currentMazeRandSeedCache;    
    public int currentMazeLevel = 0;

    public int pveProgress;

    public System.Collections.Generic.List<SafeBoxData> safeBoxDatas = new System.Collections.Generic.List<SafeBoxData>();
    public System.Collections.Generic.List<TrickData> tricks = new System.Collections.Generic.List<TrickData>();
    public System.Collections.Generic.List<TrickUsingSlotData> slotsDatas = new System.Collections.Generic.List<TrickUsingSlotData>();
    public System.Collections.Generic.List<System.String> guardsHired = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<BuildingData> buildingDatas = new System.Collections.Generic.List<BuildingData>();
    public System.Collections.Generic.List<CloudData> cloudDatas = new System.Collections.Generic.List<CloudData>();
    public System.Collections.Hashtable defReplays = new System.Collections.Hashtable();
    public System.Collections.Hashtable atkReplays = new System.Collections.Hashtable();
    public System.Collections.Hashtable beenStolenReports = new System.Collections.Hashtable();

    public System.Collections.Generic.List<MagicianData> magicians = new System.Collections.Generic.List<MagicianData>();
    public MagicianData selectedMagician;
    public System.Collections.Generic.List<System.String> droppedItemsFromThief = new System.Collections.Generic.List<System.String>();
    public System.Collections.Generic.List<System.String> cashOnFloor = new System.Collections.Generic.List<System.String>();

    public int strengthAllot;
    public int agilityAllot;
    public int wisdomAllot;

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
        MagicianData mage_data = new MagicianData();
        mage_data.owner = this;
        mage_data.name = "Rosa";
        mage_data.desc = "Rosa_desc";
        mage_data.sneakingFactor = 0.6f;
        mage_data.runningFactor = 1.0f;
        mage_data.unlockSafeDuration = 120;
        mage_data.LifeConsumePerWeight = 0.02f;
        mage_data.strengthBase = 55;
        mage_data.agilityBase = 55;
        mage_data.wisdomBase = 55;
        mage_data.strengthGrowth = 1f;
        mage_data.agilityGrowth = 1f;
        mage_data.wisdomGrowth = 1f;
        mage_data.idx = 0;
        mage_data.stepSoundLimit = 800;
        mage_data.roseRequire = 0;
        magicians.Add(mage_data);

        mage_data = new MagicianData();
        mage_data.owner = this;
        mage_data.name = "Walter";
        mage_data.desc = "Walter_desc";
        mage_data.sneakingFactor = 0.6f;
        mage_data.runningFactor = 1.2f;
        mage_data.unlockSafeDuration = 120;
        mage_data.LifeConsumePerWeight = 0.02f;
        mage_data.strengthBase = 50;
        mage_data.agilityBase = 65;
        mage_data.wisdomBase = 50;
        mage_data.strengthGrowth = 1f;
        mage_data.agilityGrowth = 1f;
        mage_data.wisdomGrowth = 1f;
        mage_data.idx = 1;
        mage_data.stepSoundLimit = 600;
        mage_data.roseRequire = 20;
        magicians.Add(mage_data);

        selectedMagician = magicians[0];

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
        data.price = 5000;
        data.idx = 2;
        slotsDatas.Add(data);

        if (tricks.Count == 0)
        {
            TrickData trick = new TrickData();
            trick.nameKey = "hypnosis";
            trick.descriptionKey = "hypnosis_desc";
            trick.duration = 350;
            trick.powerCost = 15;
            trick.unlockRoseCount = 0;
            trick.dropOdds = 0.9f;
            trick.weight = 1.2f;
            trick.buyPrice = 100;
            trick.clickOnGuardToCast = true;
            trick.clickButtonToCast = false;
            trick.castRange = new UnityEngine.Vector2(200, 500);
            tricks.Add(trick);

            trick = new TrickData();
            trick.nameKey = "dove";
            trick.descriptionKey = "dove_desc";
            trick.duration = 500;
            trick.powerCost = 10;
            trick.unlockRoseCount = 0;
            trick.dropOdds = 0.9f;
            trick.buyPrice = 100;
            trick.clickOnGuardToCast = false;
            trick.clickButtonToCast = true;
            tricks.Add(trick);

            trick = new TrickData();
            trick.nameKey = "flashGrenade";
            trick.descriptionKey = "flashGrenade_desc";
            trick.duration = 0;
            trick.powerCost = 2;
            trick.unlockRoseCount = 10;
            trick.dropOdds = 0.9f;
            trick.buyPrice = 200;
            trick.clickOnGuardToCast = false;
            trick.clickButtonToCast = false;
            tricks.Add(trick);

            trick = new TrickData();
            trick.nameKey = "disguise";
            trick.descriptionKey = "disguise_desc";
            trick.duration = 700;
            trick.powerCost = 30;
            trick.unlockRoseCount = 30;
            trick.dropOdds = 0.9f;
            trick.weight = 0.3f;
            trick.buyPrice = 300;
            trick.clickOnGuardToCast = false;
            trick.clickButtonToCast = true;
            tricks.Add(trick);       

            trick = new TrickData();
            trick.nameKey = "shotLight";
            trick.descriptionKey = "shotLight_desc";
            trick.duration = 700;// machine fixing duration
            trick.powerCost = 5;
            trick.unlockRoseCount = 40;
            trick.dropOdds = 0.3f;
            trick.buyPrice = 100;
            trick.clickOnGuardToCast = true;
            trick.clickButtonToCast = true;
            trick.castRange = new UnityEngine.Vector2(300, 1600);
            tricks.Add(trick);

            trick = new TrickData();
            trick.nameKey = "flyUp";
            trick.descriptionKey = "flyUp_desc";
            trick.duration = 300;
            trick.powerCost = 15;
            trick.unlockRoseCount = 60;
            trick.dropOdds = 0.3f;
            trick.buyPrice = 1800;
            trick.clickOnGuardToCast = false;
            trick.clickButtonToCast = true;
            tricks.Add(trick);
        }        
    }

    public DataBasedOnRoseCount GetDataBasedOnRoseCount()
    {
        DataBasedOnRoseCount data = new DataBasedOnRoseCount();
        data.levelIdxMin = 0;
        data.levelIdxMax = 0;
        data.LifeDelta = 1;
        data.SpeedDelta = 0.1f;
        data.PowerDelta = 1;
        return data;
    }

    public MagicianData GetMageDataByName(System.String name)
    {
        foreach (MagicianData data in magicians)
        {
            if (data.name == name)
            {
                return data;
            }
        }        
        Globals.Assert(false, "no guard named:" + name);
        return null;
    }

    int counter = 0;
    public void SyncWithServer()
    {
        defReplays.Clear();
        atkReplays.Clear();

        buildingDatas.Clear();
        for (int idx = 0; idx < 25; ++idx)
        {
            BuildingData building = new BuildingData();
            building.posID = idx.ToString();
            buildingDatas.Add(building);
        }        

        Globals.playersOnRank.Clear();
        MsgActions();

        

        GuardData guard_data = new GuardData();
        guard_data.name = "Police";
        guard_data.price = 100;
        guard_data.roomConsume = 2;
        guard_data.magicianOutVisionTime = 100;
        guard_data.atkCd = 100;
        guard_data.attackValue = 30;
        guard_data.atkShortestDistance = 120f;
        guard_data.doveOutVisionTime = 50;
        guard_data.attackSpeed = 1.0f;
        guard_data.moveSpeed = 6f;        
        Globals.guardDatas.Add(guard_data);

        guard_data = new GuardData();
        guard_data.name = "joker";
        guard_data.price = 100;
        guard_data.roomConsume = 2;
        guard_data.magicianOutVisionTime = 100;
        guard_data.atkCd = 100;
        guard_data.attackValue = 30;
        guard_data.atkShortestDistance = 120f;
        guard_data.doveOutVisionTime = 50;
        guard_data.attackSpeed = 1.0f;
        guard_data.moveSpeed = 6;
        guard_data.income = 150;
        Globals.guardDatas.Add(guard_data);

        guard_data = new GuardData();
        guard_data.name = "dog";
        guard_data.price = 200;
        guard_data.roomConsume = 1;
        guard_data.magicianOutVisionTime = 100;
        guard_data.attackValue = 30;
        guard_data.atkShortestDistance = 100f;
        guard_data.doveOutVisionTime = 300;
        guard_data.moveSpeed = 8;
        guard_data.income = 50;
        Globals.guardDatas.Add(guard_data);

        guard_data = new GuardData();
        guard_data.name = "Spider";
        guard_data.price = 500;
        guard_data.roomConsume = 2;
        guard_data.magicianOutVisionTime = 450;
        guard_data.atkCd = 260;
        guard_data.attackValue = 40;
        guard_data.atkShortestDistance = 150f;
        guard_data.doveOutVisionTime = 50;
        guard_data.attackSpeed = 1.0f;
        Globals.guardDatas.Add(guard_data);

        guard_data = new GuardData();
        guard_data.name = "Monkey";
        guard_data.price = 1000;
        guard_data.roomConsume = 3;
        guard_data.magicianOutVisionTime = 70;
        guard_data.atkCd = 150;
        guard_data.attackValue = 40;
        guard_data.atkShortestDistance = 500f;
        guard_data.doveOutVisionTime = 50;
        guard_data.attackSpeed = 1.0f;
        guard_data.moveSpeed = 6;
        guard_data.income = 250;
        Globals.guardDatas.Add(guard_data);


        guard_data = new GuardData();
        guard_data.name = "lamp";
        guard_data.price = 2000;
        guard_data.roomConsume = 1;
        Globals.guardDatas.Add(guard_data);

        MazeLvData maze_data = new MazeLvData();
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.playerEverClickGuards = true;
        maze_data.playerEverClickSafebox = true;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 0;
        maze_data.price = 1000;
        maze_data.roomSupport = 5;
        maze_data.lockGuardsName = new System.String[] { "joker", "dog" };
        maze_data.safeBoxCount = 2;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 5;
        maze_data.price = 1000;
        maze_data.roomSupport = 6;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 2;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 15;
        maze_data.price = 4000;
        maze_data.roomSupport = 7;
        maze_data.lockGuardsName = new System.String[] { "Monkey" };
        maze_data.safeBoxCount = 3;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 30;
        maze_data.price = 7000;
        maze_data.roomSupport = 8;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 3;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 40;
        maze_data.price = 10000;
        maze_data.roomSupport = 9;
        maze_data.lockGuardsName = new System.String[] {"Spider"};
        maze_data.safeBoxCount = 3;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 50;
        maze_data.price = 13000;
        maze_data.roomSupport = 11;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 4;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 60;
        maze_data.price = 16000;
        maze_data.roomSupport = 13;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 4;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 70;
        maze_data.price = 19000;
        maze_data.roomSupport = 15;
        maze_data.lockGuardsName = new System.String[] { "lamp" };
        maze_data.safeBoxCount = 4;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 80;
        maze_data.price = 22000;
        maze_data.roomSupport = 17;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 5;
        Globals.mazeLvDatas.Add(maze_data);

        maze_data = new MazeLvData();
        maze_data.roseRequire = 90;
        maze_data.price = 25000;
        maze_data.roomSupport = 19;
        maze_data.lockGuardsName = new System.String[] { };
        maze_data.safeBoxCount = 5;
        Globals.mazeLvDatas.Add(maze_data);

        Globals.buySafeBoxPrice = 3000;
        Globals.safeBoxLvDatas = new SafeBoxLvData[] { 
        new SafeBoxLvData(500, 3000), 
        new SafeBoxLvData(1000, 4000), 
        new SafeBoxLvData(1500, 5000),
        new SafeBoxLvData(2000, 6000),
        new SafeBoxLvData(2500, 7000),
        new SafeBoxLvData(3000, 8000),
        new SafeBoxLvData(3500, 9000),
        new SafeBoxLvData(4000, 10000),
        new SafeBoxLvData(4500, 11000),
        new SafeBoxLvData(5000, 12000)};

        Globals.socket.SetReady(false);
        
        // 杩欎釜椤哄簭鏍规湰娌℃湁淇濊瘉鍟?..鏃?.
        Globals.socket.Send("self_stealing_info" + separator + name);
        Globals.socket.Send("download_buildings");
        Globals.socket.Send("download_replays");
        Globals.socket.Send("download_clouds");
        Globals.socket.Send("download_ranks");
    }

    public void MsgActions()
    {
        if (!Globals.socket.serverReplyActions.ContainsKey("self_stealing_info"))
        {
            Globals.socket.serverReplyActions.Add("self_stealing_info", (reply) => SelfStealingInfo(reply));

            Globals.socket.serverReplyActions.Add("buildings_ready", (reply) => BuildingsOver(reply));
            Globals.socket.serverReplyActions.Add("replays_ready", (reply) => ReplaysOver(reply));
            Globals.socket.serverReplyActions.Add("one_cloud", (reply) => OneCloud(reply));
            Globals.socket.serverReplyActions.Add("clouds_ready", (reply) => CloudsOver(reply));

            Globals.socket.serverReplyActions.Add("download_cloud_rank", (reply) => OneRank(reply));
            Globals.socket.serverReplyActions.Add("download_clouds_over", (reply) => PlayersRankOver(reply));

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

            Globals.socket.serverReplyActions.Add("been_stolen", (reply) => BeenStolen(reply));

            Globals.socket.serverReplyActions.Add("performing_income", (reply) => PerformingIncome(reply));            
        }
    }

    public TrickData GetTrickByName(System.String trickname)
    {
        foreach (TrickData data in tricks)
        {
            if (trickname == data.nameKey)
            {
                return data;
            }
        }
        return null;
    }

    public int GetTrickIdx(System.String trickname)
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

    public TrickData GetTrickBySlotIdx(int slotIdx)
    {
        for (int idx = 0; idx < tricks.Count; ++idx)
        {
            TrickData data = tricks[idx];
            if (data.slotIdxInUsingPanel == slotIdx)
            {
                return data;
            }
        }
        return null;
    }


    public float GetTrickTotalWeight()
    {
        float total_weight = 1.0f;
        foreach (TrickData trick in tricks)
        {
            if (trick.IsInUse())
            {
                total_weight += trick.weight;
            }
        }
        return total_weight;
    }

    public int GetTotalIncomePerHour()
    {
        int income = 0;

        IniFile ini = new IniFile();
        ini.loadFromText(summonedGuardsStr);
        int guard_count = ini.get("GuardCount", 0);
        System.String[] keys = ini.keys();
        for (int i = 1; i <= guard_count; ++i)
        {
            GuardData data = Globals.GetGuardData(ini.get(keys[i]));
            income += data.income;
        }
                
        return income;
    }

    public void PerformingIncome(System.String[] reply)
    {                
        cashOnFloor.Add(reply[0]);
        // 如果在这里生成，下次进来看到的不一致
//         if(Globals.canvasForMyMaze != null)
//         {
//             Globals.canvasForMyMaze.UpdateIncomeIntro();
//             if (Globals.maze.isGenerateFinished)
//             {
//                 PickedItem cash = OneCashOnFloor(reply[0]);
//                 cash.Falling(60);            
//             }            
//         }
    }

    public int GetCashAmountOnMazeFloor()
    {
        int cash_on_floor = 0;
        foreach(System.String cash in cashOnFloor)
        {
            System.String[] cash_data = cash.Split(',');
            cash_on_floor += System.Convert.ToInt32(cash_data[2]);
        }
        return cash_on_floor;
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
                if (TutorialLevelIdx == TutorialLevel.GetChest || 
                    TutorialLevelIdx == TutorialLevel.Sneaking || 
                    TutorialLevelIdx == TutorialLevel.FirstTrick ||
                    TutorialLevelIdx == TutorialLevel.NewEnemy)
                {
                    Globals.asyncLoad._ToLoadingScene("StealingLevel");
                }
                else if (TutorialLevelIdx == TutorialLevel.UnlockNewTrick || TutorialLevelIdx == TutorialLevel.Over)
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
        }        
    }

    public void StealingInfo(System.String info)
    {
        System.String[] reply = info.Split('&');

        TutorialLevelIdx = (TutorialLevel)System.Enum.Parse(typeof(TutorialLevel), reply[0]);
        cashAmount = float.Parse(reply[1]);
        roseCount = int.Parse(reply[2]);
        roseLast = int.Parse(reply[3]);
        currentMazeLevel = int.Parse(reply[4]);
        currentMazeRandSeedCache = (int)long.Parse(reply[5]);
        selectedMagician = GetMageDataByName(reply[6]);

        System.String[] temp;
        temp = reply[7].Split(',');
        for (int idx = 0; idx < slotsDatas.Count; ++idx)
        {
            TrickUsingSlotData slot = slotsDatas[idx];
            // 鐢变簬瀛楃涓叉嫾鎺ラ棶棰橈紝temp[0]鏄┖瀛楃涓?
            slot.statu = temp[idx+1];
        }

        safeBoxDatas.Clear();
        temp = reply[8].Split(',');
        for (int idx = 1; idx < temp.Length; ++idx)
        {            
            SafeBoxData data = new SafeBoxData();
            data.owner = this;
            safeBoxDatas.Add(data);
            data.Lv = System.Convert.ToInt32(temp[idx]);
        }

        guardsHired.Clear();
        temp = reply[9].Split(',');
        for (int idx = 1; idx < temp.Length; ++idx)
        {
            guardsHired.Add(temp[idx]);
        }

        summonedGuardsStr = reply[10];

        temp = reply[11].Split(',');
        for (int i = 0; i < temp.Length - 1; )
        {
            System.String trick_name = temp[i++];
            TrickData trick_data = GetTrickByName(trick_name);
            System.Convert.ToBoolean(temp[i++]);
            trick_data.inventory = System.Convert.ToInt32(temp[i++]);
            trick_data.Use(System.Convert.ToInt32(temp[i++]));
        }

        isBot = System.Convert.ToBoolean(reply[12]);

        name = reply[13];

        pveProgress = System.Convert.ToInt32(reply[14]);

        punishRoseCount = System.Convert.ToInt32(reply[15]);

        strengthAllot = System.Convert.ToInt32(reply[16]);
        agilityAllot = System.Convert.ToInt32(reply[17]);
        wisdomAllot = System.Convert.ToInt32(reply[18]);
        
        UnpackDroppedItemStr(reply[19]);
        UnpackCashOnFloorStr(reply[20]);
    }

    public void UnpackDroppedItemStr(System.String item_str)
    {
        droppedItemsFromThief.Clear();
        System.String[] temp = item_str.Split('_');
        for (int i = 1; i < temp.Length; ++i)
        {
            droppedItemsFromThief.Add(temp[i]);
        }
    }

    public void UnpackCashOnFloorStr(System.String cashOnFloor_str)
    {
        cashOnFloor.Clear();
        System.String[] temp = cashOnFloor_str.Split('_');
        for (int i = 1; i < temp.Length; ++i)
        {
            cashOnFloor.Add(temp[i]);
        }
    }
    

    void BuildingsOver(System.String[] reply)
    {
        
    }
   
    void ReplaysOver(System.String[] reply)
    {
        
    }

    void OneCloud(System.String[] reply)
    {
        CloudData cloud = new CloudData();
        cloud.idx = cloudDatas.Count;
        cloudDatas.Add(cloud);

        Globals.Assert(System.Convert.ToSingle(reply[0]) == cloud.idx);

        cloud.price = System.Convert.ToSingle(reply[1]);
        cloud.locked = System.Convert.ToBoolean(reply[2]);
    }

    void CloudsOver(System.String[] reply)
    {
        
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
        building.roseGrowLastDuration = System.Convert.ToSingle(reply[8]);
        building.roseGrowTotalDuration = System.Convert.ToSingle(reply[9]);
        building.roseGrowCycle = System.Convert.ToSingle(reply[10]);
        building.bornNewTargetLastDuration = System.Convert.ToSingle(reply[11]);
        building.maze_lv = System.Convert.ToInt32(reply[12]);
    }

    public void RoseGrow(System.String[] reply)
    {
        BuildingData building = GetBuildingDataByID(reply[0]);
        building.unpickedRose += 1;
        if (Globals.city != null && Globals.city.buildings.Count != 0)
        {
            Globals.city.RoseGrow(building);
        }
    }

    public void RoseBuildingEnd(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.bornNewTargetLastDuration = System.Convert.ToSingle(reply[1]);
        data.type = "None";        
        if (Globals.city != null && Globals.city.buildings.Count != 0)
        {
            Globals.city.UpdateBuilding(Globals.city.GetBuilding(data), data);
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
        data.maze_lv = System.Convert.ToInt32(reply[6]);
        if (Globals.city != null && Globals.city.buildings.Count != 0)
        {
            Building building = Globals.city.UpdateBuilding(Globals.city.GetBuilding(data), data);            
        }
    }

    public void NewPoor(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.type = "Poor";
        data.targetName = "";
        if (Globals.city != null && Globals.city.buildings.Count != 0)
        {
            Globals.city.UpdateBuilding(Globals.city.GetBuilding(data), data);
        }
    }

    public void NewRoseBuilding(System.String[] reply)
    {
        BuildingData data = GetBuildingDataByID(reply[0]);
        data.unpickedRose = System.Convert.ToInt32(reply[1]);
        data.roseGrowLastDuration = System.Convert.ToSingle(reply[2]);
        data.roseGrowTotalDuration = System.Convert.ToSingle(reply[3]);
        data.roseGrowCycle = System.Convert.ToSingle(reply[4]);
        data.type = "Rose";
        
        if (Globals.city != null && Globals.city.buildings.Count != 0)
        {
            Globals.city.UpdateBuilding(Globals.city.GetBuilding(data), data);
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

    public void AddTrickItem(TrickData trickdata)
    {
        trickdata.inventory += 1;
        Globals.socket.Send("add_trickitem" + separator + trickdata.nameKey);
    }

    public System.Collections.Generic.List<System.String> ConsumeItem()
    {
        System.Collections.Generic.List<System.String> itemsConsumed = new System.Collections.Generic.List<System.String>();
        foreach (TrickSlot slot in Globals.canvasForMagician.trickInUseSlots)
        {
            if (slot.trickItem)
            {
                itemsConsumed.Add(slot.trickItem.trickData.nameKey);
                slot.trickItem.trickData.inventory -= 1;
                Globals.socket.Send("consume_trickitem" + separator + slot.trickItem.trickData.nameKey);
                if (slot.trickItem.trickData.inventory == 0)
                {
                    RemoveUsingTrick(slot.trickItem.trickData);
                }
            }
        }
        return itemsConsumed;
    }

    public System.Collections.Generic.List<System.String> DropItems()
    {
        System.Collections.Generic.List<System.String> itemsDropingWhenEscape = new System.Collections.Generic.List<System.String>();
        foreach (TrickSlot slot in Globals.canvasForMagician.trickInUseSlots)
        {
            if (slot.trickItem)
            {
                if (slot.trickItem.trickData.inventory > 0 && UnityEngine.Random.Range(0.0f, 1.0f) < slot.trickItem.trickData.dropOdds)
                {
                    Cell corridor = Globals.maze.GetRandomCorridorCell();
                    System.String item_id = UnityEngine.Random.Range(0.0f, 1.0f) + "," + UnityEngine.Random.Range(0.0f, 1.0f) + "," + slot.trickItem.trickData.nameKey;
                    itemsDropingWhenEscape.Add(item_id);
                    slot.trickItem.trickData.inventory -= 1;
                    Globals.socket.Send("drop_trickitem" + separator + item_id);
                    if (slot.trickItem.trickData.inventory == 0)
                    {
                        RemoveUsingTrick(slot.trickItem.trickData);
                    }
                }                
            }
        }
        return itemsDropingWhenEscape;
    }

    public void RemoveDroppedItem(System.String itemID)
    {
        droppedItemsFromThief.Remove(itemID);
        Globals.socket.Send("RemoveDroppedItem" + separator + itemID);
    }

    public void SpreadItemsDroppedFromThiefInMaze()
    {
        foreach (System.String item in droppedItemsFromThief)
        {
            System.String[] item_data = item.Split(',');
            UnityEngine.GameObject DroppedItem_prefab = UnityEngine.Resources.Load("Misc/DroppedItem") as UnityEngine.GameObject;
            UnityEngine.GameObject DroppedItemObject = UnityEngine.GameObject.Instantiate(DroppedItem_prefab) as UnityEngine.GameObject;
            DroppedItemObject.name = item_data[2];
            DroppedItemObject.GetComponent<UnityEngine.SpriteRenderer>().sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("Misc/" + item_data[2] + "_itemImageOnFloor");
            DroppedItemObject.transform.position = Globals.maze.GetPickedItemBasedOnRandomPos(item_data);
            DroppedItemObject.GetComponent<PickedItem>().item_id = item;
        }
    }

    public void RemoveCashOnFloor(System.String cashID)
    {
        cashOnFloor.Remove(cashID);
        Globals.socket.Send("RemoveCashOnFloor" + separator + cashID);
    }

    public void SpreadCashOnFloor()
    {
        foreach (System.String cash_id in cashOnFloor)
        {
            OneCashOnFloor(cash_id,null);
        }
    }

    public PickedItem OneCashOnFloor(System.String cash_id, Cell cell)
    {
        System.String[] cash_data = cash_id.Split(',');
        UnityEngine.GameObject cash_on_floor_prefab;
        if (this == Globals.self)
        {
            cash_on_floor_prefab = UnityEngine.Resources.Load("Avatar/CashOnMyMazeFloor") as UnityEngine.GameObject;
        }
        else
        {
            cash_on_floor_prefab = UnityEngine.Resources.Load("Avatar/CashOnTargetMazeFloor") as UnityEngine.GameObject;
        }

        UnityEngine.GameObject cash_on_floor = UnityEngine.GameObject.Instantiate(cash_on_floor_prefab) as UnityEngine.GameObject;

        if (cell != null)
        {
            float offset = Globals.GetCellSideLength() / 4.0f;
            cash_on_floor.transform.position = cell.GetFloorPos() +
                new UnityEngine.Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), 0);
        }
        else
        {
            cash_on_floor.transform.position = Globals.maze.GetPickedItemBasedOnRandomPos(cash_data);
        }
        
        int cash_amount = System.Convert.ToInt32(cash_data[2]);
        if (cash_amount == 0)
        {
            cash_amount = 1;
        }
        PickedItem pickedCash = cash_on_floor.GetComponent<PickedItem>();
        pickedCash.SetCash(cash_amount);
        pickedCash.item_id = cash_id;
        return pickedCash;
    }

    public void UsingTrick(TrickData trick, int slotIdx)
    {
        trick.Use(slotIdx);
        Globals.socket.Send("using_trick" + separator + trick.nameKey + separator + slotIdx.ToString());        
    }

    public void RemoveUsingTrick(TrickData trick)
    {
        trick.Unuse();
        Globals.socket.Send("unuse_trick" + separator + trick.nameKey);
    }

    public void TrickSlotBought(TrickUsingSlotData data)
    {
        data.statu = "0";
        Globals.socket.Send("slot_bought" + separator + data.idx.ToString());        
    }

    public bool IsAnyTricksInUse()
    {
        foreach (TrickData data in tricks)
        {
            if (data.IsInUse())
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

    public bool IsMoneyFull()
    {
        return UnityEngine.Mathf.Abs(cashAmount - Globals.AccumulateSafeboxCapacity(this)) <= UnityEngine.Mathf.Epsilon;
    }

    public void PickRose(int rose_delta, BuildingData building)
    {
        roseCount += rose_delta;
        roseLast += rose_delta;
        if (building != null)
        {
            Globals.socket.Send("pick_rose" + separator + rose_delta.ToString() + separator + building.posID);
        }
        else
        {
            Globals.socket.Send("pick_rose" + separator + rose_delta.ToString() + separator + "-1");
        }
        UploadRoseAllot();
    }

    public int ChangeRose(int rose_delta)
    {
        int temp = roseCount;
        temp += rose_delta;
        if (temp < 0)
        {
            rose_delta = -roseCount;
            roseCount = 0;
        }
        else
        {
            roseCount = temp;
        }
        Globals.socket.Send("change_rose" + separator + rose_delta.ToString());
        roseLast += rose_delta;
        if(rose_delta < 0)
        {            
            if (roseLast < 0)
            {
                strengthAllot += roseLast;
                if (strengthAllot < 0)
                {
                    agilityAllot += strengthAllot;
                    if (agilityAllot < 0)
                    {
                        wisdomAllot += agilityAllot;
                        Globals.Assert(wisdomAllot >= 0);
                        agilityAllot = 0;
                    }

                    strengthAllot = 0;
                }

                roseLast = 0;
            }                        
        }
        UploadRoseAllot();
        // 返回实际减少的玫瑰花数量
        return rose_delta;
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
            Globals.socket.Send("upload_summoned_guards" + separator + summonedGuardsStr + separator + GetTotalIncomePerHour());
        }        
    }

    public void UploadRoseAllot()
    {
        Globals.socket.Send("UploadRoseAllot" + separator + roseLast +  
            separator + strengthAllot +
            separator + agilityAllot +
            separator + wisdomAllot);
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

    public void StealingOver(float Stealing_Cash_In_Safebox, float PickedCash, int PerfectStealingBonus, bool bIsPerfectStealing)
    {
        ReplayData replay = new ReplayData();
        replay.date = System.DateTime.Now;
        replay.StealingCashInSafebox = Stealing_Cash_In_Safebox;
        replay.PickedCash = PickedCash;
        replay.ini = Globals.replaySystem.Pack();
        replay.everClicked = false;

        Globals.canvasForMagician.ChangeCash(Stealing_Cash_In_Safebox + PerfectStealingBonus);
        Globals.socket.Send(
            "stealing_over" + separator +
            replay.date + separator +
            replay.StealingCashInSafebox.ToString("F0") + separator +
            replay.PickedCash.ToString("F0") + separator +
            replay.ini.toString() + separator +
            replay.everClicked.ToString() + separator +
            bIsPerfectStealing.ToString() + separator +
            Globals.guardPlayer.cashAmount.ToString() + separator +
            stealingTarget.posID);        

        Globals.transition.BlackOut(() => (Globals.LevelController as StealingLevelController).EndingUI());
    }

    public void OneAtkReplay(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        if (!atkReplays.Contains(replay.date.ToString()))
        {
            atkReplays.Add(replay.date.ToString(), replay);
        }       
    }        

    public void OneDefReplay(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        defReplays.Add(replay.date.ToString(), replay);
        if (!replay.everClicked)
        {
            beenStolenReports.Add(replay.date.ToString(), replay);
        }               
    }

    ReplayData UnpackOneReplayData(System.String[] reply)
    {
        ReplayData replay = new ReplayData();
        replay.date = System.Convert.ToDateTime(reply[0]);                
        replay.StealingCashInSafebox = System.Convert.ToSingle(reply[1]);
        replay.PickedCash = System.Convert.ToSingle(reply[2]);
        replay.ini.loadFromText(reply[3]);
        replay.everClicked = System.Convert.ToBoolean(reply[4]);
        replay.thief.StealingInfo(reply[5]);
        replay.guard.StealingInfo(reply[6]);
        replay.reward_rose_count = System.Convert.ToInt32(reply[7]);
        replay.rewardAccepted = System.Convert.ToBoolean(reply[8]);
        return replay;
    }

    public void BeenStolen(System.String[] reply)
    {
        ReplayData replay = UnpackOneReplayData(reply);
        Globals.canvasForMagician.ChangeCash(replay.StealingCashInSafebox);
        beenStolenReports.Add(replay.date.ToString(), replay);
        if(Globals.city != null)
        {
            Globals.city.BeenStolen();
        }
    }

    public void ReplayClicked(ReplayData replay)
    {
        replay.everClicked = true;
        Globals.socket.Send("click_replay" + separator + replay.date.ToString());
    }

    public void RewardAccepted(ReplayData replay)
    {
        replay.rewardAccepted = true;
        Globals.socket.Send("reward_accepted" + separator + replay.date.ToString());
    }

    public void CloudUnlock(CloudData data)
    {
        Globals.socket.Send("cloud_unlock" + separator + data.idx.ToString());
    }

    public void BuildingUnlock(BuildingData data)
    {
        Globals.socket.Send("building_unlock" + separator + data.posID.ToString());
    }

    public void DontNeedReply(System.String[] reply)
    {

    }

    public void SelectMagician(MagicianData mage)
    {
        selectedMagician = mage;
        Globals.socket.Send("select_magician" + separator + mage.name);
    }
}



public class Globals
{
    public static UnityEngine.GameObject cell_prefab;
    public static UnityEngine.GameObject wave_prefab;
    public static float cell_side_length;
    public static float GetCellSideLength()
    {
        return cell_side_length;
    }    
    public static System.String versionString = "0.1";
    public static readonly System.String EAST = "E";
    public static readonly System.String SOUTH = "S";
    public static readonly System.String WEST = "W";
    public static readonly System.String NORTH = "N";
    public static readonly System.String[] DIRECTIONS = { EAST, SOUTH, WEST, NORTH };
    public static bool SHOW_MACE_GENERATING_PROCESS = false;
    public static bool SHOW_ROOMS = false;
    public static float CREATE_MAZE_TIME_STEP = 0.1f;
    public static int cameraMoveDuration = 7;
    public static int uiMoveAndScaleDuration = 10;
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
    public static City city;
    public static MyMazeLevelController myMazeController;
    public static StealingLevelController stealingController;
    public static LoadingSceneLevelController loadingLevelController;    
    public static ClientSocket socket;
    public static System.String iniFileName = "MyMaze_1";
    public static float FLOOR_HEIGHT = 0.1f;
    public static Replay replaySystem;
    public static ReplayData playingReplay;
    public static bool DEBUG_REPLAY = false;
    public static UnityEngine.Sprite[] buildingSprites = null;
    
        
    public static System.Collections.Generic.List<MazeLvData> mazeLvDatas = new System.Collections.Generic.List<MazeLvData>();
    public static System.Collections.Generic.List<GuardData> guardDatas = new System.Collections.Generic.List<GuardData>();    
    public static int buySafeBoxPrice;
    public static SafeBoxLvData[] safeBoxLvDatas = null;


    public static PlayerInfo self = new PlayerInfo();
    public static System.Collections.Generic.List<PlayerInfo> playersOnRank = new System.Collections.Generic.List<PlayerInfo>();
    public static PlayerInfo playerDownloading;
    public static PlayerInfo visitPlayer;
    public static PlayerInfo thiefPlayer;
    public static PlayerInfo guardPlayer;

    public static System.Collections.Generic.List<Actor> actors = new System.Collections.Generic.List<Actor>();
    public static System.Collections.Generic.List<Actor> to_add_actors = new System.Collections.Generic.List<Actor>();

    
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
        Globals.maze.droppedItemsStr = ini.get("droppedItemsStr");
        Globals.maze.cashOnFloorStr = ini.get("cashOnFloorStr");
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
        ini.set("droppedItemsStr", Globals.maze.droppedItemsStr);
        ini.set("cashOnFloorStr", Globals.maze.cashOnFloorStr);

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
//             content = " rand seed:" + UnityEngine.Random.seed.ToString() + " " + content;
//             content = " frame:" + Globals.LevelController.frameCount.ToString() + content;
// 
//             System.String filename = file;
//             if (playingReplay == null)
//             {
//                 filename += "_pvp";
//             }
//             else
//             {
//                 filename += "_reply";
//             }
// 
//             System.IO.StreamWriter stream = new System.IO.StreamWriter(UnityEngine.Application.dataPath + "/Resources/" + filename + ".txt",
//                 true, System.Text.Encoding.UTF8);
//             stream.WriteLine(content);
//             stream.Close();
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

    public static UnityEngine.Vector3 CalcMazeLeftUpCornerPos(int X_CELLS_COUNT, int Y_CELLS_COUNT)
    {
        return new UnityEngine.Vector3(
            -(X_CELLS_COUNT * Globals.GetCellSideLength()) / 2.0f - Globals.GetCellSideLength() / 2.0f,
           (Y_CELLS_COUNT * Globals.GetCellSideLength()) / 2.0f - Globals.GetCellSideLength() / 2.0f,
           0);
    }

}

