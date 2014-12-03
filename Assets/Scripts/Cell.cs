using System;
using System.Collections;
using System.Collections.Generic;

public class Cell : UnityEngine.MonoBehaviour
{
    private bool visited;
    public int z;
    public int x;
    public MapGenerate map;
    public bool Visited
    {
        get { return visited; }
        set { visited = value; }
    }

    public bool IsCorridor
    {
        get 
        { 
            foreach (String dir in Globals.DIRECTIONS)
            {
                // 只要有空的墙，那就是个走廊
                if (Globals.getChildGameObject(gameObject, dir) == null)
                {
                    return true;
                }
            }
            
            return false; 
        }
    }

    public String GetOppositeDir(String direction)
    {
        if (direction == Globals.EAST)
        {
            return Globals.WEST;
        }
        else if (direction == Globals.SOUTH)
        {
            return Globals.NORTH;
        }
        else if (direction == Globals.WEST)
        {
            return Globals.EAST;
        }
        else if (direction == Globals.NORTH)
        {
            return Globals.SOUTH;
        }

        return "";
    }

    public Cell GetAdjacentCell(String direction)
    {
        if (!HasAdjacentCellInDirection(direction))
            throw new InvalidOperationException("No adjacent cell exists for the location and direction provided.");

        if (direction == Globals.EAST)
        {
            return map.GetCell(z, x + 1);
        }
        else if (direction == Globals.SOUTH)
        {
            return map.GetCell(z + 1, x);
        }
        else if (direction == Globals.WEST)
        {
            return map.GetCell(z, x - 1);
        }
        else if (direction == Globals.NORTH)
        {
            return map.GetCell(z - 1, x);
        }
        else
        {
            throw new InvalidOperationException("direction string error");
        }
    }

    public bool HasWallInDirection(String direction)
    {
        return Globals.getChildGameObject(gameObject, direction) != null;
    }

    public int WallCount()
    {
        int wallCount = 0;
        if(HasWallInDirection(Globals.EAST))
        {
            ++wallCount;
        }
        if(HasWallInDirection(Globals.WEST))
        {
            ++wallCount;
        }
        if(HasWallInDirection(Globals.SOUTH))
        {
            ++wallCount;
        }
        if (HasWallInDirection(Globals.NORTH))
        {
            ++wallCount;
        }
        return wallCount;
    }

    public bool HasAdjacentCellInDirection(String direction)
    {
        // Check that the location falls within the bounds of the map
        if (map.LocationIsOutsideBounds(this))
        {
            return false;
        }

        // Check if there is an adjacent cell in the direction
        if (direction == Globals.EAST)
        {
            return x < (map.X_CELLS_COUNT - 1);
        }
        else if (direction == Globals.SOUTH)
        {
            return z < (map.Z_CELLS_COUNT - 1);
        }
        else if (direction == Globals.WEST)
        {
            return x > 0;
        }
        else if (direction == Globals.NORTH)
        {
            return z > 0;
        }
        return true;
    }

    public bool AdjacentCellInDirectionIsVisited(String direction)
    {
        if (!HasAdjacentCellInDirection(direction))
            throw new InvalidOperationException("No adjacent cell exists for the location and direction provided.");
        Cell adjacent_cell = GetAdjacentCell(direction);        
        return adjacent_cell.Visited;
    }

    public bool AdjacentCellInDirectionIsCorridor(String direction)
    {
        if (HasAdjacentCellInDirection(direction))
        {
            Cell adjacent_cell = GetAdjacentCell(direction);
            return adjacent_cell.IsCorridor;
        }
        return false;        
    }

    public Cell DestroyWall(String direction)
    {
        UnityEngine.GameObject wall_to_be_delete_1 = Globals.getChildGameObject(gameObject, direction);
        DestroyImmediate(wall_to_be_delete_1);

        if (HasAdjacentCellInDirection(direction))
        {
            Cell target = GetAdjacentCell(direction);
            direction = GetOppositeDir(direction);
            UnityEngine.GameObject target_cell = target.gameObject;
            UnityEngine.GameObject wall_to_be_delete_2 = Globals.getChildGameObject(target_cell, direction);
            DestroyImmediate(wall_to_be_delete_2);
            return target;
        }
        
        return null;
    }


    public bool IsDeadEnd
    {
        get
        {
            List<String> walls = new List<String>(Globals.DIRECTIONS);
            int wall_count = 0;
            UnityEngine.Transform[] children = transform.GetComponentsInChildren<UnityEngine.Transform>();
            foreach (UnityEngine.Transform child in children)
            {
                if (walls.Contains(child.name))
                {
                    ++wall_count;
                }
            }
            return wall_count == 3;
        }
    }

