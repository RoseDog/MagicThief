public class Replay : UnityEngine.MonoBehaviour 
{
    public int frameBeginNo;
    public class TrickRecord
    {
        public int frame_no;
        public TrickData data = new TrickData();
    }
    public System.Collections.Generic.List<TrickRecord> trickRecords = new System.Collections.Generic.List<TrickRecord>();

    public class ClickRecord
    {
        public int frame_no;
        public UnityEngine.Vector3 ray_origin;
        public UnityEngine.Vector3 ray_direction;
    }
    public System.Collections.Generic.List<ClickRecord> clickRecords = new System.Collections.Generic.List<ClickRecord>();
    
    public int mage_falling_down_frame_no = -1;
    int mage_escape_frame_no;

    System.String safeBoxesStr;
    System.String magePositionsStr;

    public System.Collections.Generic.List<UnityEngine.Vector3> magePositions = new System.Collections.Generic.List<UnityEngine.Vector3>();

    public void Awake()
    {
        Globals.replay = this;
        ResetData();
    }

    public void ResetData()
    {
        Globals.replay_key = "";
        mage_falling_down_frame_no = -1;
        mage_escape_frame_no = -1;
        magePositionsStr = "";
        safeBoxesStr = "";
        trickRecords.Clear();
        clickRecords.Clear();
        magePositions.Clear();
    }

    public void RecordSafeboxes(PlayerInfo enemy)
    {
        if (Globals.replay_key == "")
        {
            safeBoxesStr = "";
            for (int idx = 0; idx < enemy.safeBoxDatas.Count; ++idx)
            {
                safeBoxesStr += enemy.safeBoxDatas[idx].Lv.ToString();
                if (idx != enemy.safeBoxDatas.Count-1)
                {
                    safeBoxesStr += ",";
                }
            }
        }
    }

    public void RecordMagicCast(TrickData data)
    {
        if (Globals.replay_key == "")
        {
            TrickRecord record = new TrickRecord();
            record.frame_no = UnityEngine.Time.frameCount - frameBeginNo;
            data.CopyTo(record.data);
            trickRecords.Add(record);
        }                
    }

    public void RecordClick(UnityEngine.Ray ray)
    {
        if (Globals.replay_key == "")
        {
            ClickRecord record = new ClickRecord();
            record.frame_no = UnityEngine.Time.frameCount - frameBeginNo;
            record.ray_origin = ray.origin;
            record.ray_direction = ray.direction;
            clickRecords.Add(record);
        }        
    }

    public void RecordMagePosition(UnityEngine.Vector3 pos)
    {
        if (Globals.replay_key == "")
        {
            if (magePositionsStr != "")
            {
                magePositionsStr += "_";
            }
            magePositionsStr += pos.ToString("F3");
        }        
    }

    public void RecordMagicianEscape()
    {
        if (Globals.replay_key == "")
        {
            mage_escape_frame_no = UnityEngine.Time.frameCount - frameBeginNo;
        }        
    }

    public void RecordMageFallingDown()
    {
        if (Globals.replay_key == "")
        {
            mage_falling_down_frame_no = UnityEngine.Time.frameCount - frameBeginNo;
        }
    }

    public IniFile Pack()
    {
        IniFile ini = Globals.SaveMazeIniFile("", Globals.LevelController.randSeedCache, Globals.self.stealingTarget.isPvP);

        ini.set("SafeBoxes", safeBoxesStr);        
       
        System.String TrickRecords = "";
        foreach (TrickRecord record in trickRecords)
        {
            TrickRecords += record.frame_no + ",";
            TrickRecords += record.data.nameKey + ",";
            TrickRecords += record.data.duration.ToString() + ",";
            TrickRecords += record.data.powerCost.ToString();
            if (record != trickRecords[trickRecords.Count - 1])
            {
                TrickRecords += " ";
            }
        }
        ini.set("TrickRecords", TrickRecords);

        System.String ClickRecords = "";
        foreach (ClickRecord record in clickRecords)
        {
            ClickRecords += record.frame_no + "_";
            ClickRecords += record.ray_origin.ToString("F3") + "_";
            ClickRecords += record.ray_direction.ToString("F3");

            if (record != clickRecords[clickRecords.Count - 1])
            {
                ClickRecords += ";";
            }
        }
        ini.set("ClickRecords", ClickRecords);
        ini.set("mage_falling_down_frame_no", mage_falling_down_frame_no);
        mage_falling_down_frame_no = -1;
        ini.set("mage_escape_frame_no", mage_escape_frame_no);
        

//         System.String magePositionsChars = "";
//         foreach (UnityEngine.Vector3 pos in magePositions)
//         {
//             magePositionsChars += System.Text.Encoding.UTF8.GetString(Globals.ConvertVector3ToByteArray(pos));
//         }        
        ini.set("magePositions", magePositionsStr);
        magePositionsStr = "";
        magePositions.Clear();        

        return ini;        
    }

    public void Unpack(IniFile ini)
    {        
        Globals.ReadMazeIni(ini);

        safeBoxesStr = ini.get("SafeBoxes");
        System.String[] safe_data_str = safeBoxesStr.Split(',');
        foreach(System.String data_str in safe_data_str)
        {
            SafeBoxData data = new SafeBoxData();
            Globals.self.enemy.safeBoxDatas.Add(data);
            data.Lv = System.Convert.ToInt32(data_str);
        }
        
        System.String records_str = ini.get("TrickRecords");
        System.String[] temp;
        if (records_str != "")
        {
            temp = records_str.Split(' ');
            foreach (System.String trick_str in temp)
            {
                System.String[] trick_data_str = trick_str.Split(',');
                TrickRecord record = new TrickRecord();
                record.frame_no = int.Parse(trick_data_str[0]);
                record.data.nameKey = trick_data_str[1];
                record.data.duration = int.Parse(trick_data_str[2]);
                record.data.powerCost = int.Parse(trick_data_str[3]);
                trickRecords.Add(record);
            }
        }
        
        records_str = ini.get("ClickRecords");
        if (records_str != "")
        {
            temp = records_str.Split(';');
            foreach (System.String click_str in temp)
            {
                System.String[] strs = click_str.Split('_');
                ClickRecord record = new ClickRecord();
                record.frame_no = int.Parse(strs[0]);
                record.ray_origin = Globals.StringToVector3(strs[1]);
                record.ray_direction = Globals.StringToVector3(strs[2]);
                clickRecords.Add(record);
            }
        }

        mage_falling_down_frame_no = ini.get("mage_falling_down_frame_no", -1);
        mage_escape_frame_no = ini.get("mage_escape_frame_no", -1);

//         System.String magePositionsChars = ini.get("magePositions");
//         var bytes = System.Text.Encoding.UTF8.GetBytes(magePositionsChars);
//         magePositions = Globals.ConvertByteArrayToVector3List(bytes)        
        magePositionsStr = ini.get("magePositions");
        System.String[] posStrs = magePositionsStr.Split('_');
        foreach (System.String pos_str in posStrs)
        {
            magePositions.Add(Globals.StringToVector3(pos_str));
        }
    }

    public void FrameFunc()
    {
        if (Globals.replay_key == "")
        {
            return;
        }        

        if (clickRecords.Count != 0)
        {
            ClickRecord record = clickRecords[0];
            while (record.frame_no == UnityEngine.Time.frameCount - frameBeginNo)
            {
                UnityEngine.Ray ray = new UnityEngine.Ray(record.ray_origin, record.ray_direction);
                (Globals.LevelController as TutorialLevelController).RayOnMap(ray);
                clickRecords.RemoveAt(0);
                if (clickRecords.Count == 0)
                {
                    break;
                }
                record = clickRecords[0];
            }
        }

        if (mage_falling_down_frame_no == UnityEngine.Time.frameCount - frameBeginNo)
        {
            (Globals.LevelController as TutorialLevelController).MagicianFallingDown();
        }

        if (mage_escape_frame_no == UnityEngine.Time.frameCount - frameBeginNo)
        {
            (Globals.LevelController as TutorialLevelController).Leave();
        }        
    }

    void Update()
    {
        if (Globals.replay_key == "")
        {
            return;
        }  

        if (trickRecords.Count != 0)
        {
            TrickRecord record = trickRecords[0];
            while (record.frame_no == UnityEngine.Time.frameCount - frameBeginNo)
            {
                Globals.magician.CastMagic(record.data);
                trickRecords.RemoveAt(0);
                if (trickRecords.Count == 0)
                {
                    break;
                }
                record = trickRecords[0];
            }
        }
    }
}
