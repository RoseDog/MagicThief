public class LevelController : Actor
{
    public int randSeedCache;
    public UnityEngine.Canvas mainCanvas;
    UnityEngine.Camera MiniMapCamera;
    public UnityEngine.GameObject fogPlane;
    public UnityEngine.Camera fogCam;
    protected UnityEngine.Texture2D fogTex;
    
    public override void Awake()
    {
        base.Awake();
        Globals.LevelController = this;
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(mgrs_prefab);
        }
        if (Globals.magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Rosa") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(magician_prefab);
            Globals.magician.gameObject.SetActive(false);
        }
        
        UnityEngine.GameObject minimapCamObj = UnityEngine.GameObject.Find("MiniMapCamera");
        if (minimapCamObj != null)
        {
            MiniMapCamera = minimapCamObj.GetComponent<UnityEngine.Camera>();
        }

        if (UnityEngine.GameObject.Find("FogCamera") != null)
        {
            fogCam = UnityEngine.GameObject.Find("FogCamera").GetComponent<UnityEngine.Camera>();
            fogPlane = UnityEngine.GameObject.Find("FogPlane");            
        }        
    }

    public override void Start()
    {
        base.Start();
        if (Globals.canvasForMagician == null && (this as LevelEditor) == null)
        {
            UnityEngine.GameObject canvas_prefab = UnityEngine.Resources.Load("CanvasForMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(canvas_prefab);
        }
        // 这个是防止LevelPassed重复调用的，
        bIsPerfectStealing = false;   
    }

    public virtual void BeforeGenerateMaze()
    {
        
    }

    public virtual IniFile GetGuardsIniFile()
    {
        return new IniFile(Globals.iniFileName);
    }

    public virtual void MazeFinished()
    {        
        // 创建相机，不允许跟随
        if (Globals.cameraFollowMagician == null)
        {
            UnityEngine.GameObject camera_follow_prefab = UnityEngine.Resources.Load("CameraFollowMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(camera_follow_prefab);
        }
        Globals.maze.SetRestrictToCamera(Globals.cameraFollowMagician);

        UnityEngine.Debug.Log(Globals.iniFileName);
        IniFile ini = GetGuardsIniFile();
        if (ini != null)
        {
            // 关卡守卫               
            int guard_count = ini.get("GuardCount", 0);
            System.String[] keys = ini.keys();
            for (int i = 1; i <= guard_count; ++i)
            {
                UnityEngine.Vector3 pos = Globals.StringToVector3(keys[i]);
                Pathfinding.Node birthNode = Globals.maze.pathFinder.GetSingleNode(pos, false);

                Guard guard = Globals.CreateGuard(Globals.GetGuardData(ini.get(keys[i])), birthNode);
                guard.BeginPatrol();
            }
        }
        
        Globals.transition.BlackIn();

        MiniMapCamera.transform.position = new UnityEngine.Vector3(
            Globals.maze.center_pos.x,
            Globals.maze.center_pos.y,
            MiniMapCamera.transform.position.z);
        MiniMapCamera.orthographicSize = 
            (Globals.maze.X_CELLS_COUNT > Globals.maze.Y_CELLS_COUNT ? Globals.maze.X_CELLS_COUNT : Globals.maze.Y_CELLS_COUNT) 
            * Globals.maze.GetCellSideLength() * 0.5f;
        Globals.cameraFollowMagician.OpenMinimap();
	}

    protected bool bIsPerfectStealing = false;
    protected int perfect_stealing_bonus = 3000;
    protected int rose_bonus = 5;
    public virtual void MagicianGotCash(float value)
    {        
//         if (bIsPerfectStealing)
//         {
//             return;
//         }
//        bIsPerfectStealing = true;
//        PerfectStealing();
//        return;
        // 检查宝石
        Gem[] gems = UnityEngine.GameObject.FindObjectsOfType<Gem>();
        foreach(Gem gem in gems)
        {
            if (gem.GetCash() != 0)
            {
                return;
            }
        }
        // 检查箱子
        foreach (Chest chest in Globals.maze.chests)
        {
            if (chest.IsVisible() && chest.goldLast > 1)
            {
                return;
            }
        }
        bIsPerfectStealing = true;
        PerfectStealing();
    }

    public virtual void PerfectStealing()
    {
        
    }

    public virtual void AfterMagicianFalling()
    {

    }

    public virtual void AfterStealingSuccessedEscaped()
    {

    }

    public virtual void MagicianLifeOver()
    {
        
    }

    public virtual void GuardCreated(Guard guard)
    {     
    }

    public virtual void GuardDestroyed(Guard guard)
    {        
    }

    public virtual void GuardChoosen(Guard guard)
    {

    }

    public virtual void GuardDropped(Guard guard)
    {

    }

    public virtual void OneChestGoldAllLost(Chest chest)
    {
        
    }

    public void StopAllGuards()
    {
        foreach (Guard guard in Globals.maze.guards)
        {
            guard.StopAttacking();
        }
    }

    public virtual void ClickOnMap(UnityEngine.Vector2 pos)
    {

    }

    public virtual void RightClickOnMap(UnityEngine.Vector2 pos)
    {

    }

    public override void Update()
    {
        base.Update();
        UnityEngine.SpriteRenderer[] sprites = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.SpriteRenderer>();
        foreach (UnityEngine.SpriteRenderer sprite in sprites)
        {
            // floor
            if (sprite.gameObject.layer != 9)
            {
                // 魔术师下落的过程中，看得到魔术师。但是不开拓出任何视野
                if (sprite.gameObject.layer == 11 && 
                    (Globals.magician.currentAction == Globals.magician.falling || Globals.magician.currentAction == Globals.magician.escape))
                {
                    sprite.sortingOrder = 10;
                }
                // LightSource,FlyUp
                else if (sprite.gameObject.layer == 21 || sprite.gameObject.layer == 26)
                {
                    sprite.sortingOrder = -1;
                }
                // HeadOnMiniMap
                else if (sprite.gameObject.layer == 28)
                {
                    //sprite.sortingOrder = 1;
                }
                else if (sprite.transform.parent != null && sprite.transform.parent.GetComponent<UnityEngine.SpriteRenderer>() != null)
                {
                    sprite.sortingOrder = sprite.transform.parent.GetComponent<UnityEngine.SpriteRenderer>().sortingOrder + 1;
                }
                else
                {
                    sprite.sortingOrder = UnityEngine.Mathf.RoundToInt((1000 + sprite.transform.position.y) * 10f) * -1;
                }
            }
            else
            {
                sprite.sortingOrder = -20000;
            }
        }
    }

    public void SyncWithChestData(PlayerInfo player)
    {
        PutCashInBox(player);
        for (int idx = 0; idx < player.safeBoxDatas.Count; ++idx)
        {            
            Globals.maze.chests[idx].SyncWithData(player.safeBoxDatas[idx]);
        }        
    }

    public void PutCashInBox(PlayerInfo player)
    {
        // 给safebox分配金钱
        float cash = player.cashAmount;
        int box_count = player.safeBoxDatas.Count;
        while (box_count > 0)
        {
            float average_cash = cash / box_count;
            SafeBoxData box_data = player.safeBoxDatas[box_count - 1];
            float cash_limit = Globals.safeBoxLvDatas[box_data.Lv].capacity;
            float cash_put_in = 0;
            if (cash_limit >= average_cash)
            {
                cash_put_in = average_cash;
            }
            else
            {
                cash_put_in = cash_limit;
            }
            cash -= cash_put_in;
            box_data.cashInBox = cash_put_in;
            --box_count;
        }

        if (player == Globals.self)
        {
            Globals.self.cashAmount = Globals.AccumulateCashInBox(player);
            Globals.canvasForMagician.UpdateCash();
        }
    }

    public virtual void MoneyFull(bool full)
    {

    }
}
