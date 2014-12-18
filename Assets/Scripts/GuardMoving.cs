using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.RVO;


/** AI controller specifically made for the spider robot.
 * The spider robot (or mine-bot) which is got from the Unity Example Project
 * can have this script attached to be able to pathfind around with animations working properly.\n
 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
 * This component requires Z+ to be forward and Y+ to be up.\n
 * 
 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
 * The awake animation will only be sampled at the end frame and will not play.\n
 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
 * done so that it won't spawn effects too close to the previous spawn-point.
 * \shadowimage{mine-bot.png}
 * 
 * \note This script assumes Y is up and that character movement is mostly on the XZ plane.
 */
[UnityEngine.RequireComponent(typeof(Seeker))]
public class GuardMoving : AIPath
{

    Actor actor;
    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4f;

    /** Speed relative to velocity with which to play animations */
    public float animationSpeed = 0.2F;    

    public new void Awake()
    {
        actor = GetComponent<Actor>();
        base.Awake();
    }

    public new void Start()
    {
        base.Start();
    }

    public Seeker GetSeeker()
    {
        return seeker;
    }
    
    /** Point for the last spawn of #endOfPathEffect */
    protected UnityEngine.Vector3 lastTarget;

    /**
     * Called when the end of path has been reached.
     * An effect (#endOfPathEffect) is spawned when this function is called
     * However, since paths are recalculated quite often, we only spawn the effect
     * when the current position is some distance away from the previous spawn-point
    */
    public override void OnTargetReached()
    {
        actor.OnTargetReached();
    }

    public override UnityEngine.Vector3 GetFeetPosition()
    {
        return tr.position;
    }

    public override void Update()
    {
        //Get velocity in world-space
        UnityEngine.Vector3 velocity = UnityEngine.Vector3.zero;
        if (canMove)
        {         
            //Calculate desired velocity
            UnityEngine.Vector3 dir = CalculateVelocity(GetFeetPosition());

            //Rotate towards targetDirection (filled in by CalculateVelocity)
            RotateTowards(targetDirection);


            if (controller != null)
            {
                controller.Move(dir);
                velocity = controller.velocity;
            }                        
        }

        if (canMove)
        {
            //Animation
            //Calculate the velocity relative to this transform's orientation
            UnityEngine.Vector3 relVelocity = tr.InverseTransformDirection(velocity);
            relVelocity.y = 0;
            if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity)
            {
                // 轻微的颤抖，玩家看不出来，但是这样FOV trigger才会触发
                controller.Move(new UnityEngine.Vector3(0.001f, 0.0f, 0.001f));
                controller.Move(new UnityEngine.Vector3(-0.001f, 0.0f, -0.001f));
                //Fade out walking animation
                if (target == null)
                {
                    actor.anim.CrossFade("idle");
                }
                else
                {
                    // 太奇怪了。。。如果不把动画和移动拆分开，canMove == false的时候这里也会执行
                    if (!canMove)
                    {
                        throw new InvalidOperationException("guard moving error");
                    }
                    actor.anim.CrossFade("atkReady");
                }
            }
            else
            {
                //Fade in walking animation
                actor.anim.CrossFade("moving");

                //Modify animation speed to match velocity
                UnityEngine.AnimationState state = actor.anim["moving"];

                float speed = relVelocity.z;
                state.speed = speed * animationSpeed;
            }
        }
    }    
}
