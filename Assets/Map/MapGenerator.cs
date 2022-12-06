using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // debug map generator
    private void Update()
    {
        if (_buildMode)
        {
			if (Input.GetKeyDown(KeyCode.I)) { ResetGrid(); }
			if (Input.GetKeyDown(KeyCode.O)) { GenerateLevel(); }
			if (Input.GetKeyDown(KeyCode.P)) { SpawnLevel(); }
			if (Input.GetKeyDown(KeyCode.K)) { SaveGrid(); }
			if (Input.GetKeyDown(KeyCode.L)) { LoadGrid(); }

			if (_grid != null && _grid.Length > 0 && Input.GetMouseButton(0))
			{
				Vector3 position = GeneralUtility.GetMouseWorldPosition();
				SetGridSpace(GetGridFromWorldPosition(position), _currentGridSpace);
			}
		}
    }
    public enum GridSpace { empty, floor, wall, unbreakablewall, spawner, shopkeeper };
	private GridSpace[,] _grid;
	private int _roomHeight, _roomWidth;
	private float _worldUnitsInOneGridCell = 1;
	[SerializeField] private bool _buildMode;
	[SerializeField] private GridSpace _currentGridSpace;
	[Header("Map Generator Setup")]
	[SerializeField] private string loadGridName = "gridMap_0";
	[SerializeField] private Vector2 _roomSizeWorldUnits = new Vector2(50, 50);
	[SerializeField] private Vector2 _roomWorldOffset = new Vector2(0.5f, 0.5f);
	[SerializeField] private GameObject _wallObj, _floorObj, _unbreakableWallObj;
	[SerializeField] private GameObject[] _pfEnemySpawners;
	[SerializeField] private GameObject _pfShopkeeper;
	[Header("Fill Settings")]
	[SerializeField] private bool _removeSingleWalls;
	[SerializeField] private bool _unbreakableBorder;
	[SerializeField] private float _percentToFill = 0.4f;
	[Header("Spawner Settings")]
	[SerializeField] private float _percentToStartSpawners = 0.20f;
	[SerializeField] private float _chanceToCreateSpawner = 0.05f;
	[SerializeField] private int _maxNoSpawners = 10;
	[Header("Shopkeepr Settings")]
	[SerializeField] private float _percentToStartShopkeepers = 0.05f;
	[SerializeField] private float _chanceToCreateShopkeeper = 0.05f;
	[SerializeField] private int _maxNoShopkeepers = 2;
	private struct Walker
	{
		public Vector2 dir;
		public Vector2 pos;
	}
	private List<Walker> _walkers;
	public Vector3 FirstWalkerSpawnWorldPosition { get { return GetGridWorldPosition(SetWalkerSpawnPoint(_firstWalkerSpawnPoint)); } }
	[Header("Walker Settings")]
	[SerializeField] private float _chanceWalkerChangesDir = 0.5f;
	[SerializeField] private float _chanceWalkerSplits = 0.1f;
	[SerializeField] private float _chanceWalkerSpawns = 0.0f;
	[SerializeField] private float _chanceWalkerDies = 0.05f;
	[SerializeField][Range(0, 8)] private int _firstWalkerSpawnPoint = 4;
	[SerializeField] private int _initNoWalkers = 1, _minWalkers = 1, _maxWalkers = 10;
	// interfaces
	public void SetFirstWalkerSpawnPoint(int firstWalkerSpawnPont) => _firstWalkerSpawnPoint = Mathf.Clamp(firstWalkerSpawnPont, 0, 8);
	public void SetRandomFirstWalkerSpawnPoint() => SetFirstWalkerSpawnPoint(Random.Range(0, 9));
	public void SetupMapSize(Vector2 mapSize, float worldUnitsInOneGridCell = 1.0f)
	{
		_roomSizeWorldUnits = mapSize;
		_worldUnitsInOneGridCell = worldUnitsInOneGridCell;
	}
	public void SetupFillSettings(float percentToFill, bool removeSingleWalls = true, bool unbreakableBorder = true)
	{
		_percentToFill = percentToFill;
		_removeSingleWalls = removeSingleWalls;
		_unbreakableBorder = unbreakableBorder;
	}
	public void SetupSpawnerSettings(int maxNoSpawners, float percentToStartSpawners, float chanceToCreateSpawner)
	{
		_maxNoSpawners = maxNoSpawners;
		_percentToStartSpawners = percentToStartSpawners;
		_chanceToCreateSpawner = chanceToCreateSpawner;
	}
	public void SetupShopkeeperSettings(int maxNoShopkeers, float percentToStartShopkeepers, float chanceToCreateShopkeeper)
	{
		_maxNoShopkeepers = maxNoShopkeers;
		_percentToStartShopkeepers = percentToStartShopkeepers;
		_chanceToCreateShopkeeper = chanceToCreateShopkeeper;
	}
	public void SetupWalkerSettings(int initNoWalkers, int minWalkers, int maxWalkers, float chanceWalkerChangesDir, float chanceWalkerSplits, float chanceWalkerSpawns, float chanceWalkerDies)
    {
		_initNoWalkers = initNoWalkers;
		_minWalkers = minWalkers;
		_maxWalkers = maxWalkers;
		_chanceWalkerChangesDir = chanceWalkerChangesDir;
		_chanceWalkerSplits = chanceWalkerSplits;
		_chanceWalkerSpawns = chanceWalkerSpawns;
		_chanceWalkerDies = chanceWalkerDies;
	}
	public void SpawnLevel()
	{
		if (transform.childCount > 0)
			DestroyLevel();
		SetupParentObjects();
		LoopThroughAllGridSpaces(Spawn); // instatiates map
		if (_unbreakableBorder)
			SurroundLevelWithUnbreakableWalls();
	}
	public void DestroyLevel()
	{
		DestroyChildren();
	}
	// drunkard walk algorithm 
	public void GenerateLevel()
	{
		SetupInitialWalkers();
		int iterations = 0; // loop will not run forever
		do
		{
			LoopThroughWalkers(SetGridSpaceUnderWalker);
			LoopThroughWalkers(DestroyWalker);
			LoopThroughWalkers(DecideWalkersNextDirection);
			LoopThroughWalkers(SplitWalker);
			LoopThroughWalkers(SpawnWalkerInRandomRegion);
			LoopThroughWalkers(MoveWalker);
			LoopThroughWalkers(EnsureWalkerAvoidsBorder);
			// check whether there are enough floors, then exit loop
			if (PercentOfGridSpaceObject(GridSpace.floor) > _percentToFill)
			{
				ReplaceAllGridSpaceWith(GridSpace.empty, GridSpace.wall);
				break;
			}
			iterations++;
		} while (iterations < 100000); // caps the number of times this loop can iterate, to avoid overloading
		/* helper functions */
		void LoopThroughWalkers(System.Func<int, int, bool> action)
		{
			int noWalkers = _walkers.Count;
			for (int i = 0; i < noWalkers; i++)
				if (action(noWalkers, i))
					break;
		}
		bool SetGridSpaceUnderWalker(int noWalkers, int i)
		{
			Walker myWalker = _walkers[i];
			if (iterations > 5 && PercentOfGridSpaceObject(GridSpace.floor) >= _percentToStartSpawners
				&& Random.value < _chanceToCreateSpawner
				&& NumberOfGridSpaceObject(GridSpace.spawner) < _maxNoSpawners)
			{
				_grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = GridSpace.spawner;
			}
			else if (iterations > 5 && PercentOfGridSpaceObject(GridSpace.floor) >= _percentToStartShopkeepers
				&& Random.value < _chanceToCreateShopkeeper
				&& NumberOfGridSpaceObject(GridSpace.shopkeeper) < _maxNoShopkeepers)
			{
				_grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = GridSpace.shopkeeper;
			}
			else
			{
				_grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = GridSpace.floor;
			}
			return false;
		}
		bool DestroyWalker(int noWalkers, int i)
		{
			if (Random.value < _chanceWalkerDies && noWalkers > _minWalkers)
			{
				_walkers.RemoveAt(i);
				return true;
			}
			return false;
		}
		bool DecideWalkersNextDirection(int noWalkers, int i)
		{
			if (Random.value < _chanceWalkerChangesDir)
			{
				Walker thisWalker = _walkers[i];
				thisWalker.dir = RandomDirection();
				_walkers[i] = thisWalker;
			}
			return false;
		}
		bool MoveWalker(int noWalkers, int i)
		{
			Walker thisWalker = _walkers[i];
			thisWalker.pos += thisWalker.dir;
			_walkers[i] = thisWalker;
			return false;
		}
		bool SplitWalker(int noWalkers, int i)
		{
			if (Random.value < _chanceWalkerSplits && noWalkers < _maxWalkers)
			{
				Walker newWalker = new Walker
				{
					dir = RandomDirection(),
					pos = _walkers[i].pos
				};
				_walkers.Add(newWalker);
			}
			return false;
		}
		bool SpawnWalkerInRandomRegion(int noWalkers, int i)
		{
			if (Random.value < _chanceWalkerSpawns && noWalkers < _maxWalkers)
			{
				// pick random int between 0 and 8
				int spawnPoint = Mathf.FloorToInt(Random.Range(0, 8.99f));
				Walker newWalker = new Walker
				{
					dir = RandomDirection(),
					pos = SetWalkerSpawnPoint(spawnPoint)
				};
				_walkers.Add(newWalker);
			}
			return false;
		}
		bool EnsureWalkerAvoidsBorder(int noWalkers, int i)
		{
			Walker thisWalker = _walkers[i];
			// clamp x,y to leave a 1 space border: leave room for walls
			thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, _roomWidth - 2);
			thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, _roomHeight - 2);
			_walkers[i] = thisWalker;
			return false;
		}
	}
	public void ResetGrid()
	{
		// set internal grid size variables depending on the size of worldUnitsInOneGridCell - set this to your tileSize, this variable should be 1 for simplicity
		_roomHeight = Mathf.RoundToInt(_roomSizeWorldUnits.x / _worldUnitsInOneGridCell);
		_roomWidth = Mathf.RoundToInt(_roomSizeWorldUnits.y / _worldUnitsInOneGridCell);
		_grid = new GridSpace[_roomWidth, _roomHeight];
		LoopThroughAllGridSpaces(SetGridSpaceToEmpty);
		/* helper functions */
		void SetGridSpaceToEmpty(int x, int y) => _grid[x, y] = GridSpace.empty;
	}
	// internal functions
	private void SetupInitialWalkers()
	{
		// init walker list
		_walkers = new List<Walker>();
		// create a walker & set its direction
		Walker firstWalker = new Walker { dir = RandomDirection() };
		// set first walker spawn position
		firstWalker.pos = SetWalkerSpawnPoint(_firstWalkerSpawnPoint);
		// add walker to list
		_walkers.Add(firstWalker);
		if (_initNoWalkers > 1)
		{
			for (int i = 1; i < _initNoWalkers; i++)
			{
				Walker newWalker = new Walker
				{
					dir = RandomDirection(),
					pos = _walkers[i].pos
				};
				_walkers.Add(newWalker);
			}
		}
	}
	private void ReplaceAllGridSpaceWith(GridSpace targetGridSpace, GridSpace replacementGridSpace)
	{
		// loop though every grid space
		for (int x = 0; x < _roomWidth; x++)
		{
			for (int y = 0; y < _roomHeight; y++)
			{
				if (_grid[x, y] == targetGridSpace)
				{
					_grid[x, y] = replacementGridSpace;
				}
			}
		}
	}
	private void LoopThroughAllGridSpaces(System.Action<int, int> function)
	{
		// loop though every grid space
		for (int x = 0; x < _roomWidth; x++)
		{
			for (int y = 0; y < _roomHeight; y++)
			{
				function(x, y);
			}
		}
	}
	private void SurroundFloorWithWalls(int x, int y)
	{
		// if theres a floor, check the spaces around it
		if (_grid[x, y] == GridSpace.floor)
		{
			//if any surrounding spaces are empty, place a wall
			if (_grid[x, y + 1] == GridSpace.empty)
			{
				_grid[x, y + 1] = GridSpace.wall;
			}
			if (_grid[x, y - 1] == GridSpace.empty)
			{
				_grid[x, y - 1] = GridSpace.wall;
			}
			if (_grid[x + 1, y] == GridSpace.empty)
			{
				_grid[x + 1, y] = GridSpace.wall;
			}
			if (_grid[x - 1, y] == GridSpace.empty)
			{
				_grid[x - 1, y] = GridSpace.wall;
			}
		}
	}
	private void RemoveSingleWalls(int x, int y)
	{
		if (_grid[x, y] != GridSpace.wall)
			return;
		bool noWallsAdjacent = true; // assume no walls adjacent
		for (int checkX = -1; checkX <= 1; checkX++)
		{
			for (int checkY = -1; checkY <= 1; checkY++)
			{
				if (IfCheckIsOutsideOfGrid(x, y, checkX, checkY))
					continue; // skip checks that are outside of grid
				if (IfCheckIsInCorners(checkX, checkY))
					continue; // skip corners and center
				if (_grid[x + checkX, y + checkY] == GridSpace.wall)
					noWallsAdjacent = false;
			}
		}
		if (noWallsAdjacent)
			_grid[x, y] = GridSpace.floor; // if no walls adjacent, set to floor
		/* helper functions */
		bool IfCheckIsOutsideOfGrid(int x, int y, int checkX, int checkY) => (x + checkX < 0 || x + checkX > _roomWidth - 1) || (y + checkY < 0 || y + checkY > _roomHeight - 1);
		bool IfCheckIsInCorners(int checkX, int checkY) => (checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0);
	}
	// spawn appropriate game objects at grid space
	private void Spawn(int x, int y)
	{
		switch (_grid[x, y])
		{
			case GridSpace.empty:
				break;
			case GridSpace.floor:
				Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString()));
				break;
			case GridSpace.wall:
				Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString()));
				Spawn(x, y, _wallObj).transform.SetParent(this.transform.Find(GridSpace.wall.ToString()));
				break;
			case GridSpace.unbreakablewall:
				Spawn(x, y, _unbreakableWallObj).transform.SetParent(this.transform.Find(GridSpace.empty.ToString()));
				break;
			case GridSpace.spawner:
				Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString()));
				Spawn(x, y, _pfEnemySpawners[Random.Range(0, _pfEnemySpawners.Length)]).transform.SetParent(this.transform.Find(GridSpace.spawner.ToString()));
				break;
			case GridSpace.shopkeeper:
				Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString()));
				Spawn(x, y, _pfShopkeeper).transform.SetParent(this.transform.Find(GridSpace.shopkeeper.ToString()));
				break;
		}
	}
	// surround generated level with unbreakable walls
	private void SurroundLevelWithUnbreakableWalls()
	{
		for (int x = 0; x < _roomWidth; x++)
		{
			Spawn(x, -1, _unbreakableWallObj).transform.SetParent(this.transform.Find(GridSpace.empty.ToString()));
			Spawn(x, _roomHeight, _unbreakableWallObj).transform.SetParent(this.transform.Find(GridSpace.empty.ToString()));
		}
		for (int y = 0; y < _roomHeight; y++)
		{
			Spawn(-1, y, _unbreakableWallObj).transform.SetParent(this.transform.Find(GridSpace.empty.ToString()));
			Spawn(_roomWidth, y, _unbreakableWallObj).transform.SetParent(this.transform.Find(GridSpace.empty.ToString()));
		}
	}
	// returns a random direction (up, down, left or right)
	private Vector2 RandomDirection()
	{
		// pick random int between 0 and 3
		int choice = Mathf.FloorToInt(Random.Range(0, 3.99f));
		// use that int to choose a direction
		switch (choice)
		{
			case 0:
				return Vector2.down;
			case 1:
				return Vector2.left;
			case 2:
				return Vector2.up;
			default:
				return Vector2.right;
		}
	}
	// counts the number of specified grid space object in grid space and returns the value
	private int NumberOfGridSpaceObject(GridSpace gridSpace)
	{
		int count = 0;
		foreach (GridSpace space in _grid)
			if (space == gridSpace)
				count++;
		return count;
	}
	// calculates the percentage of grid space objects in grid space
	private float PercentOfGridSpaceObject(GridSpace gridSpace) => (float)NumberOfGridSpaceObject(gridSpace) / (float)_grid.Length;
	// Generic GameObject spawner function
	GameObject Spawn(int x, int y, GameObject toSpawn)
	{
		Vector3 spawnPos = GetGridPosition(x, y);
		GameObject spawnedGO = Instantiate(toSpawn, transform.position + spawnPos, Quaternion.identity);
		return spawnedGO;
	}
	// calculates world position from x and y position in grid[]
	private Vector2 GetGridPosition(int x, int y)
	{
		Vector2 offset = _roomSizeWorldUnits / 2.0f;
		return new Vector2(x, y) * _worldUnitsInOneGridCell - offset + _roomWorldOffset;
	}
	private Vector2 GetGridWorldPosition(Vector2 gridPos)
	{
		return GetGridPosition((int)gridPos.x, (int)gridPos.y);
	}
	private Vector2 GetGridFromWorldPosition(Vector3 worldPos) => GetGridFromWorldPosition(new Vector2(worldPos.x, worldPos.y));
	private Vector2 GetGridFromWorldPosition(Vector2 worldPos)
    {
		Vector2 offset = _roomSizeWorldUnits / 2.0f;
		Vector2 gridXY = (worldPos - _roomWorldOffset + offset) / _worldUnitsInOneGridCell;
		return new Vector2((int)gridXY.x, (int)gridXY.y);
	}
	private void SetGridSpace(Vector2 xy, GridSpace gridSpace) => SetGridSpace((int) xy.x, (int) xy.y, gridSpace);
	private void SetGridSpace(int x, int y, GridSpace gridSpace)
    {
		_grid[x, y] = gridSpace;
    }
	// 0 = top-left   , 1 = top-middle   , 2 = top-right,        
	// 3 = middle-left, 4 = center       , 5 = middle-right,  
	// 6 = bottom-left, 7 = bottom-middle, 8 = bottom-right
	private Vector2 SetWalkerSpawnPoint(int walkerSpawnPoint)
	{
		// choose a position
		switch (walkerSpawnPoint)
		{
			case 0:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 4.0f), Mathf.RoundToInt(_roomHeight / 4.0f));
			case 1:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 2.0f), Mathf.RoundToInt(_roomHeight / 4.0f));
			case 2:
				return new Vector2(Mathf.RoundToInt(3 * _roomWidth / 4.0f), Mathf.RoundToInt(_roomHeight / 4.0f));
			case 3:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 4.0f), Mathf.RoundToInt(_roomHeight / 2.0f));
			default:
			case 4:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 2.0f), Mathf.RoundToInt(_roomHeight / 2.0f));
			case 5:
				return new Vector2(Mathf.RoundToInt(3 * _roomWidth / 4.0f), Mathf.RoundToInt(_roomHeight / 2.0f));
			case 6:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 4.0f), Mathf.RoundToInt(3 * _roomHeight / 4.0f));
			case 7:
				return new Vector2(Mathf.RoundToInt(_roomWidth / 2.0f), Mathf.RoundToInt(3 * _roomHeight / 4.0f));
			case 8:
				return new Vector2(Mathf.RoundToInt(3 * _roomWidth / 4.0f), Mathf.RoundToInt(3 * _roomHeight / 4.0f));
		}
	}
	// creates game objects which are used to parent spawned objects
	private void SetupParentObjects()
	{
		GameObject emptyGameobject = new GameObject();
		foreach (GridSpace gridSpace in (GridSpace[])System.Enum.GetValues(typeof(GridSpace)))
		{
			GameObject GO = Instantiate(emptyGameobject, Vector3.zero, Quaternion.identity);
			GO.name = gridSpace.ToString();
			GO.transform.SetParent(this.transform);
		}
		GameObject.Destroy(emptyGameobject);
	}
	private void DestroyChildren()
	{
		while(transform.childCount > 0)
        {
			GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
	}
	private void AnalyzeMap()
	{
		// divide the map into 9 sections
		// divide those sections into 4 sections
	}

	// save / load grid
	[System.Serializable]
	private class GridSaveObject
	{
		public GridSpace gridSpace;
		public int x;
		public int y;
		public GridSaveObject(int x, int y, GridSpace gridSpace)
        {
			this.x = x;
			this.y = y;
			this.gridSpace = gridSpace;
        }
	}

	private class SaveObject
	{
		public GridSaveObject[] gridSaveObjectArray;
		// add more fields here to save more data
	}

	public void SaveGrid()
	{
		List<GridSaveObject> gridSaveObjectList = new List<GridSaveObject>();
		/*for (int x = 0; x < _roomWidth; x++)
		{
			for (int y = 0; y < _roomHeight; y++)
			{
				gridSaveObjectList.Add(new GridSaveObject(x, y, _grid[x,y]));
			}
		}*/
		LoopThroughAllGridSpaces(AddGridSpaceToList);

		SaveObject saveObject = new SaveObject { gridSaveObjectArray = gridSaveObjectList.ToArray() }; // json arrays work easier than json lists

		SaveLoadSystem.SaveObject("gridMap", saveObject, false);
		/* helper functions */
		void AddGridSpaceToList(int x, int y) => gridSaveObjectList.Add(new GridSaveObject(x, y, _grid[x, y]));
	}
	public void LoadGrid() => LoadGrid(loadGridName);
	public void LoadGrid(string loadName)
	{
		SaveObject saveObject = SaveLoadSystem.LoadObject<SaveObject>(loadName);
		foreach (GridSaveObject gridSaveObject in saveObject.gridSaveObjectArray)
		{
			_grid[gridSaveObject.x, gridSaveObject.y] = gridSaveObject.gridSpace;
		}
	}

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position - (Vector3) _roomWorldOffset*2, new Vector3(_roomHeight, _roomWidth, 1));
		if (_grid != null &&  _grid.Length > 0)
		{
			LoopThroughAllGridSpaces(DrawGizmoOnGridSpace);
		}
		/* helper functions */
		void DrawGizmoOnGridSpace(int x, int y)
        {
			Color gizmoColor = Color.green;
			bool drawCircle = false;
            switch (_grid[x,y])
            {
                case GridSpace.empty:
                    break;
                case GridSpace.floor:
					gizmoColor = _floorObj.GetComponentInChildren<SpriteRenderer>().color;
					gizmoColor = Color.gray;
					break;
                case GridSpace.wall:
					gizmoColor = _floorObj.GetComponentInChildren<SpriteRenderer>().color;
					gizmoColor = Color.blue;
					break;
				case GridSpace.unbreakablewall:
					gizmoColor = _floorObj.GetComponentInChildren<SpriteRenderer>().color;
					gizmoColor = Color.black;
					break;
				case GridSpace.spawner:
					gizmoColor = _floorObj.GetComponentInChildren<SpriteRenderer>().color;
					gizmoColor = Color.red;
					drawCircle = true;
					break;
                case GridSpace.shopkeeper:
					gizmoColor = _floorObj.GetComponentInChildren<SpriteRenderer>().color;
					gizmoColor = Color.cyan;
					drawCircle = true;
					break;
                default:
                    break;
			}
			DrawGridSpace(GetGridPosition(x,y) + _roomWorldOffset, gizmoColor, drawCircle);

			Gizmos.color = Color.magenta;
			Vector3 pos = GeneralUtility.GetMouseWorldPosition();
			Gizmos.DrawSphere(pos, 0.3f);

			void DrawGridSpace(Vector2 worldSpace,Color color, bool drawCircle)
            {
				Gizmos.color = color;
                if (drawCircle)
                {
					Gizmos.DrawSphere(worldSpace, (_worldUnitsInOneGridCell <= 0.1f ? 0.1f : (_worldUnitsInOneGridCell - 0.1f) )/2 );
				}
                else
                {
					Gizmos.DrawCube(worldSpace, Vector3.one * (_worldUnitsInOneGridCell <= 0.1f ? 0.1f : (_worldUnitsInOneGridCell - 0.1f) ) );
				}
            }
        };
	}
}
