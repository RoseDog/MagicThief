public class LevelEditor : LevelController 
{    
    public override void Awake()
    {
        base.Awake();
        mainCanvas = GetComponent<UnityEngine.Canvas>();

        UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(0.0f, 10.0f), UnityEngine.Random.Range(0.0f, 10.0f), UnityEngine.Random.Range(0.0f, 10.0f));

//         System.String str = System.Text.Encoding.UTF8.GetString(Globals.ConvertVector3ToByteArray(pos));        
//         var bytes = System.Text.Encoding.UTF8.GetBytes(str);
//         System.Collections.Generic.List<UnityEngine.Vector3> vecs = Globals.ConvertByteArrayToVector3List(bytes);
//         UnityEngine.Vector3 pos2 = vecs[0];

        var floatArray = new float[] { pos.x, pos.y, pos.z };

        // create a byte array and copy the floats into it...
        var byteArray = new byte[floatArray.Length * 4];
        System.Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);


        System.String str = System.Convert.ToBase64String(byteArray);
        var byteArray2 = System.Convert.FromBase64String(str);

        UnityEngine.Vector3 vec = UnityEngine.Vector3.zero;
        vec.x = System.BitConverter.ToSingle(byteArray2, 0 * 4);
        vec.y = System.BitConverter.ToSingle(byteArray2, 1 * 4);
        vec.z = System.BitConverter.ToSingle(byteArray2, 2 * 4);
    }
    public override void BeforeGenerateMaze()
    {
        // 如果没有iniFile或iniFile内容为空，生成新的迷宫        
        UnityEngine.TextAsset textAssets = UnityEngine.Resources.Load(Globals.maze.IniFileNameForEditor) as UnityEngine.TextAsset;
        if (textAssets == null || textAssets.text.Length == 0)
        {           
            randSeedCache = (int)System.DateTime.Now.Ticks;
            UnityEngine.Random.seed = randSeedCache;            
        }
        // 如果有，就读文件
        else
        {
            Globals.iniFileName = Globals.maze.IniFileNameForEditor;
            Globals.ReadMazeIniFile(Globals.iniFileName);
            randSeedCache = UnityEngine.Random.seed;
            base.BeforeGenerateMaze();
        }        
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.selectGuard.mover.BeginMove(Globals.uiMoveAndScaleDuration);
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);
        Globals.maze.RegistGuardArrangeEvent();
        foreach (Chest chest in Globals.maze.chests)
        {
            chest.Visible(true);
        }
        Globals.maze.PlaceGemsAtBoarder();
        Globals.cameraFollowMagician.Reset();
    }

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        AstarPath.CalculatePaths(AstarPath.threadInfos[0]);
    }
}
