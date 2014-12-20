public class MyMazeBuilding : Building
{   
    public void IntoHomeMaze()
    {
        base.DivedIn();
        Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");        
    }
}