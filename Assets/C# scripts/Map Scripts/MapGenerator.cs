using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour 
{
	public TextAsset puzzleFile;
	public TextAsset puzzleFile1;
	public TextAsset puzzleFile2;
	public TextAsset puzzleFile3;
	string PuzzelRoom = "";
	string [] puzzleStringArr;
	int [,] puzzleIntArr;

	public List <Tile> TilePrefabs = new List<Tile>();

	List <Tile> Tiles = new List<Tile>();

	public int mapWidth;
	public int mapHeight;
	public int smoothAmounth;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	int [,] map;
	int [,] popMap;
	Tile[,] tileArray;
	
	void Start()
	{
		//GenerateMap();

	}

	void Update ()
	{

	}

	public List<Tile> getTiles(){
		return Tiles;
	}

	public int[,] getMap(){
		return map;
	}

	public Tile[,] getTileArray(){
		return tileArray;
	}

	public int[,] getPopMap(){
		return popMap;
	}

	void ClearMap(){
		foreach (Tile tile in Tiles) {
			Destroy(tile.gameObject);
		}
	}

	void GetRoom(TextAsset puzzle)
	{
		PuzzelRoom = puzzle.text;
		//puzzleStringArr = new string[];
		puzzleStringArr =  PuzzelRoom.Split('\n');
		puzzleIntArr = new int[puzzleStringArr.Length, puzzleStringArr.Length];
		for(int x = 0; x < puzzleStringArr.Length; x++)
		{
			string [] tempArr = puzzleStringArr[x].Split(' ');

			for(int y = 0; y < puzzleStringArr.Length; y++)
			{
				int tempInt = Int32.Parse(tempArr[y]);
				puzzleIntArr[x,y] = tempInt;

				//Debug.Log(tempArr[y]);
			}
		}
	}

	void PlaceRoom(int X, int Y)
	{
		for(int x = 0; x < puzzleStringArr.Length; x++)
		{
			for(int y = 0; y < puzzleStringArr.Length; y++)
			{
				popMap[X + x, Y + y] = puzzleIntArr[x,y];
				//Debug.Log(popMap[X + x, Y + y]);

			}
		}
	}

	public void GenerateMap ()
	{
		if (Tiles.Count > 0) {
			ClearMap();
		}
		map = new int[mapWidth, mapHeight];
		popMap = new int[mapWidth, mapHeight];

		//test
		tileArray = new Tile[mapWidth, mapHeight];
		//test

		RandomFillMap();

		for(int i = 0; i < smoothAmounth; i++)
		{
			SmoothMap();
		}

		PopulateMap();
		GetRoom(puzzleFile);
		PlaceRoom(8,90);
		PlaceRoom(14,84);
		PlaceRoom(6,80);
		GetRoom(puzzleFile1);
		PlaceRoom(5,70);
		PlaceRoom(16,64);
		PlaceRoom(11,58);
		GetRoom(puzzleFile2);
		PlaceRoom(12,41);
		PlaceRoom(6,36);
		PlaceRoom(17,30);
		GetRoom(puzzleFile3);
		PlaceRoom(5,21);
		PlaceRoom(12,15);
		PlaceRoom(6,9);
		InstantiaTiles();
	}

	void RandomFillMap ()
	{
		if(useRandomSeed)
		{
			seed = Time.time.ToString();
		}

		System.Random pseudoRandomNumberGen = new System.Random(seed.GetHashCode());

		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				if(x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
				{
					map[x,y] = 1;
				}
				else
				{
					map[x,y] = (pseudoRandomNumberGen.Next(0,100) < randomFillPercent)? 1:0;
				}
			}
		}
	}

	void SmoothMap()
	{
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				int neighbourWallTiles = GetSuroundingWallCount(x,y);

				if(neighbourWallTiles > 5)
				{
					map[x,y] = 1;
				}
				else if (neighbourWallTiles < 4)
				{
					map[x,y] = 0;
				}
			}
		}
	}

	public List<Vector2> getCone(int x, int y, int range, Vector2 direction){
		List<Vector2> tempList = new List<Vector2> ();

		for(int i=0;i<=range;i++){
			tempList.Add(new Vector2(x + (direction.x*i),y + (direction.y*i)));

			for(int j=0;j<i;j++){
				if(direction.y == -1 || direction.y == 1){
					tempList.Add(new Vector2(x-j, y + (direction.y*i)));
					tempList.Add(new Vector2(x+j, y + (direction.y*i)));
				}
				else{
					tempList.Add(new Vector2(x + (direction.x*i), y+j));
					tempList.Add(new Vector2(x + (direction.x*i), y-j));
				}
			}
		}

		return tempList;
	}

	public List<Vector2> testNeighbours(int x, int y){
		List<Vector2> tempList = new List<Vector2> ();

		tempList.Add(new Vector2(x,y+1));
		tempList.Add(new Vector2(x+1,y));
		tempList.Add(new Vector2(x,y-1));
		tempList.Add(new Vector2(x-1,y));

		return tempList;
	}

	public List<Tile> getNeighbours(int x, int y){
		List<Tile> tempList = new List<Tile> ();
		
		//top
		if (y < mapHeight-1 && y > 0 && x <= mapWidth - 1 && x >= 0) {
			tempList.Add(tileArray[x,y+1]);
			
		}
		
		//right
		if (y > 0 && y < mapHeight - 1 && x < mapWidth - 1 && x > 0) {
			tempList.Add(tileArray[x+1,y]);
			
		}
		
		//bottom
		if (y > 0 && y <= mapHeight && x <= mapWidth - 1 && x >= 0) {
			tempList.Add(tileArray[x,y-1]);
			
		}
		
		//left
		if (y > 0 && y < mapHeight - 1 && x > 0 && x < mapWidth -1) {
			tempList.Add(tileArray[x-1,y]);
			
		}
		
		return tempList;
	}

	int GetSuroundingWallCount(int gridX, int gridY)
	{
		int wallCount = 0;
		for(int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for(int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if(neighbourX >= 0 && neighbourX < mapWidth && neighbourY >= 0 && neighbourY < mapHeight)
				{
					if(neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else
				{
					wallCount ++;
				}
			}
		}
		return wallCount;
	}

	
	void PopulateMap()
	{
		Tiles = new List<Tile>();
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				if(map[x,y] == 1 && popMap[x,y] == 0)
				{	
					if( y < mapHeight / 4)
					{
						int temp = UnityEngine.Random.Range(0,40);
						switch (temp)
						{
						case 0:
							popMap[x,y] = 14;
							break;
						case 1:
							popMap[x,y] = 15;
							break;
						case 2:
							popMap[x,y] = 16;
							break;
						case 3:
							popMap[x,y] = 21;
							break;
						default:
							popMap[x,y] = 4;
							break;
						}
					}
					if( y < mapHeight / 2 && y >= mapHeight / 4)
					{
						
						int temp = UnityEngine.Random.Range(0,40);
						switch (temp)
						{
						case 0:
							popMap[x,y] = 11;
							break;
						case 1:
							popMap[x,y] = 12;
							break;
						case 2:
							popMap[x,y] = 13;
							break;
						case 3:
							popMap[x,y] = 21;
							break;
						default:
							popMap[x,y] = 3;
							break;
						}
						
					}
					if( y < mapHeight - mapHeight / 4 && y >= mapHeight / 2)
					{
						int temp = UnityEngine.Random.Range(0,40);
						switch (temp)
						{
						case 0:
							popMap[x,y] = 8;
							break;
						case 1:
							popMap[x,y] = 9;
							break;
						case 2:
							popMap[x,y] = 10;
							break;
						case 3:
							popMap[x,y] = 21;
							break;
						default:
							popMap[x,y] = 2;
							break;
						}
						
					}
					if( y >= mapHeight - mapHeight / 4 )
					{
						int temp = UnityEngine.Random.Range(0,40);
						switch (temp)
						{
						case 0:
							popMap[x,y] = 5;
							break;
						case 1:
							popMap[x,y] = 6;
							break;
						case 2:
							popMap[x,y] = 7;
							break;
						case 3:
							popMap[x,y] = 21;
							break;
						default:
							popMap[x,y] = 1;
							break;
						}
					}
				}
				else
				{
					popMap[x,y] = 0;
				}
			}
		}
	}

	void InstantiaTiles()
	{
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				if(popMap[x,y] != 0)
				{

					Tile tile;
					Vector3 pos = new Vector3(x, y, 0);
					tile = Instantiate(TilePrefabs[popMap[x,y]],pos,Quaternion.identity) as Tile;
					Tiles.Add(tile);
					tileArray[x,y] = tile;
					tile.setHealth(300);
					tile.setPoints(5);
					tile.setID(popMap[x,y]);
					tile.pos = pos;
					if(popMap[x,y] == 21)
					{
						tile.setHealth(800);
						tile.setPoints(-200);
					}
					if(popMap[x,y] > 4 && popMap[x,y] < 21)
					{
						tile.setHealth(600);
						tile.setPoints(popMap[x,y] * 100);
					}


				}

				
			}
		}
	}
	/*
	void PopulateMap()
	{
		Tiles = new List<Tile>();
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				if(map[x,y] == 1)
				{
					//Vector3 pos = new Vector3(-mapWidth/2 + x + .5f, -mapHeight/2 + y + .5f, 0);
					Vector3 pos = new Vector3(x, y, 0);
					int temp = UnityEngine.Random.Range(0,100);

					Tile tile;

					if(temp == 95)
					{
						tile = Instantiate(_TileSpecial1,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;

						tile.setPoints(50);
					}
					else if(temp == 96)
					{
						tile = Instantiate(_TileSpecial2,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;

						tile.setPoints(30);
					}
					else if(temp == 97)
					{
						tile = Instantiate(_TileSpecial3,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;

						tile.setPoints(20);
					}
					else if(temp == 98)
					{
						tile = Instantiate(_TileSpecial4,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;

						tile.setPoints(60);
					}
					else if(temp == 99)
					{
						tile = Instantiate(_TileSpecial5,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;

						tile.setPoints(10);
					}
					else 
					{
						tile = Instantiate(_TileDirt,pos,Quaternion.identity) as Tile;
						Tiles.Add(tile);
						tileArray[x,y] = tile;
					}
				}
			}
		}
	}
	*/
//	void OnDrawGizmos ()
//	{
//		if(map != null)
//		{
//			for(int x = 0; x < mapWidth; x++)
//			{
//				for(int y = 0; y < mapHeight; y++)
//				{
//					Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
//					Vector3 pos = new Vector3(-mapWidth/2 + x + .5f, 0, -mapHeight/2 + y + .5f);
//					Gizmos.DrawCube(pos, Vector3.one);
//				}
//			}
//		}
//	}

}
