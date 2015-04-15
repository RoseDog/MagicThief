using System;
using System.Collections;
using System.Collections.Generic;


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
    Guard guard;
    Magician mage;
    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4f;

    [UnityEngine.HideInInspector]
    public float heightOriginCache;

    public UnityEngine.Vector3 currentDir;

    public bool needAnimation = true;

    public new void Awake()
    {
        actor = GetComponent<Actor>();
        guard = actor as Guard;
        mage = actor as Magician;
        heightOriginCache = transform.position.z;
        base.Awake();

//        animation["moving"].speed = animationSpeed;
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
        UnityEngine.Vector3 velocity = UnityEngine.Vector3.zero;

//         if (Globals.PLAY_RECORDS && mage && Globals.replay.magePositions.Count != 0)
//         {
//             transform.position = Globals.replay.magePositions[0];
//             Globals.replay.magePositions.RemoveAt(0);
//         }
//         else
        {            
            if (canMove)
            {
                //Calculate desired velocity
                float s = speed;
                if (guard != null && guard.currentAction == guard.patrol)
                {
                    s *= 0.5f;
                }
                UnityEngine.Vector3 dir = CalculateVelocity(GetFeetPosition(), s);

                System.String content = gameObject.name;
                content +=  "movement:" + dir.ToString("F5");
                Globals.record("testReplay", content);

                if (controller != null)
                {
                    controller.Move(dir);
                    velocity = controller.velocity;
                    //transform.position += dir;
                    //velocity = dir;
                }
                if (dir.magnitude > UnityEngine.Mathf.Epsilon)
                {
                    currentDir = dir;
                    if (velocity.sqrMagnitude > 0.1f)
                    {
                        actor.FaceDir(velocity);
                    }
                }
                transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y, heightOriginCache);                
            }                        
        }

        if (Globals.replay_key != "" && mage && Globals.replay.magePositions.Count != 0)
        {
            transform.position = Globals.replay.magePositions[0];
            Globals.replay.magePositions.RemoveAt(0);
        }

        if (mage)
        {
            Globals.replay.RecordMagePosition(transform.position);
        }

        if (canMove && needAnimation)
        {
            //Animation
            //Calculate the velocity relative to this transform's orientation
            UnityEngine.Vector3 relVelocity = tr.InverseTransformDirection(velocity);
            relVelocity.z = 0;
            if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity)
            {
                //Fade out walking animation
                if (target == null)
                {
                    //actor.anim.CrossFade("idle");
                    if (actor.spriteSheet != null)
                    {
                        actor.spriteSheet.Play("idle");
                    }
                }
                else
                {
                    // 太奇怪了。。。如果不把动画和移动拆分开，canMove == false的时候这里也会执行
                    if (!canMove)
                    {
                        throw new InvalidOperationException("guard moving error");
                    }
                    //actor.anim.CrossFade("atkReady");
                }
            }
            else
            {

                //Fade in walking animation
                //actor.anim.CrossFade("moving");
                if (actor.spriteSheet != null)
                {
                    if (actor.spriteSheet.HasAnimation("walking"))
                    {
                        if (guard != null && guard.currentAction == guard.patrol)
                        {
                            actor.spriteSheet.Play("walking");
                        }
                        else
                        {
                            actor.spriteSheet.Play("running");
                        }
                    }
                    else
                    {
                        actor.spriteSheet.Play("moving");
                    }
                }
            }
        }
        

        System.String content_test = gameObject.name;
        content_test += " pos:" + transform.position.ToString("F5");
        Globals.record("testReplay", content_test);
    }

   
    public System.Collections.Generic.List<Pathfinding.Node> preStandNodes = new System.Collections.Generic.List<Pathfinding.Node>();

    public UnityEngine.Vector3 GetNearestWalkableNodePosition()
    {
        System.Collections.Generic.List<Pathfinding.Node> nodes =
                Globals.maze.pathFinder.graph.GetNodesInArea(new UnityEngine.Bounds(transform.position, new UnityEngine.Vector3(1.5f, 1.5f, 10.0f)));
        foreach (Pathfinding.Node node in nodes)
        {
            if (node.walkable)
            {
                return Globals.GetPathNodePos(node);
            }
        }

        Globals.Assert(false,"no walkable node nearby");
        return transform.position;
    }
}
