
public class MultiLanguageTable : UnityEngine.MonoBehaviour 
{
    public enum Languages
    {
        Chn = 0,
        Eng,
        ChnTraditional,
    }
    public Languages current;

    
    UnityEngine.Font chn_simple_font;
    UnityEngine.Font chn_traditional_font;
    public System.Collections.Generic.Dictionary<System.String, System.String[]> table = 
        new System.Collections.Generic.Dictionary<System.String, System.String[]>();
    void Awake()
    {
        if (UnityEngine.Application.systemLanguage == UnityEngine.SystemLanguage.ChineseSimplified)
        {
            current = Languages.Chn;
        }
        else if (UnityEngine.Application.systemLanguage == UnityEngine.SystemLanguage.ChineseTraditional)
        {
            current = Languages.Eng;
        }

        chn_simple_font = UnityEngine.Resources.Load("UI/simhei") as UnityEngine.Font;
        chn_traditional_font = UnityEngine.Resources.Load("UI/FZCuYuan-M03T") as UnityEngine.Font;
                
        Globals.languageTable = this;
        table.Add("version_error", new System.String[] { "版本过期，请先更新再进入游戏", "Outdated game version, please update to latest" });
        table.Add("leave", new System.String[]{"离开", "Leave"});
        table.Add("level_tip", new System.String[] { "关卡提示信息", "Level Tip" });
        table.Add("watch_out_guard", new System.String[] { "“小心那些守卫”", "“Watch out for guards”" });        
        table.Add("tricks", new System.String[] { "魔术", "Tricks" });
        table.Add("outfits", new System.String[] { "服装", "Outfits" });
        table.Add("hypnosis", new System.String[] { "催眠", "Hypnosis" });
        table.Add("hypnosis_desc", new System.String[] { "可对警卫和马戏猴子催眠。如果距离目标过远或目标发现你，催眠效果会很糟糕。", "Hypnotize the pointed security guard" });
        table.Add("shotLight", new System.String[] { "魔术牌手枪", "Poker Pistol" });
        table.Add("shotLight_desc", new System.String[] { "可以暂时破坏蜘蛛和提灯鹦鹉。\n也可直接释放吸引守卫注意力并配合“浮空”盗取迷宫财物。", "Can be used to break spiders, you can also use the sound to draw security guards' attention." });
        table.Add("disguise", new System.String[] { "易容", "Disguise" });
        table.Add("disguise_desc", new System.String[] { "魔术师仪容后可混进守卫之中，但是蜘蛛会发现你不对劲。", "Magician can disguise himself as a security guard." });
        table.Add("flyUp", new System.String[] { "浮空", "Levitate" });
        table.Add("flyUp_desc", new System.String[] { "可以飞起来并瞬间脱离守卫的追击。移动无视地形。", "Enable the magician to move across landforms" });
        table.Add("dove", new System.String[] { "鸽子戏法", "Dove Trick" });
        table.Add("dove_desc", new System.String[] { "放出鸽子吸引守卫的注意力。用来对付玩具狗效果显著。", "Release a dove to distract security guards.It is remarkbly effective for Robo-dog." });
        table.Add("flashGrenade", new System.String[] { "照明弹", "Flare" });
        table.Add("flashGrenade_desc", new System.String[] { "拖拽照明弹进行投掷，可驱散迷雾。只能在潜入之前使用。", "Drag the flare and drop it to clear the mist, you can only use this before you sneak in." });                       
        table.Add("level_loading_progress", new System.String[] { "场景加载{0}%", "Level Loading:{0}%" });
        table.Add("power_cost", new System.String[] { "魔力消耗:{0}", "Power Cost:{0}" });
        table.Add("drop_odds", new System.String[] { "潜入失败时有几率遗落:{0}%", "drop_odds:{0}%" });
        table.Add("weight", new System.String[] { "重量:{0}", "weight:{0}" });        
        table.Add("duration", new System.String[] { "持续时间:{0}", "Duration:{0}" });
        table.Add("unlock_need_rose", new System.String[] { "玫瑰达到 {0} 即可解锁", "Roses {0}" });
        table.Add("restart_tutorial_level_tip", new System.String[] { 
            "潜入失败！<b><color=red><size=30>{0}</size></color></b> 秒后重新开始该教程关卡 ", 
            "Failed！Restart After <b><color=red><size=30>{0}</size></color></b> seconds" });
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
        table.Add("COC", new System.String[] { "部落冲突", "V4Vendetta" });
        table.Add("boom_beach", new System.String[] { "海岛奇兵", "boom_beach" });
        table.Add("spy_mouse", new System.String[] { "间谍鼠", "Spy Mouse, EA" });
        table.Add("VR", new System.String[] { "VR潜入, Sphinx Entertainment", "VR潜入, Sphinx Entertainment" });
        table.Add("mark_of_the_ninja", new System.String[] { "忍者印记", "mark_of_the_ninja" });
        table.Add("stealth_inc", new System.String[] { "隐形公司", "stealth_inc" });
        table.Add("crap_game", new System.String[] { "烂游戏", "crap_game" });


        table.Add("guard_cant_be_here", new System.String[] { "守卫不能放在这里", "Guard Can't be placed here" });
        table.Add("guard_cant_be_on_wall", new System.String[] { "守卫不能放在墙体上", "Guard Can't be on the Wall" });
        table.Add("lamp_cant_be_on_road", new System.String[] { "这个装置不能放在路上", "The Machine can't be on the road" });
        table.Add("not_enough_power", new System.String[] { "魔力值不够了", "MP used up" });
        table.Add("city_events", new System.String[] { "城市事件", "City Events" });
        table.Add("my_maze_intro", new System.String[] { "自己的迷宫，来自马戏团的守卫们在这里进行演出，为你带来收入：{0}/h\n未装入保险箱的现金：{1}", "My Maze" });
        table.Add("income_intro", new System.String[] { "守卫表演收入：{0}/h\n未装入保险箱的现金：{1}", "income_intro" });
        table.Add("dive_in", new System.String[] { "潜入", "Sneak in" });
        table.Add("enter", new System.String[] { "进入", "Enter" });
        table.Add("spy", new System.String[] { "窥视", "Take a peep" });
        table.Add("target_tonight", new System.String[] { "你是个小偷，这是你今晚的目标", "you are a thief,\nthis is your target tonight" });
        table.Add("buy", new System.String[] { "购买:${0}", "Buy:${0}" });
        table.Add("learn", new System.String[] { "学习:${0}", "Learn:${0}" });
        table.Add("inventory", new System.String[] { "库存:{0}", "inventory:{0}" });
        table.Add("inventory zero", new System.String[] { "没有库存了", "no inventory" });        
        table.Add("not_learned", new System.String[] { "未学习", "Not learned yet" });
        
        table.Add("not_enough_roses", new System.String[] { "尚未解锁", "Locked" });
        table.Add("not_enough_cash", new System.String[] { "金钱不够", "Not enough cash" });
        table.Add("not_enough_room", new System.String[] { "坑位不够了", "Not enough toilet" });
        table.Add("room", new System.String[] { "坑位：{0}", "Toilet：{0}" });
        table.Add("property_is_safe_now", new System.String[] { "你的财产暂时安全了", "Your property is safe now" });
        table.Add("guards", new System.String[] { "守卫", "Guards" });
        table.Add("maze", new System.String[] { "迷宫", "Maze" });
        table.Add("reach_rose_count_will_unlock", new System.String[] { "达到 {0} 魅力值即可解锁", "Reach {0} Roses to unlock" });// 中间的空格是给玫瑰图标留出来的        
        table.Add("armed", new System.String[] { "雇佣兵", "Armed Slug" });
        table.Add("armed_desc", new System.String[] { "对来犯的窃贼从不手软。", "Will go to guard another safe box when realise his own has been stolen." });
        table.Add("joker", new System.String[] { "小丑", "Joker" });
        table.Add("joker_desc", new System.String[] { "使用舞步进行近距离攻击。\n可以被催眠。", "Melee unit,can be hypnotized." });
        table.Add("Police", new System.String[] { "警卫", "Security guard" });
        table.Add("Police_desc", new System.String[] { "近距离攻击。\n可以被催眠。", "Melee unit,can be hypnotized." });
        table.Add("guard", new System.String[] { "警卫", "Guard" });
        table.Add("guard_desc", new System.String[] { "接受过良好教育的执法者。他们会奋力制服并活捉来犯的盗贼。<color=red>当发现自己守护的保险箱箱被偷的时候，会去支援其他保险箱的守卫工作。\n它的视野不会驱散迷雾。</color>", "guard description.." });
        table.Add("dog", new System.String[] { "玩具狗", "Robo-Dog" });
        table.Add("dog_desc", new System.String[] { "当发现异常情况会立刻发出警报，呼叫周围的守卫。\n自爆攻击。它的视野不会驱散迷雾。\n自爆攻击\n容易被鸽子引开。", "Bark to call teammates when spot anything unusual.not so smart, like to chase anything movable.Vision won't clear mist.Explose to cause damage." });
        table.Add("Spider", new System.String[] { "蜘蛛", "Robo-Spider" });
        table.Add("Spider_desc", new System.String[] { "栖息在墙上，喷射蛛网。\n它的视野不会驱散迷雾。\n可被魔术牌手枪破坏。", "Perch on wall, spit web. Vision won't clear mist. Can be broken by Poker Pistol." });
        table.Add("Monkey", new System.String[] { "马戏猴子", "Circus Monkey" });
        table.Add("Monkey_desc", new System.String[] { "弹射出小鸟，远程攻击。\n它的视野不会驱散迷雾。\n移动速度缓慢。\n可以被催眠。", "Catapult big fat bird to enemy. Vision won't clear mist. Move slowly.Can be hypnotized." });
        table.Add("lamp", new System.String[] { "提灯鹦鹉", "Lantern Parrot" });
        table.Add("lamp_desc", new System.String[] { "如果窃贼在灯照下，守卫可以从远处看到他，当然，他们中间没有隔着墙的时候。", " When the thief is under the light of lattern, the guard will notice him from far away. Of course, at no time are there walls between them." });
        table.Add("upgrade_maze_to_unlock", new System.String[] { "升级迷宫到{0}级以解锁", "Upgrade Maze to Lv.{0} to unlock" });
        table.Add("upgrade_maze_to_add_safe_box", new System.String[] { "升级迷宫到{0}级以增加更多保险箱", "Upgrade Maze to Lv.{0} to add more safes" });
        table.Add("upgrade_maze_to_upgrade_safe_box", new System.String[] { "升级迷宫到{0}级以进一步升级保险箱", "Upgrade Maze to Lv.{0} to upgrade safe" });
        table.Add("top_safe_box", new System.String[] { "保险箱已达到最高等级", "Top Lv Safe" });
        table.Add("upgrade", new System.String[] { "升级", "Upgrade" });
        table.Add("guard_full_fill_his_duty", new System.String[] { "你的守卫履行了他的职责！", "your guard has fulfilled his responsibility!" });
        table.Add("excellent", new System.String[] { "很好！", "Excellent！" });        
        
        
        table.Add("top_maze", new System.String[] { "迷宫已达到最高级", "Top Maze" });        
        table.Add("bigger_maze", new System.String[] { "更复杂的迷宫可以放入更多宝物箱以及守卫", "bigger maze support more safe cases and guards" });
        table.Add("current_lv", new System.String[] { "当前等级：", "Current Lv:" });
        table.Add("next_lv", new System.String[] { "下一等级：", "Next Lv:" });        
        table.Add("safebox", new System.String[] { "保险箱", "Safe" });
        table.Add("safe_box_desc", new System.String[] { "增加保险箱可以增加金钱的储量", "Add Safe Box to store more money" });
        table.Add("safe_box_upgrade_desc", new System.String[] { "这是用来存放你金钱的箱子。升级可增加容量。", "This is a box to store your money, upgrade will increase its capacity." });
        table.Add("cash_in_box", new System.String[] { "当前现金量：", "Cash in this:" });
        table.Add("capacity", new System.String[] { "容量：{0}", "Capacity：{0}" });
        table.Add("capacity_lable", new System.String[] { "容量：", "Capacity：" });
        
        table.Add("you_have_some_cash", new System.String[] { "你有<color=yellow>{0}</color>现金", "you have Cash {0}" });
        table.Add("you_have_some_safeboxes", new System.String[] { "你有<color=yellow>{0}</color>个保险箱，总容量：", "you have {0} safes,Total Capacity:" });

        table.Add("cash_average_put_in_box", new System.String[] { "现金被平均放入保险箱", "All cash is put into safes on the average." });

        table.Add("this_is_tricks_in_use", new System.String[] { "道具槽", "tricks in use" });
        table.Add("click_guard_to_show_info", new System.String[] { "{0}级迷宫，点击守卫可查看信息", "Maze Lv.{0}\nClick on guard for more info" });
        table.Add("sure_to_buy_slot", new System.String[] { "确定要花费{0}现金购买这个道具使用槽吗？", "Are you sure spend {0} cash for this trick item slot?" });
        table.Add("sure_to_unlock_cloud", new System.String[] { "确定要花费{0}现金驱散这片雾霭吗？", "Are you sure to spend {0} cash to clear the mist?" });
        
        table.Add("already_bought", new System.String[] { "已购买", "Already Purchased" });
        table.Add("already_unlocked", new System.String[] { "已解锁", "Already Unlocked" });
        table.Add("no_landmark_yet", new System.String[] { "还没有标记降落点", "No landmark yet" });
        table.Add("no_hypnosis_target_nearby", new System.String[] { "附近没有可以催眠的目标", "no_hypnosis_target_nearby" });
        table.Add("cant_bring_flash_to_stealing", new System.String[] { "不能携带照明弹进行潜入", "can't bring flare to sneak in" });
        table.Add("place_guard_in_maze_to_protect_your_property", new System.String[] { "在迷宫中放置守卫来保护你的财产", "Let the guard join in the maze to protect your assets." });
        table.Add("hire", new System.String[] { "雇佣", "Hire" });
        table.Add("hired", new System.String[] { "已雇佣", "Hired" });
        table.Add("not_hired_yet", new System.String[] { "尚未雇佣", "Not hired yet" });
        table.Add("no_more_safebox", new System.String[] { "没有更多的保险箱了", "No More Safebox" });
        table.Add("room_not_full_used", new System.String[] { "坑位未完全使用，请放置更多守卫", "toilet not full used, place more guards" });
        table.Add("enter_name", new System.String[] { "请输入你的名字", "Enter your name" });
        table.Add("name_duplicated", new System.String[] { "名字重复，请重新输入", "Name Duplicated" });
        table.Add("spotted_cant_escape", new System.String[] { "被守卫发现！无法逃离！", "Spotted！Can't escape！" });
        table.Add("stealing_cash_info", new System.String[] { "箱子中的金钱:{0}\n散落在迷宫角落中的金钱：{1}\n从箱子里失窃的金钱:{2}\n从地上捡起来的金钱：{3}", "Total Cash:{0}" });
        table.Add("stealing_cash", new System.String[] { "失窃金钱：{0}", "Total Cash:{0}" });
        
        table.Add("stolen_by_others_event_guards_failed", new System.String[] { "{0}(魅力{1})盗取了你的钱财！", "{0}(Rose {1}) steal your wealth!" });
        table.Add("stolen_by_others_event_guards_success", new System.String[] { "你的守卫成功的赶走了{0}(魅力{1})！", "Your guard successfully drive {0}(Rose {1}) away！" });
        table.Add("you_stole_others_event", new System.String[] { "你潜入了{0}的迷宫", "You sneak into {0}'s Maze" });
        table.Add("replay", new System.String[] { "回放", "replay" });
        table.Add("few_days_ago", new System.String[] { "盗窃发生于{0}天前", "{0} days ago" });
        table.Add("few_hours_ago", new System.String[] { "盗窃发生于{0}小时前", "{0} hours ago" });
        table.Add("few_minutes_ago", new System.String[] { "盗窃发生于几分钟前", "few minutes ago" });
        table.Add("someone_intrude_your_maze", new System.String[] { "监控系统报告：有人潜入了你的迷宫", "Monitoring System Report:Someone has sneaked into your maze" });
        
        table.Add("news_success_1", new System.String[] { 
            "News\n-------\n珍宝失窃", 
            "News\n-------\nPrecious Stolen！" });

        table.Add("news_success_2", new System.String[] { 
            "EXTRA!!\n-------\n惊天魔盗\n作风华丽", 
            "EXTRA!!\n-------\n惊天魔盗\n作风华丽" });        

        table.Add("news_success_3", new System.String[] { 
            "损失财产价值：\n<color=red><size=40>{0}</size></color>\n", 
            "Stolen Property Valued：\n<color=red><size=40>{0}</size></color>\nMr.Rich Broken！\n-----\n-----\n----\n" });

        table.Add("get_paid", new System.String[] { 
            "你在深夜敲开黑市交易员的房门。他打开你带来的袋子，看到里面闪动的微光，脸上绽放出无比热情的笑容，立刻对你的货付了钱：\n<color=yellow>金钱+{0}</color>\n", 
            "midnight,you knock at a black market dealer's door. He opens your bag and something shiny revealed, he pays you at once with a splendous smile:\n<color=yellow>Cash +{0}</color>\n" });

        table.Add("then_you_leave", new System.String[] { 
            "你总是对虚伪的笑容感到不适。你收好现金，一言不发的转身离去。", 
            "while you don't like such a fake smile so you turn away with the cash and leave silently" });
        
        table.Add("perfect_stealing", new System.String[] { 
            "你正准备离开，他突然又叫住了你：“我差点忘了，为了谢谢你帮我们教训了那个傲慢的盗贼，这是额外付给你的”：\n<color=yellow>金钱+{0}</color>\n<color=red>玫瑰+{1}</color>\n", 
            "" });

        table.Add("lose_rose", new System.String[] { 
            "虽然你最终逃了出来，没有被守卫当场抓住，但是“绅士怪盗败走”的消息仍然四散开来，这显然对你在地下交易市场原本良好的声誉产生了影响：\n<color=red>玫瑰 -{0}</color>\n", 
            "You escaped eventually, but news about your failure still spreading out, which influenced on your reputation at black market.\n<color=red>Rose -{0}</color>\n" });

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
            "虽然罪恶被你铲除，但贫穷带来的消极情绪已经快要毁掉这里...\n你要花些钱来做点什么吗？", 
            "Even though you have had riddance of the evil here, desperat poverty have almost ruined this place...\nHow about costing some money to do something?" });
        table.Add("people_admire_you", new System.String[] { 
            "这里的人们敬仰你。", 
            "people living here admire you." });

        table.Add("rose_total", new System.String[] { 
            "玫瑰总数：{0}", 
            "Total roses：{0}" });
        table.Add("roseTimeLastText", new System.String[] { 
            "剩余玫瑰增长时间：{0}", 
            "Time left to receive roses：{0}" });
        
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
        table.Add("other_player_maze_name", new System.String[] { "{0}的迷宫，{1}级", "{0}'s Maze, Lv.{1}" });
        table.Add("rose_pick_tip", new System.String[] { "玫瑰 + {0}", "Rose + {0}" });
        table.Add("rose_intro", new System.String[] { "玫瑰可以分配到各项属性上，对魔术师进行强化。", "As an magician,nothing is more valuable than your charm." });
        table.Add("rose_to_be_allot", new System.String[] { "待分配的玫瑰", "rose_to_be_allot" });
        table.Add("base", new System.String[] { "基础值", "basic" });        


        table.Add("personal_storage", new System.String[] { "你买下这栋楼的房间，存放“你的”私人财产", "you purchase this room to keep “your” private assets." });
        table.Add("create_maze_to_stop_thief", new System.String[] { "用幻术创造一个迷宫，阻止窃贼", "Use magic to create a maze to stop the thief" });
        table.Add("you_are_not_the_only_thief", new System.String[] { "有人正打算偷窃你的钱财！", "someone is planning to steal your properties ." });
        table.Add("guard_failed", new System.String[] { "守卫没能抓住这个贼！", "Guard couldn't caught the thief." });
        table.Add("guard_again", new System.String[] { "重来一次", "try again" });
        table.Add("another_guard", new System.String[] { "再来一个守卫", "One more guard" });
        table.Add("another_thief", new System.String[] { "又一个小偷", "Another thief" });
        table.Add("what_do_i_do", new System.String[] { "我该怎么办？", "What should I do?" });
        table.Add("box_not_finished", new System.String[] { "还有钱可以拿", "Not finished" });
        table.Add("click_to_summon_guard", new System.String[] { "点击召唤守卫", "Calling guard" });
        table.Add("pve_intro", new System.String[] { "{0}级迷宫\n\"这里的主人是个臭名昭著的恶人，你决定拿走他的每一分钱。\"\n<color=red>直到成功才会更换目标</color>", "Lv.{0} Maze\nThe master here is a notorious man, you decide to take every peny of him.\n\n<color=red>Only complete this object successfully for another object</color>" });

        table.Add("pvp_intro", new System.String[] { "{0}级迷宫\n\"这里属于另一个玩家。\"\n<color=red>有额外奖励\n只有一次潜入机会</color>", "Lv.{0} Maze\nThis place belongs to another player, you got only one chance to sneak in.\n\n<color=red>There will be extra bonus if you successd.</color>" });
        table.Add("money_full", new System.String[] { "金钱已满", "Cash is full" });

        table.Add("try_other_trick", new System.String[] { "潜入失败，试试其他魔术道具？或购买更多的槽？", "Stealing failed!try another item? or buy more slots?" });

        table.Add("who_is_your_target", new System.String[] { "“今晚谁会是你的目标呢？”", "who_is_your_target" });
        table.Add("go_add_box", new System.String[] { "买箱子", "add box" });
        table.Add("add_box", new System.String[] { "增加箱子", "add box" });
        table.Add("MP:", new System.String[] { "MP:", "MP:" });
        table.Add("HP:", new System.String[] { "HP:", "HP:" });
        table.Add("buy_this", new System.String[] { "买下它", "buy_it" });
        table.Add("open_fog", new System.String[] { "开启迷雾", "Fog" });
        table.Add("close_fog", new System.String[] { "关闭迷雾", "Disable Fog" });

        table.Add("plz_bring_more_item", new System.String[] { "请携带更多道具", "you should carry more items" });
        table.Add("unlock_to_bring_more_item", new System.String[] { "解锁后可携带更多道具", "Unlock to carry more items." });
        table.Add("click_guard_to_cast", new System.String[] { "鼠标左键点击警卫或马戏猴子释放", "Left-click on guard to cast(no need to click the icon)" });
        table.Add("click_to_cast", new System.String[] { "点击图标或快捷键释放", "Click icon to cast" });
        table.Add("shortcut", new System.String[] { "<color=red>快捷键：{0}</color>", "<color=red>Shortcut:{0}</color>" });
        
        table.Add("drag_to_cast", new System.String[] { "拖拽释放", "Drag to cast" });

        table.Add("speed", new System.String[] { "追击速度", "Speed" });
        table.Add("vision", new System.String[] { "视野", "Vision" });
        table.Add("damage", new System.String[] { "伤害", "Damage" });
        table.Add("Performing_income", new System.String[] { "表演收入", "Performing income" });

        table.Add("operate_guide_info", new System.String[] { "<color=red>点击鼠标右键移动角色并盗取宝箱财物</color>", "<color=red>Right-click to move\nMove Mouse close to border to move the screen</color>" });
        table.Add("sneaking_guide_info", new System.String[] { "<color=red>移动时按住<size=30>R</size>键可消除脚步声</color>", "<color=red>Right-click to move\nMove Mouse close to border to move the screen</color>" });
        table.Add("hypnosis_guide_info", new System.String[] { "<color=red>按住<size=30>R</size>键靠近警卫将其催眠并盗取财物</color>", "<color=red>Right-click to move\nMove Mouse close to border to move the screen</color>" });
        

        table.Add("replay_speed", new System.String[] { "快进 x {0}", "Speed x {0}" });
        table.Add("circus", new System.String[] { "来自马戏团的守卫们", "Guards from circus" });
        table.Add("reward_accepted", new System.String[] { "已领取", "Circus" });
        table.Add("no", new System.String[] { "不给", "No" });


        table.Add("strength", new System.String[] { "力量：{0}", "Strength:{0}" });
        table.Add("agility", new System.String[] { "敏捷：{0}", "Agility:{0}" });
        table.Add("wisdom", new System.String[] { "智力：{0}", "Wisdom:{0}" });
        table.Add("Speed", new System.String[] { "移动速度：{0}", "Moving Speed:{0}" });
        table.Add("Unlocking Duration", new System.String[] { "开锁时间：{0}", "Unlocking Duration:{0}" });
        table.Add("TricksTotalWeight", new System.String[] { "身体负重：{0}", "Tricks total weights:{0}" });
        table.Add("magician_speed", new System.String[] { "移动速度{0},开锁时间减少{1}", "Speed {0}, Unlocking {1}" });

        table.Add("need_rose", new System.String[] { "玫瑰：{0}", "Rose:{0}" });
        table.Add("allot", new System.String[] { "分配", "allot" });
        table.Add("growth", new System.String[] { "增长", "growth" });

        table.Add("item_consumed_label", new System.String[] { "道具被消耗了：", "growth" });
        table.Add("item_consumed", new System.String[] { "<color=red>{0} -{1}</color>", "<color=red>{0} -{1}</color>" });
        table.Add("item_picked_label", new System.String[] { "你在迷宫里获得了一些道具：", "growth" });
        table.Add("item_picked", new System.String[] { "<color=yellow>{0} +{1}</color>", "<color=yellow>{0} +{1}</color>" });
        table.Add("item_dropped_label", new System.String[] { "狼狈逃离的你遗落了一些道具：", "growth" });
        table.Add("item_dropped", new System.String[] { "<color=red>{0} -{1}</color>", "<color=red>{0} -{1}</color>" });

        table.Add("character_require_rose", new System.String[] { "???:达到{0}朵玫瑰可使用该角色", "" });
        table.Add("cant_use_locked_magician", new System.String[] { "该角色还不能使用", "" });

        table.Add("Rosa", new System.String[] { "罗萨", "Rosa" });
        table.Add("Rosa_desc", new System.String[] { "德雷斯罗萨", "Rosa_desc" });

        table.Add("Walter", new System.String[] { "沃尔特", "Walter" });
        table.Add("Walter_desc", new System.String[] { "沃尔特", "Rosa_desc" });
   }    

    public void SetText(UnityEngine.UI.Text uiText, System.String key, System.String[] strings = null)
    {        
        if (uiText == null)
        {
            Globals.Assert(false);
            return;
        }

        if (current == Languages.Chn)
        {
            uiText.font = chn_simple_font;
        }
        else if (current == Languages.ChnTraditional)
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

    public System.String GetText(System.String key, System.String[] strings = null)
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
