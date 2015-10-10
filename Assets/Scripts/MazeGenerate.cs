using System;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerate : UnityEngine.MonoBehaviour
{          
    [UnityEngine.Range(0, 30)]
    public int Y_CELLS_COUNT;
    [UnityEngine.Range(0, 30)]
    public int X_CELLS_COUNT;
    [UnityEngine.Range(0, 100)]
    public int CHANGE_DIRECTION_MODIFIER;
    [UnityEngine.Range(0, 100)]
    public int sparsenessModifier = 100;
    [UnityEngine.HideInInspector]
    public int deadEndRemovalModifier;

    [UnityEngine.Range(0, 8)]
    public int noOfRoomsToPlace;
    // 最小要2x2才能放下一个宝箱
    [UnityEngine.Range(2, 4)]
    public int minRoomXCellsCount;
    [UnityEngine.Range(2, 10)]
    public int maxRoomXCellsCount;
    [UnityEngine.Range(2, 4)]
    public int minRoomZCellsCount;
    [UnityEngine.Range(2, 10)]
    public int maxRoomZCellsCount;

    public int GEMS_COUNT = 3;
    public int LAMPS_COUNT = 3;

    public System.String IniFileNameForEditor;
    public System.String LevelTipText;

    public int CASH = 0;

    public System.String droppedItemsStr;
    public System.String cashOnFloorStr;
    

    UnityEngine.GameObject maze;

    public UnityEngine.Vector3 left_up_corner_pos;
    public UnityEngine.Vector3 center_pos;
    Cell EastNorthCornerCell;
    Cell EastSouthCornerCell;
    Cell WestSouthCornerCell;
    Cell WestNorthCornerCell;

    public PathFinder pathFinder;
    public Guard choosenGuard;
    public System.Collections.Generic.List<Guard> guards = new System.Collections.Generic.List<Guard>();
    public System.Collections.Generic.List<Chest> chests = new System.Collections.Generic.List<Chest>();
    public System.Collections.Generic.List<UnityEngine.GameObject> gemHolders = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public System.String pieces_dir = "";        
    
    // 每一次挖走廊，都会重新创建这个类型的对象来选择方向
    public class DirectionPicker
    {
        private readonly List<String> directionsPicked = new List<String>();
        private readonly String previousDirection;
        private readonly int changeDirectionModifier;


        public DirectionPicker(String previousDirection, int changeDirectionModifier)
        {
            this.previousDirection = previousDirection;
            this.changeDirectionModifier = changeDirectionModifier;
        }

        public bool HasNextDirection
        {
            get { return directionsPicked.Count < 4; }
        }

        private bool MustChangeDirection
        {
            get
            {
                // changeDirectionModifier of 100 will always change direction
                // value of 0 will never change direction
                return ((directionsPicked.Count > 0) ||
                changeDirectionModifier > UnityEngine.Random.Range(0, 99));
            }
        }

        private String PickDifferentDirection()
        {
            String directionPicked;
            do
            {
                directionPicked = Globals.DIRECTIONS[UnityEngine.Random.Range(0, 4)];
            } while ((directionPicked == previousDirection) && (directionsPicked.Count < 3));
            return directionPicked;
        }

        public String GetNextDirection()
        {
            if (!HasNextDirection)
                throw new System.InvalidOperationException("No directions available");

            String directionPicked;
            do
            {
                directionPicked = MustChangeDirection ? PickDifferentDirection() : previousDirection;
            } while (directionsPicked.Contains(directionPicked));

            directionsPicked.Add(directionPicked);
            return directionPicked;
        }
    }

    private UnityEngine.Rect bounds;
    public UnityEngine.Rect Bounds
    {
        get { return bounds; }
    }

    public UnityEngine.Rect BoundsInPixel
    {
        get 
        {
            UnityEngine.Rect r = new UnityEngine.Rect(WestNorthCornerCell.GetFloorPos().x - Globals.GetCellSideLength() * 0.5f,
                WestNorthCornerCell.GetFloorPos().y + Globals.GetCellSideLength()*0.5f,
                EastNorthCornerCell.GetFloorPos().x - WestNorthCornerCell.GetFloorPos().x + Globals.GetCellSideLength(),
                WestNorthCornerCell.GetFloorPos().y - WestSouthCornerCell.GetFloorPos().y + Globals.GetCellSideLength());
            return r; 
        }
    }

    public float EastPosInPixel()
    {
        return EastNorthCornerCell.transform.position.x + Globals.GetCellSideLength()*0.5f;
    }

    public float SouthPosInPixel()
    {
        return EastSouthCornerCell.transform.position.y - Globals.GetCellSideLength() * 0.5f;
    }

    public float WestPosInPixel()
    {
        return WestNorthCornerCell.transform.position.x - Globals.GetCellSideLength() * 0.5f;
    }

    public float NorthPosInPixel()
    {
        return WestNorthCornerCell.transform.position.y + Globals.GetCellSideLength() * 0.5f;
    }

    public UnityEngine.Vector2 GetMiniMapProjectPosition(float x_ratio, float y_ratio)
    {
        UnityEngine.Vector2 left_bottom_corner_pos = new UnityEngine.Vector3(
            left_up_corner_pos.x,
            left_up_corner_pos.y - Globals.GetCellSideLength() * Y_CELLS_COUNT,
            0);
        return new UnityEngine.Vector2(
            left_bottom_corner_pos.x + x_ratio * Globals.GetCellSideLength() * X_CELLS_COUNT,
            left_bottom_corner_pos.y + y_ratio * Globals.GetCellSideLength() * Y_CELLS_COUNT);
    }

    private Cell[,] cells;
    public void MarkCellsUnvisited()
    {
        for (int x = 0; x < X_CELLS_COUNT; x++)
            for (int y = 0; y < Y_CELLS_COUNT; y++)
                cells[x, y].Visited = false;
    }

    public Cell GetCell(int y, int x)
    {
        return cells[y, x];
    }

    public Cell GetCellByPos(UnityEngine.Vector3 pos)
    {
        int y = UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Abs(left_up_corner_pos.y - pos.y) / Globals.GetCellSideLength());
        int x = UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Abs(left_up_corner_pos.x - pos.x) / Globals.GetCellSideLength());
        return GetCell(y, x);
    }

    public Cell PickRandomCellAndMarkItVisited()
    {
        int rand_x = UnityEngine.Random.Range(0, X_CELLS_COUNT - 1);
        int rand_y = UnityEngine.Random.Range(0, Y_CELLS_COUNT - 1);
        Cell cell = GetCell(rand_y, rand_x);
        cell.Visited = true;
        visitedCells.Add(cell);
        return cell;
    }
    
    private readonly List<Cell> visitedCells = new List<Cell>();
    public void FlagCellAsVisited(Cell cell)
    {
        if (LocationIsOutsideBounds(cell))
            throw new ArgumentException("Location is outside of Map bounds", "location");

        if (cell.Visited)
            throw new ArgumentException("Location is already visited", cell.y.ToString() + "," +  cell.x.ToString());

        cell.Visited = true;
        visitedCells.Add(cell);
    }

    public bool LocationIsOutsideBounds(Cell cell)
    {
        return ((cell.x < 0) || (cell.x >= X_CELLS_COUNT) || (cell.y < 0) || (cell.y >= Y_CELLS_COUNT));
    }

    public bool AllCellsVisited
    {
        get { return visitedCells.Count == (X_CELLS_COUNT * Y_CELLS_COUNT); }
    }

    public Cell GetRandomVisitedCell(Cell cell)
    {
        if (visitedCells.Count == 0)
            throw new InvalidOperationException("There are no visited cells to return.");

        int index = UnityEngine.Random.Range(0, visitedCells.Count);

        // Loop while the current cell is the visited cell
        while (visitedCells[index] == cell)
            index = UnityEngine.Random.Range(0, visitedCells.Count);

        return visitedCells[index];
    }

    public IEnumerable<Cell> DeadEndCell
    {
        get
        {
            for (int z = 0; z < Y_CELLS_COUNT; z++)
                for (int x = 0; x < X_CELLS_COUNT; x++)
                {
                    Cell cell = GetCell(z, x);
                    if (cell.IsDeadEnd)
                    {
                        yield return cell;
                    }
                }
        }
    }

    public bool AllDeadEndRemoved
    {
        get
        {
            for (int y = 0; y < Y_CELLS_COUNT; y++)
                for (int x = 0; x < X_CELLS_COUNT; x++)
                {
                    Cell cell = GetCell(y, x);
                    if (cell.IsDeadEnd)
                    {
                        return false;
                    }
                }
            return true;
        }
    }

    public bool ShouldRemoveDeadend()
    {
        return UnityEngine.Random.Range(1, 99) < deadEndRemovalModifier;
    }

    

    public class Room
    {
        public Cell upper_left;
        public int Y_CELLS_COUNT;
        public int X_CELLS_COUNT;
        public System.Collections.Generic.List<Cell> walls = new System.Collections.Generic.List<Cell>();
        public System.Collections.Generic.List<Cell> doors = new System.Collections.Generic.List<Cell>();
        public System.Collections.Generic.List<Cell> couldBeDoors = new System.Collections.Generic.List<Cell>();
        public System.Collections.Generic.List<UnityEngine.Vector3> doorsPositions = new System.Collections.Generic.List<UnityEngine.Vector3>();
        public Room(int XCount, int YCount)
        {
            X_CELLS_COUNT = XCount;
            Y_CELLS_COUNT = YCount;            
        }

        public void SetLocation(Cell location)
        {
            upper_left = location;
            for (int x = 0; x < X_CELLS_COUNT; ++x)
            {
                for (int y = 0; y < Y_CELLS_COUNT; ++y)
                {
                    Cell cell = Globals.maze.GetCell(upper_left.y + y, upper_left.x + x);
                    cell.room = this;
                }
            }
        }

        public bool IsContain(Cell cell)
        {
            if (cell.y < upper_left.y || cell.y > upper_left.y + Y_CELLS_COUNT)
            {
                return false;
            }

            if (cell.x < upper_left.x || cell.x > upper_left.x + X_CELLS_COUNT)
            {
                return false;
            }
            return true;
        }

        public bool IsCorner(Cell cell)
        {
            if (IsContain(cell))
            {
                if (cell == upper_left ||
                    (cell.x == upper_left.x + X_CELLS_COUNT-1 && cell.y == upper_left.y + Y_CELLS_COUNT-1) ||
                    (cell.x == upper_left.x && cell.y == upper_left.y + Y_CELLS_COUNT-1)||
                    (cell.x == upper_left.x + X_CELLS_COUNT-1 && cell.y == upper_left.y))
                {
                    return true;
                }
                return false;
            }
            else
            {
                throw new InvalidOperationException("cell not contained in this room");
            }
        }

        public System.Collections.Generic.List<Cell> GetCornersWithTwoWall()
        {
            System.Collections.Generic.List < Cell >  corners = new System.Collections.Generic.List<Cell>();
            for (int x = 0; x < X_CELLS_COUNT; ++x)
            {
                for (int y = 0; y < Y_CELLS_COUNT; ++y)
                {
                    Cell cell = Globals.maze.GetCell(upper_left.y + y, upper_left.x + x);
                    if ((IsCorner(cell) && cell.WallCount() == 2)
                        || (!IsCorner(cell) && cell.WallCount() == 1))
                    {
                        corners.Add(cell);
                    }
                }
            }
            return corners;
        }

        public void TurnToWhite()
        {
            for (int x = 0; x < X_CELLS_COUNT; ++x)
            {
                for (int y = 0; y < Y_CELLS_COUNT; ++y)
                {
                    Cell cell = Globals.maze.GetCell(upper_left.y + y, upper_left.x + x);
                    cell.FloorTurnToRed();
                }
            }
        }

        public UnityEngine.Vector3 GetRandomRoomPosition()
        {
            Cell bottom_right = Globals.maze.GetCell(upper_left.y + Y_CELLS_COUNT-1, upper_left.x + X_CELLS_COUNT-1);
            UnityEngine.Vector3 upper_left_pos = upper_left.GetFloorPos();
            UnityEngine.Vector3 bottom_right_pos = bottom_right.GetFloorPos();
            return new UnityEngine.Vector3(
                UnityEngine.Random.Range(upper_left_pos.x, bottom_right_pos.x), 
                UnityEngine.Random.Range(upper_left_pos.y, bottom_right_pos.y), 
                UnityEngine.Random.Range(upper_left_pos.z, bottom_right_pos.z)
                );
        }        
    }

   

    IEnumerator CreateDenseMaze(float waitTime)
    {
        // 挖墙，一直到所有Cell都Visited
        // 一定不会出现有Cell四面都是墙
        Cell currentLocation = PickRandomCellAndMarkItVisited();

        String previousDirection = Globals.NORTH;

        while (!AllCellsVisited)
        {
            DirectionPicker directionPicker = new DirectionPicker(previousDirection, CHANGE_DIRECTION_MODIFIER);
            String direction = directionPicker.GetNextDirection();
            // 循环直到找到可以挖的墙
            // 这个方向有墙，而且这个Cell的墙还没被挖过，那就可以挖了
            while (!currentLocation.HasAdjacentCellInDirection(direction) || currentLocation.AdjacentCellInDirectionIsVisited(direction))
            {
                if (directionPicker.HasNextDirection)
                    direction = directionPicker.GetNextDirection();

                else
                {
                    // 如果已经没有方向可以挖了，重新选一个已经挖过的Cell开始向四周探寻可以挖的墙
                    currentLocation = GetRandomVisitedCell(currentLocation); // Get a new previously visited location
                    directionPicker = new DirectionPicker(previousDirection, CHANGE_DIRECTION_MODIFIER); // Reset the direction picker
                    direction = directionPicker.GetNextDirection(); // Get a new direction
                }
            }

            // 执行挖墙
            currentLocation = currentLocation.DestroyWall(direction);
            FlagCellAsVisited(currentLocation);
            previousDirection = direction;
            if (Globals.SHOW_MACE_GENERATING_PROCESS)
            {
                yield return new UnityEngine.WaitForSeconds(waitTime);
            }
        }

        // SparsifyMaze
        // 封闭DeadEnd
        // DeadEnd：只有一个出口的Cell
        // sparsenessModifier：要封闭的DeadEnd占所有Cells的百分比。这个数字越低，迷宫的感官复杂度会越高。
        // noOfDeadEndCellsToRemove：要封闭的DeadEnd具体数量

        // Calculate the number of cells to remove as a percentage of the total number of cells in the map
        int noOfDeadEndCellsToRemove = (int)Math.Ceiling((decimal)sparsenessModifier / 100 * (X_CELLS_COUNT * Y_CELLS_COUNT));
        IEnumerator<Cell> enumerator = DeadEndCell.GetEnumerator();
        for (int i = 0; i < noOfDeadEndCellsToRemove; i++)
        {
            if (!enumerator.MoveNext()) // Check if there is another item in our enumerator
            {
                enumerator = DeadEndCell.GetEnumerator(); // Get a new enumerator
                if (!enumerator.MoveNext())
                    break; // No new items exist so break out of loop
            }
            Cell cell = enumerator.Current;
            cell.SealCorridor(cell.CalculateDeadEndCorridorDirection());
            if (Globals.SHOW_MACE_GENERATING_PROCESS)
            {
                yield return new UnityEngine.WaitForSeconds(waitTime);
            }
        }

        // 
        yield return StartCoroutine(RemoveDeadEnds(waitTime));
    }

    IEnumerator RemoveDeadEnds(float waitTime)
    {
        while (!AllDeadEndRemoved)
        {
            foreach (Cell dead_cell in DeadEndCell)
            {
                if (ShouldRemoveDeadend())
                {
                    Cell cell = dead_cell;
                    do
                    {
                        // Initialize the direction picker not to select the dead-end corridor direction
                        DirectionPicker directionPicker = new DirectionPicker(cell.CalculateDeadEndCorridorDirection(), 100);
                        String direction = directionPicker.GetNextDirection();

                        while (!cell.HasAdjacentCellInDirection(direction))
                        {
                            if (directionPicker.HasNextDirection)
                                direction = directionPicker.GetNextDirection();
                            else
                                throw new InvalidOperationException("This should not happen");
                        }

                        // Create a corridor in the selected direction
                        cell.CreateFloor();
                        cell = cell.DestroyWall(direction);
                        cell.CreateFloor();

                    } while (cell.IsDeadEnd); // Stop when you intersect an existing corridor.

                    if (Globals.SHOW_MACE_GENERATING_PROCESS)
                    {
                        yield return new UnityEngine.WaitForSeconds(waitTime);
                    }
                }
            }
        }

        yield return StartCoroutine(PlaceRooms(waitTime));
    }

    public List<Room> rooms = new List<Room>();
    public System.Collections.ObjectModel.ReadOnlyCollection<Room> Rooms
    {
        get { return rooms.AsReadOnly(); }
    }

    public static Room CreateRoom(int minXCellsCount, int maxXCellsCount, int minZCellsCount, int maxZCellsCount)
    {
        return new Room(UnityEngine.Random.Range(minXCellsCount, maxXCellsCount+1), UnityEngine.Random.Range(minZCellsCount, maxZCellsCount+1));
    }

    public int CalculateRoomPlacementScore(Cell upper_left_cell, Room room)
    {
        // Check if the room at the given location will fit inside the bounds of the map
        // 如果左上角放在这里，整个Room会溢出地图外，那就肯定不能放在这里
        UnityEngine.Rect room_rect = new UnityEngine.Rect(upper_left_cell.x, upper_left_cell.y, room.X_CELLS_COUNT-1, room.Y_CELLS_COUNT-1);
        if (Bounds.Contains(room_rect.min) && Bounds.Contains(room_rect.max))
        {
            int roomPlacementScore = 0;
            // Loop for each cell in the room            
            for (int y = 0; y < room.Y_CELLS_COUNT; y++)
            {
                for (int x = 0; x < room.X_CELLS_COUNT; x++)
                {
                    // location in the dungeon(Cell of Maze)
                    // 假设这个Cell会成为房间的Cell，计算Score的增长
                    Cell currentMazeCell = GetCell(upper_left_cell.y + y, upper_left_cell.x + x);

                    // Add 1 point for each adjacent corridor to the cell
                    if (currentMazeCell.AdjacentCellInDirectionIsCorridor(Globals.NORTH))
                        roomPlacementScore++;

                    if (currentMazeCell.AdjacentCellInDirectionIsCorridor(Globals.SOUTH))
                        roomPlacementScore++;

                    if (currentMazeCell.AdjacentCellInDirectionIsCorridor(Globals.WEST))
                        roomPlacementScore++;

                    if (currentMazeCell.AdjacentCellInDirectionIsCorridor(Globals.EAST))
                        roomPlacementScore++;

                    // Add 3 points if the cell overlaps an existing corridor

                    if (currentMazeCell.IsCorridor)
                        roomPlacementScore += 3;

                    // Add 100 points if the cell overlaps any existing room cells
                    foreach (Room otherRoom in Rooms)
                        if (otherRoom.IsContain(currentMazeCell))
                        {
                            roomPlacementScore += 100;
                        }
                }
            }

            // 完全不与任何走廊相连或重合
            if (roomPlacementScore == 0)
            {
                roomPlacementScore = int.MaxValue;
            }

            return roomPlacementScore;
        }
        else
        {
            return int.MaxValue;
        }
    }

    public void PlaceRoom(Cell upper_left_cell, Room room)
    {
        room.SetLocation(upper_left_cell);
        rooms.Add(room);

        // 留下房间边缘的墙
        // 四角
        // 左上
        Cell cell = GetCell(upper_left_cell.y, upper_left_cell.x);
        cell.CreateFloor();
        cell.DestroyWall(Globals.EAST);
        cell.DestroyWall(Globals.SOUTH);
        //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-E-Corner"));
        room.walls.Add(cell);
        if (cell.AdjacentCellInDirectionIsCorridor(Globals.NORTH) || cell.AdjacentCellInDirectionIsCorridor(Globals.WEST))
        {
            room.couldBeDoors.Add(cell);
        }

        // 右上
        cell = GetCell(upper_left_cell.y, upper_left_cell.x + room.X_CELLS_COUNT - 1);
        cell.CreateFloor();
        cell.DestroyWall(Globals.WEST);
        cell.DestroyWall(Globals.SOUTH);
        //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-W-Corner"));
        room.walls.Add(cell);
        if (cell.AdjacentCellInDirectionIsCorridor(Globals.EAST) || cell.AdjacentCellInDirectionIsCorridor(Globals.NORTH))
        {
            room.couldBeDoors.Add(cell);
        }

        // 左下
        cell = GetCell(upper_left_cell.y + room.Y_CELLS_COUNT - 1, upper_left_cell.x);
        cell.CreateFloor();
        cell.DestroyWall(Globals.EAST);
        cell.DestroyWall(Globals.NORTH);
        //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-E-Corner"));
        room.walls.Add(cell);
        if (cell.AdjacentCellInDirectionIsCorridor(Globals.WEST) || cell.AdjacentCellInDirectionIsCorridor(Globals.SOUTH))
        {
            room.couldBeDoors.Add(cell);
        }

        // 右下
        cell = GetCell(upper_left_cell.y + room.Y_CELLS_COUNT - 1, upper_left_cell.x + room.X_CELLS_COUNT - 1);
        cell.CreateFloor();        
        cell.DestroyWall(Globals.WEST);
        cell.DestroyWall(Globals.NORTH);
        //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-W-Corner"));
        room.walls.Add(cell);
        if (cell.AdjacentCellInDirectionIsCorridor(Globals.EAST) || cell.AdjacentCellInDirectionIsCorridor(Globals.SOUTH))
        {
            room.couldBeDoors.Add(cell);
        }

        // 第一行和最后一行，留下南北墙
        for (int x = 1; x < room.X_CELLS_COUNT - 1; x++)
        {
            cell = GetCell(upper_left_cell.y + room.Y_CELLS_COUNT - 1, upper_left_cell.x + x);
            room.walls.Add(cell);
            cell.CreateFloor();
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.EAST);
            cell.DestroyWall(Globals.WEST);
            

            if (cell.AdjacentCellInDirectionIsCorridor(Globals.SOUTH))
            {
                room.couldBeDoors.Add(cell);
            }

            cell = GetCell(upper_left_cell.y, upper_left_cell.x + x);
            room.walls.Add(cell);
            cell.CreateFloor();
            cell.DestroyWall(Globals.SOUTH);
            cell.DestroyWall(Globals.EAST);
            cell.DestroyWall(Globals.WEST);
            //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-E-Corner"));
            //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-W-Corner"));

            if (cell.AdjacentCellInDirectionIsCorridor(Globals.NORTH))
            {
                room.couldBeDoors.Add(cell);
            }
        }
        // 第一列和最后一列，留下东西墙
        for (int y = 1; y < room.Y_CELLS_COUNT - 1; y++)
        {
            cell = GetCell(upper_left_cell.y + y, upper_left_cell.x + room.X_CELLS_COUNT - 1);
            room.walls.Add(cell);
            cell.CreateFloor();
            if (cell.AdjacentCellInDirectionIsCorridor(Globals.EAST))
            {
                room.couldBeDoors.Add(cell);
            }
            cell.DestroyWall(Globals.WEST);
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.SOUTH);
            //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-W-Corner"));
            cell = GetCell(upper_left_cell.y + y, upper_left_cell.x);
            room.walls.Add(cell);
            cell.CreateFloor();
            cell.DestroyWall(Globals.EAST);
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.SOUTH);
            //DestroyObject(Globals.getChildGameObject(cell.gameObject, "S-E-Corner"));
            if (cell.AdjacentCellInDirectionIsCorridor(Globals.WEST))
            {
                room.couldBeDoors.Add(cell);
            }
        }

        // Loop for each cell in the room center
        for (int y = 1; y < room.Y_CELLS_COUNT-1; y++)
        {
            for (int x = 1; x < room.X_CELLS_COUNT-1; x++)
            {
                // Translate the room cell location to its location in the dungeon
                Cell current_room_cell = GetCell(upper_left_cell.y + y, upper_left_cell.x + x);
                current_room_cell.CreateFloor();
                // 拆掉房间中间所有的墙
                current_room_cell.DestroyEverythingExceptFloor();
//                 current_room_cell.DestroyWall(Globals.EAST);
//                 current_room_cell.DestroyWall(Globals.WEST);
//                 current_room_cell.DestroyWall(Globals.NORTH);
//                 current_room_cell.DestroyWall(Globals.SOUTH);
            }
        }

        // 统计出口。注意，四个角如果只剩一面墙，就已经是开门的情况了。其他的要完全没有墙才是门。
        for (int i = 0; i < room.walls.Count; ++i)
        {
            Cell wall_cell = room.walls[i];
            if(i < 4 && wall_cell.WallCount() < 2)
            {
                room.doors.Add(wall_cell);
            }
            else if (wall_cell.WallCount() == 0)
            {
                room.doors.Add(wall_cell);
            }
        }
        // 如果一扇门都没有，随机选择一面墙来开门
        if (room.doors.Count == 0 && room.couldBeDoors.Count != 0)
        {
            Cell door = room.couldBeDoors[UnityEngine.Random.Range(0, room.couldBeDoors.Count)];
            foreach(System.String dir in Globals.DIRECTIONS)
            {
                if (door.HasWallInDirection(dir) && door.AdjacentCellInDirectionIsCorridor(dir))
                {
                    door.DestroyWall(dir);
                    room.doors.Add(door);
                    break;
                }
            }
            
        }



        // 第一行和最后一行，留下南北墙
        for (int x = 0; x < room.X_CELLS_COUNT; x++)
        {
            cell = GetCell(upper_left_cell.y, upper_left_cell.x + x);
            if (!cell.HasWallInDirection(Globals.NORTH))
            {
                room.doorsPositions.Add(cell.GetFloorPos() + new UnityEngine.Vector3(0,Globals.GetCellSideLength()*0.5f,0));
            }            

            cell = GetCell(upper_left_cell.y + room.Y_CELLS_COUNT - 1, upper_left_cell.x + x);
            if (!cell.HasWallInDirection(Globals.SOUTH))
            {
                room.doorsPositions.Add(cell.GetFloorPos() - new UnityEngine.Vector3(0, Globals.GetCellSideLength() * 0.5f, 0));
            }            
        }
        // 第一列和最后一列，留下东西墙
        for (int y = 0; y < room.Y_CELLS_COUNT; y++)
        {
            cell = GetCell(upper_left_cell.y + y, upper_left_cell.x + room.X_CELLS_COUNT - 1);
            if (!cell.HasWallInDirection(Globals.EAST))
            {
                room.doorsPositions.Add(cell.GetFloorPos() + new UnityEngine.Vector3(Globals.GetCellSideLength() * 0.5f, 0,0));
            }            

            cell = GetCell(upper_left_cell.y + y, upper_left_cell.x);
            if (!cell.HasWallInDirection(Globals.WEST))
            {
                room.doorsPositions.Add(cell.GetFloorPos() - new UnityEngine.Vector3(Globals.GetCellSideLength() * 0.5f, 0,0));
            }            
        }

        if(Globals.SHOW_ROOMS)
        {
            room.TurnToWhite();
        }        
    }

    public IEnumerable<Cell> CorridorCellLocations
    {
        get
        {
            for (int x = 0; x < X_CELLS_COUNT; x++)
                for (int y = 0; y < Y_CELLS_COUNT; y++)
                {
                    Cell cell = GetCell(y, x);
                    if (cell.IsCorridor)
                        yield return cell;
                }
        }
    }

    public Cell GetRandomCorridorCell()
    {
        Cell corridor = null;
        while (corridor == null)
        {
            int x = UnityEngine.Random.Range(0, X_CELLS_COUNT);

            // Loop while the current cell is the visited cell

            int y = UnityEngine.Random.Range(0, Y_CELLS_COUNT);

            Cell random_cell = GetCell(y,x);
            if (random_cell.IsCorridor)
            {
                corridor = random_cell;
            }
        }
        return corridor;
    }

    public IEnumerable<Cell> EveryCells
    {
        get
        {
            for (int x = 0; x < X_CELLS_COUNT; x++)
                for (int y = 0; y < Y_CELLS_COUNT; y++)
                {
                    yield return GetCell(y, x);
                }
        }
    }

    void PlaceChests()
    {        
        for (int idx = 0; idx < rooms.Count;++idx)
        {
            Room room = rooms[idx];
            Chest chest = chests[idx];
            Cell cell = null;
            float rotate_angle = 0.0f;

            System.Collections.Generic.List < Cell >  corners = room.GetCornersWithTwoWall();
            int rand_no = UnityEngine.Random.Range(0, corners.Count);
            cell = corners[rand_no];
            if(cell.HasWallInDirection(Globals.EAST))
            {
                rotate_angle = UnityEngine.Random.Range(130.0f, 170.0f);
            }
            else if (cell.HasWallInDirection(Globals.WEST))
            {
                rotate_angle = UnityEngine.Random.Range(0.0f, 20.0f);
            }
            else if (cell.HasWallInDirection(Globals.NORTH))
            {
                rotate_angle = 90.0f;
            }
            chest.PlaceOnCell(cell, rotate_angle);            
        }

        // 为了放灯
        // 先把靠左上最近的箱子排在前面
        if (chests.Count != 0)
        {
            chests.Sort();
            // 然后逆时针排列Chest
            System.Collections.Generic.List<Chest> sortedChests = new System.Collections.Generic.List<Chest>();
            System.Collections.Generic.List<Chest> unsortedChests = new System.Collections.Generic.List<Chest>();
            unsortedChests.AddRange(chests);
            unsortedChests.RemoveAt(0);
            sortedChests.Add(chests[0]);
            chests[0].name = "Chest0";
            while (unsortedChests.Count != 0)
            {
                Chest lastSortedChest = sortedChests[sortedChests.Count - 1];
                Chest clockSequenceChest = null;
                float minAngle = UnityEngine.Mathf.Infinity;
                for (int idx = 0; idx < unsortedChests.Count; ++idx)
                {
                    Chest unsored_chest = unsortedChests[idx];
                    UnityEngine.Vector3 dir = unsored_chest.transform.position - lastSortedChest.transform.position;
                    float angle = Globals.Angle(dir, UnityEngine.Vector3.right);
                    if (angle < minAngle)
                    {
                        clockSequenceChest = unsored_chest;
                        minAngle = angle;
                    }
                }
                clockSequenceChest.name = "Chest" + sortedChests.Count.ToString();
                sortedChests.Add(clockSequenceChest);
                unsortedChests.Remove(clockSequenceChest);
            }
            chests = sortedChests;
        }
        

        // 调试代码，在入口处创建一个宝箱
//         UnityEngine.GameObject test_chest = UnityEngine.GameObject.Instantiate(chest_prefab) as UnityEngine.GameObject;
//         test_chest.transform.position = entryOfMaze.GetFloorPos();
    }

    System.Collections.Generic.List<UnityEngine.Vector3> propertyPoses = new System.Collections.Generic.List<UnityEngine.Vector3>();
    int cashes_to_place;
