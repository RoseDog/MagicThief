public class Chest : Actor, System.IComparable<Chest>
{
    public bool isMagicianNear = false;
    bool isPlayingBack = false;
    GoldPoper goldPoper = null;
    public float goldLast;
    public float goldLostPersecond = 500;
    UnityEngine.Sprite openSprite;
    UnityEngine.Sprite closedSprite;
    UnityEngine.UI.Image unlockProgressSprite;
    UnityEngine.Vector3 progressScaleCache;
    System.Collections.Generic.List<UnityEngine.Renderer> goldMeshes = new System.Collections.Generic.List<UnityEngine.Renderer>();

    public Cell locate;

    UnityEngine.GameObject coinPrefab;
    UnityEngine.GameObject SafeboxUpgradeUIPrefab;
    UnityEngine.GameObject SafeboxNotFinishedPrefab;
    UnityEngine.Canvas canvasForSafeboxBtns;
    UnityEngine.GameObject SafeboxNotFinishedTip;
    

    public SafeBoxData data;

    public System.String stolenTrickItem;

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
        SafeboxUpgradeUIPrefab = UnityEngine.Resources.Load("Misc/CanvasOnSafebox") as UnityEngine.GameObject;
        SafeboxNotFinishedPrefab = UnityEngine.Resources.Load("Misc/SafeboxNotFinished") as UnityEngine.GameObject;
        

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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (Globals.maze != null)
        {
            Globals.maze.chests.Remove(this);
        }

        if (SafeboxNotFinishedTip != null)
        {
            Destroy(SafeboxNotFinishedTip);
            SafeboxNotFinishedTip = null;
        }
    }

    public void SyncWithData(SafeBoxData boxdata)
    {        
        data = boxdata;
        data.unlocked = true;
        if (goldPoper == null)
        {
            UnityEngine.GameObject effectPrefab = (UnityEngine.GameObject)UnityEngine.Resources.Load("Props/Chest/GoldPoper/GoldPoper", typeof(UnityEngine.GameObject));
            goldPoper = (Instantiate(effectPrefab, transform.position + UnityEngine.Vector3.up * 50f + new UnityEngine.Vector3(0, 0, -0.1f), UnityEngine.Quaternion.identity) as UnityEngine.GameObject).GetComponent<GoldPoper>();
            goldPoper.chest = this;
            goldPoper.transform.localScale = new UnityEngine.Vector3(200.0f, 200.0f, 200.0f);
            goldPoper.transform.parent = transform;
        }
        
        ResetGold();
        Visible(true);
    }

    public override void Visible(bool visibility)
    {
        base.Visible(visibility);
        characterController.enabled = visibility;        
    }
	
	public void PlaceOnCell(Cell cell, float rotate_angle)
    {
        locate = cell;
        locate.chest = this;
        transform.position = cell.GetFloorPos();
        //transform.localEulerAngles = new UnityEngine.Vector3(0.0f, rotate_angle, 0.0f);
    }

    public override void TouchBegin(Actor other)
    {
        base.TouchBegin(other);
        Magician mage = other.GetComponent<Magician>();
        if (mage != null && mage.currentAction == mage.beenPressDown)
        {
            return;
        }
        mage.isOpenChest = true;
        UnityEngine.Debug.Log("touch chest");
        isMagicianNear = true;
        if (SafeboxNotFinishedTip != null)
        {
            Destroy(SafeboxNotFinishedTip);
            SafeboxNotFinishedTip = null;
        }
        if (goldLast > 1)
        {
            unlockProgressSprite.transform.parent.gameObject.SetActive(true);
            AddAction(new Sequence(new Cocos2dProgress(unlockProgressSprite, mage.GetUnlockSafeDuration()), new FunctionCall(() => ChestOpened())));
        }        
    }

    public override void TouchStay(Actor other)
    {
        if (other.IsLifeOver())
        {
            TouchOut(other);
        }
    }

    public override void TouchOut(Actor other)
    {
        base.TouchOut(other);
        UnityEngine.Debug.Log("leave chest");
        isMagicianNear = false;
        if (Globals.stealingController != null)
        {
            Globals.stealingController.magician.isOpenChest = false;
            Globals.stealingController.magician.isTakingMoneny = false;
            if (goldLast > 1)
            {
                ClearAllActions();
                unlockProgressSprite.transform.parent.gameObject.SetActive(false);
                if (spriteRenderer.sprite == openSprite)
                {
                    ChestClosed();
                }

                if (Globals.stealingController.magician.Stealing && SafeboxNotFinishedTip == null)
                {
                    SafeboxNotFinishedTip = UnityEngine.GameObject.Instantiate(SafeboxNotFinishedPrefab) as UnityEngine.GameObject;
                    SafeboxNotFinishedTip.GetComponent<UnityEngine.Canvas>().worldCamera = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>();
                    SafeboxNotFinishedTip.GetComponentInChildren<UIMover>().Jump();
                    SafeboxNotFinishedTip.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 170.0f, 0.0f);
                }
            }
            goldPoper.StopPop();
        }                        
    }

    public void ChestOpened()
    {
        isPlayingBack = true;
        
        UnityEngine.Debug.Log("ChestOpened");
        if (isMagicianNear)
        {
            unlockProgressSprite.transform.parent.gameObject.SetActive(false);
            spriteRenderer.sprite = openSprite;
            Globals.stealingController.magician.isOpenChest = false;
            Globals.stealingController.magician.isTakingMoneny = true;
            goldPoper.Pop();            
        }        
    }

    public void ChestClosed()
    {
        if(goldLast > 1)
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
        Globals.stealingController.magician.isOpenChest = false;
        Globals.stealingController.magician.isTakingMoneny = false;
    }

    public void LostGold()
    {        
        goldLast -= goldLostPersecond;        
        if (goldLast < 1)
        {
            if (SafeboxNotFinishedTip != null)
            {
                Destroy(SafeboxNotFinishedTip);
                SafeboxNotFinishedTip = null;
            }

            renderers.Remove(head_on_minimap.GetComponent<UnityEngine.Renderer>());
            Destroy(head_on_minimap);
            head_on_minimap = null;            

            foreach (UnityEngine.Renderer renderer in goldMeshes)
            {
                renderer.gameObject.SetActive(false);
            }
            goldPoper.StopPop();
            Globals.stealingController.magician.isOpenChest = false;
            Globals.stealingController.magician.isTakingMoneny = false;
            Globals.LevelController.OneChestGoldAllLost(this);
        }

        // 在教程中，TutorialThief偷东西的时候，不生成往界面上飞的金币        
        if (Globals.stealingController != null)
        {
            Sequence seq = new Sequence();
            for (int times = 0; times < 3; ++times )
            {
                seq.actions.Add(new SleepFor(3));
                seq.actions.Add(new FunctionCall(()=>Coins()));               
            }
            AddAction(seq);
        }

        audioSource.Play();                
        UnityEngine.GameObject soundPrefab = UnityEngine.Resources.Load("Misc/GunSound") as UnityEngine.GameObject;
        GuardAlertSound sound = (UnityEngine.GameObject.Instantiate(soundPrefab) as UnityEngine.GameObject).GetComponent<GuardAlertSound>();
        sound.transform.position = (transform.position + GetWorldCenterPos()) * 0.5f;
        sound.SetRadiusLimit(400);
        sound.SetRadiusStart(250);
        sound.SetOneWaveDuration(8);
        sound.StartAlert();            
    }
    
    public void ResetGold()
    {
        goldLast = data.cashInBox;
        // 总共需要3次偷完整个箱子
        goldLostPersecond = goldLast / 3.0f;
        goldPoper.InitParticleTex((int)goldLostPersecond);
        foreach (UnityEngine.Renderer renderer in goldMeshes)
        {
            renderer.gameObject.SetActive(true);
        }
        TouchOut(null);
    }

    public void Coins()
    {
        float gold_every_time = goldLostPersecond / 3.0f;        
        int count = UnityEngine.Random.Range(1, 3);
        float gold_every_coin = gold_every_time / count;
        for (int i = 0; i < count; ++i)
        {
            UnityEngine.GameObject coin = UnityEngine.GameObject.Instantiate(coinPrefab) as UnityEngine.GameObject;
            coin.transform.position = new UnityEngine.Vector3(
                transform.position.x + UnityEngine.Random.Range(-Globals.maze.GetCellSideLength() / 3, Globals.maze.GetCellSideLength() / 3),
                transform.position.y,
                transform.position.z + UnityEngine.Random.Range(-Globals.maze.GetCellSideLength() / 3, Globals.maze.GetCellSideLength() / 3));

            FlyToScreenNumber coin_fly = coin.GetComponent<FlyToScreenNumber>();
            coin_fly.numberDelta = gold_every_coin;
            coin_fly.ToCashNumber(true);
        }
    }    

    public void Falling(int fallingDuration)
    {
        UnityEngine.Vector3 to = transform.position;
        UnityEngine.Vector3 from = transform.position + new UnityEngine.Vector3(0, 2000, 0);
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
                    new Sequence(new ScaleTo(transform, new UnityEngine.Vector3(160f, 160f, 160f), 5),
                        new ScaleTo(transform, scaleCache, 5))
                        )
                        );

        if (!isShownBtn)
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(SafeboxUpgradeUIPrefab) as UnityEngine.GameObject;
            canvasForSafeboxBtns = obj.GetComponent<UnityEngine.Canvas>();
            canvasForSafeboxBtns.worldCamera = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>();

            UnityEngine.UI.Button UpgradeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "UpgradeBtn");
            UpgradeBtn.onClick.AddListener(() => UpgradeBtnClicked());

            isShownBtn = true;
            canvasForSafeboxBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 120f, 0.0f);
            canvasForSafeboxBtns.GetComponent<Actor>().AddAction(
                new ScaleTo(canvasForSafeboxBtns.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 8));
        } 
    }

    public void UpgradeBtnClicked()
    {
        UnityEngine.GameObject SafeboxUpgradeUI_prefab = UnityEngine.Resources.Load("UI/SafeboxUpgradeUI") as UnityEngine.GameObject;
        SafeboxUpgradeUI upgradeUI = (UnityEngine.GameObject.Instantiate(SafeboxUpgradeUI_prefab) as UnityEngine.GameObject).GetComponentInChildren<SafeboxUpgradeUI>();
        upgradeUI.SetSafebox(data);
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
