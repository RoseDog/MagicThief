public class Magician : Actor
{
    public bool isMoving;    

    public BattleUI battleUI;

    public Escape escape;
    public Victory victory;
    public Falling falling;
    public Incant incant;
        
    public Hypnosis hypnosis;
    public ShotLight shot;
    public Disguise disguise;
    
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        escape = GetComponent<Escape>();
        victory = GetComponent<Victory>();
        falling = GetComponent<Falling>();
        incant = GetComponent<Incant>();
        
        TrickData trick = new TrickData();        
        trick.nameKey = "hypnosis";
        trick.descriptionKey = "hypnosis_desc";
        trick.duration = 10.0f;
        trick.powerCost = 20;
        trick.unlockRoseCount = 0;
        trick.price = 0;
        trick.bought = true;
        hypnosis = GetComponent<Hypnosis>();
        hypnosis.data = trick;
        Globals.tricks.Add(trick);

        
        trick = new TrickData();
        trick.nameKey = "shotLight";
        trick.descriptionKey = "shotLight_desc";
        trick.duration = 0.0f;
        trick.powerCost = 10;
        trick.unlockRoseCount = 3;
        trick.price = 1000;
        shot = GetComponent<ShotLight>();
        shot.data = trick;
        Globals.tricks.Add(trick);

        trick = new TrickData();
        trick.nameKey = "disguise";
        trick.descriptionKey = "disguise_desc";
        trick.duration = 8.0f;
        trick.powerCost = 30;
        trick.unlockRoseCount = 10;
        trick.price = 3000;
        disguise = GetComponent<Disguise>();
        disguise.data = trick;
        Globals.tricks.Add(trick);

        moving.canMove = false;  
        gameObject.name = "Mage_Girl";              
        Globals.magician = this;
        LifeAmount = 100;
        LifeCurrent = LifeAmount;
        PowerAmount = 100;
        PowerCurrent = PowerAmount;

        MazeLvData data = new MazeLvData();
        data.roseRequire = 0;
        data.price = 0;
        data.guards = new System.Collections.Generic.List<GuardHireInfo>();
        data.playerEverClickGuards = true;
        data.playerEverClickSafebox = true;
        Globals.mazeLvDatas.Add(data);

        data = new MazeLvData();
        data.roseRequire = 10;
        data.price = 5000;
        data.guards = new System.Collections.Generic.List<GuardHireInfo>(new GuardHireInfo[] { 
            new GuardHireInfo("lancer", 500), 
            new GuardHireInfo("tiger", 1500), 
            new GuardHireInfo("lancer", 2500)});
        data.safeBoxCount = 3;
        Globals.mazeLvDatas.Add(data);

        data = new MazeLvData();
        data.roseRequire = 30;
        data.price = 10000;
        data.guards = new System.Collections.Generic.List<GuardHireInfo>(new GuardHireInfo[] { 
            new GuardHireInfo("lamp", 4000), 
            new GuardHireInfo("lancer", 6000), 
            new GuardHireInfo("lamp", 10000),
            new GuardHireInfo("tiger", 12000),
            new GuardHireInfo("lamp", 15000)});
        data.safeBoxCount = 4;
        Globals.mazeLvDatas.Add(data);

        data = new MazeLvData();
        data.roseRequire = 50;
        data.price = 30000;
        data.guards = new System.Collections.Generic.List<GuardHireInfo>(new GuardHireInfo[] { 
            new GuardHireInfo("lamp", 20000), 
            new GuardHireInfo("lancer", 25000), 
            new GuardHireInfo("lamp", 35000),
            new GuardHireInfo("tiger", 50000),
            new GuardHireInfo("lamp", 70000)});
        data.safeBoxCount = 5;
        Globals.mazeLvDatas.Add(data);
    }

    public void RegistEvent()
    {
        
    }

    public void UnRegistEvent()
    {
        
    }

    public bool stopMoving(string value)
    {
        isMoving = false;
        return true;
    }
	public override void FixedUpdate()
    {
		base.FixedUpdate();
        if (currentAction != null)
        {
            return;
        }
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        //Debug.Log("magician trigger : 碰撞到的物体的名字是：" + other.gameObject.name);
    }

    public override void InStealing()
    {
        base.InStealing();        
        RegistEvent();
        moving.canMove = true;
        Globals.EnableAllInput(true);
        Globals.cameraFollowMagician.SetDragSpeed(0.02f);
    }

    public override void OutStealing()
    {
        base.OutStealing();
        UnRegistEvent();
        moving.canMove = false;  
        Globals.cameraFollowMagician.MoveToPoint(
            transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f),
            new UnityEngine.Vector3(Globals.cameraFollowMagician.disOffset.x * 0.7f,
                Globals.cameraFollowMagician.disOffset.y * 0.1f,
                Globals.cameraFollowMagician.disOffset.z * 0.5f), 0.7f);
        isMoving = false;        
    }

    public void CoverInMoonlight()
    {
        if(currentAction != null)
        {
            currentAction.Stop();
        }
        anim.Play("idle");
        for (int idx = 0; idx < skinnedMeshRenderers.Length; ++idx)
        {
            skinnedMeshRenderers[idx].material.shader = UnityEngine.Shader.Find("Diffuse");            
        }
    }

    public void TrickBtnClicked(TrickData data)
    {
        if(Stealing)
        {
            if (data.nameKey == "disguise")
            {
                if (ChangePower(-data.powerCost))
                {
                    disguise.Excute();
                }
            }
        }        
    }

    public void ShotLight(UnityEngine.GameObject bulb)
    {
        if (ChangePower(-shot.data.powerCost))
        {
            shot.Shot(bulb);
        }
    }

    public void CastHypnosis(Guard guard, UnityEngine.UI.Button btn)
    {
        if (ChangePower(-hypnosis.data.powerCost))
        {
            hypnosis.Cast(guard);
        }
    }

    public void FingerDown(Finger finger)
    {
        return;
        if (currentAction == null && incant.dove == null)
        {
            incant.FingerDown(finger);
        }
    }

    public void FingerMoving(Finger finger)
    {
        if (currentAction == incant)
        {
            incant.FingerMoving(finger);
        }        
    }

    public void FingerUp(Finger finger)
    {
        if (currentAction == incant)
        {
            incant.FingerUp(finger);
        }       
    }

    public override bool ChangePower(int delta)
    {
        if (base.ChangePower(delta))
        {
            Globals.canvasForMagician.PowerNumber.UpdateCurrentLife(PowerCurrent, PowerAmount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ChangeLife(int delta)
    {
        base.ChangeLife(delta);
        Globals.canvasForMagician.lifeNumber.UpdateCurrentLife(LifeCurrent, LifeAmount);
    }

    public override void ResetLifeAndPower()
    {
        base.ResetLifeAndPower();
        Globals.canvasForMagician.lifeNumber.UpdateText(LifeCurrent, LifeAmount);
        Globals.canvasForMagician.PowerNumber.UpdateText(PowerCurrent, PowerAmount);
    }
}