//     int gemsToPlace;
//     void PlaceGem(Cell cell)
//     {
//         UnityEngine.GameObject gem_prefab = UnityEngine.Resources.Load("Props/purple diamond base") as UnityEngine.GameObject;
//         UnityEngine.GameObject gem = UnityEngine.GameObject.Instantiate(gem_prefab) as UnityEngine.GameObject;
//         float offset = Globals.GetCellSideLength() / 4.0f;
//         UnityEngine.Vector3 gem_pos = cell.GetFloorPos() +
//             new UnityEngine.Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), 0);
//         gem.transform.position = gem_pos;
//         gem.gameObject.name = "Gem" + gemHolders.Count.ToString();
//         gemHolders.Add(gem);
//         propertyPoses.Add(gem.transform.position);
//         --gemsToPlace;
//     }

    void PlaceCash(System.String cash_id, Cell cell)
    {
        PickedItem item = Globals.guardPlayer.OneCashOnFloor(cash_id, cell);
        --cashes_to_place;
        propertyPoses.Add(item.transform.position);
    }


    public void PlaceCashesAtBoarder()
    {
        foreach (Chest chest in chests)
        {
            propertyPoses.Add(chest.transform.position);
        }

        cashes_to_place = Globals.guardPlayer.cashOnFloor.Count-1;
        while (cashes_to_place >= 0)
        {

            // 找到离所有箱子和宝石总距离最远的Cell
            Cell farestCell = null;
            Cell shortestCell = null;
            float maxDis = UnityEngine.Mathf.NegativeInfinity;
            float minDis = UnityEngine.Mathf.Infinity;
            foreach (Cell corrido in CorridorCellLocations)
            {
                float logDis = 0.0f;
                float Dis = 0.0f;
                foreach (UnityEngine.Vector3 pos in propertyPoses)
                {
                    float temp = UnityEngine.Vector3.Distance(pos, corrido.GetFloorPos());
                    logDis += UnityEngine.Mathf.Log(temp);
                    Dis += UnityEngine.Mathf.Sqrt(temp);
                }
                if (logDis > maxDis)
                {
                    maxDis = logDis;
                    farestCell = corrido;
                }
                if (Dis < minDis)
                {
                    minDis = Dis;
                    shortestCell = corrido;
                }
            }

            // 暂时不往中间放宝石。这个算法还有点问题。
            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.0f)
            {
                PlaceCash(Globals.guardPlayer.cashOnFloor[cashes_to_place], farestCell);
            }
            else
            {
                PlaceCash(Globals.guardPlayer.cashOnFloor[cashes_to_place], farestCell);
            }
        }        
    }