    public String CalculateDeadEndCorridorDirection()
    {
        if (!IsDeadEnd) 
            throw new InvalidOperationException();

        List<String> walls = new List<String>(Globals.DIRECTIONS);
        UnityEngine.Transform[] children = transform.GetComponentsInChildren<UnityEngine.Transform>();
        foreach (UnityEngine.Transform child in children)
        {
            if (walls.Contains(child.name))
            {
                walls.Remove(child.name);
            }
        }

        if (walls.Count > 1)
        {
            throw new InvalidOperationException();
        }

        return walls[0];
    }

    public void SealCorridor(String direction)
    {
        CreateWall(this, direction);

        if (HasAdjacentCellInDirection(direction))
        {
            Cell adjacent_cell = GetAdjacentCell(direction);
            String adjacent_wall_dir = GetOppositeDir(direction);
            CreateWall(adjacent_cell, adjacent_wall_dir);
        }

        DestroyImmediate(GetFloor());
    }

    public UnityEngine.GameObject CreateWall(Cell cell, String dir)
    {
        UnityEngine.GameObject wall = Globals.getChildGameObject(cell.gameObject, dir);
        if (wall == null)
        {
            UnityEngine.GameObject cell_prefab = UnityEngine.Resources.Load("Props/Cell") as UnityEngine.GameObject;
            UnityEngine.GameObject wall_prefab = Globals.getChildGameObject(cell_prefab, dir);
            wall = UnityEngine.GameObject.Instantiate(wall_prefab) as UnityEngine.GameObject;
            wall.name = dir;
            wall.transform.position += cell.transform.position;
            wall.transform.parent = cell.transform;            
        }
        
        return wall;
    }

    public UnityEngine.GameObject CreateFloor()
    {
        if (GetFloor() == null)
        {
            UnityEngine.GameObject cell_prefab = UnityEngine.Resources.Load("Props/Cell") as UnityEngine.GameObject;
            UnityEngine.GameObject floor_prefab = Globals.getChildGameObject(cell_prefab, "floor_tile");
            UnityEngine.GameObject floor = UnityEngine.GameObject.Instantiate(floor_prefab) as UnityEngine.GameObject;
            floor.name = "floor_tile";
            floor.transform.position += transform.position;
            floor.transform.parent = transform;
            return floor;
        }
        return null;
    }

    public void FloorTurnToWhile()
    {
        UnityEngine.GameObject floor = GetFloor();
        if (floor == null)
            throw new InvalidOperationException();
        floor.renderer.material.shader = UnityEngine.Shader.Find("Mobile/Unlit (Supports Lightmap)");
    }

    public void CreateTorchLight()
    {
        foreach (String dir in Globals.DIRECTIONS)
        {
            UnityEngine.GameObject wall = Globals.getChildGameObject(gameObject, dir);
            if (wall != null)
            {
                UnityEngine.GameObject touch_prefab = UnityEngine.Resources.Load("Props/Prefab_Pieces/Torch_Lit_Prefab") as UnityEngine.GameObject;
                UnityEngine.GameObject touch = UnityEngine.GameObject.Instantiate(touch_prefab) as UnityEngine.GameObject;                

                if (wall.name == Globals.EAST)
                {
                    touch.transform.localEulerAngles = new UnityEngine.Vector3(0.0f, 180.0f, 0.0f);
                }
                else if (wall.name == Globals.SOUTH)
                {
                    touch.transform.localEulerAngles = new UnityEngine.Vector3(0.0f, 90.0f, 0.0f);
                }
                else if (wall.name == Globals.WEST)
                {
                    ;
                }
                else if (wall.name == Globals.NORTH)
                {
                    touch.transform.localEulerAngles = new UnityEngine.Vector3(0.0f, -90.0f, 0.0f);
                }

                //touch.transform.parent = wall.transform;
                //touch.transform.localPosition = UnityEngine.Vector3.zero;
                touch.transform.position = wall.transform.position + new UnityEngine.Vector3(0.0f, 2.0f, 0.0f);
                touch.transform.parent = wall.transform;
                break;
            }
        }        
    }

    public UnityEngine.GameObject GetFloor()
    {
        return Globals.getChildGameObject(gameObject, "floor_tile");
    }

    public UnityEngine.Vector3 GetFloorPos()
    {
        UnityEngine.GameObject floor = GetFloor();
        return floor.transform.position + new UnityEngine.Vector3(0.0f, 0.2f, 0.0f);
    }
    

    void Awake()
    {
        visited = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
