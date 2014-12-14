[UnityEditor.CustomEditor(typeof(MapGenerate))]
public class MapSaver : UnityEditor.Editor 
{
    public override void OnInspectorGUI () 
    {
        base.OnInspectorGUI();
        if (UnityEngine.GUILayout.Button("Save"))
        {
            IniFile ini = new IniFile(Globals.map.IniFileNameForEditor);
            ini.clear();
            Guard[] guards = UnityEngine.GameObject.FindObjectsOfType<Guard>();
            ini.set("GuardCount", guards.Length);
            foreach (Guard guard in guards)
            {
                ini.set(Globals.GetPathNodePos(guard.birthNode).ToString("F4"), guard.gameObject.name);
            }
            ini.set("randSeedCacheWhenEditLevel", Globals.map.randSeedCacheWhenEditLevel);
            ini.set("Z_CELLS_COUNT", Globals.map.Z_CELLS_COUNT);
            ini.set("X_CELLS_COUNT", Globals.map.X_CELLS_COUNT);
            ini.set("CHANGE_DIRECTION_MODIFIER", Globals.map.CHANGE_DIRECTION_MODIFIER);
            ini.set("sparsenessModifier", Globals.map.sparsenessModifier);
            ini.set("deadEndRemovalModifier", Globals.map.deadEndRemovalModifier);
            ini.set("noOfRoomsToPlace", Globals.map.noOfRoomsToPlace);
            ini.set("minRoomXCellsCount", Globals.map.minRoomXCellsCount);
            ini.set("maxRoomXCellsCount", Globals.map.maxRoomXCellsCount);            
            ini.set("minRoomZCellsCount", Globals.map.minRoomZCellsCount);
            ini.set("maxRoomZCellsCount", Globals.map.maxRoomZCellsCount);
            ini.set("GEMS_COUNT", Globals.map.GEMS_COUNT);
            ini.set("LevelTipText", Globals.map.LevelTipText);

            ini.save(Globals.map.IniFileNameForEditor);
            UnityEngine.Debug.Log("save level data to " + Globals.map.IniFileNameForEditor + ".txt");
        }
    }    
}
