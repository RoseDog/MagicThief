public class Magician : Actor
{
    bool isSneaking = false;
    public TryEscape tryEscape;
    public Escape escape;
    public Victory victory;
    public Falling falling;
    public Incant incant;    
        
    public Hypnosis hypnosis;
    public GunShot shot;
    public Disguise disguise;
    public FlyUp flyUp;    
    public ReleaseDove releaseDove;

    [UnityEngine.HideInInspector]
    public UnityEngine.GameObject TrickTimerPrefab;    
    
    System.Collections.Generic.List<UnityEngine.AudioClip> stepSounds = new System.Collections.Generic.List<UnityEngine.AudioClip>();
    public UnityEngine.AudioClip openChestSound;
    public MagicianData data;

    float sneakingAnimSpeed;
    [UnityEngine.HideInInspector]
    public float normalMovingAnimSpeed;
    float runningAnimSpeed;
    public float speedModifier;

    public UnityEngine.Material flyingMat;
    public UnityEngine.Material groundMat;
        
    public override void Awake()
    {
        base.Awake();
        
        stepSounds.Add(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/StepSounds/footstepSound_A_00"));
        stepSounds.Add(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/StepSounds/footstepSound_A_01"));
        stepSounds.Add(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/StepSounds/footstepSound_A_02"));
        stepSounds.Add(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/StepSounds/footstepSound_A_03"));
        stepSounds.Add(UnityEngine.Resources.Load<UnityEngine.AudioClip>("Audio/StepSounds/footstepSound_A_04"));

        sneakingAnimSpeed = 2.5f;
        normalMovingAnimSpeed = 1.8f;
        runningAnimSpeed = 3.0f;
        speedModifier = 1.0f;
      
        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("moving", 6, normalMovingAnimSpeed);
        spriteSheet.AddAnimationEvent("moving", 0, () => StepSound());
        spriteSheet.AddAnimationEvent("moving", 3, () => StepSound());
        spriteSheet.AddAnim("victoryEscape", 1);
        spriteSheet.AddAnim("catch_by_net", 4);
        spriteSheet.AddAnim("hitted_in_net", 1, 0.2f);
        spriteSheet.AddAnim("break_net", 8, 2.0f);
        spriteSheet.AddAnim("disguise", 8, 1.7f);
        spriteSheet.AddAnim("falling_failed", 7, 1.0f, true);
        spriteSheet.AddAnim("falling_success", 5, 1.0f, true);
        spriteSheet.AddAnim("falling", 1, 0.3f);
        spriteSheet.AddAnim("flying", 8);
        spriteSheet.AddAnim("hitted", 1, 0.3f);
        spriteSheet.AddAnim("Hypnosis", 4,1.5f);
        spriteSheet.AddAnim("landing", 1, 0.3f);
        spriteSheet.AddAnim("lifeOver", 5, 1.5f, true);
        spriteSheet.AddAnim("lifeOverEscape", 1, 1.0f, true);
        spriteSheet.AddAnim("take_out_gun", 6, 2.8f);
        spriteSheet.AddAnim("shot_machine", 3,1.0f);
        spriteSheet.AddAnim("shot_empty", 2,1.0f);
        spriteSheet.AddAnim("TryEscape", 6, 1.0f);
        spriteSheet.AddAnim("falling_failed_loop", 12, 1.3f);
        spriteSheet.AddAnim("flyup_0",7, 2.0f);
        spriteSheet.AddAnim("flyup_1", 1, 1.7f);
        spriteSheet.AddAnim("flyup_2", 6, 1.9f);
        spriteSheet.AddAnim("open_chest", 4);
        spriteSheet.AddAnimationEvent("open_chest", 0, () => OpenChestSound());
        spriteSheet.AddAnim("take_money", 16);        
        spriteSheet.AddAnim("dove_trick", 12, 3.0f, true);
        spriteSheet.AddAnim("sneaking", 6, sneakingAnimSpeed);
        
        
        tryEscape = GetComponent<TryEscape>();        
        escape = GetComponent<Escape>();        
        victory = GetComponent<Victory>();
        falling = GetComponent<Falling>();        
        incant = GetComponent<Incant>();
        hypnosis = GetComponent<Hypnosis>();
        disguise = GetComponent<Disguise>();
        releaseDove = GetComponent<ReleaseDove>();
        shot = GetComponent<GunShot>();
        flyUp = GetComponent<FlyUp>();

        moving.canMove = false;
        
        TrickTimerPrefab = UnityEngine.Resources.Load("UI/FakeGuardTimer") as UnityEngine.GameObject;

        eye.gameObject.SetActive(false);
        characterController.enabled = false;
        moving.move_anim = "moving";

        data = Globals.thiefPlayer.GetMageDataByName(gameObject.name);
        LifeCurrent = data.GetLifeAmount();
        PowerCurrent = data.GetPowerAmount();
        hypnosis.data = Globals.thiefPlayer.GetTrickByName("hypnosis");
        disguise.data = Globals.thiefPlayer.GetTrickByName("disguise");
        shot.data = Globals.thiefPlayer.GetTrickByName("shotLight");
        releaseDove.data = Globals.thiefPlayer.GetTrickByName("dove");
        flyUp.data = Globals.thiefPlayer.GetTrickByName("flyUp");
    }

    public int GetUnlockSafeDuration()
    {        
        return data.GetUnlockSafeDuration();
    }

    public void RegistEvent()
    {
        Globals.input.Evt_KeyQ += OnItemKeyPressed;
        Globals.input.Evt_KeyW += OnItemKeyPressed;
        Globals.input.Evt_KeyE += OnItemKeyPressed;
        Globals.input.Evt_KeyDownR += OnKeyDownR;
        Globals.input.Evt_KeyUpR += OnKeyUpR;
    }

    public void UnRegistEvent()
    {
        ;
    }

    public bool OnItemKeyPressed(string key)
    {
        if(key == "Q")
        {
            TrickData trick = Globals.thiefPlayer.GetTrickBySlotIdx(0);
            if (trick != null)
            {
                CastMagic(trick);
            }
        }
        else if (key == "W")
        {
            TrickData trick = Globals.thiefPlayer.GetTrickBySlotIdx(1);
            if (trick != null)
            {
                CastMagic(trick);
            }
        }
        else if (key == "E")
        {
            TrickData trick = Globals.thiefPlayer.GetTrickBySlotIdx(2);
            if (trick != null)
            {
                CastMagic(trick);
            }
        }
        
        return true;
    }
    
    public bool OnKeyDownR(string key)
    {
        moving.move_anim = "sneaking";
        isSneaking = true;
        Globals.replaySystem.RecordKeyDown(key);
        stepCount = 0;
        return true;
    }

    public bool OnKeyUpR(string key)
    {
        moving.move_anim = "moving";
        isSneaking = false;
        Globals.replaySystem.RecordKeyUp(key);
        return true;
    }

    public override void InStealing()
    {
        base.InStealing();        
        RegistEvent();
        eye.gameObject.SetActive(true);
        characterController.enabled = true;
        isSneaking = false;
        //moving.canMove = true;
        Globals.EnableAllInput(true);
        Globals.cameraFollowMagician.SetDragSpeed(2f);
        Globals.cameraFollowMagician.SetTouchBorderMoveSpeed(25.0f);
        Globals.canvasForMagician.gameObject.SetActive(true);
    }

    public override void OutStealing()
    {
        base.OutStealing();
        UnRegistEvent();
        characterController.enabled = false;
        moving.canMove = false;  
//         Globals.cameraFollowMagician.MoveToPoint(
//             transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f),
//             new UnityEngine.Vector3(Globals.cameraFollowMagician.disOffset.x * 0.7f,
//                 Globals.cameraFollowMagician.disOffset.y * 0.1f,
//                 Globals.cameraFollowMagician.disOffset.z * 0.5f), 0.7f);
        eye.gameObject.SetActive(false);
        Globals.cameraFollowMagician.CloseMinimap();
        Globals.canvasForMagician.gameObject.SetActive(false);
        (Globals.LevelController as StealingLevelController).canvasForStealing.gameObject.SetActive(false);

        isOpenChest = false;
        isTakingMoneny = false;
    }

    public override void FrameFunc()
    {
        base.FrameFunc();        

        if((isSneaking || chasingGuards.Count != 0) && moving.isMoving)
        {
            ChangeLife(-data.GetLifeConsume() * Globals.thiefPlayer.GetTrickTotalWeight(), false);
            if (LifeCurrent < UnityEngine.Mathf.Epsilon)
            {
                lifeOver.Excute();
            }
        }
    }

    int stepCount;
    

    public void StepSound()
    {
        if (!isSneaking)
        {
            UnityEngine.AudioClip clip = stepSounds[UnityEngine.Random.Range(0, stepSounds.Count)];
            audioSource.PlayOneShot(clip);
            audioSource.volume = 0.7f;                
            
            if (stepCount%3==0)
            {
                BarkSoundWave wave = (UnityEngine.GameObject.Instantiate(Globals.wave_prefab) as UnityEngine.GameObject).GetComponent<BarkSoundWave>();
                wave.transform.position = (transform.position + GetWorldCenterPos()) * 0.5f;
                wave.radiusLimit = data.stepSoundLimit;
                wave.radiusStart = 250;
                wave.oneWaveDuration = 8;
            }
            ++stepCount;
        }        
    }

    public void OpenChestSound()
    {        
        audioSource.PlayOneShot(openChestSound);
        audioSource.volume = 0.7f;

        BarkSoundWave wave = (UnityEngine.GameObject.Instantiate(Globals.wave_prefab) as UnityEngine.GameObject).GetComponent<BarkSoundWave>();
        wave.transform.position = (transform.position + GetWorldCenterPos()) * 0.5f;
        wave.radiusLimit = 400;
        wave.radiusStart = 250;
        wave.oneWaveDuration = 8;  
    }

    public void CastMagic(TrickData data)
    {
        if (!Globals.canvasForMagician.tricksInUsingPanel.activeSelf)
        {
            return;
        }
        if (Stealing && data.clickButtonToCast && ChangePower(-data.powerCost))
        {
            Globals.replaySystem.RecordMagicCast(data);            
            if (data.nameKey == "disguise")
            {
                disguise.Excute();
            }
            if (data.nameKey == "flyUp")
            {                
                flyUp.Excute();
            }
            if (data.nameKey == "dove")
            {
                releaseDove.Excute();                
            }
            
            if (data.nameKey == "shotLight")
            {
                shot.Shot(null);
            }
        }        
    }

    public void CastFlash(UnityEngine.Vector3 finger_pos)
    {
        UnityEngine.GameObject flashPrefab = UnityEngine.Resources.Load("Avatar/Flash") as UnityEngine.GameObject;
        UnityEngine.GameObject flash = UnityEngine.GameObject.Instantiate(flashPrefab) as UnityEngine.GameObject;
        flash.transform.position = finger_pos;
    }
    
    public override bool GoTo(UnityEngine.Vector3 pos, OnPathDelegate callback = null)
    {
        System.String content = "Mage go to:";
        content += pos.ToString("F3");
        Globals.record("testReplay", content);

        moving.canSearch = false;
        moving.GetSeeker().StartPath(moving.GetFeetPosition(), pos, OnPathComplete);             
        return true;
    }

    public void Shot(UnityEngine.GameObject target)
    {
        if (shot.data.IsInUse()&& 
            ChangePower(-shot.data.powerCost))
        {
            shot.Shot(target);
        }
    }

    public bool ChangePower(float delta)
    {
        float powerTemp = PowerCurrent;
        powerTemp += delta;
        if (powerTemp < 0)
        {
            Globals.tipDisplay.Msg("not_enough_power");
            return false;
        }
        else
        {            
            PowerCurrent = powerTemp;
            Globals.canvasForMagician.PowerNumber.UpdateCurrentLife(PowerCurrent.ToString("F1"), data.GetPowerAmount());
            return true;
        }
    }

    public override void ChangeLife(float delta, bool needUIJump = true)
    {
        LifeCurrent += delta;
        LifeCurrent = UnityEngine.Mathf.Clamp(LifeCurrent, 0, data.GetLifeAmount());
        Globals.canvasForMagician.lifeNumber.UpdateCurrentLife(LifeCurrent.ToString("F1"), data.GetLifeAmount(), needUIJump);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        UnityEngine.Debug.Log("Magician Destroyed");
    }

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        stepCount = 0;
        UnityEngine.Debug.Log("OnTargetReached");
    }

    System.Collections.Generic.HashSet<Guard> chasingGuards = new System.Collections.Generic.HashSet<Guard>();
    public override void SpotByEnemy(Guard guard)
    {
        base.SpotByEnemy(guard);
        chasingGuards.Add(guard);    
    }

    public override void EnemyStopChasing(Guard guard)
    {
        base.EnemyStopChasing(guard);
        chasingGuards.Remove(guard);
    }

    public override double GetSpeed()
    {        
        if (isSneaking)
        {
            spriteSheet.ModifyAnimSpeed("sneaking", speedModifier * sneakingAnimSpeed * data.GetSneakingSpeed() / data.sneakingSpeed);
            return speedModifier * data.GetSneakingSpeed();
        }  
        else if(chasingGuards.Count != 0)
        {
            spriteSheet.ModifyAnimSpeed("moving", speedModifier * runningAnimSpeed * data.GetRunningSpeed() / data.runningSpeed);
            return speedModifier * data.GetRunningSpeed();
        }

        spriteSheet.ModifyAnimSpeed("moving", speedModifier * normalMovingAnimSpeed * data.GetNormalSpeed() / data.normalSpeed);
        return speedModifier * data.GetNormalSpeed();
    }
}
