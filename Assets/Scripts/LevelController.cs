public class LevelController : Actor
{
    public int randSeedCache;
    public UnityEngine.Canvas mainCanvas;
    public UnityEngine.Camera MiniMapCamera;
    public UnityEngine.GameObject fogPlane;
    public UnityEngine.Camera fogCam;
    public UnityEngine.Camera fogCam_2;
    public UnityEngine.Texture2D fogTex;
    public int frameCount;
    public Magician magician;
    
    public override void Awake()
    {
        base.Awake();
        Globals.LevelController = this;
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(mgrs_prefab);
        }                       
        
        frameCount = 0;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (Globals.maze != null)
        {
            Globals.maze.ClearMaze();
        }        
    }

    public override void Start()
    {
        base.Start();
        if (Globals.canvasForMagician == null && (this as LevelEditor) == null && (this as LoadingSceneLevelController) == null)
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
            * Globals.GetCellSideLength() * 0.5f;
        Globals.cameraFollowMagician.OpenMinimap();
        if (Globals.maze.droppedItemsStr != "")
        {
            System.String str = "";
            System.String[] item_names = Globals.maze.droppedItemsStr.Split(',');            
            foreach(System.String name in item_names)
            {
                str += "_" + UnityEngine.Random.Range(0.0f, 1.0f) + "," + UnityEngine.Random.Range(0.0f, 1.0f) + "," +name;
            }
            Globals.maze.owner_of_maze.UnpackDroppedItemStr(str);            
        }
        if (Globals.maze.cashOnFloorStr != "")
        {
            System.String str = "";
            System.String[] cashes = Globals.maze.cashOnFloorStr.Split(',');
            foreach (System.String cash in cashes)
            {
                str += "_" + UnityEngine.Random.Range(0.0f, 1.0f) + "," + UnityEngine.Random.Range(0.0f, 1.0f) + "," + cash;
            }
            Globals.maze.owner_of_maze.UnpackCashOnFloorStr(str);
        }

        if (!Globals.maze.owner_of_maze.isBot)
        {
            Globals.maze.owner_of_maze.SpreadItemsDroppedFromThiefInMaze();
            Globals.maze.owner_of_maze.SpreadCashOnFloor();
        }        
	}

    protected bool bIsPerfectStealing = false;
    protected int perfect_stealing_bonus = 3000;
    protected int rose_bonus = 5;
    public virtual void MagicianGotCash(float value)
    {        
        if (bIsPerfectStealing)
        {
            return;
        }
// 
//        bIsPerfectStealing = true;
//        PerfectStealing();
//        return;       
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

    public virtual void Update()
    {
        FrameFunc();
    }

    public override void FrameFunc()
    {
        // 和Globals.replaySystem.UpdateInStealthLevel()的调用点保持一致
        Globals.input.FrameFunc();

        base.FrameFunc();

        foreach (Actor actor in Globals.to_add_actors)
        {
            if (!Globals.actors.Contains(actor))
            {
                Globals.actors.Add(actor);
            }
        }
            
        Globals.to_add_actors.Clear();

        for (int idx_a = 0; idx_a < Globals.actors.Count;++idx_a )
        {
            Actor a = Globals.actors[idx_a];
            for (int idx_b = idx_a + 1; idx_b < Globals.actors.Count; ++idx_b)
            {
                Actor b = Globals.actors[idx_b];                
                if(a.characterController && b.characterController &&
                    !UnityEngine.Physics.GetIgnoreLayerCollision(a.gameObject.layer, b.gameObject.layer))// 如果是要碰撞的
                {
                    if (a.characterController.enabled && b.characterController.enabled)
                    {
                        UnityEngine.Vector3 a_pos = a.GetWorldCenterPos();
                        UnityEngine.Vector3 b_pos = b.GetWorldCenterPos();

                        float dis = UnityEngine.Vector3.Distance(a_pos, b_pos);

                        if (Globals.DEBUG_REPLAY)
                        {
                            System.String content_test = a.gameObject.name + " " + b.gameObject.name;
                            content_test += " dis:" + dis.ToString("F5") + " a_radius:" + a.GetWorldRadius().ToString("F5") + " b_radius:" + b.GetWorldRadius().ToString("F5");
                            Globals.record("testReplay", content_test);
                        }

                        if (dis < a.GetWorldRadius() + b.GetWorldRadius())
                        {
                            if (!a.IsTouching(b))
                            {
                                a.TouchBegin(b);
                                b.TouchBegin(a);
                            }
                            else
                            {
                                a.TouchStay(b);
                                b.TouchStay(a);
                            }
                        }
                            
                        else
                        {
                            //本来相交的两个， 现在不想交了
                            if (a.IsTouching(b))
                            {
                                a.TouchOut(b);
                                b.TouchOut(a);
                            }
                        }
                    }
                    else
                    {
                        //本来相交的两个， 突然有一个enable=false了
                        if (a.IsTouching(b))
                        {
                            a.TouchOut(b);
                            b.TouchOut(a);
                        }
                    }                                        
                }
            }
        }

        foreach (Actor a in Globals.actors)
        {
            if(a != null)
            {
                if(a != this && a.gameObject.activeSelf)
                {
                    a.FrameFunc();
                }
            }            
        }

        foreach (Actor a in Actor.to_be_remove)
        {
            if (a != null)
            {
                if (a as Guard)
                {
                    Globals.DestroyGuard(a as Guard);
                }
                else
                {
                    DestroyObject(a.gameObject);
                }
            }
            
            Globals.actors.Remove(a);
        }

        Actor.to_be_remove.Clear();        

        UnityEngine.SpriteRenderer[] sprites = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.SpriteRenderer>();
        foreach (UnityEngine.SpriteRenderer sprite in sprites)
        {            
            // floor
            if (sprite.gameObject.layer != 9)
            {
                // 魔术师下落的过程中，看得到魔术师。但是不开拓出任何视野                
                if (sprite.gameObject.layer == 11 &&
                    (magician != null && (magician.currentAction == magician.falling || magician.currentAction == magician.escape)))
                {
                    sprite.sortingOrder = 10000;
                }
                    // shadow,guard fov,dog fov, bark
                else if (sprite.gameObject.layer == 18 || sprite.gameObject.layer == 10 || sprite.gameObject.layer == 27 || sprite.gameObject.layer == 19)
                {
                }
                    // Wall, E, W
                else if (sprite.gameObject.layer == 8 && sprite.sortingOrder == 5000)
                {

                }
                // LightSource,FlyUp
                else if (sprite.gameObject.layer == 21 || sprite.gameObject.layer == 26)
                {
                    sprite.sortingOrder = 10000-1;
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
                    //sprite.sortingOrder = UnityEngine.Mathf.RoundToInt((2000 + sprite.transform.position.y)) * -1;

                    sprite.sortingOrder = UnityEngine.Mathf.RoundToInt((2000 - sprite.transform.position.y));
                }
            }
            else
            {
                sprite.sortingOrder = -20000;
            }
        }        

        ++frameCount;        
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
