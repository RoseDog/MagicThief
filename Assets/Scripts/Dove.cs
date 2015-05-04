public class Dove : Actor 
{
    public System.String animalName;
    TrickTimer timer;
    public override void Awake()
    {
        base.Awake();

        double rand = UnityEngine.Random.Range(0.0f, 1.0f);
        if (rand < 0.25f)
        {
            animalName = "dove";
        }
        else if (rand >= 0.2f && rand < 0.4f)
        {
            animalName = "fog";
            spriteSheet.transform.localPosition = UnityEngine.Vector3.zero;
            moving.speed *= 0.7f;
        }
        else if (rand >= 0.4f && rand < 0.6f)
        {
            animalName = "chicken";
            spriteSheet.transform.localPosition = UnityEngine.Vector3.zero;
            moving.speed *= 0.7f;
        }
        else if (rand >= 0.6f && rand < 0.8f)
        {
            animalName = "cat";
            spriteSheet.transform.localPosition = UnityEngine.Vector3.zero;
            moving.speed *= 0.7f;
        }
        else if (rand >= 0.8f && rand <= 1.0f)
        {
            animalName = "mouse";
            spriteSheet.transform.localPosition = UnityEngine.Vector3.zero;
            moving.speed *= 0.8f;
        }
        
        spriteSheet.CreateAnimationByName(animalName + "_moving_left");
        spriteSheet.CreateAnimationByName(animalName + "_moving_down");
        spriteSheet.CreateAnimationByName(animalName + "_moving_up");

        moving.needAnimation = false;        
    }

    public void StartOut(UnityEngine.Vector3 pos, TrickData data)
    {
        System.String content_test = gameObject.name;
        content_test += " start out:" + pos.ToString("F5");
        Globals.record("testReplay", content_test);
        GoTo(pos);
        SleepThenCallFunction(data.duration, () => Vanish());
        timer = (UnityEngine.GameObject.Instantiate(Globals.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
        timer.BeginCountDown(spriteSheet.gameObject, data.duration, new UnityEngine.Vector3(0, 0.7f, 0));
    }

    System.String flyingDir;
    public override void Update()
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

        base.Update();
    }

    void Vanish()
    {
        Globals.maze.GuardsTargetVanish(gameObject);        
        DestroyImmediate(timer.gameObject);
        Destroy(gameObject);
    }

    public override void OnTargetReached()
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
        GoTo(destinationPosition);
        base.OnTargetReached();
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
