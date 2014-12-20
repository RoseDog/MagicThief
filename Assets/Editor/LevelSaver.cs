[UnityEditor.CustomEditor(typeof(MazeGenerate))]
public class MapSaver : UnityEditor.Editor 
{
    public override void OnInspectorGUI () 
    {
        base.OnInspectorGUI();
        if (UnityEngine.GUILayout.Button("Save"))
        {
            Globals.SaveMazeIniFile(Globals.maze.IniFileNameForEditor);
            UnityEngine.Debug.Log("save level data to " + Globals.maze.IniFileNameForEditor + ".txt");
        }
    }    
}
