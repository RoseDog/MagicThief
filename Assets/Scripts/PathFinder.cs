using System.Collections;

public class PathFinder : UnityEngine.MonoBehaviour
{
    public AstarPath path;
    MazeGenerate map;
    public Pathfinding.GridGraph graph;
    float grideNodeSize = 0.5f;
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

    void Start()
    {        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GenerateGridGraph()
    {
        graph = path.astarData.graphs[0] as Pathfinding.GridGraph;
        graph.width = Globals.maze.X_CELLS_COUNT * Globals.maze.GetCellSideLength() * (int)(1 / grideNodeSize);
        graph.depth = Globals.maze.Z_CELLS_COUNT * Globals.maze.GetCellSideLength() * (int)(1 / grideNodeSize);
        graph.center = new UnityEngine.Vector3(-Globals.maze.GetCellSideLength(), -Globals.maze.GetCellSideLength() * 0.5f, 0);
        //graph.nodeSize = map.cell_side_length / 2.0f;
        graph.nodeSize = grideNodeSize;
        graph.UpdateSizeFromWidthDepth();
        path.Scan();        
    }

    public Pathfinding.Node GetSingleNode(UnityEngine.Vector3 pos, bool needWalkable)
    {
        System.Collections.Generic.List<Pathfinding.Node> nodes =
            graph.GetNodesInArea(new UnityEngine.Bounds(pos, new UnityEngine.Vector3(grideNodeSize, 10.0f, grideNodeSize)));

        foreach (Pathfinding.Node node in nodes)
        {
            if (!needWalkable || node.walkable)
            {
                return node;
            }
        }        
        return null;
    }
}
