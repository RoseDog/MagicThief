public class LevelController : Actor
{
    public int randomSeed;
    public UnityEngine.Canvas mainCanvas;
    UnityEngine.Camera MiniMapCamera;
    public override void Awake()
    {
        base.Awake();
        Globals.LevelController = this;        
        if (Globals.magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Magician") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(magician_prefab);
            Globals.magician.gameObject.SetActive(false);
        }
        UnityEngine.GameObject minimapCamObj = UnityEngine.GameObject.Find("MiniMapCamera");
        if (minimapCamObj != null)
        {
            MiniMapCamera = minimapCamObj.GetComponent<UnityEngine.Camera>();
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
        levelPassed = false;
    }

    public virtual void BeforeGenerateMaze()
    {
        
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

        if (Globals.iniFileName != "")
        {
            UnityEngine.Debug.Log(Globals.iniFileName);
            IniFile ini = new IniFile(Globals.iniFileName);
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

    bool levelPassed = false;
    public virtual void MagicianGotCash(float value)
    {        
        if (levelPassed)
        {
            return;
        }
        levelPassed = true;
        Invoke("LevelPassed", 0.5f);
        return;

        Gem[] gems = UnityEngine.GameObject.FindObjectsOfType<Gem>();        
        if (gems.Length == 0)
        {            
            foreach(Chest chest in Globals.maze.chests)
            {
                if (chest.goldLast > 0)
                {
                    return;
                }
            }
            levelPassed = true;
            Invoke("LevelPassed", 0.5f);
        }
    }

    public virtual void LevelPassed()
    {
        
    }

    public virtual void AfterMagicianFalling()
    {

    }

    public virtual void AfterMagicianSuccessedEscaped()
    {

    }

    public virtual void AfterMagicianLifeOverEscaped()
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
                    sprite.sortingOrder = 1;
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
                    sprite.sortingOrder = UnityEngine.Mathf.RoundToInt((50 + sprite.transform.position.y) * 10f) * -1;
                }
            }
            else
            {
                sprite.sortingOrder = -20000;
            }
        }
    }
}
