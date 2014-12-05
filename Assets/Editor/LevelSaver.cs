[UnityEditor.CustomEditor(typeof(MapGenerate))]
public class MapSaver : UnityEditor.Editor 
{
    System.String levelfile = "Tutorial_Level_1";
    public override void OnInspectorGUI () 
    {
        base.OnInspectorGUI();
        if (UnityEngine.GUILayout.Button("Save"))
        {
            IniFile ini = new IniFile(levelfile);
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
            ini.save(levelfile);
            UnityEngine.Debug.Log("save level data to " + levelfile + ".ini");
        }
    }    
}
