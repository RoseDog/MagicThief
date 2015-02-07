﻿using System;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerate : UnityEngine.MonoBehaviour
{
    int cell_side_length = 4;
    public int GetCellSideLength()
    {
        return cell_side_length;
    }
    [UnityEngine.HideInInspector]
    public int randSeedCacheWhenEditLevel;    

    [UnityEngine.Range(0, 30)]
    public int Z_CELLS_COUNT;
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
    

    UnityEngine.GameObject maze;

    public UnityEngine.Vector3 left_up_corner_pos;
    Cell EastNorthCornerCell;
    Cell EastSouthCornerCell;
    Cell WestSouthCornerCell;
    Cell WestNorthCornerCell;

    public PathFinder pathFinder;
    public Guard choosenGuard;
    public System.Collections.Generic.List<Guard> guards = new System.Collections.Generic.List<Guard>();
    public System.Collections.Generic.List<Chest> chests = new System.Collections.Generic.List<Chest>();
    public System.Collections.Generic.List<UnityEngine.GameObject> gemHolders = new System.Collections.Generic.List<UnityEngine.GameObject>();
    
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
            UnityEngine.Rect r = new UnityEngine.Rect(WestNorthCornerCell.GetFloorPos().x - cell_side_length * 0.5f,
                WestNorthCornerCell.GetFloorPos().z + cell_side_length*0.5f,
                EastNorthCornerCell.GetFloorPos().x - WestNorthCornerCell.GetFloorPos().x + cell_side_length,
                WestNorthCornerCell.GetFloorPos().z - WestSouthCornerCell.GetFloorPos().z + cell_side_length);
            return r; 
        }
    }

    public float EastPosInPixel()
    {
        return EastNorthCornerCell.transform.position.x + cell_side_length*0.5f;
    }

    public float SouthPosInPixel()
    {
        return EastSouthCornerCell.transform.position.z - cell_side_length * 0.5f;
    }

    public float WestPosInPixel()
    {
        return WestNorthCornerCell.transform.position.x - cell_side_length * 0.5f;
    }

    public float NorthPosInPixel()
    {
        return WestNorthCornerCell.transform.position.z + cell_side_length * 0.5f;
    }

    private Cell[,] cells;
    public void MarkCellsUnvisited()
    {
        for (int x = 0; x < X_CELLS_COUNT; x++)
            for (int y = 0; y < Z_CELLS_COUNT; y++)
                cells[x, y].Visited = false;
    }

    public Cell GetCell(int z, int x)
    {
        return cells[z, x];
    }

    public Cell GetCellByPos(UnityEngine.Vector3 pos)
    {
        int z = UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Abs(left_up_corner_pos.z - pos.z) / cell_side_length);
        int x = UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Abs(left_up_corner_pos.x - pos.x) / cell_side_length);
        return GetCell(z, x);
    }

    public Cell PickRandomCellAndMarkItVisited()
    {
        int rand_x = UnityEngine.Random.Range(0, X_CELLS_COUNT - 1);
        int rand_z = UnityEngine.Random.Range(0, Z_CELLS_COUNT - 1);
        Cell cell = GetCell(rand_z, rand_x);
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
            throw new ArgumentException("Location is already visited", cell.z.ToString() + "," +  cell.x.ToString());

        cell.Visited = true;
        visitedCells.Add(cell);
    }

    public bool LocationIsOutsideBounds(Cell cell)
    {
        return ((cell.x < 0) || (cell.x >= X_CELLS_COUNT) || (cell.z < 0) || (cell.z >= Z_CELLS_COUNT));
    }

    public bool AllCellsVisited
    {
        get { return visitedCells.Count == (X_CELLS_COUNT * Z_CELLS_COUNT); }
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
            for (int z = 0; z < Z_CELLS_COUNT; z++)
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
            for (int z = 0; z < Z_CELLS_COUNT; z++)
                for (int x = 0; x < X_CELLS_COUNT; x++)
                {
                    Cell cell = GetCell(z, x);
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
        public int Z_CELLS_COUNT;
        public int X_CELLS_COUNT;        
        public Room(int XCount, int ZCount)
        {
            X_CELLS_COUNT = XCount;
            Z_CELLS_COUNT = ZCount;
        }

        public void SetLocation(Cell location)
        {
            upper_left = location;            
        }

        public bool IsContain(Cell cell)
        {
            if (cell.z < upper_left.z || cell.z > upper_left.z + Z_CELLS_COUNT)
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
                    (cell.x == upper_left.x + X_CELLS_COUNT-1 && cell.z == upper_left.z + Z_CELLS_COUNT-1) ||
                    (cell.x == upper_left.x && cell.z == upper_left.z + Z_CELLS_COUNT-1)||
                    (cell.x == upper_left.x + X_CELLS_COUNT-1 && cell.z == upper_left.z))
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
                for (int z = 0; z < Z_CELLS_COUNT; ++z)
                {
                    Cell cell = Globals.maze.GetCell(upper_left.z + z, upper_left.x + x);
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
                for (int z = 0; z < Z_CELLS_COUNT; ++z)
                {
                    Cell cell = Globals.maze.GetCell(upper_left.z + z, upper_left.x + x);
                    cell.FloorTurnToWhite();
                }
            }
        }

        public UnityEngine.Vector3 GetRandomRoomPosition()
        {
            Cell bottom_right = Globals.maze.GetCell(upper_left.z + Z_CELLS_COUNT-1, upper_left.x + X_CELLS_COUNT-1);
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
        int noOfDeadEndCellsToRemove = (int)Math.Ceiling((decimal)sparsenessModifier / 100 * (X_CELLS_COUNT * Z_CELLS_COUNT));
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
        return new Room(UnityEngine.Random.Range(minXCellsCount, maxXCellsCount), UnityEngine.Random.Range(minZCellsCount, maxZCellsCount));
    }

    public int CalculateRoomPlacementScore(Cell upper_left_cell, Room room)
    {
        // Check if the room at the given location will fit inside the bounds of the map
        // 如果左上角放在这里，整个Room会溢出地图外，那就肯定不能放在这里
        UnityEngine.Rect room_rect = new UnityEngine.Rect(upper_left_cell.x, upper_left_cell.z, room.X_CELLS_COUNT-1, room.Z_CELLS_COUNT-1);
        if (Bounds.Contains(room_rect.min) && Bounds.Contains(room_rect.max))
        {
            int roomPlacementScore = 0;
            // Loop for each cell in the room            
            for (int z = 0; z < room.Z_CELLS_COUNT; z++)
            {
                for (int x = 0; x < room.X_CELLS_COUNT; x++)
                {
                    // location in the dungeon(Cell of Maze)
                    // 假设这个Cell会成为房间的Cell，计算Score的增长
                    Cell currentMazeCell = GetCell(upper_left_cell.z + z, upper_left_cell.x + x);

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
        Cell cell = GetCell(upper_left_cell.z, upper_left_cell.x);
        cell.CreateFloor();
        //cell.SealCorridor(Globals.NORTH);
        //cell.SealCorridor(Globals.WEST);
        cell.DestroyWall(Globals.EAST);
        cell.DestroyWall(Globals.SOUTH);

        // 右上
        cell = GetCell(upper_left_cell.z, upper_left_cell.x + room.X_CELLS_COUNT - 1);
        cell.CreateFloor();
        //cell.SealCorridor(Globals.NORTH);
        //cell.SealCorridor(Globals.EAST);
        cell.DestroyWall(Globals.WEST);
        cell.DestroyWall(Globals.SOUTH);

        // 左下
        cell = GetCell(upper_left_cell.z + room.Z_CELLS_COUNT - 1, upper_left_cell.x);
        cell.CreateFloor();
        //cell.SealCorridor(Globals.SOUTH);
        //cell.SealCorridor(Globals.WEST);
        cell.DestroyWall(Globals.EAST);
        cell.DestroyWall(Globals.NORTH);

        // 右下
        cell = GetCell(upper_left_cell.z + room.Z_CELLS_COUNT - 1, upper_left_cell.x + room.X_CELLS_COUNT - 1);
        cell.CreateFloor();
        //cell.SealCorridor(Globals.SOUTH);
        //cell.SealCorridor(Globals.EAST);
        cell.DestroyWall(Globals.WEST);
        cell.DestroyWall(Globals.NORTH);

        // 第一行和最后一行，留下南北墙
        for (int x = 1; x < room.X_CELLS_COUNT - 1; x++)
        {
            cell = GetCell(upper_left_cell.z + room.Z_CELLS_COUNT - 1, upper_left_cell.x + x);
            cell.CreateFloor();
            //cell.SealCorridor(Globals.SOUTH);
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.EAST);
            cell.DestroyWall(Globals.WEST);
            cell = GetCell(upper_left_cell.z, upper_left_cell.x + x);
            cell.CreateFloor();
            //cell.SealCorridor(Globals.NORTH);
            cell.DestroyWall(Globals.SOUTH);
            cell.DestroyWall(Globals.EAST);
            cell.DestroyWall(Globals.WEST);
        }
        // 第一列和最后一列，留下东西墙
        for (int z = 1; z < room.Z_CELLS_COUNT - 1; z++)
        {
            cell = GetCell(upper_left_cell.z + z, upper_left_cell.x + room.X_CELLS_COUNT - 1);
            cell.CreateFloor();
            //cell.SealCorridor(Globals.EAST);
            cell.DestroyWall(Globals.WEST);
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.SOUTH);
            cell = GetCell(upper_left_cell.z + z, upper_left_cell.x);
            cell.CreateFloor();
            cell.DestroyWall(Globals.EAST);
            //cell.SealCorridor(Globals.WEST);
            cell.DestroyWall(Globals.NORTH);
            cell.DestroyWall(Globals.SOUTH);
        }

        // Loop for each cell in the room center
        for (int z = 1; z < room.Z_CELLS_COUNT-1; z++)
        {
            for (int x = 1; x < room.X_CELLS_COUNT-1; x++)
            {
                // Translate the room cell location to its location in the dungeon
                Cell current_room_cell = GetCell(upper_left_cell.z + z, upper_left_cell.x + x);
                current_room_cell.CreateFloor();
                // 拆掉房间中间所有的墙
                current_room_cell.DestroyWall(Globals.EAST);
                current_room_cell.DestroyWall(Globals.WEST);
                current_room_cell.DestroyWall(Globals.NORTH);
                current_room_cell.DestroyWall(Globals.SOUTH);
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
                for (int z = 0; z < Z_CELLS_COUNT; z++)
                {
                    Cell cell = GetCell(z, x);
                    if (cell.IsCorridor)
                        yield return cell;
                }
        }
    }

    public IEnumerable<Cell> EveryCells
    {
        get
        {
            for (int x = 0; x < X_CELLS_COUNT; x++)
                for (int z = 0; z < Z_CELLS_COUNT; z++)
                {
                    yield return GetCell(z, x);
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
                    float angle = UnityEngine.Vector3.Angle(dir.normalized, UnityEngine.Vector3.left);
                    int sign = UnityEngine.Vector3.Cross(dir, UnityEngine.Vector3.left).z < 0 ? -1 : 1;
                    if (sign == -1)
                    {
                        angle = 360 - angle;
                    }
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
    int gemsToPlace;
    void PlaceGem(Cell cell)
    {
        UnityEngine.GameObject gem_prefab = UnityEngine.Resources.Load("Props/purple diamond base") as UnityEngine.GameObject;
        UnityEngine.GameObject gem = UnityEngine.GameObject.Instantiate(gem_prefab) as UnityEngine.GameObject;
        UnityEngine.Vector3 gem_pos = cell.GetFloorPos() +
            new UnityEngine.Vector3(UnityEngine.Random.Range(-cell_side_length / 3.0f, cell_side_length / 3.0f), 0.9f, UnityEngine.Random.Range(-cell_side_length / 3.0f, cell_side_length / 3.0f));
        gem.transform.position = gem_pos;
        gem.gameObject.name = "Gem" + gemHolders.Count.ToString();
        gemHolders.Add(gem);
        propertyPoses.Add(gem.transform.position);
        --gemsToPlace;
    }

    public void PlaceGemsAtBoarder()
    {        
        foreach (Chest chest in chests)
        {
            propertyPoses.Add(chest.transform.position);
        }

        gemsToPlace = GEMS_COUNT;
        while (gemsToPlace > 0)
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
                    logDis +=  UnityEngine.Mathf.Log(temp);
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
            if(UnityEngine.Random.Range(0.0f, 1.0f) > 0.0f)
            {
                PlaceGem(farestCell);
            }
            else 
            {
                PlaceGem(shortestCell);
            }            
        }        
    }
    
    public void PlaceLamp()
    {
        // 优先放箱子附近
        if (LAMPS_COUNT > 0)
        {
            int lampsToPlace = LAMPS_COUNT;
            for (int idx = 0; idx < chests.Count; ++idx)
            {
                Globals.CreateGuard("Lamp", pathFinder.GetNearestUnwalkableNode(chests[idx].transform.position));
                --lampsToPlace;
                if (lampsToPlace == 0)
                {
                    return;
                }
            }

            // 然后放路中间。Chests的顺序是按逆时针方向排列的。
            int a = 0;
            while (lampsToPlace != 0)
            {
                int b = a + 1;
                b = b % chests.Count;
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.3f)
                {
                    Chest chestA = chests[a];
                    Chest chestB = chests[b];
                    Pathfinding.Path p = Pathfinding.ABPath.Construct(chestA.transform.position, chestB.transform.position, null);
                    p.callback += OnPathBetweenTwoChest;
                    AstarPath.StartPath(p);
                    --lampsToPlace;
                }
                ++a;
                a = a % chests.Count;
            }
        }
    }

    public void OnPathBetweenTwoChest(Pathfinding.Path p)
    {
        UnityEngine.Vector3 midpos = p.vectorPath[p.vectorPath.Count/2];
        Globals.CreateGuard("Lamp", pathFinder.GetNearestUnwalkableNode(midpos));
    }   


    IEnumerator PlaceRooms(float waitTime)
    {
        // 把room尝试放置在每一个Cell，该Cell获得一个Score。最后把Room（左上角）放在得分最低的Cell处
        for (int roomCounter = 0; roomCounter < noOfRoomsToPlace; roomCounter++)
        {
            Room room = CreateRoom(minRoomXCellsCount, maxRoomXCellsCount, minRoomZCellsCount, maxRoomZCellsCount);
            int bestRoomPlacementScore = int.MaxValue;
            Cell bestRoomPlacementCell = null;

            foreach (Cell currentRoomPlacementCell in CorridorCellLocations)
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
        // 如果没有地板，墙体也不需要显示出来
        foreach (Cell cell in EveryCells)
        {
            if (cell.GetFloor() == null)
            {
                cell.HideEverythingExceptFloor();
            }
        }
        // 创造一个入口，找到地图最靠近东南角的走廊作为入口
        foreach (Cell corrido in CorridorCellLocations)
        {
            allCorridorsAfterMazeCompleted.Add(corrido);
        }
        entryOfMaze = allCorridorsAfterMazeCompleted[allCorridorsAfterMazeCompleted.Count - 1];

        

        // 要等一下生成路径，不然会出问题。
        Invoke("finished", 0.3f);        
    }

    void finished()
    {
        pathFinder.GenerateGridGraph();
        // 宝箱
        PlaceChests();

        if ((Globals.LevelController as MyMazeLevelController) == null)
        {
            PlaceGemsAtBoarder();
            PlaceLamp();
        }        

        rayCastPlane = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
        rayCastPlane.transform.localPosition = new UnityEngine.Vector3(0, Globals.FLOOR_HEIGHT, 0);
        rayCastPlane.transform.localScale = new UnityEngine.Vector3(1000, 0.2f, 1000);
        rayCastPlane.renderer.enabled = false;
        rayCastPlane.layer = 9;

        // pve关卡
        Globals.LevelController.MazeFinished();
    }

    public void SetRestrictToCamera(MagicThiefCamera camera)
    {
        float half_cell_length = cell_side_length / 2.0f;
        camera.restriction_x = new UnityEngine.Vector2(WestPosInPixel() + half_cell_length, EastPosInPixel() - half_cell_length);
        camera.restriction_z = new UnityEngine.Vector2(SouthPosInPixel() + cell_side_length, NorthPosInPixel() - cell_side_length);        
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

    public Cell GetCorridorCloseToCameraLookAt()
    {
        float min_dis = UnityEngine.Mathf.Infinity;
        Cell guard_birth = null;
        foreach (Cell corridor in allCorridorsAfterMazeCompleted)
        {
            float dis = UnityEngine.Vector3.Distance(corridor.GetFloorPos(), Globals.cameraFollowMagician.lookAt);
            if (dis < min_dis)
            {
                min_dis = dis;
                guard_birth = corridor;
            }
        }
        return guard_birth;
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
        }
    }

    public void OnDestroy()
    {
        UnRegistChallengerEvent();
        UnRegisterGuardArrangeEvent();
    }

    Finger fingerDownOnMap;
    Magician fingerDownMage;
    public bool OnChallengerFingerDown(object sender)
    {
        Finger finger = sender as Finger;
        fingerDownOnMap = finger;
        return true;
    }

    public bool OnChallengerFingerMoving(object sender)
    {
        if (fingerDownOnMap != null)
        {
            Globals.cameraFollowMagician.DragToMove(fingerDownOnMap);
        }        
        return true;
    }

    public bool OnChallengerFingerUp(object sender)
    {
        Finger finger = sender as Finger;
        if (fingerDownMage != null)
        {
            fingerDownMage.FingerUp(finger);
            fingerDownMage = null;
        }
        if (finger == fingerDownOnMap)
        {
            // 点击地板
            if (fingerDownOnMap.timeSinceTouchBegin < 0.5f &&
                UnityEngine.Vector2.Distance(fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition) < 10.0f)
            {
                UnityEngine.RaycastHit hitInfo;
                int layermask = 1 << 9 | 1 << 21;
                UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(fingerDownOnMap.nowPosition);
                if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
                {
                    Pathfinding.Node node = pathFinder.GetNearestWalkableNode(hitInfo.point);
                    UnityEngine.Vector3 pos = Globals.GetPathNodePos(node);
                    if (!Globals.magician.gameObject.activeSelf)
                    {
                        TutorialLevelController controller = (Globals.LevelController as TutorialLevelController);
                        controller.landingMark.SetActive(true);
                        controller.landingMark.transform.position = pos;
                    }
                    else if (Globals.magician.Stealing)
                    {
                        if (hitInfo.collider.gameObject.layer == 9)
                        {
                            Globals.magician.MoveTo(pos);
                        }
                        else
                        {
                            Globals.magician.ShotLight(hitInfo.collider.gameObject);
                        }                        
                    }                                                                              
                }
            }                     
        }

        fingerDownOnMap = null;
        return true;
    }

    System.Collections.Generic.List<UnityEngine.GameObject> tempGems = new System.Collections.Generic.List<UnityEngine.GameObject>();
    System.Collections.Generic.List<Pathfinding.Path> tempPathes = new System.Collections.Generic.List<Pathfinding.Path>();
    System.Collections.Generic.List<UnityEngine.GameObject> gemsSequence = new System.Collections.Generic.List<UnityEngine.GameObject>();
    System.Collections.Generic.List<UnityEngine.GameObject> sequenceNumbers = new System.Collections.Generic.List<UnityEngine.GameObject>();
    void FindNearestGem(UnityEngine.Vector3 pathBeginPos)
    {
        for (ushort idx = 0; idx < tempGems.Count; ++idx)
        {
            UnityEngine.GameObject gem = tempGems[idx];
            Pathfinding.Path p = Pathfinding.ABPath.Construct(pathBeginPos, gem.transform.position, null);
            p.callback += OnPathToGemComplete;
            p.gem = gem;
            AstarPath.StartPath(p);
        }
    }

    void OnPathToGemComplete(Pathfinding.Path p)
    {
        tempPathes.Add(p);
        if (tempPathes.Count == tempGems.Count)
        {
            float shortest = UnityEngine.Mathf.Infinity;
            UnityEngine.GameObject nearestGem = null;
            for(int idx = 0; idx < tempPathes.Count; ++idx)
            {
                Pathfinding.Path path = tempPathes[idx];
                float length = path.GetTotalLength();
                if (length < shortest)
                {
                    shortest = length;
                    nearestGem = path.gem;
                }
            }
            tempGems.Remove(nearestGem);
            gemsSequence.Add(nearestGem);
            // 路径以全部生成，标记出数字
            if (gemsSequence.Count == gemHolders.Count)
            {
                for (int idx = 0; idx < gemsSequence.Count; ++idx)
                {
                    UnityEngine.GameObject gem = gemsSequence[idx];
                    UnityEngine.GameObject number_prefab = UnityEngine.Resources.Load("UI/TargetNumber") as UnityEngine.GameObject;
                    UnityEngine.GameObject number = UnityEngine.GameObject.Instantiate(number_prefab) as UnityEngine.GameObject;
                    number.transform.position = gem.transform.position + new UnityEngine.Vector3(0,1,0);
                    number.GetComponentInChildren<UnityEngine.UI.Text>().text = idx.ToString();
                    sequenceNumbers.Add(number);
                }
            }
            tempPathes.Clear();
            FindNearestGem(nearestGem.transform.position);
        }
    }

    public Guard draggingGuard;
    public Chest choosenChest;
    public bool OnDragFingerDown(object sender)
    {
        if (choosenChest != null)
        {
            choosenChest.HideBtn();
        }

        fingerDownOnMap = sender as Finger;
        Guard guard = Globals.FingerRayToObj<Guard>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), 13, fingerDownOnMap);
        if (guard != null)
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
            Chest chest = Globals.FingerRayToObj<Chest>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), 14, fingerDownOnMap);
            if (chest != null)
            {
                chest.ShowUpgradeBtn();
                choosenChest = chest;
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
                draggingGuard.transform.position = new UnityEngine.Vector3(node.position.x / 1000.0f, node.position.y / 1000.0f, node.position.z / 1000.0f);
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
        }
        
        return true;
    }    

    public bool OnDragFingerUp(object sender)
    {
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
                Guard guard = Globals.FingerRayToObj<Guard>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), 13, fingerDownOnMap);
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

    // 为房间开门的代码，大致就是遍历四面墙，如果墙外刚好是走廊，就开一个门。比如西墙的其中一个Cell，西边刚好是走廊，就造一个门
    /*
    public void PlaceDoors(Dungeon dungeon)
    {
        foreach (Room room in dungeon.Rooms)
        {
            bool hasNorthDoor = false;
            bool hasSouthDoor = false;
            bool hasWestDoor = false;
            bool hasEastDoor = false;
            
            foreach (Point cellLocation in room.CellLocations)
            {
                // Translate the room cell location to its location in the dungeon
                Point dungeonLocation = new Point(room.Bounds.X + cellLocation.X, room.Bounds.Y + cellLocation.Y);
                // Check if we are on the west boundary of our room
                // and if there is a corridor to the west
                if ((cellLocation.X == 0) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, DirectionType.West)) &&
                    (!hasWestDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, DirectionType.West);
                    hasWestDoor = true;
                }
                
                // Check if we are on the east boundary of our room
                // and if there is a corridor to the east
                if ((cellLocation.X == room.Width – 1) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, DirectionType.East)) &&
                    (!hasEastDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, DirectionType.East);
                    hasEastDoor = true;
                }
                
                // Check if we are on the north boundary of our room
                // and if there is a corridor to the north
                if ((cellLocation.Y == 0) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, DirectionType.North)) &&
                    (!hasNorthDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, DirectionType.North);
                    hasNorthDoor = true;
                }
                
                // Check if we are on the south boundary of our room
                // and if there is a corridor to the south
                if ((cellLocation.Y == room.Height – 1) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, DirectionType.South)) &&
                    (!hasSouthDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, DirectionType.South);
                    hasSouthDoor = true;
                } 
            }
        }
    }
   */

    void Awake()
    {
        Globals.maze = this;        
        pathFinder = GetComponent<PathFinder>();        
    }

    UnityEngine.GameObject chest_prefab;
    // Use this for initialization
    public void Start()
    {
        Globals.LevelController.BeforeGenerateMaze();
                
        cells = new Cell[Z_CELLS_COUNT, X_CELLS_COUNT];
        bounds = new UnityEngine.Rect(0, 0, X_CELLS_COUNT, Z_CELLS_COUNT);

        if(chests.Count == 0)
        {
            chest_prefab = UnityEngine.Resources.Load("Props/Chest") as UnityEngine.GameObject;
            for (int idx = 0; idx < noOfRoomsToPlace; ++idx)
            {
                Chest chest = (UnityEngine.GameObject.Instantiate(chest_prefab) as UnityEngine.GameObject).GetComponent<Chest>();
                chest.Visible(false);
            }
        }               

        // 所有Cell的父节点
        maze = new UnityEngine.GameObject("Maze");

        // 左上角
        left_up_corner_pos = new UnityEngine.Vector3(-(X_CELLS_COUNT * cell_side_length) / 2.0f - cell_side_length / 2.0f,
           0, (Z_CELLS_COUNT * cell_side_length) / 2.0f - cell_side_length / 2.0f);

        UnityEngine.GameObject cell_prefab = UnityEngine.Resources.Load("Props/Cell") as UnityEngine.GameObject;
        // 从左上开始，一一构造cells
        for (int z = 0; z < Z_CELLS_COUNT; ++z)
        {
            for (int x = 0; x < X_CELLS_COUNT; ++x)
            {
                UnityEngine.Vector3 cell_pos = new UnityEngine.Vector3(left_up_corner_pos.x + x * cell_side_length,
                  0, left_up_corner_pos.z - z * cell_side_length);
                UnityEngine.GameObject cell_gameobj = UnityEngine.GameObject.Instantiate(cell_prefab) as UnityEngine.GameObject;
                Cell cell = cell_gameobj.GetComponent<Cell>();
                cell_gameobj.transform.parent = maze.transform;
                cell_gameobj.transform.position = cell_pos;
                cells[z, x] = cell;
                cell.name = z.ToString() + "," + x.ToString();
                cell.maze = this;
                cell.z = z;
                cell.x = x;
            }
        }

        // 四个角落
        EastNorthCornerCell = GetCell(0, X_CELLS_COUNT - 1);
        EastSouthCornerCell = GetCell(Z_CELLS_COUNT - 1, X_CELLS_COUNT - 1);
        WestSouthCornerCell = GetCell(Z_CELLS_COUNT - 1, 0);
        WestNorthCornerCell = GetCell(0, 0);

        StartCoroutine(CreateDenseMaze(Globals.CREATE_MAZE_TIME_STEP));
    }

    public void ClearMaze()
    {
        foreach(Chest chest in chests)
        {
            chest.Visible(false);
            chest.data = null;
        }
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

    // Update is called once per frame
    void Update()
    {
        foreach(UnityEngine.GameObject gem in gemHolders)
        {
            gem.GetComponent<UnityEngine.CharacterController>().Move(new UnityEngine.Vector3(0.001f, 0.0f, 0.001f));
            gem.GetComponent<UnityEngine.CharacterController>().Move(new UnityEngine.Vector3(-0.001f, 0.0f, -0.001f));
        }
    }
}
