using System;
using System.Collections;
using System.Collections.Generic;

public class Cell : Actor
{
    private bool visited;
    public int y;
    public int x;
    public MazeGenerate maze;
    public Chest chest;
    public MazeGenerate.Room room;
    public UnityEngine.SpriteRenderer Floor_Sprite;
    public UnityEngine.SpriteRenderer E_Sprite;
    public UnityEngine.SpriteRenderer W_Sprite;
    public UnityEngine.SpriteRenderer S_Sprite;
    public UnityEngine.SpriteRenderer N_Sprite;    
    public UnityEngine.SpriteRenderer S_W_Corner_Sprite;
    public UnityEngine.SpriteRenderer S_E_Corner_Sprite;
    public UnityEngine.Collider N_E_Corner_Collider;
    public UnityEngine.Collider N_W_Corner_Collider;
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

    public Cell GetAdjacentCell(String direction)
    {
        if (!HasAdjacentCellInDirection(direction))
        {
            return null;
        }
            

        if (direction == Globals.EAST)
        {
            return maze.GetCell(y, x + 1);
        }
        else if (direction == Globals.SOUTH)
        {
            return maze.GetCell(y + 1, x);
        }
        else if (direction == Globals.WEST)
        {
            return maze.GetCell(y, x - 1);
        }
        else if (direction == Globals.NORTH)
        {
            return maze.GetCell(y - 1, x);
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
        if (maze.LocationIsOutsideBounds(this))
        {
            return false;
        }

        // Check if there is an adjacent cell in the direction
        if (direction == Globals.EAST)
        {
            return x < (maze.X_CELLS_COUNT - 1);
        }
        else if (direction == Globals.SOUTH)
        {
            return y < (maze.Y_CELLS_COUNT - 1);
        }
        else if (direction == Globals.WEST)
        {
            return x > 0;
        }
        else if (direction == Globals.NORTH)
        {
            return y > 0;
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
            direction = Globals.GetOppositeDir(direction);
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
            String adjacent_wall_dir = Globals.GetOppositeDir(direction);
            CreateWall(adjacent_cell, adjacent_wall_dir);
        }

        DestroyImmediate(GetFloor());        
    }

    public UnityEngine.GameObject CreateWall(Cell cell, String dir)
    {
        UnityEngine.GameObject wall = Globals.getChildGameObject(cell.gameObject, dir);
        if (wall == null)
        {            
            UnityEngine.GameObject wall_prefab = Globals.getChildGameObject(Globals.cell_prefab, dir);
            wall = UnityEngine.GameObject.Instantiate(wall_prefab) as UnityEngine.GameObject;
            wall.name = dir;            
            UnityEngine.Vector3 pos_cache = wall.transform.localPosition;
            UnityEngine.Vector3 scale_cache = wall.transform.localScale;
            wall.transform.parent = cell.transform;
            wall.transform.localScale = scale_cache;
            wall.transform.localPosition = pos_cache;


            if (dir == Globals.EAST)
            {
                cell.E_Sprite = wall.GetComponent<UnityEngine.SpriteRenderer>();
            }
            else if (dir == Globals.SOUTH)
            {
                cell.S_Sprite = wall.GetComponent<UnityEngine.SpriteRenderer>();
            }
            else if (dir == Globals.WEST)
            {
                cell.W_Sprite = wall.GetComponent<UnityEngine.SpriteRenderer>();
            }
            else if (dir == Globals.NORTH)
            {
                cell.N_Sprite = wall.GetComponent<UnityEngine.SpriteRenderer>();
            }
        }
        
        return wall;
    }

    public UnityEngine.GameObject CreateFloor()
    {
        if (GetFloor() == null)
        {
            UnityEngine.GameObject floor_prefab = Globals.getChildGameObject(Globals.cell_prefab, "floor_tile");
            UnityEngine.GameObject floor = UnityEngine.GameObject.Instantiate(floor_prefab) as UnityEngine.GameObject;
            floor.name = "floor_tile";
            UnityEngine.Vector3 scale_cache = floor.transform.localScale;
            floor.transform.position += transform.position;
            floor.transform.parent = transform;
            floor.transform.localScale = scale_cache;
            return floor;
        }
        return null;
    }

    public void FloorTurnToRed()
    {
        UnityEngine.GameObject floor = GetFloor();
        if (floor == null)
            throw new InvalidOperationException();
        UnityEngine.SpriteRenderer[] sprites = floor.GetComponentsInChildren<UnityEngine.SpriteRenderer>();
        foreach(UnityEngine.SpriteRenderer sprite in sprites)
        {
            sprite.color = UnityEngine.Color.red;
        }
    }

    public UnityEngine.GameObject GetFloor()
    {
        return Globals.getChildGameObject(gameObject, "floor_tile");
    }

    public UnityEngine.Vector3 GetFloorPos()
    {
        UnityEngine.GameObject floor = GetFloor();
        return floor.transform.position/* + new UnityEngine.Vector3(0.0f, 0.0f, -Globals.FLOOR_HEIGHT)*/;
    }

    public UnityEngine.Vector3 GetRandFloorPos()
    {
        UnityEngine.GameObject floor = GetFloor();
        float offset_limit = Globals.GetCellSideLength() * 0.3f;
        return floor.transform.position + new UnityEngine.Vector3(UnityEngine.Random.Range(-offset_limit, offset_limit), UnityEngine.Random.Range(-offset_limit, offset_limit), 0.0f);
    }

    public void HideEverythingExceptFloor()
    {
        UnityEngine.GameObject floor = GetFloor();
        UnityEngine.Renderer[] renderers = GetComponentsInChildren<UnityEngine.Renderer>();
        foreach (UnityEngine.Renderer renderer in renderers)
        {
            if (renderer.gameObject != floor)
            {
                renderer.enabled = false;
                UnityEngine.Collider collider = renderer.GetComponent<UnityEngine.Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }                
            }            
        }
    }

    public void ShowEverythingExceptFloor()
    {
        UnityEngine.GameObject floor = GetFloor();
        UnityEngine.SpriteRenderer[] renderers = GetComponentsInChildren<UnityEngine.SpriteRenderer>();
        foreach (UnityEngine.SpriteRenderer renderer in renderers)
        {
            if (renderer.gameObject != floor)
            {
                renderer.enabled = true;
            }
        }
    }

    public void ShowEverything()
    {        
        UnityEngine.SpriteRenderer[] renderers = GetComponentsInChildren<UnityEngine.SpriteRenderer>();
        foreach (UnityEngine.SpriteRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    public void DestroyEverythingExceptFloor()
    {
        UnityEngine.GameObject floor = GetFloor();
        UnityEngine.Renderer[] renderers = GetComponentsInChildren<UnityEngine.Renderer>();
        foreach (UnityEngine.Renderer renderer in renderers)
        {
            if (renderer.gameObject != floor)
            {
                Destroy(renderer.gameObject);
            }
        }
    }
    

    public override void Awake()
    {
        visited = false;
        base.Awake();
    }
}
