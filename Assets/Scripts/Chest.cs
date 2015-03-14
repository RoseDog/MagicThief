public class Chest : Actor, System.IComparable<Chest>
{
    bool isMagicianNear = false;
    bool isPlayingBack = false;
    GoldPoper goldPoper = null;
    public int goldAmount = 100;
    public int goldLast;
    public int goldLostPersecond = 500;
    UnityEngine.Sprite openSprite;
    UnityEngine.Sprite closedSprite;
    UnityEngine.UI.Image unlockProgressSprite;
    UnityEngine.Vector3 progressScaleCache;
    System.Collections.Generic.List<UnityEngine.Renderer> goldMeshes = new System.Collections.Generic.List<UnityEngine.Renderer>();

    Cell locate;

    UnityEngine.GameObject coinPrefab;
    UnityEngine.GameObject SafeboxUpgradeUIPrefab;
    UnityEngine.Canvas canvasForSafeboxBtns;

    public SafeBoxData data;

    public int CompareTo(Chest other)
    {
        if(other == this)
        {
            return 0;
        }
        if (UnityEngine.Vector3.Distance(transform.position, Globals.maze.left_up_corner_pos) >
            UnityEngine.Vector3.Distance(other.transform.position, Globals.maze.left_up_corner_pos))
        {
            return 1;
        }
        return -1;
    }

    public override void Awake()
    {
        base.Awake();
        coinPrefab = UnityEngine.Resources.Load("Props/GoldCoin") as UnityEngine.GameObject;
        SafeboxUpgradeUIPrefab = UnityEngine.Resources.Load("Avatar/CanvasOnSafebox") as UnityEngine.GameObject;

        unlockProgressSprite = Globals.getChildGameObject<UnityEngine.UI.Image>(gameObject, "progress");
        unlockProgressSprite.transform.parent.gameObject.SetActive(false);

        UnityEngine.Sprite[] sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Props/chest_open");
        openSprite = sprites[0];
        sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Props/chest_closed");
        closedSprite = sprites[0];

        foreach (UnityEngine.Renderer renderer in renderers)
        {
            if (renderer.tag == "GoldMesh")
            {
                goldMeshes.Add(renderer);
            }
        }
        Globals.maze.chests.Add(this);
    }

    public void OnDestroy()
    {
        if (Globals.maze != null)
        {
            Globals.maze.chests.Remove(this);
        }        
    }

	public override void Start () 
    {
        base.Start();
        UnityEngine.GameObject effectPrefab = (UnityEngine.GameObject)UnityEngine.Resources.Load("Props/Chest/GoldPoper/GoldPoper", typeof(UnityEngine.GameObject));
        goldPoper = (Instantiate(effectPrefab, transform.position + UnityEngine.Vector3.up * 0.5f, UnityEngine.Quaternion.identity) as UnityEngine.GameObject).GetComponent<GoldPoper>();
        goldPoper.InitParticleTex(goldLostPersecond);
        goldPoper.chest = this;
        goldPoper.transform.localScale = new UnityEngine.Vector3(2.0f, 2.0f, 2.0f);
        goldPoper.transform.parent = transform;
        ResetGold();
	}

    public override void Visible(bool visibility)
    {
        base.Visible(visibility);
        collider.enabled = visibility;        
    }
	
	public void PlaceOnCell(Cell cell, float rotate_angle)
    {
        locate = cell;
        locate.chest = this;
        //cell.CreateTorchLight();
        transform.position = cell.GetFloorPos();
        //transform.localEulerAngles = new UnityEngine.Vector3(0.0f, rotate_angle, 0.0f);
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        UnityEngine.Debug.Log("touch chest");
        isMagicianNear = true;
        if (goldAmount > 0)
        {
            unlockProgressSprite.transform.parent.gameObject.SetActive(true);
            AddAction(new Sequence(new Cocos2dProgress(unlockProgressSprite, 120), new FunctionCall(()=> ChestOpened())));
        }        
    }

    void OnTriggerStay(UnityEngine.Collider other)
    {
        if (other.GetComponent<Actor>().IsLifeOver())
        {
            OnTriggerExit(other);
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        UnityEngine.Debug.Log("leave chest");
        isMagicianNear = false;
                        
        if (goldLast > 0)
        {
            ClearAllActions();
            unlockProgressSprite.transform.parent.gameObject.SetActive(false);
            if (spriteRenderer.sprite == openSprite)
            {
                ChestClosed();
            }            
        }

        goldPoper.StopPop();       
    }

    public void ChestOpened()
    {
        isPlayingBack = true;
        UnityEngine.Debug.Log("ChestOpened");
        if (isMagicianNear)
        {
            unlockProgressSprite.transform.parent.gameObject.SetActive(false);
            spriteRenderer.sprite = openSprite;
            goldPoper.Pop();
        }        
    }

    public void ChestClosed()
    {
        if(goldLast > 0)
        {
            spriteRenderer.sprite = closedSprite;
        }
        if (isPlayingBack)
        {
            UnityEngine.Debug.Log("ChestClosed");
            if (!isMagicianNear)
            {
                spriteRenderer.sprite = closedSprite;
            }
        }
        isPlayingBack = false;
    }

    public void LostGold()
    {
        goldLast -= goldLostPersecond;
        if (goldLast <= 0)
        {
            foreach (UnityEngine.Renderer renderer in goldMeshes)
            {
                renderer.gameObject.SetActive(false);
            }
            goldPoper.StopPop();
            Globals.LevelController.OneChestGoldAllLost(this);
        }

        // 在教程中，TutorialThief偷东西的时候，不生成往界面上飞的金币
        TutorialLevelController controller = Globals.LevelController as TutorialLevelController;
        if (controller != null)
        {
            StartCoroutine(Coins());
        }               
    }

    public void ResetGold()
    {
        goldLast = goldAmount;
        foreach (UnityEngine.Renderer renderer in goldMeshes)
        {
            renderer.gameObject.SetActive(true);
        }
        OnTriggerExit(null);
    }

    System.Collections.IEnumerator Coins()
    {
        int time = 3;
        float gold_every_time = (float)goldLostPersecond / time;
        while (time > 0)
        {
            int count = UnityEngine.Random.Range(1,3);
            float gold_every_coint = gold_every_time / count;
            for (int i = 0; i < count; ++i)
            {
                UnityEngine.GameObject coin = UnityEngine.GameObject.Instantiate(coinPrefab) as UnityEngine.GameObject;
                coin.transform.position = new UnityEngine.Vector3(
                    transform.position.x + UnityEngine.Random.Range(-Globals.maze.GetCellSideLength() / 3, Globals.maze.GetCellSideLength() / 3),
                    transform.position.y,
                    transform.position.z + UnityEngine.Random.Range(-Globals.maze.GetCellSideLength() / 3, Globals.maze.GetCellSideLength() / 3));

                FlyToScreenNumber coin_fly = coin.GetComponent<FlyToScreenNumber>();
                coin_fly.numberDelta = gold_every_coint;
                coin_fly.ToCashNumber(true);
            }
            --time;
            yield return new UnityEngine.WaitForSeconds(0.1f); 
        }
    }    

    public void Falling(int fallingDuration)
    {
        UnityEngine.Vector3 to = transform.position;
        UnityEngine.Vector3 from = transform.position + new UnityEngine.Vector3(0, 20, 0);
        transform.position = from;
        AddAction(new MoveTo(transform, to, fallingDuration));
        Invoke("FallingOver", fallingDuration + 0.3f);
    }

    void FallingOver()
    {
        ClearAllActions();
    }

    bool isShownBtn = false;
    public void ShowUpgradeBtn()
    {
        AddAction(
                new Cocos2dParallel(
                    new Sequence(new ScaleTo(transform, new UnityEngine.Vector3(1.6f, 1.6f, 1.6f), 5),
                        new ScaleTo(transform, scaleCache, 5))
                        )
                        );

        if (!isShownBtn)
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(SafeboxUpgradeUIPrefab) as UnityEngine.GameObject;
            canvasForSafeboxBtns = obj.GetComponent<UnityEngine.Canvas>();
            canvasForSafeboxBtns.worldCamera = Globals.cameraFollowMagician.camera;

            UnityEngine.UI.Button UpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "UpgradeBtn");
            UpgradeBtn.onClick.AddListener(() => UpgradeBtnClicked());

            isShownBtn = true;
            canvasForSafeboxBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f);
            canvasForSafeboxBtns.GetComponent<Actor>().AddAction(
                new ScaleTo(canvasForSafeboxBtns.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 8));
        } 
    }

    public void UpgradeBtnClicked()
    {
        UnityEngine.GameObject SafeboxUpgradeUI_prefab = UnityEngine.Resources.Load("UI/SafeboxUpgradeUI") as UnityEngine.GameObject;
        SafeboxUpgradeUI upgradeUI = (UnityEngine.GameObject.Instantiate(SafeboxUpgradeUI_prefab) as UnityEngine.GameObject).GetComponentInChildren<SafeboxUpgradeUI>();
        upgradeUI.SetSafebox(this);
    }

    public void HideBtn()
    {
        isShownBtn = false;
        if (canvasForSafeboxBtns != null)
        {
            Destroy(canvasForSafeboxBtns.gameObject);
            canvasForSafeboxBtns = null;
        }
    }
}
