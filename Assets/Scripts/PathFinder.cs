using System.Collections;

public class PathFinder : UnityEngine.MonoBehaviour
{
    AstarPath path;
    MazeGenerate map;
    public Pathfinding.GridGraph graph;
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
        graph.width = Globals.maze.X_CELLS_COUNT * Globals.maze.cell_side_length;
        graph.depth = Globals.maze.Z_CELLS_COUNT * Globals.maze.cell_side_length;
        graph.center = new UnityEngine.Vector3(-Globals.maze.cell_side_length, -Globals.maze.cell_side_length * 0.5f, 0);
        //graph.nodeSize = map.cell_side_length / 2.0f;
        graph.nodeSize = 1.0f;
        graph.UpdateSizeFromWidthDepth();
        path.Scan();        
    }

    public Pathfinding.Node GetSingleNode(UnityEngine.Vector3 pos, bool needWalkable)
    {
        System.Collections.Generic.List<Pathfinding.Node> nodes =
            graph.GetNodesInArea(new UnityEngine.Bounds(pos, new UnityEngine.Vector3(1.0f, 10.0f, 1.0f)));

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
