using System.Collections;

public class Magician : Actor
{
    public bool isMoving;

    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4F;

    /** Speed relative to velocity with which to play animations */
    public float animationSpeed = 0.2F;
    public float speed = 3.0f;
    public BattleUI battleUI;

    public bool isInAir = false;
   
    public override void Awake()
    {
        base.Awake();        
        UnityEngine.GameObject uiPrefab = UnityEngine.Resources.Load("UI/UI") as UnityEngine.GameObject;
        UnityEngine.GameObject ui = Instantiate(uiPrefab) as UnityEngine.GameObject;
        battleUI = ui.GetComponent<BattleUI>();
        battleUI.magician = this;
        battleUI.Prepare();
        RegistEvent();
        Globals.magician = this;
    }

    public void RegistEvent()
    {
        battleUI.joystick.Evt_FirstMove += beginMoving;
        battleUI.joystick.Evt_BtnDown += beginMoving;
        battleUI.joystick.Evt_Moving += beginMoving;
        battleUI.joystick.Evt_BtnUp += stopMoving;
    }

    public void UnRegistEvent()
    {
        battleUI.joystick.Evt_FirstMove -= beginMoving;
        battleUI.joystick.Evt_BtnDown -= beginMoving;
        battleUI.joystick.Evt_Moving -= beginMoving;
        battleUI.joystick.Evt_BtnUp -= stopMoving;
    }

    public bool beginMoving(string value)
    {
        isMoving = true;
        anim.CrossFade("moving");
        return true;
    }

    public bool stopMoving(string value)
    {
        isMoving = false;
        return true;
    }
    public override void Update()
    {
        base.Update();
        if (currentAction == hitted || currentAction == lifeOver)
        {
            return;
        }
        if(isInAir)
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
            controller.SimpleMove(movementDirection * speed);
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
            controller.SimpleMove(new UnityEngine.Vector3(0.001f, 0.0f, 0.001f));
            controller.SimpleMove(new UnityEngine.Vector3(-0.001f, 0.0f, -0.001f));
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

    public void Falling(UnityEngine.Vector3 from, UnityEngine.Vector3 to, float time)
    {
        transform.position = from;
        AddAction(new MoveTo(transform, to, time));
        anim.Play("A_Falling_1");
        isInAir = true;
        Invoke("FallingOver", time);
    }

    void FallingOver()
    {
        anim.Play("A_JumpLanding_1");
    }

    public void JumpLandingOver()
    {
        anim.CrossFade("idle");
    }

    public override void Dead()
    {
        base.Dead();
        isMoving = false;
        UnRegistEvent();
    }
}
