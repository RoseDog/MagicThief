public class Magician : Actor
{
    public bool isMoving;    

    public BattleUI battleUI;

    public Escape escape;
    public Victory victory;
    public Falling falling;
    public Incant incant;
    public Disguise disguise;
    public ShotLight shot;
    public Hypnosis hypnosis;
    
    public override void Awake()
    {
        base.Awake();
        escape = GetComponent<Escape>();
        victory = GetComponent<Victory>();
        falling = GetComponent<Falling>();
        incant = GetComponent<Incant>();
        disguise = GetComponent<Disguise>();
        shot = GetComponent<ShotLight>();
        hypnosis = GetComponent<Hypnosis>();
        moving.canMove = false;  
        gameObject.name = "Mage_Girl";              
        Globals.magician = this;
        LifeAmount = 100;
        LifeCurrent = LifeAmount;
        PowerAmount = 100;
        PowerCurrent = PowerAmount;        
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
        for (int idx = 0; idx < skinnedMeshRenderers.Length; ++idx)
        {
            skinnedMeshRenderers[idx].material.shader = UnityEngine.Shader.Find("Diffuse");            
        }
    }

    public void TrickBtnClicked(UnityEngine.UI.Button btn)
    {
        int powerDelta = System.Convert.ToInt32(btn.GetComponentInChildren<UnityEngine.UI.Text>().text);
        if (ChangePower(-powerDelta))
        {
            if (btn.name == "Dove")
            {
            }
            else if (btn.name == "Disguise")
            {
                disguise.Excute();
            }
        }
    }

    public void ShotLight(UnityEngine.GameObject bulb)
    {
        if (ChangePower(-10))
        {
            shot.Shot(bulb);
        }
    }

    public void CastHypnosis(Guard guard, UnityEngine.UI.Button btn)
    {
        int powerDelta = System.Convert.ToInt32(btn.GetComponentInChildren<UnityEngine.UI.Text>().text);
        if (ChangePower(-powerDelta))
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
