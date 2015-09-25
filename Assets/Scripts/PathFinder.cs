using System.Collections;

public class PathFinder : UnityEngine.MonoBehaviour
{
    public AstarPath path;
    MazeGenerate map;
    public Pathfinding.GridGraph graph;
    float grideNodeSize = 25f;
    public float GetGrideNodeSize()
    {
        return grideNodeSize;
    }
	// Use this for initialization
	void Awake () 
    {
        path = GetComponent<AstarPath>();
        path.logPathResults = PathLog.OnlyErrors;
	}
	
    public void GenerateGridGraph()
    {
        graph = path.astarData.graphs[0] as Pathfinding.GridGraph;
        graph.nodes = null;
        graph.width = UnityEngine.Mathf.RoundToInt(Globals.maze.X_CELLS_COUNT * Globals.maze.GetCellSideLength() * (1 / grideNodeSize));
        graph.depth = UnityEngine.Mathf.RoundToInt(Globals.maze.Y_CELLS_COUNT * Globals.maze.GetCellSideLength() * (1 / grideNodeSize));
        graph.center = new UnityEngine.Vector3(-Globals.maze.GetCellSideLength(), 0, 0);
        //graph.nodeSize = map.cell_side_length / 2.0f;
        graph.nodeSize = grideNodeSize;
        graph.collision.diameter = 0.1f;
        graph.UpdateSizeFromWidthDepth();
        path.Scan();
    }

    public Pathfinding.Node GetSingleNode(UnityEngine.Vector3 pos, bool needWalkable)
    {
        System.Collections.Generic.List<Pathfinding.Node> nodes =
            graph.GetNodesInArea(new UnityEngine.Bounds(pos, new UnityEngine.Vector3(grideNodeSize, grideNodeSize, 1.0f)));

        foreach (Pathfinding.Node node in nodes)
        {
            if (!needWalkable || node.walkable)
            {
                return node;
            }
        }        
        return null;
    }

    public Pathfinding.Node GetNearestWalkableNode(UnityEngine.Vector3 pos)
    {
        Pathfinding.NNInfo nodeInfo = graph.GetNearestForce(pos, Pathfinding.NNConstraint.Default);
        return nodeInfo.node;
    }

    public Pathfinding.Node GetNearestUnwalkableNode(UnityEngine.Vector3 pos)
    {
        Pathfinding.NNConstraint nnc = Pathfinding.NNConstraint.Default;
        nnc.walkable = false;
        Pathfinding.NNInfo nodeInfo = graph.GetNearestForce(pos, nnc);
        return nodeInfo.node;
    }

    public bool IsPositionWalkable(UnityEngine.Vector3 pos)
    {
        System.Collections.Generic.List<Pathfinding.Node> nodes =
            graph.GetNodesInArea(new UnityEngine.Bounds(pos, new UnityEngine.Vector3(0.0001f, 0.0001f, 1.0f)));

        foreach (Pathfinding.Node node in nodes)
        {
            if (node.walkable)
            {
                return true;
            }
        }
        return false;
    }
}
