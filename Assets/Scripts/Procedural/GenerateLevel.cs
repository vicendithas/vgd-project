using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateLevel : MonoBehaviour
{
	private bool win;

	// spawn chances	
	public float enemyChance = 0.01f,
					treasureChance = 0.003f,
					trapChance = 0.015f,
					torchChance = 0.05f;

	// world related variables
	public int levelSize = 50;
	public int roomSize = 5;
	public float tileSpawnChance = 0.75f;
	public float roomSpawnChance = 0.008f;
	
	public int tileStyle; // 0 for dev, 1 for default dungeon
	private int[] startingLocation;
	private int[,] levelMatrix;
	
	// tiles
	private GameObject basicTile,
						pathTile,
						wallTile,
						enemyTile,
						treasureTile,
						trapTile,
						exitTile;
	
	// holders for gameobjects
	private List<GameObject> tileList = new List<GameObject>();
	private List<GameObject> enemyList = new List<GameObject>();
	private List<GameObject> chestList = new List<GameObject>();
	private List<int[]> rooms = new List<int[]>();
	private List<GameObject> trapList = new List<GameObject>();
	private List<GameObject> torchList = new List<GameObject>();

	[HideInInspector]
	public List<GameObject> swordList = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> shieldList = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> spellList = new List<GameObject>();
	
	// enemies
	private GameObject goblinObject;
	
	// items
	private GameObject chestObject;
	
	// environment
	private GameObject torchObject;
	
	// Use this for initialization
	void Start ()
	{
		win = false;
		// use to get reliable dungeons
		//Random.seed = 140394581;
		//Random.seed = 608781738;
		
		//Debug.Log (Random.seed);
		
		
		setupDungeon();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if (Input.GetKeyDown (KeyCode.E))
		//	Application.LoadLevel("mainGame");
			
		/*if(Input.GetKeyDown(KeyCode.G)){
			foreach (GameObject enemy in enemyList)
			{
				enemy.GetComponent<EnemyStats>().currentHealth--;
			}
		}*/

		if (win)
		{
			Time.timeScale = 0f;

			//GameObject.FindWithTag ("Player").GetComponent<PlayerStats> ().currentLevel = 10;

			GameObject.Find ("WinText").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("KillsGUI").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("DamageGUI").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("QuitGUI").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("Exit Textured(Clone)").GetComponent<LevelTransition>().popup.text = "";

			if(Input.GetKeyDown (KeyCode.Q) || Input.GetButtonDown (MyInput.B_name)){
				Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
            }
		}
		
		// cull particle systems
		/*foreach (GameObject ps in GameObject.FindGameObjectsWithTag("SpellEffect"))
		{
			if (ps.GetComponent<ParticleSystem>().IsAlive())
				GameObject.Destroy(ps);
		}*/
	}
	
	// deletes current level and generates the next one. if level > 10, the player wins!
	public void proceedToNextLevel()
	{
		GameObject.FindWithTag ("Player").GetComponent<PlayerStats> ().currentLevel++;
		
		//if (GameObject.FindWithTag ("Player").GetComponent<PlayerStats> ().currentLevel > 10)
		if (GameObject.FindWithTag ("Player").GetComponent<PlayerStats> ().currentLevel > 5)
			win = true;
		else
		{
			//levelSize += 2;
			levelSize += 4;
		
			clearDungeon();
			setupDungeon();
		}	
	}
	
	// does everything to ready and display the dungeon
	void setupDungeon()
	{
		levelMatrix = new int[levelSize,levelSize];
		
		// load in prefabs
		loadPrefabs();
		
		// create the dungeon
		generateDungeon();
		
		// litter dungeon with tiles
		tileDungeon();
		
		// scan dungeon to make small tweaks
		refineDungeon();
		
		// draw the dungeon
		showDungeon();
		
		// populate dungeon with actual stuff
		populateDungeon();
		
		// move player to starting area
		GameObject.FindWithTag("Player").transform.position = new Vector3(startingLocation[0], 0, startingLocation[1]);
		
		//Debug.Log("Done!");
		
		return;
	}
	
	
	// loads in the appropriate prefabs
	void loadPrefabs()
	{
		// tiles
		if (tileStyle == 0)
		{
			// dev tiles
			basicTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Basic");
			pathTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Corridor");
			wallTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Wall");
			enemyTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Enemy");
			treasureTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Treasure");
			trapTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Trap");
			exitTile = (GameObject)Resources.Load ("Prefabs/Dev Tiles/Exit");
		}
		else if (tileStyle == 1)
		{
			// textured dungeon tiles
			basicTile = (GameObject)Resources.Load ("Prefabs/Dungeon Tiles/Basic Textured");
			wallTile = (GameObject)Resources.Load ("Prefabs/Dungeon Tiles/Wall Textured");
			exitTile = (GameObject)Resources.Load ("Prefabs/Dungeon Tiles/Exit Textured");
			
			// load in traps
			trapList.Add((GameObject)Resources.Load ("Prefabs/Dungeon Tiles/Trap - Spikes"));
			trapList.Add((GameObject)Resources.Load ("Prefabs/Dungeon Tiles/Trap - Boulder"));
			
		}
		
		// enemies
		goblinObject = (GameObject)Resources.Load("Prefabs/Enemies/Goblin");
		
		// items
		chestObject = (GameObject)Resources.Load ("Prefabs/Items/Chest");
		
		// environment
		torchObject = (GameObject)Resources.Load ("Prefabs/Environment/Torch");
	}
	
	// fills dungeon matrix with empty "0" values (correspond to "darkness" tiles)
	void clearDungeon()
	{
		//remove all weapons on ground
		List<GameObject> templist = new List<GameObject> ();
		foreach (GameObject sword in swordList)
		{
			if(sword.transform.root.tag != "Player"){
				GameObject.Destroy (sword);
				templist.Add (sword);
			}
		}
		foreach(GameObject obj in templist){
			swordList.Remove (obj);
		}

		//remove all shields on ground
		templist = new List<GameObject>();
		foreach (GameObject shield in shieldList)
		{
			if(shield.transform.root.tag != "Player"){
				GameObject.Destroy (shield);
				templist.Add(shield);
			}
		}
		foreach(GameObject obj in templist){
			shieldList.Remove(obj);
		}

		//remove all spells on ground
		templist = new List<GameObject> ();
		foreach (GameObject spell in spellList)
		{
			if(spell.transform.root.tag != "MainCamera"){
				GameObject.Destroy (spell);
				templist.Add (spell);
			}
		}
		foreach(GameObject obj in templist){
			spellList.Remove (obj);
		}

		// remove tiles
		foreach (GameObject tile in tileList)
		{
			GameObject.Destroy(tile);
		}
		
		// remove enemies
		foreach (GameObject enemy in enemyList)
		{
			GameObject.Destroy(enemy);
		}
		
		// remove chests
		foreach (GameObject chest in chestList)
		{
			GameObject.Destroy(chest);
		}
		
		// remove boulders
		foreach (GameObject rock in GameObject.FindGameObjectsWithTag("Boulder"))
		{
			GameObject.Destroy (rock);
		}
		
		// remove environmental objects
		foreach (GameObject torch in GameObject.FindGameObjectsWithTag("Environment"))
		{
			GameObject.Destroy (torch);
		}
		
		// remove rooms
		rooms.Clear();
	
		
		/*for (int x = 0; x < levelMatrix.GetLength(0); x++)
		{
			for (int z = 0; z < levelMatrix.GetLength(1); z++)
			{
				levelMatrix[x,z] = -1;
			}
		}*/

		//remove the "proceed to next level" message
		GameObject.Find ("Exit Textured(Clone)").GetComponent<LevelTransition> ().popup.text = "";
		
		return;
	}
	
	// seeds, grows, and connects rooms to form the dungeon
	void generateDungeon()
	{
		//bool isExitSet = false;
		
		// randomly choose a corner or middle to place the starting room
		switch (Random.Range(1,5))
		{
			// top left
			case 1:
				startingLocation = new int[2] {levelSize/4, 3*levelSize/4};
				break;
			// top right
			case 2:
				startingLocation = new int[2] {3*levelSize/4, 3*levelSize/4};
				break;
			// bottom left
			case 3:
				startingLocation = new int[2] {levelSize/4, levelSize/4};
				break;
			// bottom right
			case 4:
				startingLocation = new int[2] {3*levelSize/4, levelSize/4};
				break;
			// middle
			/*case 5:
				startingLocation = new int[2] {levelSize/2, levelSize/2};
				break;*/
		}
		
		// add the starting room to rooms list
		rooms.Add(startingLocation);
		
		// variable for storing random values determined room spawn chances
		float roomSpawn;
		
		// go through all tiles and randomly set room seeds
		for (int x = 0; x < levelMatrix.GetLength(0); x++)
		{
			for (int z = 0; z < levelMatrix.GetLength(1); z++)
			{
				// edge cases, always darkness
				if (x <= 1 || z <= 1 || x >= levelMatrix.GetLength(0) - 2 || z >= levelMatrix.GetLength(1) - 2)
				{
					levelMatrix[x,z] = 0;
					continue;
				}
				
				roomSpawn = Random.value;
				
				// seed rooms
				if (roomSpawn < roomSpawnChance)
					rooms.Add(new int[2] {x,z});
				//else if (randomNum > 0.6)
				else
					levelMatrix[x,z] = 0;
			}
		}
		
		int[] furthestRoomFromStart = {startingLocation[0], startingLocation[1]};
		
		// create each room
		foreach (int[] item in rooms)
		{
			//Debug.Log ("Expanding tile " + item[0] + ", " + item[1]);
			
			//levelMatrix[item[0], item[1]] = 5;
			if (rooms.IndexOf(item) == 0)
			{
				setupStartingRoom(item[0], item[1]);
			}
			else
			{
				expandRoom(item[0], item[1], Random.Range (4, 4+roomSize));
				
				// calculate furthest room from start
				if (distance(startingLocation, furthestRoomFromStart) < distance(startingLocation, item))
				{
					furthestRoomFromStart = new int[2] {item[0], item[1]};
				}
			}
		}
		
		// create corridors
		generateCorridors();
		
		// set ending tile
		levelMatrix[furthestRoomFromStart[0], furthestRoomFromStart[1]] = 99;
		
		return;
	}
	
	// creates the starting room that the player starts in
	void setupStartingRoom(int x, int z)
	{
		for (int i = -3; i < 4; i++)
		{
			for (int j = -3; j < 4; j++)
			{
				if (Mathf.Abs(i+j) <= 3)
					levelMatrix[x+i,z+j] = 11;
			}
		}
	}
	
	// creates the starting room that the player starts in
	void setupEndingRoom()
	{
		// setup area to proceed to next level
		while (true)
		{
			int[] end = { Random.Range(0,levelMatrix.GetLength(0)), Random.Range(0,levelMatrix.GetLength(1)) };
				                                                                   
			if (distance(end, startingLocation) > levelMatrix.GetLength(0)/1.5f)
			{
				for (int i = -2; i < 2; i++)
				{
					for (int j = -2; j < 2; j++)
					{
						levelMatrix[end[0]+i,end[1]+j] = 99;
					}
				}
				break;
			}
		}
	}
	
	// recursive function to fill out rooms
	void expandRoom(int x, int z, int lifetime)
	{
		lifetime--;
		
		if (lifetime <= 0)
			return;
	
		// call approrpriate recursion calls on surrounding cells
		levelMatrix[x,z] = 5;
	
		// start with top left cell, go right across then down a row, typewriter style
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{					
				if (levelMatrix[x+i,z+j] == 0 && (x+i != 0) && (x+i != levelMatrix.GetLength(0) - 2) && (z+j != 0) && (z+j != levelMatrix.GetLength(1) - 2))
				{
					if (Random.value < tileSpawnChance)
					{
						//Debug.Log ("Expanding tile " + (x+i) + ", " + (z+j) + ", Lifetime = " + lifetime);
						expandRoom((x+i), (z+j), lifetime);
					}
				}
			}
		}
		
		return;
	}
	
	// creates a relative neighbourhood graph of room nodes and connects them with corridors
	void generateCorridors()
	{
		int roomOne = 0, roomTwo = 0;
		float maxDistance = 0.0f;
		
		// horrible O(n^3) approach for finding relative neighbourhood graph
		for (int i = 0; i < rooms.Count; i++)
		{
			for (int j = i + 1; j < rooms.Count; j++)
			{
				bool isEdge = true;
				float testDistance = distance(rooms[i], rooms[j]);
				
				// store overall maximum distance to create long corridor for loop(s)
				if (testDistance > maxDistance)
				{
					maxDistance = testDistance;
					roomOne = i;
					roomTwo = j;
				}
				
				// go through all other rooms not currently being compared
				for (int k = 0; k < rooms.Count; k++)
				{
					if (k == i || k == j)
						continue;
					
					float tempOne = distance(rooms[i], rooms[k]);
					float tempTwo = distance(rooms[j], rooms[k]);
					
					if (testDistance > ((tempOne > tempTwo) ? tempOne : tempTwo))
					{
						// max distance to point k is closer to i and j than distance from i to j, this isn't an edge
						isEdge = false;
						break;
					}
				}
				
				if (isEdge)
					spawnCorridor(rooms[i][0], rooms[i][1], rooms[j][0], rooms[j][1]);
			}
		}
		
		// create overarching corridor to hopefully make some cheap loops
		spawnCorridor(rooms[roomOne][0], rooms[roomOne][1], rooms[roomTwo][0], rooms[roomTwo][1]);
		
		
		return;
	}
	
	// returns distance between points a and b
	float distance(int[] a, int[] b)
	{
		return Mathf.Sqrt( Mathf.Pow(a[0]-b[0],2) + Mathf.Pow(a[1]-b[1],2) );
	}
	
	// creates L-corridor of varying size from point a to point b
	void spawnCorridor(int ax, int az, int bx, int bz)
	{
		int[] a = new int[2] {ax, az};
		int[] b = new int[2] {bx, bz};
		
		int corridorWidth = 1;
		int corridorColor = 5; // 5 for white, 11 for green
		
		// decide how wide these corridors will be
		float tempCorridor = Random.value;
		
		if (tempCorridor > 0.85f)
			corridorWidth = 3;
		else
			corridorWidth = 2;
		
		int xSlope = b[0] - a[0];
		int zSlope = b[1] - a[1];
		
		// first handle side corresponding to the X-axis
		if (xSlope > 0)
		{
			while (xSlope > 0)
			{
				levelMatrix[a[0],a[1]] = corridorColor;
				
				switch (corridorWidth)
				{
					case 2:
						if (a[1] != 1 || a[1] != levelSize - 2)
							levelMatrix[a[0],a[1]-1] = corridorColor;
						break;
					case 3:
						if (a[1] != 1 || a[1] != levelSize - 2)
						{
							levelMatrix[a[0],a[1]-1] = corridorColor;
							levelMatrix[a[0],a[1]+1] = corridorColor;
						}
						break;
				}
				
				xSlope--;
				a[0]++;
			}
		}
		else
		{
			while (xSlope < 0)
			{
				levelMatrix[a[0],a[1]] = corridorColor;
				
				switch (corridorWidth)
				{
					case 2:
						if (a[1] != 1 || a[1] != levelSize - 2)
							levelMatrix[a[0],a[1]-1] = corridorColor;
						break;
					case 3:
						if (a[1] != 1 || a[1] != levelSize - 2)
						{
							levelMatrix[a[0],a[1]-1] = corridorColor;
							levelMatrix[a[0],a[1]+1] = corridorColor;
						}
						break;
				}
				
				xSlope++;
				a[0]--;
			}
		}
		
		// then handle side corresponding to the Z-axis
		if (zSlope > 0)
		{
			while (zSlope > 0)
			{
				levelMatrix[a[0],a[1]] = corridorColor;
				
				switch (corridorWidth)
				{
					case 2:
						if (a[0] != 1 || a[0] != levelSize - 2)
							levelMatrix[a[0]-1,a[1]] = corridorColor;
						break;
					case 3:
						if (a[0] != 1 || a[0] != levelSize - 2)
						{
							levelMatrix[a[0]-1,a[1]] = corridorColor;
							levelMatrix[a[0]+1,a[1]] = corridorColor;
						}
						break;
				}
				
				zSlope--;
				a[1]++;
			}
		}
		else
		{
			while (zSlope < 0)
			{
				levelMatrix[a[0],a[1]] = corridorColor;
				
				switch (corridorWidth)
				{
					case 2:
						if (a[0] != 1 || a[0] != levelSize - 2)
							levelMatrix[a[0]-1,a[1]] = corridorColor;
						break;
					case 3:
						if (a[0] != 1 || a[0] != levelSize - 2)
						{
							levelMatrix[a[0]-1,a[1]] = corridorColor;
							levelMatrix[a[0]+1,a[1]] = corridorColor;
						}
						break;
				}
				
				zSlope++;
				a[1]--;
			}
		}
		
		return;
	}
	
	// passes over the levelMatrix to put in tiles for enemy, treasure, and trap spawns
	void tileDungeon()
	{
		// only go over tiles that can have floor tiles
		for (int x = 2; x < levelMatrix.GetLength(0) - 2; x++)
		{
			for (int z = 2; z < levelMatrix.GetLength(1) - 2; z++)
			{
				if (levelMatrix[x,z] == 5 || levelMatrix[x,z] == 11)
				{					
					// chance to spawn special tiles
					float specialChance = Random.value;
					
					// 13 = enemy, 14 = treasure, 15 = trap
					if (specialChance > 1.0f - enemyChance)
						levelMatrix[x,z] = 13;
					else if (specialChance > 1.0f - enemyChance - treasureChance)
						levelMatrix[x,z] = 14;
					else if (specialChance > 1.0f - enemyChance - treasureChance - trapChance)
						levelMatrix[x,z] = 15;
					else if (specialChance > 1.0f - enemyChance - treasureChance - trapChance - torchChance)
						levelMatrix[x,z] = 6;
				}
			}
		}
		
		return;
	}
	
	// passes over the levelMatrix to check each tile for possible tweaks
	void refineDungeon()
	{
		// go over all tiles
		for (int x = 0; x < levelMatrix.GetLength(0); x++)
		{
			for (int z = 0; z < levelMatrix.GetLength(1); z++)
			{
				// if border tiles, check if they can be hidden
				if (x == 0 || x == levelMatrix.GetLength(0) - 1 || z == 0 || z == levelMatrix.GetLength(1) - 1)
				{
					if (x == 0 && levelMatrix[x+1,z] != 5)
					{
						levelMatrix[x,z] = -1;
					}
					else if (x == levelMatrix.GetLength(0) - 1 && levelMatrix[x-1,z] != 5)
					{
						levelMatrix[x,z] = -1;
					}
					else if (z == 0 && levelMatrix[x,z+1] != 5)
					{
						levelMatrix[x,z] = -1;
					}
					else if (z == levelMatrix.GetLength(1) - 1 && levelMatrix[x,z-1] != 5)
					{
						levelMatrix[x,z] = -1;
					}
				}
				// tweaks regarding surrounding tiles
				else //if (levelMatrix[x,z] == 5 || levelMatrix[x,z] == 11)
				{
					int wallCount = 0;
					
					// check surrounding tiles
					for (int i = -1; i < 2; i++)
					{
						for (int j = -1; j < 2; j++)
						{					
							if (i == 0 && j == 0)
								continue;
							else if (levelMatrix[x+i,z+j] == 0 || levelMatrix[x+i,z+j] == -1)
								wallCount++;
						}
					}
					
					// don't draw unnecessary tiles
					if (wallCount == 8)
						levelMatrix[x,z] = -1;
					// fill in unreachable floor tiles
					else if (wallCount >= 5)
						levelMatrix[x,z] = 0;
				}
			}
		}
		
		return;
	}
	
	// draws the contents of the dungeon in the scene
	void showDungeon()
	{
		for (int x = 0; x < levelMatrix.GetLength(0); x++)
		{
			for (int z = 0; z < levelMatrix.GetLength(1); z++)
			{
				// code for if we want to not draw certain tiles
				// -1 = empty, 0 = black walls, 5 = white floor
				if (levelMatrix[x,z] == -1)// || levelMatrix[x,z] == 0)
					continue;
				
				// 0 = dev, 1 = dungeon (not implemented yet)
				if (tileStyle == 0)
				{
					GameObject obj = getDevTile(levelMatrix[x,z]);
					obj.transform.position = new Vector3(x, 0.0f, z);
					tileList.Add(obj);
				}
				else if (tileStyle == 1)
				{
					GameObject obj = getDungeonTile(levelMatrix[x,z]);
					obj.transform.position = new Vector3(x, 0.0f, z);
					tileList.Add(obj);
				}
				
			}
		}
		
		return;
	}
	
	// actually creates enemies, chests, traps, and other things
	void populateDungeon()
	{		
		for (int x = 0; x < levelMatrix.GetLength(0); x++)
		{
			for (int z = 0; z < levelMatrix.GetLength(1); z++)
			{
				GameObject obj;
				
				// spawns enemies on enemy tiles (will eventually be weighted randomness, currently is just goblins)
				if (levelMatrix[x,z] == 13 && distance(startingLocation, new int[2] {x,z}) > 15f)
				{
					obj = (GameObject)Instantiate(goblinObject, new Vector3(x, 0.2f, z), new Quaternion());
					enemyList.Add(obj);
				}
				// spawns treasure chests
				else if (levelMatrix[x,z] == 14)
				{
					obj = (GameObject)Instantiate(chestObject, new Vector3(x, 0.15f, z), new Quaternion() * Quaternion.Euler(0f,Random.Range(0,4)*90f,0f) );
					chestList.Add(obj);
				}
				// spawns torches
				else if (levelMatrix[x,z] == 6)
				{
					obj = (GameObject)Instantiate(torchObject, new Vector3(x, 0.0f, z), new Quaternion() * Quaternion.Euler(0f,Random.Range(0,4)*90f,0f));
					torchList.Add(obj);
				}
			}
		}
		
		return;
	}
	
	// returns the appropriate dev tile
	GameObject getDevTile(int tileNum)
	{
		switch (tileNum)
		{
			case 0:
				return (GameObject)Instantiate(wallTile);
			case 5:
				return (GameObject)Instantiate(basicTile);
			case 11:
				return (GameObject)Instantiate(pathTile);
			case 13:
				return (GameObject)Instantiate(enemyTile);
			case 14:
				return (GameObject)Instantiate(treasureTile);
			case 15:
				return (GameObject)Instantiate(trapTile);
			case 99:
				return (GameObject)Instantiate(exitTile);
			default:
				return GameObject.CreatePrimitive(PrimitiveType.Sphere);
		}
	}
	
	// returns the appropriate dungeon tile
	GameObject getDungeonTile(int tileNum)
	{
		switch (tileNum)
		{
			case 0:
				return (GameObject)Instantiate(wallTile);
			case 5: // plain tile
			case 6: // torch tile
			case 11:// corridor tile
			case 13:// enemy tile
			case 14:// treasure tile
				return (GameObject)Instantiate(basicTile);
			case 15:
				return (GameObject)Instantiate(trapList[Random.Range (0,trapList.Count)]);
				//return (GameObject)Instantiate(trapTile);
			case 99:
				return (GameObject)Instantiate(exitTile);
			default:
				return GameObject.CreatePrimitive(PrimitiveType.Sphere);
		}
	}
}
