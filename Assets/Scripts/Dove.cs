public class Dove : Actor 
{
    public System.String animalName;
    TrickTimer timer;
    public override void Awake()
    {
        base.Awake();

        animalName = "dove";               
        spriteSheet.CreateAnimationByName(animalName + "_moving_left",4.0f);
        spriteSheet.CreateAnimationByName(animalName + "_moving_down", 4.0f);
        spriteSheet.CreateAnimationByName(animalName + "_moving_up", 4.0f);
        LifeAmount = 1;
        LifeCurrent = 1;

        moving.needAnimation = false;        
    }

    public void StartOut(UnityEngine.Vector3 pos, TrickData data)
    {
        System.String content_test = gameObject.name;
        content_test += " start out:" + pos.ToString("F5");
        Globals.record("testReplay", content_test);
        GoTo(pos);
        SleepThenCallFunction(data.duration, () => Vanish());
        timer = (UnityEngine.GameObject.Instantiate(Globals.stealingController.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(spriteSheet.gameObject, data.duration, new UnityEngine.Vector3(0, 70f, 0));
    }

    System.String flyingDir;
    public override void FrameFunc()
    {
        double angle = Globals.Angle(moving.currentDir, UnityEngine.Vector3.right);
        if (angle >= 315 || angle < 45)
        {
            flyingDir = Globals.EAST;
            spriteSheet.Play(animalName + "_moving_left");
        }
        else if (angle >= 45 && angle < 135)
        {
            flyingDir = Globals.NORTH;
            spriteSheet.Play(animalName + "_moving_up");
        }
        else if (angle >= 135 && angle < 225)
        {
            flyingDir = Globals.WEST;
            spriteSheet.Play(animalName + "_moving_left");
        }
        else if (angle >= 225 && angle < 315)
        {
            flyingDir = Globals.SOUTH;
            spriteSheet.Play(animalName + "_moving_down");
        }

        base.FrameFunc();
    }

    void Vanish()
    {
        Globals.maze.GuardsTargetVanish(gameObject);
        Actor.to_be_remove.Add(timer.GetComponent<Actor>());
        Actor.to_be_remove.Add(this);        
    }

    public override void OnTargetReached()
    {
        GoTo(FindFarestWallDestination());
        base.OnTargetReached();
    }

    public UnityEngine.Vector3 FindFarestWallDestination()
    {
        double farest = UnityEngine.Mathf.NegativeInfinity;
        UnityEngine.Vector3 destinationPosition = UnityEngine.Vector3.zero;
        System.String farestDir = "";
        System.Collections.Generic.List<System.String> dirs =
            new System.Collections.Generic.List<System.String>(Globals.DIRECTIONS);
        // 不能原路返回
        dirs.Remove(Globals.GetOppositeDir(flyingDir));
        foreach (System.String dir in dirs)
        {
            UnityEngine.Vector3 endPos = GetFarestOnDir(dir);
            double dis = UnityEngine.Vector3.Distance(endPos, transform.position);
            if (dis > farest)
            {
                farest = dis;
                destinationPosition = endPos;
                farestDir = dir;
            }
        }
        flyingDir = farestDir;
        return destinationPosition;
    }

    UnityEngine.Vector3 GetFarestOnDir(System.String direction)
    {
        Pathfinding.Node node = null;
        UnityEngine.Vector3 nodePos = transform.position;
        float nodeSize = Globals.maze.pathFinder.graph.nodeSize;
        UnityEngine.Vector3 lastWalkablePos = nodePos;
        do
        {
            lastWalkablePos = nodePos;
            if (direction == Globals.EAST)
            {
                nodePos += new UnityEngine.Vector3(nodeSize, 0.0f, 0.0f);
            }
            else if (direction == Globals.SOUTH)
            {
                nodePos += new UnityEngine.Vector3(0.0f, -nodeSize, 0.0f);
            }
            else if (direction == Globals.WEST)
            {
                nodePos += new UnityEngine.Vector3(-nodeSize, 0.0f, 0.0f);
            }
            else if (direction == Globals.NORTH)
            {
                nodePos += new UnityEngine.Vector3(0.0f, nodeSize, 0.0f);
            }
            node = Globals.maze.pathFinder.GetSingleNode(nodePos, true);            
        } while (node != null && node.walkable);
        return lastWalkablePos;
    }
}
