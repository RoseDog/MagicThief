public class MultiLanguageTable : UnityEngine.MonoBehaviour 
{
    public enum Languages
    {
        Chn = 0,        
        Eng,
        ChnTraditional,        
    }
    public Languages current = Languages.Chn;
    public System.Collections.Generic.Dictionary<System.String, System.String[]> table = 
        new System.Collections.Generic.Dictionary<System.String, System.String[]>();
    void Awake()
    {
        Globals.languageTable = this;
        table.Add("leave", new System.String[]{"离开", "Leave"});
        table.Add("level_tip", new System.String[] { "关卡提示信息", "Level Tip" });
        table.Add("tricks", new System.String[] { "魔术", "Tricks" });
        table.Add("outfits", new System.String[] { "服装", "Outfits" });
        table.Add("hypnosis", new System.String[] { "催眠", "Hypnosis" });
        table.Add("hypnosis_desc", new System.String[] { "让一个守卫沉沉睡去", "make a guard go to sleep" });
        table.Add("shotLight", new System.String[] { "左轮枪", "Gun" });
        table.Add("shotLight_desc", new System.String[] { "帅气的左轮枪", "A fancy Gun" });
        table.Add("disguise", new System.String[] { "易容", "Disguise" });
        table.Add("disguise_desc", new System.String[] { "改变自己的容貌", "Disguise" });
        table.Add("flyUp", new System.String[] { "银翼的魔术师", "Glider" });
        table.Add("flyUp_desc", new System.String[] { "展开长袍，利用空气动力将自己托到空中", "Use a glider to fly" });
        table.Add("dove", new System.String[] { "鸽子戏法", "Dove Trick" });
        table.Add("dove_desc", new System.String[] { "放出小动物吸引守卫的注意力", "release animals to distract guards" });
        table.Add("flash_grenade", new System.String[] { "照明弹", "Flare" });
        table.Add("flash_grenade_desc", new System.String[] { "向降落点投掷照明弹，驱散迷雾。<color=red>只能在潜入之前使用</color>", "Flare" });                       
        table.Add("level_loading_progress", new System.String[] { "场景加载{0}%", "Level Loading:{0}%" });
        table.Add("power_cost", new System.String[] { "魔力消耗:{0}", "Power Cost:{0}" });
        table.Add("duration", new System.String[] { "持续时间:{0}", "Duration:{0}" });
        table.Add("unlock_need_rose", new System.String[] { "解锁：达到{0}朵玫瑰", "unlock: reach {0} rose"});
        table.Add("restart_tutorial_level_tip", new System.String[] { 
            "任务失败！<b><color=red><size=30>{0}</size></color></b> 秒后重新开始\n\n 教程关卡你都能输，能专心点儿么 =.= ", 
            "Mission Failed！Restart After <b><color=red><size=30>{0}</size></color></b> seconds" });
        table.Add("poker_face", new System.String[] { "扑克脸", "Poker Face" });
        table.Add("cat_eye_lady", new System.String[] { "猫眼三姐妹", "Cat Eye Lady" });
        table.Add("cash_eye", new System.String[] { "现金眼", "Cash Eye" });
        table.Add("guard_cant_be_here", new System.String[] { "守卫不能放在这里", "Guard Can't be placed here" });
        table.Add("guard_cant_be_on_wall", new System.String[] { "守卫不能放在墙体上", "Guard Can't be on the Wall" });
        table.Add("lamp_cant_be_on_road", new System.String[] { "灯不能放在路上", "Lamp can't be on the road" });
        table.Add("not_enough_power", new System.String[] { "魔力值不够了", "Not enough power" });
        table.Add("city_events", new System.String[] { "城市事件", "City Events" });
        table.Add("my_maze", new System.String[] { "我的迷宫", "My Maze" });
        table.Add("dive_in", new System.String[] { "潜入", "Dive In" });
        table.Add("target_tonight", new System.String[] { "你是个小偷，\n这是你今晚的目标", "you are a thief,\nthis is your target tonight" });
        table.Add("buy", new System.String[] { "购买", "Buy" });
        table.Add("not_enough_roses", new System.String[] { "魅力值不够", "not enough charm points" });
        table.Add("not_enough_cash", new System.String[] { "金钱不够", "not enough cash" });
        table.Add("not_enough_room", new System.String[] { "迷宫空间不够了", "not enough maze supported room" });
        table.Add("room", new System.String[] { "迷宫空间", "Room" });
        table.Add("property_is_safe_now", new System.String[] { "你的财产暂时安全了", "your property is safe now" });
        table.Add("guards", new System.String[] { "守卫", "Guards" });
        table.Add("maze", new System.String[] { "迷宫", "Maze" });
        table.Add("reach_rose_count_will_unlock", new System.String[] { "达到 {0} 魅力值即可解锁", "Reach {0} charm points will unlock" });// 中间的空格是给玫瑰图标留出来的        
        table.Add("armed", new System.String[] { "重装守卫", "Armed Slug" });
        table.Add("armed_desc", new System.String[] { "当发现自己守护的钱箱被偷的时候，会去守卫迷宫中别的钱箱。", "Will go to guard another safe box when realise his own has been stolen." });
        table.Add("guard", new System.String[] { "警卫", "Guard" });
        table.Add("guard_desc", new System.String[] { "接受过良好教育的执法者。他们表情严肃，心地善良。他们会奋力制服并活捉来犯的盗贼。", "guard description.." });
        table.Add("dog", new System.String[] { "狗", "Dog" });
        table.Add("dog_desc", new System.String[] { "忠实的警犬。当发现异常情况会立刻大叫，呼叫周围的守卫。不过智力不高，很容易把将各种会动的东西都当成追击的目标。", "Loyalty dog,  bark to call teammates around when spot anything unusual.not so smart, like to chase anything movable" });
        table.Add("lamp", new System.String[] { "灯", "Lamp" });
        table.Add("lamp_desc", new System.String[] { "如果窃贼在灯照下，守卫可以从远处看到他，当然，他们中间没有隔着墙的时候。", "guard will easily see thief when thief is under light." });
        table.Add("upgrade_maze_to_unlock", new System.String[] { "升级迷宫到{0}级以解锁", "Upgrade Maze to Lv.{0} to unlock" });
        table.Add("upgrade_maze_to_add_safe_box", new System.String[] { "升级迷宫到{0}级以增加更多保险箱", "Upgrade Maze to Lv.{0} to add more safe box" });
        table.Add("upgrade_maze_to_upgrade_safe_box", new System.String[] { "升级迷宫到{0}级以进一步升级保险箱", "Upgrade Maze to Lv.{0} to upgrade safe box" });
        table.Add("top_maze", new System.String[] { "迷宫已达到最高级", "Top Maze" });        
        table.Add("bigger_maze", new System.String[] { "更复杂的迷宫可以放入更多宝物箱以及守卫", "bigger maze support more safe cases and guards" });
        table.Add("current_lv", new System.String[] { "当前等级：", "Current Lv:" });
        table.Add("safebox", new System.String[] { "保险箱", "Safe Box" });
        table.Add("safe_box_desc", new System.String[] { "增加保险箱可以增加金钱的储量", "Add Safe Box to store more money" });
        table.Add("safe_box_upgrade_desc", new System.String[] { "这是用来存放你金钱的箱子。升级可增加容量。", "This is a box to store your money, upgrade will increase its capacity." });
        table.Add("cash_in_box", new System.String[] { "当前现金量", "Cash in this" });
        table.Add("capacity", new System.String[] { "容量", "Capacity" });
        
        table.Add("you_have_some_cash", new System.String[] { "你有{0}现金", "you have {0} Cash" });
        table.Add("you_have_some_safeboxes", new System.String[] { "你有{0}个保险箱", "you have {0} safeboxes" });
        table.Add("total_capacity", new System.String[] { "总容量：", "Total Capacity:" });
        table.Add("cash_average_put_in_box", new System.String[] { "现金被平均放入保险箱", "All cash been averaged put in safe box" });

        table.Add("this_is_tricks_in_use", new System.String[] { "使用中的魔术道具", "tricks in use" });
        table.Add("click_guard_to_show_info", new System.String[] { "点击守卫可查看信息", "click guard to show info" });
        table.Add("sure_to_buy_slot", new System.String[] { "确定要花费{0}现金购买这个道具使用槽吗？", "Are you sure cost {0} cash to buy this trick item slot?" });
        table.Add("already_bought", new System.String[] { "已购买", "Already Bought" });
        table.Add("already_unlocked", new System.String[] { "已解锁", "Already Unlocked" });
        table.Add("no_landmark_yet", new System.String[] { "还没有标记降落点", "no_landmark_yet" });
        table.Add("stealing_cant_use_flash", new System.String[] { "潜入中，不能使用照明弹", "stealing_cant_use_flash" });
        table.Add("place_guard_in_maze_to_protect_your_property", new System.String[] { "在迷宫中放置守卫来保护你的财产", "place_guard_in_maze_to_protect_your_property" });
        table.Add("hired", new System.String[] { "已雇佣", "Hired" });
        table.Add("not_hired_yet", new System.String[] { "尚未雇佣", "Not hired yet" });
        table.Add("no_more_safebox", new System.String[] { "没有更多的保险箱了", "No More Safebox" });
        table.Add("room_not_full_used", new System.String[] { "迷宫空间未完全使用，请放置更多守卫，以防其他玩家太轻易的盗取你的钱财", "room not full used" });        
   }    

    public void SetText(UnityEngine.UI.Text uiText, System.String key, System.String[] strings = null)
    {
        if (uiText == null)
        {
            Globals.Assert(false);
            return;
        }
        if (table.ContainsKey(key))
        {         
            uiText.text = GetText(key, strings);
        }
        else
        {
            uiText.text = key;
        }                
    }

    public System.String GetText(System.String key, System.String[] strings = null)
    {
        System.String str = "";
        if (strings == null)
        {
            str = (table[key])[(int)current];
        }
        else
        {
            str = (table[key])[(int)current];
            str = System.String.Format(str, strings);
        }
        return str;
    }
}
