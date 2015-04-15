﻿public class LevelEditor : LevelController 
{    
    public override void Awake()
    {
        base.Awake();
        mainCanvas = GetComponent<UnityEngine.Canvas>();
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
    }

    public override void GuardCreated(Guard guard)
    {
        base.GuardCreated(guard);
        guard.InitArrangeUI();
    }
}
