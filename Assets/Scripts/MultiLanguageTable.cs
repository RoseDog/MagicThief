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
        table.Add("level_loading_progress", new System.String[] { "场景加载{0}%", "Level Loading:{0}%" });
        table.Add("power_cost", new System.String[] { "魔力消耗:{0}", "Power Cost:{0}" });
        table.Add("duration", new System.String[] { "持续时间:{0}", "Duration:{0}" });
        table.Add("unlock_need_rose", new System.String[] { "解锁：达到{0}朵玫瑰", "unlock: reach {0} rose"});
        table.Add("restart_tutorial_level_tip", new System.String[] { 
            "<b><color=red><size=30>{0}</size></color></b> 秒后重新开始\n\n 教程关卡你都能输，能专心点儿么 =.= ", 
            "Restart After <b><color=red><size=30>{0}</size></color></b> seconds" });
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
        table.Add("property_is_safe_now", new System.String[] { "你的财产暂时安全了", "your property is safe now" });
        table.Add("guards", new System.String[] { "守卫", "Guards" });
        table.Add("maze", new System.String[] { "迷宫", "Maze" });
        table.Add("reach_rose_count_will_unlock", new System.String[] { "达到 {0} 魅力值即可解锁", "Reach {0} charm points will unlock" });// 中间的空格是给玫瑰图标留出来的        
        table.Add("lancer", new System.String[] { "熊猫枪卫", "Panda Lancer" });
        table.Add("lancer_desc", new System.String[] { "当发现自己守护的钱箱被偷的时候，会去守卫迷宫中别的钱箱。", "Will go to guard another safe box when realise his own has been stolen." });
        table.Add("tiger", new System.String[] { "虎", "Tiger" });
        table.Add("tiger_desc", new System.String[] { "忠实的警犬。当发现异常情况会立刻大叫，呼叫周围的守卫。不过智力不高，很容易被将各种会动的东西都当成追击的目标。", "Loyalty dog,  bark to call teammates around when spot anything unusual.not so smart, like to chase anything movable" });
        table.Add("lamp", new System.String[] { "灯", "Lamp" });
        table.Add("lamp_desc", new System.String[] { "如果窃贼在灯照下，守卫可以从远处看到他，当然，他们中间没有隔着墙的时候。", "guard will easily see thief when thief is under light." });
        table.Add("upgrade_maze_to_unlock", new System.String[] { "升级迷宫到{0}级以得到更多守卫", "Upgrade Maze to Lv.{0} to summon more guards" });
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
   }    

    public void SetText(UnityEngine.UI.Text uiText, System.String key, System.String[] strings = null)
    {
        if (table.ContainsKey(key))
        {
            if (strings == null)
            {
                uiText.text = (table[key])[(int)current];
            }
            else
            {
                System.String str = GetText(key);
                uiText.text = System.String.Format(str, strings);
            }
        }
        else
        {
            uiText.text = key;
        }                
    }

    public System.String GetText(System.String key)
    {
        return (table[key])[(int)current];
    }
}
