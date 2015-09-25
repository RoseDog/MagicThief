public class MyMazeBuilding : BuildingCouldDivedIn
{   
    public void IntoHomeMaze()
    {
        base.DivedIn();
        Globals.asyncLoad.ToLoadSceneAsync("MyMaze");        
    }
}