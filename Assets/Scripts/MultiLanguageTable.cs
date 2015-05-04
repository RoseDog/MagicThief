
public class MultiLanguageTable : UnityEngine.MonoBehaviour 
{
    public enum Languages
    {
        Chn = 0,        
        Eng,
        ChnTraditional,        
    }
    public Languages current = Languages.Chn;
    UnityEngine.Font chn_simple_font;
    UnityEngine.Font chn_traditional_font;
    public System.Collections.Generic.Dictionary<System.String, System.String[]> table = 
        new System.Collections.Generic.Dictionary<System.String, System.String[]>();
    void Awake()
    {
        chn_simple_font = UnityEngine.Resources.Load("UI/FZCuYuan-M03S") as UnityEngine.Font;
        chn_traditional_font = UnityEngine.Resources.Load("UI/FZCuYuan-M03T") as UnityEngine.Font;
                
        Globals.languageTable = this;
        table.Add("leave", new System.String[]{"离开", "Leave"});
        table.Add("level_tip", new System.String[] { "关卡提示信息", "Level Tip" });
        table.Add("tricks", new System.String[] { "魔术", "Tricks" });
        table.Add("outfits", new System.String[] { "服装", "Outfits" });
        table.Add("hypnosis", new System.String[] { "催眠", "Hypnosis" });
        table.Add("hypnosis_desc", new System.String[] { "对距离最近的人类守卫施行催眠，让守卫沉睡一小会儿。", "make a guard go to sleep" });
        table.Add("shotLight", new System.String[] { "魔术牌手枪", "Gun" });
        table.Add("shotLight_desc", new System.String[] { "原地开枪可发出巨大声响，吸引守卫注意力。也可对机械萤火虫发射，魔术牌会卡在目标的关键部位使其一段时间内无法照明", "A fancy Gun" });
        table.Add("disguise", new System.String[] { "易容", "Disguise" });
        table.Add("disguise_desc", new System.String[] { "改变自己的容貌，守卫无法识破你。<color=red>任何时候易容都可以骗过机械狗和重装守卫\n在警卫发现你之后才进行易容将无法骗过警卫。</color>", "Disguise" });
        table.Add("flyUp", new System.String[] { "银翼的魔术师", "Glider" });
        table.Add("flyUp_desc", new System.String[] { "展开长袍，利用空气动力将自己托到空中。", "Use a glider to fly" });
        table.Add("dove", new System.String[] { "鸽子戏法", "Dove Trick" });
        table.Add("dove_desc", new System.String[] { "放出小动物吸引守卫的注意力。<color=red>用来对付机械狗效果显著。</color>", "release animals to distract guards" });
        table.Add("flash_grenade", new System.String[] { "照明弹", "Flare" });
        table.Add("flash_grenade_desc", new System.String[] { "<color=red>先设置降落点</color>，然后点击照明弹进行投掷，可驱散迷雾。<color=red>只能在潜入之前使用</color>", "Flare" });                       
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
        table.Add("kaitokid", new System.String[] { "魔术快斗", "Kaito Kid" });
        table.Add("magic_thief", new System.String[] { "怪盗基德", "magic_thief" });
        table.Add("silver_wing", new System.String[] { "银翼", "silver_wing" });
        table.Add("staring", new System.String[] { "注视", "staring" });
        table.Add("under_moon", new System.String[] { "月下", "under_moon" });
        table.Add("in_the_shadow", new System.String[] { "阴影之中", "in_the_shadow" });
        table.Add("quitely", new System.String[] { "悄无声息", "quitely" });
        table.Add("gambling", new System.String[] { "赌博", "gambling" });
        table.Add("cover_truth", new System.String[] { "掩盖真相", "cover_truth" });
        table.Add("secret_keeper", new System.String[] { "守密者", "secret_keeper" });
        table.Add("now_you_see_me", new System.String[] { "Now You See Me", "now_you_see_me" });
        table.Add("COC", new System.String[] { "部落冲突", "Clash of Clans" });
        table.Add("boom_beach", new System.String[] { "海岛奇兵", "boom_beach" });
        table.Add("spy_mouse", new System.String[] { "间谍鼠", "Spy Mouse, EA" });
        table.Add("VR", new System.String[] { "VR潜入, Sphinx Entertainment", "VR潜入, Sphinx Entertainment" });
        table.Add("mark_of_the_ninja", new System.String[] { "忍者印记", "mark_of_the_ninja" });
        table.Add("stealth_inc", new System.String[] { "隐形公司", "stealth_inc" });


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
        table.Add("room", new System.String[] { "迷宫空间：{0}", "Room：{0}" });
        table.Add("property_is_safe_now", new System.String[] { "你的财产暂时安全了", "your property is safe now" });
        table.Add("guards", new System.String[] { "守卫", "Guards" });
        table.Add("maze", new System.String[] { "迷宫", "Maze" });
        table.Add("reach_rose_count_will_unlock", new System.String[] { "达到 {0} 魅力值即可解锁", "Reach {0} charm points will unlock" });// 中间的空格是给玫瑰图标留出来的        
        table.Add("armed", new System.String[] { "重装守卫", "Armed Slug" });
        table.Add("armed_desc", new System.String[] { "隐匿在黑暗中的执法者，对来犯的窃贼从不手软。<color=red>它的视野不会驱散迷雾。</color>", "Will go to guard another safe box when realise his own has been stolen." });
        table.Add("guard", new System.String[] { "警卫", "Guard" });
        table.Add("guard_desc", new System.String[] { "接受过良好教育的执法者。他们会奋力制服并活捉来犯的盗贼。<color=red>当发现自己守护的保险箱箱被偷的时候，会去支援其他保险箱的守卫工作。</color>", "guard description.." });
        table.Add("dog", new System.String[] { "机械狗", "Dog" });
        table.Add("dog_desc", new System.String[] { "当发现异常情况会立刻发出警报，呼叫周围的守卫。不过智力不高，很容易把将各种会动的东西都当成追击的目标。\n<color=red>它的视野不会驱散迷雾。</color>\n<color=red>自爆攻击</color>", "Loyalty dog,  bark to call teammates around when spot anything unusual.not so smart, like to chase anything movable" });
        table.Add("lamp", new System.String[] { "机械萤火虫", "Lamp" });
        table.Add("lamp_desc", new System.String[] { "如果窃贼在灯照下，守卫可以从远处看到他，当然，他们中间没有隔着墙的时候。", "guard will easily see thief when thief is under light." });
        table.Add("upgrade_maze_to_unlock", new System.String[] { "升级迷宫到{0}级以解锁", "Upgrade Maze to Lv.{0} to unlock" });
        table.Add("upgrade_maze_to_add_safe_box", new System.String[] { "升级迷宫到{0}级以增加更多保险箱", "Upgrade Maze to Lv.{0} to add more safe box" });
        table.Add("upgrade_maze_to_upgrade_safe_box", new System.String[] { "升级迷宫到{0}级以进一步升级保险箱", "Upgrade Maze to Lv.{0} to upgrade safe box" });
        table.Add("top_safe_box", new System.String[] { "保险箱已达到最高等级", "top_safe_box" });
        
        table.Add("top_maze", new System.String[] { "迷宫已达到最高级", "Top Maze" });        
        table.Add("bigger_maze", new System.String[] { "更复杂的迷宫可以放入更多宝物箱以及守卫", "bigger maze support more safe cases and guards" });
        table.Add("current_lv", new System.String[] { "当前等级：", "Current Lv:" });
        table.Add("safebox", new System.String[] { "保险箱", "Safe Box" });
        table.Add("safe_box_desc", new System.String[] { "增加保险箱可以增加金钱的储量", "Add Safe Box to store more money" });
        table.Add("safe_box_upgrade_desc", new System.String[] { "这是用来存放你金钱的箱子。升级可增加容量。", "This is a box to store your money, upgrade will increase its capacity." });
        table.Add("cash_in_box", new System.String[] { "当前现金量", "Cash in this" });
        table.Add("capacity", new System.String[] { "容量：{0}", "Capacity：{0}" });
        
        table.Add("you_have_some_cash", new System.String[] { "你有{0}现金", "you have {0} Cash" });
        table.Add("you_have_some_safeboxes", new System.String[] { "你有{0}个保险箱", "you have {0} safeboxes" });
        table.Add("total_capacity", new System.String[] { "总容量：", "Total Capacity:" });
        table.Add("cash_average_put_in_box", new System.String[] { "现金被平均放入保险箱", "All cash been averaged put in safe box" });

        table.Add("this_is_tricks_in_use", new System.String[] { "道具槽", "tricks in use" });
        table.Add("click_guard_to_show_info", new System.String[] { "点击守卫可查看信息", "click guard to show info" });
        table.Add("sure_to_buy_slot", new System.String[] { "确定要花费{0}现金购买这个道具使用槽吗？", "Are you sure cost {0} cash to buy this trick item slot?" });
        table.Add("already_bought", new System.String[] { "已购买", "Already Bought" });
        table.Add("already_unlocked", new System.String[] { "已解锁", "Already Unlocked" });
        table.Add("no_landmark_yet", new System.String[] { "还没有标记降落点", "no_landmark_yet" });
        table.Add("no_hypnosis_target_nearby", new System.String[] { "附近没有可以催眠的目标", "no_hypnosis_target_nearby" });        
        table.Add("stealing_cant_use_flash", new System.String[] { "潜入中，不能使用照明弹", "stealing_cant_use_flash" });
        table.Add("place_guard_in_maze_to_protect_your_property", new System.String[] { "在迷宫中放置守卫来保护你的财产", "place_guard_in_maze_to_protect_your_property" });
        table.Add("hired", new System.String[] { "已雇佣", "Hired" });
        table.Add("not_hired_yet", new System.String[] { "尚未雇佣", "Not hired yet" });
        table.Add("no_more_safebox", new System.String[] { "没有更多的保险箱了", "No More Safebox" });
        table.Add("room_not_full_used", new System.String[] { "迷宫空间未完全使用，请放置更多守卫，以防其他玩家太轻易的盗取你的钱财", "room not full used" });
        table.Add("enter_name", new System.String[] { "请输入你的名字", "Enter a name" });
        table.Add("name_duplicated", new System.String[] { "名字重复，请重新输入", "Name Duplicated" });
        table.Add("spotted_cant_escape", new System.String[] { "被守卫发现！无法逃离！", "Spotted！Can't escape！" });        
        table.Add("cash_back_then", new System.String[] { "迷宫金钱:{0}", "Total Cash:{0}" });
        table.Add("stealing_cash", new System.String[] { "失窃金钱:{0}", "Stolen:{0}" });
        table.Add("stolen_by_others_event", new System.String[] { "{0}潜入了你的迷宫", "{0} dived in your Maze" });
        table.Add("you_stole_others_event", new System.String[] { "你潜入了{0}的迷宫", "You dived in {0}'s Maze" });        

        table.Add("news_success_1", new System.String[] { 
            "News\n-------\n珍宝失窃", 
            "News\n-------\nPrecious Stolen！" });

        table.Add("news_success_2", new System.String[] { 
            "EXTRA!!\n-------\n惊天魔盗\n作风华丽", 
            "EXTRA!!\n-------\n惊天魔盗\n作风华丽" });        

        table.Add("news_success_3", new System.String[] { 
            "损失财产价值：\n<color=red><size=40>{0}</size></color>\n", 
            "Stolen Property Valued：\n<color=red><size=40>{0}</size></color>\nMr.Rich Broken！\n-----\n-----\n----\n" });

        table.Add("perfect_stealing", new System.String[] { 
            "<size=30>你高超的技艺完美的破解了迷宫！</size>\n<color=red>完美潜入：\n<size=40>金钱+{0}\n玫瑰+{1}</size></color>\n富商破产！\n-----", 
            "Stolen Property Valued：\n<color=red><size=40>{0}</size></color>\nMr.Rich Broken！\n-----\n-----\n----\n" });

        table.Add("news_failure_1", new System.String[] { 
            "News\n-------\n魔盗再现！！", 
            "News\n-------\n魔盗再现！！" });

        table.Add("news_failure_2", new System.String[] { 
            "魔盗无机可趁！", 
            "魔盗无机可趁！" });

        table.Add("news_failure_3", new System.String[] { 
            "守卫森严\n魔盗败走", 
            "守卫森严\n魔盗败走" });

        table.Add("would_you_share", new System.String[] { 
            "奸商长年非法敛财使得这里的居民生活压力很大，犯罪率居高不下，要把你盗窃的成果分一些给他们吗？", 
            "would you share your money with poor?" });
        table.Add("new_target_event", new System.String[] { 
            "新的目标：{0}", 
            "New Target：{0}" });
        table.Add("cannot_connect_server", new System.String[] { 
            "无法连接服务器", 
            "cannot_connect_server" });
        table.Add("rank", new System.String[] { 
            "通缉令", 
            "Ranks" });
        table.Add("stealing_record", new System.String[] { 
            "潜入记录", 
            "Stealing Records" });
        table.Add("visit", new System.String[] { 
            "访问", 
            "Visit" });

        table.Add("stealing_event", new System.String[] { "{0}潜入了{1}的迷宫", "{0} dived in {1}'s Maze" });
        table.Add("other_player_maze_name", new System.String[] { "{0}的迷宫", "{0}'s Maze" });
        table.Add("rose_pick_tip", new System.String[] { "玫瑰 + {0}\n魔力 + {1}", "Rose + {0}\nPower + {1}" });
        table.Add("nothing_over_charm", new System.String[] { "作为一个魔术师，还有什么能比你的魅力更重要呢：）", "nothing_over_charm" });
        table.Add("rose_add_power", new System.String[] { "每{0}朵玫瑰增加1法力上限", "rose_add_power {0}" });
        table.Add("power_delta", new System.String[] { "+ {0}", "+ {0}" });

        table.Add("personal_storage", new System.String[] { "你买下这栋楼的房间，存放“你的”私人财产", "personal_storage" });
        table.Add("create_maze_to_stop_thief", new System.String[] { "用幻术创造一个迷宫，阻止窃贼", "create_maze_to_stop_thief" });
        table.Add("you_are_not_the_only_thief", new System.String[] { "有人正打算偷窃你的钱财！", "you_are_not_the_only_thief" });
        table.Add("guard_failed", new System.String[] { "守卫没能抓住这个贼！", "guard_failed" });
        table.Add("guard_again", new System.String[] { "重来一次", "guard_again" });
        table.Add("another_guard", new System.String[] { "再来一个守卫", "another_guard" });
        table.Add("another_thief", new System.String[] { "又一个小偷", "another_thief" });
        table.Add("what_do_i_do", new System.String[] { "我该怎么办？", "what_do_i_do" });
        table.Add("box_not_finished", new System.String[] { "还有钱可以拿", "box_not_finished" });
        table.Add("click_to_summon_guard", new System.String[] { "点击召唤守卫", "click_to_summon_guard" });
        table.Add("pve_intro", new System.String[] { "\"这里的主人是个臭名昭著的恶人，你决定拿走他的每一分钱！\"\n\n<color=white>必须成功才会更换目标</color>\n<color=white>潜入之后无法主动撤离</color>", "take every peny of him" });
        table.Add("pvp_intro", new System.String[] { "\"这个迷宫属于另一个玩家，你只有一次潜入的机会\"\n\n<color=white>偷取所有的金钱会有额外奖励</color>\n<color=white>潜入之后可以撤离</color>\n<color=white>潜入之后无论是否成功都会更换目标</color>", "another player, u got only one chance" });
   }    

    public void SetText(UnityEngine.UI.Text uiText, System.String key, System.String[] strings = null)
    {        
        if (uiText == null)
        {
            Globals.Assert(false);
            return;
        }
        if(current == Languages.Chn)
        {
            uiText.font = chn_simple_font;
        }
        else if(current == Languages.ChnTraditional)
        {
            uiText.font = chn_traditional_font;
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

    System.String GetText(System.String key, System.String[] strings = null)
    {
        System.String str = "";
        if (table.ContainsKey(key))
        {
            if (strings == null)
            {
                str = (table[key])[(int)current];
            }
            else
            {
                str = (table[key])[(int)current];
                str = System.String.Format(str, strings);
            }
        }
        else
        {
            return key;
        }
        
        return str;
    }
}
