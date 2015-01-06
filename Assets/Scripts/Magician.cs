public class Magician : Actor
{
    public bool isMoving;    
    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4F;

    /** Speed relative to velocity with which to play animations */
    public float animationSpeed = 0.2F;
    public float speed = 3.0f;
    public BattleUI battleUI;

    public Escape escape;
    public Victory victory;
    public Falling falling;
    public Incant incant;
    public override void Awake()
    {
        base.Awake();
        escape = GetComponent<Escape>();
        victory = GetComponent<Victory>();
        falling = GetComponent<Falling>();
        incant = GetComponent<Incant>();
        gameObject.name = "Mage_Girl";              
        Globals.magician = this;
    }

    public void RegistEvent()
    {
        
    }

    public void UnRegistEvent()
    {
        
    }

    public bool beginMoving(string value)
    {
        isMoving = true;
        //anim.CrossFade("moving");
        return true;
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

        //Get velocity in world-space
        UnityEngine.Vector3 velocity;
        
        if (isMoving)
        {
            UnityEngine.Vector3 cameraHorForward = Globals.cameraFollowMagician.GetHorForward();
            UnityEngine.Vector3 cameraHorRight = Globals.cameraFollowMagician.GetHorRight();
            //Vector3 cameraHorForward = new Vector3(0, 0, 1);
            //Vector3 cameraHorRight = new Vector3(1, 0, 0);

            UnityEngine.Vector2 joystickPos = battleUI.joystick.position;
            
            UnityEngine.Vector3 movementDirection = cameraHorForward * joystickPos.y + cameraHorRight * joystickPos.x;
            controller.Move(movementDirection * speed);
            velocity = controller.velocity;

            // 转向
            UnityEngine.Quaternion rot = transform.rotation;
            UnityEngine.Quaternion toTarget = UnityEngine.Quaternion.LookRotation(movementDirection);
            float turningSpeed = 6.0f;
            rot = UnityEngine.Quaternion.Slerp(rot, toTarget, turningSpeed * UnityEngine.Time.deltaTime);
            UnityEngine.Vector3 euler = rot.eulerAngles;
            euler.z = 0;
            euler.x = 0;
            rot = UnityEngine.Quaternion.Euler(euler);

            transform.rotation = rot;
        }
        else
        {
            velocity = UnityEngine.Vector3.zero;
            // 轻微的颤抖，玩家看不出来，但是这样FOV trigger才会触发
            controller.Move(new UnityEngine.Vector3(0.001f, 0.0f, 0.001f));
            controller.Move(new UnityEngine.Vector3(-0.001f, 0.0f, -0.001f));
        }

        //Calculate the velocity relative to this transform's orientation
        UnityEngine.Vector3 relVelocity = transform.InverseTransformDirection(velocity);
        relVelocity.y = 0;
        if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity)
        {
            //Fade out walking animation
            anim.CrossFade("idle");
        }
        else
        {
            //Fade in walking animation
            anim.CrossFade("moving");

            //Modify animation speed to match velocity
            UnityEngine.AnimationState state = anim["moving"];

            float speed = relVelocity.z;
            state.speed = speed * animationSpeed;
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
        Globals.EnableAllInput(true);
        Globals.cameraFollowMagician.beginFollow(Globals.magician.transform);
    }

    public override void OutStealing()
    {
        base.OutStealing();
        UnRegistEvent();                
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

    public void FingerDown(Finger finger)
    {
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
}
