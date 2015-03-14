public class ResetPlayerprefs : UnityEditor.Editor  
{
    [UnityEditor.MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs() { UnityEngine.PlayerPrefs.DeleteAll(); }	
}
