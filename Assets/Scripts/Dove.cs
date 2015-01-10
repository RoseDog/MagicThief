public class Dove : Actor 
{
    public override void Awake()
    {
        base.Awake();
        anim["idle"].speed = 5.0f;
        collider.enabled = false;
        moving.canMove = false;
        Visible(false);
    }

    System.String flyingDir;
    public void ShowFlyPath(UnityEngine.Vector3 dir, float flyDistance)
    {
        float nodeSize = Globals.maze.pathFinder.GetGrideNodeSize();
        dir.Normalize();

        // 如果魔术师站在一个unwalkable node上释放鸽子，需要找到一个最近的walkable node开始寻路
        UnityEngine.Vector3 posCanReach = transform.position;
        Pathfinding.Node nodeCanReach = Globals.maze.pathFinder.GetSingleNode(posCanReach, true);        
        if (nodeCanReach == null)
        {
            posCanReach = transform.position = moving.GetNearestWalkableNodePosition();
        }

        while (true)
        {            
            nodeCanReach = Globals.maze.pathFinder.GetSingleNode(posCanReach, true);
            if (nodeCanReach == null)
            {
                break;
            }
            posCanReach += dir * nodeSize;
        }

        ShowPathToPoint(posCanReach);

        float angle = UnityEngine.Vector3.Angle(dir, new UnityEngine.Vector3(1, 0, 0));
        if (angle >= 315 || angle < 45)
        {
            flyingDir = Globals.EAST;
        }
        else if (angle >= 45 && angle < 135)
        {
            flyingDir = Globals.NORTH;
        }
        else if (angle >= 135 && angle < 225)
        {
            flyingDir = Globals.WEST;
        }
        else if (angle >= 225 && angle < 315)
        {
            flyingDir = Globals.SOUTH;
        }
    }

    public void HideFlyPath()
    {        
        HidePath();
    }

    public void ReleaseToFly()
    {
        Visible(true);
        moving.canMove = true;
        moving.canSearch = false;
        collider.enabled = true;
        Invoke("Vanish", 5.0f);
        UnityEngine.Debug.Log("ReleaseDove");       
    }

    void Vanish()
    {        
        foreach (Guard guard in Globals.maze.guards)
        {
            if (guard.spot.target == this.transform)
            {
                guard.spot.EnemyOutVision();
            }
        }
        DestroyImmediate(gameObject);
    }

    public override void OnTargetReached()
    {       
        float farest = UnityEngine.Mathf.NegativeInfinity;
        UnityEngine.Vector3 destinationPosition = UnityEngine.Vector3.zero;
        System.String farestDir = "";
        System.Collections.Generic.List<System.String> dirs = 
            new System.Collections.Generic.List<System.String>(Globals.DIRECTIONS);
        // 不能原路返回
        dirs.Remove(Globals.GetOppositeDir(flyingDir));
        foreach (System.String dir in dirs)
        {
            UnityEngine.Vector3 endPos = GetFarestOnDir(dir);
            float dis = UnityEngine.Vector3.Distance(endPos, transform.position);
            if (dis > farest)
            {
                farest = dis;
                destinationPosition = endPos;
                farestDir = dir;
            }
        }
        flyingDir = farestDir;
        MoveTo(destinationPosition);
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
                nodePos += new UnityEngine.Vector3(0.0f, 0.0f, -nodeSize);
            }
            else if (direction == Globals.WEST)
            {
                nodePos += new UnityEngine.Vector3(-nodeSize, 0.0f, 0.0f);
            }
            else if (direction == Globals.NORTH)
            {
                nodePos += new UnityEngine.Vector3(0.0f, 0.0f, nodeSize);
            }
            node = Globals.maze.pathFinder.GetSingleNode(nodePos, true);            
        } while (node != null && node.walkable);
        return lastWalkablePos;
    }
}