//     public void PlaceGemsAtBoarder()
//     {        
//         foreach (Chest chest in chests)
//         {
//             propertyPoses.Add(chest.transform.position);
//         }
// 
//         gemsToPlace = GEMS_COUNT;
//         while (gemsToPlace > 0)
//         {
//             
//             // 找到离所有箱子和宝石总距离最远的Cell
//             Cell farestCell = null;
//             Cell shortestCell = null;
//             float maxDis = UnityEngine.Mathf.NegativeInfinity;
//             float minDis = UnityEngine.Mathf.Infinity;
//             foreach (Cell corrido in CorridorCellLocations)
//             {
//                 float logDis = 0.0f;
//                 float Dis = 0.0f;
//                 foreach (UnityEngine.Vector3 pos in propertyPoses)
//                 {
//                     float temp = UnityEngine.Vector3.Distance(pos, corrido.GetFloorPos());
//                     logDis +=  UnityEngine.Mathf.Log(temp);
//                     Dis += UnityEngine.Mathf.Sqrt(temp);
//                 }
//                 if (logDis > maxDis)
//                 {
//                     maxDis = logDis;
//                     farestCell = corrido;
//                 }
//                 if (Dis < minDis)
//                 {
//                     minDis = Dis;
//                     shortestCell = corrido;
//                 }
//             }
// 
//             // 暂时不往中间放宝石。这个算法还有点问题。
//             if(UnityEngine.Random.Range(0.0f, 1.0f) > 0.0f)
//             {
//                 PlaceGem(farestCell);
//             }
//             else 
//             {
//                 PlaceGem(shortestCell);
//             }            
//         }        
//     }


    public System.Collections.Generic.List<GuardData> guardsToPlace = new System.Collections.Generic.List<GuardData>();
    int chestIdxA = 0;
    public void PlaceRandGuard(GuardData guard)
    {
        guardsToPlace.Add(guard);
        // 灯先不要放路中间
        if (guard.name == "lamp")
        {
            UnityEngine.Vector3 pos = chests[chestIdxA].locate.room.GetRandomRoomPosition();
            Globals.CreateGuard(guard, pathFinder.GetNearestUnwalkableNode(pos));
        }
        else
        {
            Guard g = null;
            UnityEngine.Vector3 pos;
            // 除了灯以外的守卫，其他守卫有一定几率在路中间
            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
            {
                // 放箱子附近
                pos = chests[chestIdxA].locate.GetRandFloorPos();
            }
            else
            {
                // 放路中间。Chests的顺序是按逆时针方向排列的。    
//                 int b = chestIdxA + 1;
//                 b = b % chests.Count;
//                 
//                 Chest chestA = chests[chestIdxA];
//                 Chest chestB = chests[b];
//                 Pathfinding.Path p = Pathfinding.ABPath.Construct(chestA.transform.position, chestB.transform.position, null);
//                 p.callback += OnPathBetweenTwoChest;
//                 AstarPath.StartPath(p);

                // 放在房间门口
                int doors_count = chests[chestIdxA].locate.room.doorsPositions.Count;

                pos = chests[chestIdxA].locate.room.doorsPositions[UnityEngine.Random.Range(0, doors_count)];

                float offset_limit = Globals.GetCellSideLength() * 0.3f;
                pos += new UnityEngine.Vector3(UnityEngine.Random.Range(-offset_limit, offset_limit), UnityEngine.Random.Range(-offset_limit, offset_limit), 0.0f);
            }
            if (guard.name == "Spider")
            {
                g = Globals.CreateGuard(guard, pathFinder.GetNearestUnwalkableNode(pos));
            }
            else
            {
                g = Globals.CreateGuard(guard, pathFinder.GetNearestWalkableNode(pos));
                g.BeginPatrol();
            }                        
        }
        
        ++chestIdxA;
        chestIdxA = chestIdxA % chests.Count; 
    }

    public void OnPathBetweenTwoChest(Pathfinding.Path p)
    {
        GuardData guard = guardsToPlace[0];
        guardsToPlace.RemoveAt(0);
        float pos_ratio = UnityEngine.Random.Range(0.3f, 0.7f);
        UnityEngine.Vector3 midpos = p.vectorPath[(int)(p.vectorPath.Count * pos_ratio)];
        Guard g = Globals.CreateGuard(guard, pathFinder.GetNearestWalkableNode(midpos));
        
        g.BeginPatrol();
        System.String content = g.gameObject.name + " Created";
        Globals.record("testReplay", content);
    }   


    IEnumerator PlaceRooms(float waitTime)
    {
        // 把room尝试放置在每一个Cell，该Cell获得一个Score。最后把Room（左上角）放在得分最低的Cell处
        for (int roomCounter = 0; roomCounter < noOfRoomsToPlace; roomCounter++)
        {
            Room room = CreateRoom(minRoomXCellsCount, maxRoomXCellsCount, minRoomZCellsCount, maxRoomZCellsCount);
            int bestRoomPlacementScore = int.MaxValue;
            Cell bestRoomPlacementCell = null;
//             foreach (Cell currentRoomPlacementCell in CorridorCellLocations)
//             {
//                 int currentRoomPlacementScore = CalculateRoomPlacementScore(currentRoomPlacementCell, room);
// 
//                 if (currentRoomPlacementScore < bestRoomPlacementScore)
//                 {
//                     bestRoomPlacementScore = currentRoomPlacementScore;
//                     bestRoomPlacementCell = currentRoomPlacementCell;
//                 }
//             }

            foreach (Cell currentRoomPlacementCell in Globals.maze.EveryCells)
            {
                int currentRoomPlacementScore = CalculateRoomPlacementScore(currentRoomPlacementCell, room);

                if (currentRoomPlacementScore < bestRoomPlacementScore)
                {
                    bestRoomPlacementScore = currentRoomPlacementScore;
                    bestRoomPlacementCell = currentRoomPlacementCell;
                }
            }


            // Create room at best room placement cell
            if (bestRoomPlacementCell != null)
            {
                PlaceRoom(bestRoomPlacementCell, room);
                if (Globals.SHOW_MACE_GENERATING_PROCESS)
                {
                    yield return new UnityEngine.WaitForSeconds(waitTime);
                }
            }
        }
        MazeProcessOver();       
        yield return new UnityEngine.WaitForSeconds(waitTime);
    }
    UnityEngine.GameObject rayCastPlane;
    void MazeProcessOver()
    {
//         UnityEngine.Sprite up_corner = UnityEngine.Resources.Load<UnityEngine.Sprite>("Props/up_corner");
//         UnityEngine.Sprite buttom_corner = UnityEngine.Resources.Load<UnityEngine.Sprite>("Props/buttom_corner");
//         UnityEngine.Sprite wall_hor_small = UnityEngine.Resources.Load<UnityEngine.Sprite>("Props/wall_hor_small");
//         UnityEngine.Sprite wall_ver_small = UnityEngine.Resources.Load<UnityEngine.Sprite>("Props/wall_ver_small");
        

        UnityEngine.Sprite s_w_corner = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/S_W_Corner");
        UnityEngine.Sprite s_e_corner = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/S_E_Corner");
        
        // 如果没有地板，墙体也不需要显示出来
        foreach (Cell cell in EveryCells)
        {
            if (cell.GetFloor() == null)
            {
                cell.HideEverythingExceptFloor();
            }
//             // 修改Corner
//             UnityEngine.SpriteRenderer N_W_Corner = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "N-W-Corner");
//             UnityEngine.SpriteRenderer N_E_Corner = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "N-E-Corner");
//             UnityEngine.SpriteRenderer S_W_Corner = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "S-W-Corner");
//             UnityEngine.SpriteRenderer S_E_Corner = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "S-E-Corner");
// 
//             UnityEngine.SpriteRenderer N_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "N");
//             UnityEngine.SpriteRenderer W_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "W");
//             UnityEngine.SpriteRenderer E_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "E");
//             UnityEngine.SpriteRenderer S_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(cell.gameObject, "S");
// 
//             if (N_sprite == null)
//             {
//                 if (W_sprite == null)
//                 {
//                     N_W_Corner.sprite = buttom_corner;
//                 }
//                 else
//                 {
//                     N_W_Corner.sprite = wall_ver_small;
//                     N_W_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
//                 }
// 
//                 if (E_sprite == null)
//                 {
//                     N_E_Corner.sprite = buttom_corner;
//                 }
//                 else
//                 {
//                     N_E_Corner.sprite = wall_ver_small;
//                     N_E_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
//                 }
//             }
//             else
//             {
//                 if (W_sprite == null)
//                 {
//                     N_W_Corner.sprite = wall_hor_small;
//                     N_W_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
//                 }
//         
//                 if (E_sprite == null)
//                 {
//                     N_E_Corner.sprite = wall_hor_small;
//                     N_E_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
//                 }
//                 
//             }
// 
//             if (S_sprite == null)
//             {
//                 if (W_sprite == null)
//                 {
//                     S_W_Corner.sprite = buttom_corner;
//                 }
//                 else
//                 {
//                     S_W_Corner.sprite = wall_ver_small;
//                     S_W_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
//                 }
// 
//                 if (E_sprite == null)
//                 {
//                     S_E_Corner.sprite = buttom_corner;
//                 }
//                 else
//                 {
//                     S_E_Corner.sprite = wall_ver_small;
//                     S_E_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
//                 }
//             }
//             else
//             {
//                 if (W_sprite == null)
//                 {
//                     S_W_Corner.sprite = wall_hor_small;
//                     S_W_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, -180);
//                 }
//                 
//                 if (E_sprite == null)
//                 {
//                     S_E_Corner.sprite = wall_hor_small;
//                     S_E_Corner.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, -180);
//                 }                
//             }            

            
        }

        foreach (Cell cell in EveryCells)
        {
            UnityEngine.SpriteRenderer S_W_Corner = cell.S_W_Corner_Sprite;
            UnityEngine.SpriteRenderer S_E_Corner = cell.S_E_Corner_Sprite;

            UnityEngine.SpriteRenderer E_sprite = cell.E_Sprite;
            UnityEngine.SpriteRenderer W_sprite = cell.W_Sprite;

            UnityEngine.Collider N_W_Corner = cell.N_W_Corner_Collider;
            UnityEngine.Collider N_E_Corner = cell.N_E_Corner_Collider;

            Cell S_Cell = cell.GetAdjacentCell(Globals.SOUTH);
            if (S_Cell)
            {
                if (E_sprite != null && E_sprite.enabled && (S_Cell.E_Sprite == null || S_Cell.E_Sprite.enabled == false))
                {
                    S_E_Corner.sprite = s_e_corner;
                    S_E_Corner.enabled = true;
                    S_E_Corner.GetComponent<UnityEngine.Collider>().enabled = true;
                }

                if (W_sprite != null && W_sprite.enabled && (S_Cell.W_Sprite == null || S_Cell.W_Sprite.enabled == false))
                {
                    S_W_Corner.sprite = s_w_corner;
                    S_W_Corner.enabled = true;
                    S_W_Corner.GetComponent<UnityEngine.Collider>().enabled = true;
                }
            }
            else
            {
                if (W_sprite != null && W_sprite.enabled)
                {
                    S_W_Corner.sprite = s_w_corner;
                    S_W_Corner.enabled = true;
                    S_W_Corner.GetComponent<UnityEngine.Collider>().enabled = true;
                }

                if (E_sprite != null && E_sprite.enabled)
                {
                    S_E_Corner.sprite = s_e_corner;
                    S_E_Corner.enabled = true;
                    S_E_Corner.GetComponent<UnityEngine.Collider>().enabled = true;
                }
            }

            if (S_E_Corner != null && !S_E_Corner.enabled)
            {
                S_E_Corner.GetComponent<UnityEngine.Collider>().enabled = false;
            }

            if (S_W_Corner != null && !S_W_Corner.enabled)
            {
                S_W_Corner.GetComponent<UnityEngine.Collider>().enabled = false;
            }

            Cell N_Cell = cell.GetAdjacentCell(Globals.NORTH);
            
            if (N_Cell)
            {
                UnityEngine.SpriteRenderer north_cell_E_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(N_Cell.gameObject, "E");
                UnityEngine.SpriteRenderer north_cell_W_sprite = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(N_Cell.gameObject, "W");

                if (E_sprite != null && E_sprite.enabled)
                {
                    N_E_Corner.enabled = true;
                }

                if (W_sprite != null && W_sprite.enabled)
                {
                    N_W_Corner.enabled = true;
                }

                if (north_cell_E_sprite == null || north_cell_E_sprite.enabled == false)
                {
                    N_E_Corner.GetComponent<UnityEngine.BoxCollider>().center = new UnityEngine.Vector3(0, -0.87f, 0);
                }

                if (north_cell_W_sprite == null || north_cell_W_sprite.enabled == false)
                {
                    N_W_Corner.GetComponent<UnityEngine.BoxCollider>().center = new UnityEngine.Vector3(0, -0.87f, 0);
                }
            }
            else
            {
                if (W_sprite != null && W_sprite.enabled)
                {
                    N_W_Corner.enabled = true;
                }

                if (E_sprite != null && E_sprite.enabled)
                {
                    N_E_Corner.enabled = true;
                }

            }

            if (S_E_Corner != null && S_E_Corner.enabled && (E_sprite == null || E_sprite.enabled == false))
            {                
                //S_E_Corner.GetComponent<UnityEngine.BoxCollider>().center = new UnityEngine.Vector3(0, 0.7f, 0);
            }

            if (S_W_Corner != null && S_W_Corner.enabled && (W_sprite == null || W_sprite.enabled == false))
            {
                //S_W_Corner.GetComponent<UnityEngine.BoxCollider>().center = new UnityEngine.Vector3(0, 0.65f, 0);
            }

            // 阴影
            if (N_W_Corner)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(N_W_Corner.gameObject, "wall_shadow");
                shadow.enabled = N_W_Corner.enabled;
            }
            if (N_E_Corner)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(N_E_Corner.gameObject, "wall_shadow");
                shadow.enabled = N_E_Corner.enabled;
            }
            
            if(W_sprite && W_sprite.gameObject.activeSelf)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(W_sprite.gameObject,"wall_shadow");
                shadow.enabled = W_sprite.enabled;
            }

            if (S_W_Corner && S_W_Corner.gameObject.activeSelf)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(S_W_Corner.gameObject, "wall_shadow");
                shadow.enabled = S_W_Corner.enabled;
            }

            if (E_sprite && E_sprite.gameObject.activeSelf)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(E_sprite.gameObject, "wall_shadow");
                shadow.enabled = E_sprite.enabled;
            }

            if (S_E_Corner && S_E_Corner.gameObject.activeSelf)
            {
                UnityEngine.SpriteRenderer shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(S_E_Corner.gameObject, "wall_shadow");
                shadow.enabled = S_E_Corner.enabled;
            }
        }
        
        // 创造一个入口，找到地图最靠近东南角的走廊作为入口
        foreach (Cell corrido in CorridorCellLocations)
        {
            allCorridorsAfterMazeCompleted.Add(corrido);
        }
        entryOfMaze = allCorridorsAfterMazeCompleted[allCorridorsAfterMazeCompleted.Count - 1];

        
        // 要等一下生成路径，不然会出问题。
        Globals.LevelController.SleepThenCallFunction(30, () => finished());        
    }
    public bool isGenerateFinished = false;
    void finished()
    {
        pathFinder.GenerateGridGraph();
        // 宝箱
        PlaceChests();
        
        rayCastPlane = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
        rayCastPlane.transform.localPosition = new UnityEngine.Vector3(0, Globals.FLOOR_HEIGHT, 0);
        rayCastPlane.transform.localScale = new UnityEngine.Vector3(10000, 10000, 0.2f);
        rayCastPlane.GetComponent<UnityEngine.Renderer>().enabled = false;
        rayCastPlane.layer = 9;

        // pve关卡
        Globals.LevelController.MazeFinished();
        isGenerateFinished = true;
    }

    public void SetRestrictToCamera(MagicThiefCamera camera)
    {
        float half_cell_length = Globals.GetCellSideLength() / 2.0f;
        camera.restriction_x = new UnityEngine.Vector2(WestPosInPixel() + half_cell_length, EastPosInPixel() - half_cell_length);
        camera.restriction_y = new UnityEngine.Vector2(SouthPosInPixel(), NorthPosInPixel());        
    }

    public Pathfinding.Node GetNodeFromScreenRay(UnityEngine.Vector3 screenPos)
    {
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 9;
        UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(screenPos);
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {
            return pathFinder.GetSingleNode(hitInfo.point, false);
        }
        return null;
    }
    

    public void RegistGuardArrangeEvent()
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down += OnDragFingerDown;
            finger.Evt_Moving += OnDragFingerMoving;
            finger.Evt_Up += OnDragFingerUp;
        }
    }

    public void UnRegisterGuardArrangeEvent()
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down -= OnDragFingerDown;
            finger.Evt_Moving -= OnDragFingerMoving;
            finger.Evt_Up -= OnDragFingerUp;
        }        
    }
    
    public void RegistChallengerEvent()
    {                
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down += OnChallengerFingerDown;
            finger.Evt_Moving += OnChallengerFingerMoving;
            finger.Evt_Up += OnChallengerFingerUp;

            Globals.input.Evt_MouseRightDown += OnChallengerRightBtnDown;
            Globals.input.Evt_MouseRightUp += OnChallengerRightBtnUp;
        }
    }

    public void UnRegistChallengerEvent()
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down -= OnChallengerFingerDown;
            finger.Evt_Moving -= OnChallengerFingerMoving;
            finger.Evt_Up -= OnChallengerFingerUp;

            Globals.input.Evt_MouseRightDown -= OnChallengerRightBtnDown;
            Globals.input.Evt_MouseRightUp -= OnChallengerRightBtnUp;
        }
    }

    public void OnDestroy()
    {
        UnRegistChallengerEvent();
        UnRegisterGuardArrangeEvent();
    }

    Finger fingerDownOnMap;

    public bool OnChallengerFingerDown(object sender)
    {
        Finger finger = sender as Finger;
        if (Globals.cameraFollowMagician.CheckFingerDownOnMiniMap(finger))
        {
            return false;
        }
        fingerDownOnMap = finger;        
        return true;
    }
    
    public bool OnChallengerFingerMoving(object sender)
    {
        if (fingerDownOnMap != null && !Globals.canvasForMagician.draggingFlashGrenade)
        {
            Globals.cameraFollowMagician.DragToMove(fingerDownOnMap);            
        }
        else
        {
            Globals.cameraFollowMagician.DragOnMiniMap(sender as Finger);
        }
        return true;
    }

    public bool OnChallengerFingerUp(object sender)
    {
        Finger finger = sender as Finger;
        if (Globals.cameraFollowMagician.CheckFingerUpOnMiniMap(finger))
        {
            return false;
        }
        if (finger == fingerDownOnMap)
        {            
            // 点击地板
            if (fingerDownOnMap.timeSinceTouchBegin < 0.5f &&
                UnityEngine.Vector2.Distance(fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition) < 10.0f)
            {
                Globals.LevelController.ClickOnMap(fingerDownOnMap.nowPosition);
            }                     
        }

        fingerDownOnMap = null;
        return true;
    }

    float timeSinceRightDown = -1.0f;
    public bool OnChallengerRightBtnDown(UnityEngine.Vector2 pos)
    {
        timeSinceRightDown = UnityEngine.Time.time;
        return true;
    }

    public bool OnChallengerRightBtnUp(UnityEngine.Vector2 pos)
    {
        if (timeSinceRightDown > 0)
        {
            // 点击地板
            if (UnityEngine.Time.time - timeSinceRightDown < 0.5f)
            {
                Globals.LevelController.RightClickOnMap(pos);
            }
        }

        timeSinceRightDown = -1.0f;
        return true;
    }
