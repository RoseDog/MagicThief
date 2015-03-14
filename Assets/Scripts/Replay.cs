public class Replay : UnityEngine.MonoBehaviour 
{
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

    int mage_falling_down_frame_no;

    public System.Collections.Generic.List<UnityEngine.Vector3> magePositions = new System.Collections.Generic.List<UnityEngine.Vector3>();

    public void Awake()
    {
        Globals.replay = this;
    }

    public void RecordMagicCast(TrickData data)
    {
        if (!Globals.PLAY_RECORDS)
        {
            TrickRecord record = new TrickRecord();
            record.frame_no = UnityEngine.Time.frameCount;
            data.CopyTo(record.data);
            trickRecords.Add(record);
        }                
    }

    public void RecordClick(UnityEngine.Ray ray)
    {
        if (!Globals.PLAY_RECORDS)
        {
            ClickRecord record = new ClickRecord();
            record.frame_no = UnityEngine.Time.frameCount;
            record.ray_origin = ray.origin;
            record.ray_direction = ray.direction;
            clickRecords.Add(record);
        }        
    }

    public void RecordMagePosition(UnityEngine.Vector3 pos)
    {
        if (!Globals.PLAY_RECORDS)
        {
            magePositions.Add(pos);
        }        
    }

    public void RecordMageFallingDown()
    {
        if (!Globals.PLAY_RECORDS)
        {
            mage_falling_down_frame_no = UnityEngine.Time.frameCount;
        }
    }

    public void SaveToFile()
    {
        if (!Globals.PLAY_RECORDS && Globals.DEBUG_REPLAY)
        {
            System.String replay_file_name = "replay";
            IniFile ini = Globals.SaveMazeIniFile(replay_file_name);

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
            ini.save(replay_file_name, true);

            System.IO.BinaryWriter dataOut = new System.IO.BinaryWriter(new System.IO.FileStream(
                replay_file_name + ".dat", System.IO.FileMode.Create));
            foreach(UnityEngine.Vector3 pos in magePositions )
            {
                dataOut.Write(pos.x);
                dataOut.Write(pos.y);
                dataOut.Write(pos.z);
            }
            dataOut.Close();
            magePositions.Clear();
        }                
    }

    public void ReadFile()
    {
        if (Globals.PLAY_RECORDS)
        {
            System.String replay_file_name = "replay";
            IniFile ini = Globals.ReadMazeIniFile(replay_file_name, true);

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

            System.IO.BinaryReader dataIn = new System.IO.BinaryReader(new System.IO.FileStream(replay_file_name + ".dat", System.IO.FileMode.Open));
            while (dataIn.PeekChar() != -1)
            {
                float x = dataIn.ReadSingle();
                float y = dataIn.ReadSingle();
                float z = dataIn.ReadSingle();
                magePositions.Add(new UnityEngine.Vector3(x,y,z));
            }
            dataIn.Close(); 
        }
    }

    public void FrameFunc()
    {
        if (!Globals.PLAY_RECORDS)
        {
            return;
        }        

        if (clickRecords.Count != 0)
        {
            ClickRecord record = clickRecords[0];
            while (record.frame_no == UnityEngine.Time.frameCount)
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

        if (mage_falling_down_frame_no == UnityEngine.Time.frameCount)
        {
            (Globals.LevelController as TutorialLevelController).MagicianFallingDown();
        }
    }

    void Update()
    {
        if (!Globals.PLAY_RECORDS)
        {
            return;
        }  

        if (trickRecords.Count != 0)
        {
            TrickRecord record = trickRecords[0];
            while (record.frame_no == UnityEngine.Time.frameCount)
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
