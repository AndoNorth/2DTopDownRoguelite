using System.Collections.Generic;
using UnityEngine;

// drunkard walk algorithm 
public class MapGenerator : MonoBehaviour
{
	private enum GridSpace { empty, floor, wall, spawner, shopkeeper };
	private GridSpace[,] _grid;
	private int _roomHeight, _roomWidth;
	private float _worldUnitsInOneGridCell = 1;
	[Header("Map Generator Setup")]
	[SerializeField] private Vector2 _roomtSizeWorldUnits = new Vector2(50, 50);
	[SerializeField] private GameObject _wallObj, _floorObj, _unbreakableWallObj;
	[SerializeField] private GameObject[] _pfEnemySpawners;
	[SerializeField] private GameObject _pfShopkeeper; // TODO: add shopkeeper spawn
	[Header("Fill Settings")]
	[SerializeField] private bool _removeSingleWalls;
	[SerializeField] private float _percentToFill = 0.2f;
	[Header("Spawner Settings")]
	[SerializeField] private float _percentToStartSpawners = 0.05f;
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
	[Header("Walker Settings")]
	[SerializeField] private float _chanceWalkerChangesDir = 0.5f;
	[SerializeField] private float _chanceWalkerSplits = 0.1f;
	[SerializeField] private float _chanceWalkerSpawns = 0.0f;
	[SerializeField] private float _changeWalkerDies = 0.05f;
	[SerializeField] private int _firstWalkerSpawnPoint = 4, _initNoWalkers = 1, _minWalkers = 1, _maxWalkers = 10;
	void Start()
	{
		SetupParentObjects();
		Setup(); 
		CreateFloors(); // sends walkers to generate map
		//CreateWalls(); // outlines generated map
		if (_removeSingleWalls)
		{
			RemoveSingleWalls(); // removes singular walls in the middle of open spaces
		}
		SpawnLevel(); // instatiates map
	}
	void Setup()
	{
		// set internal grid size variables depending on the size of worldUnitsInOneGridCell - set this to your tileSize, this variable should be 1 for simplicity
		_roomHeight = Mathf.RoundToInt(_roomtSizeWorldUnits.x / _worldUnitsInOneGridCell);
		_roomWidth = Mathf.RoundToInt(_roomtSizeWorldUnits.y / _worldUnitsInOneGridCell);
		// create grid
		_grid = new GridSpace[_roomWidth, _roomHeight];
		// set grid's default state
		for (int x = 0; x < _roomWidth - 1; x++)
		{
			for (int y = 0; y < _roomHeight - 1; y++)
			{
				// make every cell "empty"
				_grid[x, y] = GridSpace.empty;
			}
		}
		// setup first walker(s)
		// init walker list
		_walkers = new List<Walker>();
		// create a walker
		Walker firstWalker = new Walker
		{
			dir = RandomDirection() // first walker initial direction;
		};
		// find center of grid
		Vector2 spawnPos = this.transform.position;
		// set first walker spawn position
		firstWalker.pos = SetWalkerSpawnPoint(_firstWalkerSpawnPoint);
		// add walker to list
		_walkers.Add(firstWalker);
		if (_initNoWalkers > 1)
		{
			for (int i = 1; i < _initNoWalkers-1; i++)
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
	void CreateFloors()
	{
		int iterations = 0; // loop will not run forever
		do
		{	
			// create floor at position of every walker
			foreach (Walker myWalker in _walkers)
			{
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
			}
			// chance: destroy walker
			int numberChecks = _walkers.Count; // might modify count while in this loop
			for (int i = 0; i < numberChecks; i++)
			{
				// only if # of walkers > min walkers, and at a low chance
				if (Random.value < _changeWalkerDies && _walkers.Count > _minWalkers)
				{
					_walkers.RemoveAt(i);
					break; // only destroy one per iteration
				}
			}
			// chance: walker pick new direction
			for (int i = 0; i < _walkers.Count; i++)
			{
				if (Random.value < _chanceWalkerChangesDir)
				{
					Walker thisWalker = _walkers[i];
					thisWalker.dir = RandomDirection();
					_walkers[i] = thisWalker;
				}
			}
			// chance: split into new walker
			numberChecks = _walkers.Count; // might modify count while in this loop
			for (int i = 0; i < numberChecks; i++)
			{
				// only if # of walkers < max, and at a low chance
				if (Random.value < _chanceWalkerSplits && _walkers.Count < _maxWalkers)
				{
					// create a new walker
					Walker newWalker = new Walker
					{
						dir = RandomDirection(),
						pos = _walkers[i].pos
					};
					_walkers.Add(newWalker);
				}
			}
			// chance: spawn new walker in random region
			numberChecks = _walkers.Count; // might modify count while in this loop
			for (int i = 0; i < numberChecks; i++)
			{
				// only if # of walkers < max, and at a low chance
				if (Random.value < _chanceWalkerSpawns && _walkers.Count < _maxWalkers)
				{
					// pick random int between 0 and 8
					int spawnPoint = Mathf.FloorToInt(Random.Range(0, 8.99f));
					// create a new walker
					Walker newWalker = new Walker
					{
						dir = RandomDirection(),
						pos = SetWalkerSpawnPoint(spawnPoint)
					};
					;
					_walkers.Add(newWalker);
				}
			}
			// move walkers
			for (int i = 0; i < _walkers.Count; i++)
			{
				Walker thisWalker = _walkers[i];
				thisWalker.pos += thisWalker.dir;
				_walkers[i] = thisWalker;
			}
			// avoid border of grid
			for (int i = 0; i < _walkers.Count; i++)
			{
				Walker thisWalker = _walkers[i];
				// clamp x,y to leave a 1 space boarder: leave room for walls
				thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, _roomWidth - 2);
				thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, _roomHeight - 2);
				_walkers[i] = thisWalker;
			}
			// check whether there are enough floors, then exit loop
			if (PercentOfGridSpaceObject(GridSpace.floor) > _percentToFill)
			{
				break;
			}
			iterations++;
		} while (iterations < 100000); // caps the number of times this loop can iterate, to avoid overloading
	}
	private void CreateWalls()
	{
		// loop though every grid space
		for (int x = 0; x < _roomWidth - 1; x++)
		{
			for (int y = 0; y < _roomHeight - 1; y++)
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
		}
	}
	void RemoveSingleWalls()
	{
		// loop though every grid space
		for (int x = 0; x < _roomWidth - 1; x++)
		{
			for (int y = 0; y < _roomHeight - 1; y++)
			{
				// if theres a wall, check the spaces around it
				if (_grid[x, y] == GridSpace.wall)
				{
					// assume all space around wall are floors
					bool allFloors = true;
					// check each side to see if they are all floors
					for (int checkX = -1; checkX <= 1; checkX++)
					{
						for (int checkY = -1; checkY <= 1; checkY++)
						{
							if (x + checkX < 0 || x + checkX > _roomWidth - 1 ||
								y + checkY < 0 || y + checkY > _roomHeight - 1)
							{
								// skip checks that are outside of grid space
								continue;
							}
							if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
							{
								// skip corners and center
								continue;
							}
							if (_grid[x + checkX, y + checkY] != GridSpace.floor)
							{
								// if any adjacent tiles are not floors, set allFloors to false
								allFloors = false;
							}
						}
					}
					if (allFloors)
					{
						// if singular wall found surrounding by all floors, set to floor
						_grid[x, y] = GridSpace.floor;
					}
				}
			}
		}
	}
	// sweep through grid space, spawning appropriate gameObjects
	void SpawnLevel()
	{
		for (int x = 0; x < _roomWidth; x++)
		{
			for (int y = 0; y < _roomHeight; y++)
			{
				switch (_grid[x, y])
				{
					case GridSpace.empty:
						Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString()));
						Spawn(x, y, _wallObj).transform.SetParent(this.transform.Find(GridSpace.wall.ToString())); ;
						break;
					case GridSpace.floor:
						Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString())); ;
						break;
					case GridSpace.wall:
						Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString())); ;
						Spawn(x, y, _wallObj).transform.SetParent(this.transform.Find(GridSpace.wall.ToString())); ;
						break;
					case GridSpace.spawner:
						Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString())); ;
						Spawn(x, y, _pfEnemySpawners[Random.Range(0, _pfEnemySpawners.Length)]).transform.SetParent(this.transform.Find(GridSpace.spawner.ToString())); ;
						break;
					case GridSpace.shopkeeper:
						Spawn(x, y, _floorObj).transform.SetParent(this.transform.Find(GridSpace.floor.ToString())); ;
						Spawn(x, y, _pfShopkeeper).transform.SetParent(this.transform.Find(GridSpace.shopkeeper.ToString())); ;
						break;
				}
			}
		}
		for (int x = 0; x < _roomWidth; x++)
		{
			Spawn(x , -1, _unbreakableWallObj);
			Spawn(x , _roomHeight, _unbreakableWallObj);
		}
		for (int y = 0; y < _roomHeight; y++)
		{
			Spawn(-1, y, _unbreakableWallObj);
			Spawn(_roomWidth, y, _unbreakableWallObj);
		}
		GameManager.instance._isMapSetup = true;
	}
	Vector2 RandomDirection()
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
		{
			if (space == gridSpace)
			{
				count++;
			}
		}
		return count;
	}
	// calculates the percentage of grid space objects in grid space
	private float PercentOfGridSpaceObject(GridSpace gridSpace) => (float)NumberOfGridSpaceObject(gridSpace) / (float)_grid.Length;
	// Generic GameObject spawner function
	GameObject Spawn(float x, float y, GameObject toSpawn)
	{
		//find the position to spawn
		Vector2 offset = _roomtSizeWorldUnits / 2.0f;
		Vector2 spawnPos = new Vector2(x, y) * _worldUnitsInOneGridCell - offset;
		//spawn object
		GameObject spawnedGO = Instantiate(toSpawn, spawnPos, Quaternion.identity);
		return spawnedGO;
	}
	// 0 = top-left, 1 = top-middle, 2 = top-right, 
	// 3 = middle-left, 4 = center, 5 = middle-right,
	// 6 = bottom-left, 7 = bottom-middle, 8 = bottom-right
	Vector2 SetWalkerSpawnPoint(int walkerSpawnPoint)
	{
		// choose a positions
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
	// this is to organize spawned objects
	private void SetupParentObjects()
    {
        foreach (GridSpace gridSpace in (GridSpace[]) System.Enum.GetValues(typeof(GridSpace) ) )
        {
			GameObject GO = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
			GO.name = gridSpace.ToString();
			GO.transform.SetParent(this.transform);
        }
    }
	void AnalyzeMap()
	{
		// divide the map into 9 sections
		// divide those sections into 4 sections
	}
}
