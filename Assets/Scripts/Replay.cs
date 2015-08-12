public class Replay : UnityEngine.MonoBehaviour 
{
    public System.String pveFile;
    public int playSpeed = 1;
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

    public class FlashGrenadeRecord
    {
        public int frame_no;
        public UnityEngine.Vector3 pos;
    }
    public System.Collections.Generic.List<FlashGrenadeRecord> flashRecords = new System.Collections.Generic.List<FlashGrenadeRecord>();
    
    public class ClickOnGuard
    {
        public int frame_no;
        public int guard_idx;
    }
    public System.Collections.Generic.List<ClickOnGuard> clickOnGuardRecords = new System.Collections.Generic.List<ClickOnGuard>();
    
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
        pveFile = "";
        mageTryEscapeFrameNos.Clear();
        trickRecords.Clear();
        clickRecords.Clear();
        clickOnGuardRecords.Clear();
        magePositions.Clear();
    }

    public void RecordPvEFileName(System.String pve)
    {
        pveFile = pve;
    }

    public void RecordMagicCast(TrickData data)
    {
        if (Globals.playingReplay == null)
        {
            TrickRecord record = new TrickRecord();
            record.frame_no = Globals.LevelController.frameCount;
            data.CopyTo(record.data);
            trickRecords.Add(record);
        }                
    }

    public void RecordFlash(UnityEngine.Vector3 pos)
    {
        if (Globals.playingReplay == null)
        {
            FlashGrenadeRecord record = new FlashGrenadeRecord();
            record.frame_no = Globals.LevelController.frameCount;
            record.pos = pos;
            flashRecords.Add(record);
        }
    }

    public void RecordClickOnGuard(int guard_idx)
    {
        if (Globals.playingReplay == null)
        {
            ClickOnGuard record = new ClickOnGuard();
            record.frame_no = Globals.LevelController.frameCount;
            record.guard_idx = guard_idx;
            clickOnGuardRecords.Add(record);
        }                
    }

    public void RecordRightClickToMove(UnityEngine.Ray ray)
    {
        if (Globals.playingReplay == null)
        {
            ClickRecord record = new ClickRecord();
            record.frame_no = Globals.LevelController.frameCount;
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
            mageTryEscapeFrameNos.Add(Globals.LevelController.frameCount);
        }        
    }

    public void RecordMageFallingDown()
    {
        if (Globals.playingReplay == null)
        {
            mage_falling_down_frame_no = Globals.LevelController.frameCount;
        }
    }

    public IniFile Pack()
    {
        IniFile ini = new IniFile();

        ini.set("pveFile", pveFile);

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

        System.String clickOnGuardStr = "";
        foreach (ClickOnGuard record in clickOnGuardRecords)
        {
            clickOnGuardStr += record.frame_no + ",";
            clickOnGuardStr += record.guard_idx + ",";
            if (record != clickOnGuardRecords[clickOnGuardRecords.Count - 1])
            {
                clickOnGuardStr += " ";
            }
        }
        ini.set("clickOnGuardRecords", clickOnGuardStr);


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
        pveFile = ini.get("pveFile");

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

        records_str = ini.get("clickOnGuardRecords");
        if (records_str != "")
        {
            temp = records_str.Split(' ');
            foreach (System.String click_on_guard_str in temp)
            {
                System.String[] trick_data_str = click_on_guard_str.Split(',');
                ClickOnGuard record = new ClickOnGuard();
                record.frame_no = int.Parse(trick_data_str[0]);
                record.guard_idx = int.Parse(trick_data_str[1]);
                clickOnGuardRecords.Add(record);
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
            while (record.frame_no == Globals.LevelController.frameCount)
            {
                UnityEngine.Ray ray = new UnityEngine.Ray(record.ray_origin, record.ray_direction);
                (Globals.LevelController as StealingLevelController).RayOnMap(ray);
                clickRecords.RemoveAt(0);
                if (clickRecords.Count == 0)
                {
                    break;
                }
                record = clickRecords[0];
            }
        }


        if (clickOnGuardRecords.Count != 0)
        {
            ClickOnGuard record = clickOnGuardRecords[0];
            while (record.frame_no == Globals.LevelController.frameCount)
            {
                Guard hovered = null;
                foreach (Guard guard in Globals.maze.guards)
                {
                    if (record.guard_idx == guard.idx)
                    {
                        hovered = guard;
                    }
                }
                (Globals.LevelController as StealingLevelController).ClickOnHoveredGuard(hovered);
                clickOnGuardRecords.RemoveAt(0);
                if (clickOnGuardRecords.Count == 0)
                {
                    break;
                }
                record = clickOnGuardRecords[0];
            }
        }


        if (mage_falling_down_frame_no == Globals.LevelController.frameCount)
        {
            (Globals.LevelController as StealingLevelController).MagicianFallingDown();
        }

        if (mageTryEscapeFrameNos.Count != 0)
        {
            int try_escape_no = mageTryEscapeFrameNos[0];
            while (try_escape_no == Globals.LevelController.frameCount)
            {
                (Globals.LevelController as StealingLevelController).LeaveBtnClicked();
                mageTryEscapeFrameNos.RemoveAt(0);
                if (mageTryEscapeFrameNos.Count == 0)
                {
                    break;
                }
                try_escape_no = mageTryEscapeFrameNos[0];
            }
        }

        if (Globals.playingReplay == null)
        {
            return;
        }

        if (trickRecords.Count != 0)
        {
            TrickRecord record = trickRecords[0];
            while (record.frame_no == Globals.LevelController.frameCount)
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

        if (flashRecords.Count != 0)
        {
            FlashGrenadeRecord record = flashRecords[0];
            while (record.frame_no == Globals.LevelController.frameCount)
            {
                Globals.magician.CastFlash(record.pos);
                flashRecords.RemoveAt(0);
                if (flashRecords.Count == 0)
                {
                    break;
                }
                record = flashRecords[0];
            }
        }       
    }    
}