// 
//     System.Collections.Generic.List<UnityEngine.GameObject> tempGems = new System.Collections.Generic.List<UnityEngine.GameObject>();
//     System.Collections.Generic.List<Pathfinding.Path> tempPathes = new System.Collections.Generic.List<Pathfinding.Path>();
//     System.Collections.Generic.List<UnityEngine.GameObject> gemsSequence = new System.Collections.Generic.List<UnityEngine.GameObject>();
//     System.Collections.Generic.List<UnityEngine.GameObject> sequenceNumbers = new System.Collections.Generic.List<UnityEngine.GameObject>();
//     void FindNearestGem(UnityEngine.Vector3 pathBeginPos)
//     {
//         for (ushort idx = 0; idx < tempGems.Count; ++idx)
//         {
//             UnityEngine.GameObject gem = tempGems[idx];
//             Pathfinding.Path p = Pathfinding.ABPath.Construct(pathBeginPos, gem.transform.position, null);
//             p.callback += OnPathToGemComplete;
//             p.gem = gem;
//             AstarPath.StartPath(p);
//         }
//     }
// 
//     void OnPathToGemComplete(Pathfinding.Path p)
//     {
//         tempPathes.Add(p);
//         if (tempPathes.Count == tempGems.Count)
//         {
//             float shortest = UnityEngine.Mathf.Infinity;
//             UnityEngine.GameObject nearestGem = null;
//             for(int idx = 0; idx < tempPathes.Count; ++idx)
//             {
//                 Pathfinding.Path path = tempPathes[idx];
//                 double length = path.GetTotalLength();
//                 if (length < shortest)
//                 {
//                     shortest = (float)length;
//                     nearestGem = path.gem;
//                 }
//             }
//             tempGems.Remove(nearestGem);
//             gemsSequence.Add(nearestGem);
//             // 路径以全部生成，标记出数字
//             if (gemsSequence.Count == gemHolders.Count)
//             {
//                 for (int idx = 0; idx < gemsSequence.Count; ++idx)
//                 {
//                     UnityEngine.GameObject gem = gemsSequence[idx];
//                     UnityEngine.GameObject number_prefab = UnityEngine.Resources.Load("UI/TargetNumber") as UnityEngine.GameObject;
//                     UnityEngine.GameObject number = UnityEngine.GameObject.Instantiate(number_prefab) as UnityEngine.GameObject;
//                     number.transform.position = gem.transform.position + new UnityEngine.Vector3(0,1,0);
//                     number.GetComponentInChildren<UnityEngine.UI.Text>().text = idx.ToString();
//                     sequenceNumbers.Add(number);
//                 }
//             }
//             tempPathes.Clear();
//             FindNearestGem(nearestGem.transform.position);
//         }
//     }

    public Guard draggingGuard;
    public Chest choosenChest;
    public bool OnDragFingerDown(object sender)
    {        
        if (choosenChest != null)
        {
            choosenChest.HideBtn();
        }

        fingerDownOnMap = sender as Finger;
        if (Globals.cameraFollowMagician.CheckFingerDownOnMiniMap(fingerDownOnMap))
        {
            return false;
        }
        // guard fov , guard, dog fov
        //int mask = 1 << 10 | 1 << 13 |1 << 27 | ;
        // HeadOnMiniMap
        //int mask = 1 << 28;

        int mask = 1 << 13;
        Guard guard = Globals.FingerRayToObj<Guard>(
            Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, fingerDownOnMap.nowPosition);
        if (guard != null && (guard.currentAction == null || guard.currentAction == guard.patrol))
        {            
            // 如果上一个守卫在墙面上，那么不能拖拽新的
            if (guard != choosenGuard && 
                choosenGuard != null && 
                !choosenGuard.birthNode.walkable)
            {
                return false;
            }
            _DragGuard(guard);
        }
        else if (choosenGuard == null)
        {
            mask = 1 << 14;
            PickedItem picked = Globals.FingerRayToObj<PickedItem>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, fingerDownOnMap.nowPosition);
            if (picked != null)
            {
                if (!Globals.self.IsMoneyFull())
                {
                    if (picked.GetCash() > 0)
                    {
                        Globals.canvasForMagician.ChangeCash(picked.GetCash());
                        Globals.self.RemoveCashOnFloor(picked.item_id);
                        Globals.canvasForMyMaze.UpdateIncomeIntro();
                    }
                    else
                    {
                        Globals.self.AddTrickItem(Globals.self.GetTrickByName(picked.gameObject.name));
                        Globals.self.RemoveDroppedItem(picked.item_id);
                    }
                    picked.Picked();
                }
                else
                {
                    Globals.tipDisplay.Msg("money_full");
                }                           
            }
            else
            {
                Chest chest = Globals.FingerRayToObj<Chest>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, fingerDownOnMap.nowPosition);
                if (chest != null)
                {
                    chest.ShowUpgradeBtn();
                    choosenChest = chest;
                }      
            }
        }
        
        return true;
    }

    public void _DragGuard(Guard guard)
    {
        draggingGuard = guard;        
        if (guard != choosenGuard)
        {
            if (choosenGuard != null)
            {
                choosenGuard.Unchoose();
            }
            guard.Choosen();
        }
        guard.HideBtns();
    }

    public bool OnDragFingerMoving(object sender)
    {        
        if (draggingGuard != null)
        {            
            UnityEngine.RaycastHit hitInfo;
            int layermask = 1 << 9;
            UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(fingerDownOnMap.nowPosition);
            if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
            {
                Pathfinding.Node node = pathFinder.GetSingleNode(hitInfo.point, false);
                Globals.Assert(node != null);
                choosenGuard.birthNode = node;
                draggingGuard.transform.position = new UnityEngine.Vector3(node.position.x / 1000.0f, node.position.y / 1000.0f, node.position.z / 1000.0f-0.6f);
                if (node.walkable == draggingGuard.walkable)
                {                    
                    if (draggingGuard.patrol != null)
                    {
                        draggingGuard.patrol.InitPatrolRoute();
                    }                    
                }                
            }
        }
        else
        {
			Globals.cameraFollowMagician.DragToMove(fingerDownOnMap);
            Globals.cameraFollowMagician.DragOnMiniMap(fingerDownOnMap);
        }
        
        return true;
    }    

    public bool OnDragFingerUp(object sender)
    {
        if (Globals.cameraFollowMagician.CheckFingerUpOnMiniMap(fingerDownOnMap))
        {
            return false;
        }
//		UnityEngine.Debug.Log("OnDragFingerUp:" + fingerDownOnMap.timeSinceTouchBegin.ToString("f4")+"," 
//		                      + UnityEngine.Vector2.Distance (fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition).ToString("f4"));
        if (draggingGuard != null)
        {
            draggingGuard.ShowArrangeBtns();            
            draggingGuard = null;
        }
        // 判断点击
        else if (fingerDownOnMap.timeSinceTouchBegin < 0.5f &&             
            UnityEngine.Vector2.Distance(fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition) < 10.0f)
        {            
            // 点击空地
            if (choosenGuard != null)
            {
                int mask = 1 << 10 | 1 << 27;
                Guard guard = Globals.FingerRayToObj<Guard>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, fingerDownOnMap.nowPosition);
                if (guard != choosenGuard)
                {                    
                    choosenGuard.ConfirmBtnClicked();    
                }                
            }            
        }
        return true;
    }    

    public List<Cell> allCorridorsAfterMazeCompleted = new List<Cell>();
    public Cell entryOfMaze;

    void Awake()
    {
        Globals.maze = this;                
        pathFinder = GetComponent<PathFinder>();        
    }

    UnityEngine.GameObject chest_prefab;
    public PlayerInfo owner_of_maze;
    
    // Use this for initialization
    public void Start()
    {
        if (!Globals.socket.IsFromLogin() && !Globals.socket.IsReady())
        {
            return;
        }

        Globals.LevelController.BeforeGenerateMaze();
     
        owner_of_maze = null;
        if (Globals.guardPlayer != null)
        {
            owner_of_maze = Globals.guardPlayer;
        }
        else if(Globals.visitPlayer != null)
        {
            owner_of_maze = Globals.visitPlayer;
        }
        else
        {
            owner_of_maze = Globals.self;
        }                

        if (owner_of_maze.isBot)
        {
            pieces_dir = "Props/Maze-Pieces/pve";
        }
        else
        {
            if (Globals.self.TutorialLevelIdx != PlayerInfo.TutorialLevel.Over)
            {
                pieces_dir = "Props/Maze-Pieces/1";
            }
            else
            {
                int maze_lv = owner_of_maze.currentMazeLevel;
                if (maze_lv > 5)
                {
                    maze_lv = 5;
                }
                pieces_dir = "Props/Maze-Pieces/" + maze_lv.ToString();
            }
        }
                
        cells = new Cell[Y_CELLS_COUNT, X_CELLS_COUNT];
        bounds = new UnityEngine.Rect(0, 0, X_CELLS_COUNT, Y_CELLS_COUNT);

        int count = noOfRoomsToPlace - chests.Count;
        if (count>0)
        {
            chest_prefab = UnityEngine.Resources.Load("Props/Chest") as UnityEngine.GameObject;            
            for (int idx = 0; idx < count; ++idx)
            {
                Chest chest = (UnityEngine.GameObject.Instantiate(chest_prefab) as UnityEngine.GameObject).GetComponent<Chest>();
                chest.Visible(false);
            }
        }               

        // 所有Cell的父节点
        maze = new UnityEngine.GameObject("Maze");

        // 左上角
        left_up_corner_pos = Globals.CalcMazeLeftUpCornerPos(X_CELLS_COUNT, Y_CELLS_COUNT);

//         center_pos = new UnityEngine.Vector3(-(X_CELLS_COUNT * cell_side_length) / 2.0f - cell_side_length / 2.0f,
//            (Y_CELLS_COUNT * cell_side_length) / 2.0f - cell_side_length / 2.0f, 0);        
        Cell cell_script_in_prefab = Globals.cell_prefab.GetComponent<Cell>();
        cell_script_in_prefab.Floor_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/Floor");
        cell_script_in_prefab.E_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/Wall_04");
        cell_script_in_prefab.W_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/Wall_04");
        cell_script_in_prefab.N_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/Wall_01");
        cell_script_in_prefab.S_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/Wall_01");
        cell_script_in_prefab.S_E_Corner_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/S_E_Corner");
        cell_script_in_prefab.S_W_Corner_Sprite.sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>(pieces_dir + "/S_W_Corner");
        // 从左上开始，一一构造cells
        for (int y = 0; y < Y_CELLS_COUNT; ++y)
        {
            for (int x = 0; x < X_CELLS_COUNT; ++x)
            {
                UnityEngine.Vector3 cell_pos = new UnityEngine.Vector3(
                    left_up_corner_pos.x + x * Globals.GetCellSideLength(),
                  left_up_corner_pos.y - y * Globals.GetCellSideLength(), 
                  0);
                UnityEngine.GameObject cell_gameobj = UnityEngine.GameObject.Instantiate(Globals.cell_prefab) as UnityEngine.GameObject;
                Cell cell = cell_gameobj.GetComponent<Cell>();
                cell_gameobj.transform.parent = maze.transform;
                cell_gameobj.transform.position = cell_pos;
                cells[y, x] = cell;
                cell.name = y.ToString() + "," + x.ToString();
                cell.maze = this;
                cell.y = y;
                cell.x = x;
            }
        }

        // 四个角落
        EastNorthCornerCell = GetCell(0, X_CELLS_COUNT - 1);
        EastSouthCornerCell = GetCell(Y_CELLS_COUNT - 1, X_CELLS_COUNT - 1);
        WestSouthCornerCell = GetCell(Y_CELLS_COUNT - 1, 0);
        WestNorthCornerCell = GetCell(0, 0);

        center_pos = (EastSouthCornerCell.transform.position + WestNorthCornerCell.transform.position)*0.5f;

        StartCoroutine(CreateDenseMaze(Globals.CREATE_MAZE_TIME_STEP));
    }

    public void ClearMaze()
    {
        foreach(Chest chest in chests)
        {
            if (chest != null)
            {
                DestroyObject(chest.gameObject);
            }            
        }
        chests.Clear();
        foreach(UnityEngine.GameObject holder in gemHolders)
        {
            DestroyObject(holder);
        }
        gemHolders.Clear();
        ClearGuards();
        DestroyObject(maze);
        DestroyObject(rayCastPlane);        
        allCorridorsAfterMazeCompleted.Clear();
        rooms.Clear();
        visitedCells.Clear();
        UnRegistChallengerEvent();
    }

    public void ClearGuards()
    {
        foreach (Guard guard in guards)
        {
            DestroyObject(guard.gameObject);
        }
        guards.Clear();
    }

    void Update()
    {
        
    }

    public void GuardsTargetVanish(UnityEngine.GameObject obj)
    {
        foreach (Guard guard in guards)
        {            
            if (guard.spot != null && guard.spot.target == obj.transform)
            {                
                guard.RemoveAction(ref guard.spot.outVisionCountDown);
                guard.spot.target = null;
                if (guard.currentAction == guard.chase || (guard.explode != null && guard.currentAction != guard.explode))
                {
                    guard.wandering.Excute();
                }                
            }
            if (guard.eye != null)
            {
                guard.eye.enemiesInEye.Remove(obj);
            }
            MachineActiveArea area = guard.GetComponentInChildren<MachineActiveArea>();
            if (area)
            {
                area.actorsInTouch.Remove(obj.GetComponent<Actor>());
            }
        }
    }

    public bool IsAnyGuardSpotMagician()
    {
        foreach (Guard guard in guards)
        {            
            if (guard.eye != null)
            {
                foreach(UnityEngine.GameObject spottedEnemy in guard.eye.enemiesInEye)
                {
                    if (spottedEnemy == Globals.stealingController.magician.gameObject)
                    {
                        return true;
                    }
                }
            }            
        }
        return false;
    }

    public UnityEngine.Vector3 GetPickedItemBasedOnRandomPos(System.String[] item_data)
    {
        float rand_x = System.Convert.ToSingle(item_data[0]);
        float rand_y = System.Convert.ToSingle(item_data[1]);

        return Globals.GetPathNodePos(pathFinder.GetNearestWalkableNode(new UnityEngine.Vector3(left_up_corner_pos.x + Globals.GetCellSideLength() * X_CELLS_COUNT * rand_x,
                left_up_corner_pos.y - Globals.GetCellSideLength() * Y_CELLS_COUNT * rand_y,
                0)));            
    }

    public UnityEngine.Vector3 GetRightUpPos()
    {
        return Globals.GetPathNodePos(pathFinder.GetNearestWalkableNode(new UnityEngine.Vector3(-left_up_corner_pos.x, left_up_corner_pos.y,0)));
    }

    public UnityEngine.Vector3 GetLeftBottomPos()
    {
        return Globals.GetPathNodePos(pathFinder.GetNearestWalkableNode(new UnityEngine.Vector3(left_up_corner_pos.x, -left_up_corner_pos.y, 0)));
    }
}
