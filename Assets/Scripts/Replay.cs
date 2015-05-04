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
    
    public System.Collections.Generic.List<int> mageTryEscapeFrameNos = new System.Collections.Generic.List<int>();

    public System.Collections.Generic.List<UnityEngine.Vector3> magePositions = new System.Collections.Generic.List<UnityEngine.Vector3>();

    public void Awake()
    {
        Globals.replaySystem = this;
        ResetData();
    }

    public void ResetData()
    {
        Globals.playingReplay = null;
        mage_falling_down_frame_no = -1;
        mageTryEscapeFrameNos.Clear();
        trickRecords.Clear();
        clickRecords.Clear();
        magePositions.Clear();
    } 

    public void RecordMagicCast(TrickData data)
    {
        if (Globals.playingReplay == null)
        {
            TrickRecord record = new TrickRecord();
            record.frame_no = UnityEngine.Time.frameCount - frameBeginNo;
            data.CopyTo(record.data);
            trickRecords.Add(record);
        }                
    }

    public void RecordClick(UnityEngine.Ray ray)
    {
        if (Globals.playingReplay == null)
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
        if (Globals.playingReplay == null)
        {
            magePositions.Add(pos);            
        }        
    }

    public void RecordMagicianTryEscape()
    {
        if (Globals.playingReplay == null)
        {
            mageTryEscapeFrameNos.Add(UnityEngine.Time.frameCount - frameBeginNo);
        }        
    }

    public void RecordMageFallingDown()
    {
        if (Globals.playingReplay == null)
        {
            mage_falling_down_frame_no = UnityEngine.Time.frameCount - frameBeginNo;
        }
    }

    public IniFile Pack()
    {
        IniFile ini = new IniFile();
       
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


        System.String tryEscapeNoStr = "";
        for (int idx = 0; idx < mageTryEscapeFrameNos.Count;++idx )
        {
            int frame = mageTryEscapeFrameNos[idx];
            tryEscapeNoStr += frame.ToString();
            if (idx != mageTryEscapeFrameNos.Count-1)
            {
                tryEscapeNoStr += ",";
            }
        }
        mageTryEscapeFrameNos.Clear();

        ini.set("mageTryEscapeFrameNos", tryEscapeNoStr);


        System.String magePositionsChars = System.Convert.ToBase64String(Globals.ConvertVector3ToByteArray(magePositions));                
        ini.set("magePositions", magePositionsChars);
        magePositions.Clear();

        return ini;        
    }

    public void Unpack(IniFile ini)
    {                
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
        records_str = ini.get("mageTryEscapeFrameNos");
        if (records_str != "")
        {
            temp = records_str.Split(',');
            foreach (System.String frame_no_str in temp)
            {                
                mageTryEscapeFrameNos.Add(System.Convert.ToInt32(frame_no_str));
            }
        }

        System.String magePositionsChars = ini.get("magePositions");
        var bytes = System.Convert.FromBase64String(magePositionsChars);
        magePositions = Globals.ConvertByteArrayToVector3List(bytes);
    }

    public void FrameFunc()
    {
        if (Globals.playingReplay == null)
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

        if (mageTryEscapeFrameNos.Count != 0)
        {
            int try_escape_no = mageTryEscapeFrameNos[0];
            while (try_escape_no == UnityEngine.Time.frameCount - frameBeginNo)
            {
                (Globals.LevelController as TutorialLevelController).LeaveBtnClicked();
                mageTryEscapeFrameNos.RemoveAt(0);
                if (mageTryEscapeFrameNos.Count == 0)
                {
                    break;
                }
                try_escape_no = mageTryEscapeFrameNos[0];
            }
        }          
    }

    void Update()
    {
        if (Globals.playingReplay == null)
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
