using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController{

  public enum CellType {
    grass = 0,
    path,
    build,
    enviroment,
  };

	public struct Cell {
    public int height;

    public CellType currentState;
  };

  
  public struct Build {
    public string name;
    public Vector2Int mapPosition;
    public Vector2Int cellsSize;
    public GameObject build_ptr;
  }

  int mapSize_x;
  int mapSize_y;

  Cell[] currentMap;

  Cell errorCell;

  List<Build> buildsList_;

  GameGrid pathCalculationGrid;

  public void Init(int mapSizeX, int mapSizeY, CellType defaultValue) {
        
    pathCalculationGrid = new GameGrid();
    pathCalculationGrid.Init(mapSizeX, mapSizeY, defaultValue == CellType.path ? true : false);

    currentMap = new Cell[mapSizeX * mapSizeY];
    mapSize_x = mapSizeX;
    mapSize_y = mapSizeY;

    for (int i=0; i< mapSize_x * mapSize_y; i++) {      
      currentMap[i].height = 0;
      currentMap[i].currentState = defaultValue;
    }

    buildsList_ = new List<Build>();
    errorCell = new Cell {
      currentState = CellType.build
    };
  }

  public Cell GetCell(uint x, uint y) {
    if(x > mapSize_x || y > mapSize_y) {
      return errorCell;
    }
    return currentMap[(y * mapSize_x) + x];
  }

  public void RegiterBuild(Vector2Int position, Vector2Int cellSize, string name, GameObject theBuild_ptr) {

    if(Mathf.Abs(position.x) > mapSize_x || Mathf.Abs(position.y) > mapSize_y) {
      return;
    }

    Build newBuild;

    newBuild.mapPosition = position;
    newBuild.cellsSize = cellSize;
    newBuild.name = name;
    newBuild.build_ptr = theBuild_ptr;

    buildsList_.Add(newBuild);

    for(int y=0; y < cellSize.y; y++) {
      for(int x=0; x< cellSize.x; x++) {
        currentMap[( (newBuild.mapPosition.y + y) * mapSize_x) + (newBuild.mapPosition.x + x)].currentState = CellType.build;
        pathCalculationGrid.SetObstacle(newBuild.mapPosition.x + x, newBuild.mapPosition.y + y);
      }
    }

    

  }

  public void RemoveBuild(Vector2Int position, Vector2Int cellSize, string name, GameObject theBuild_ptr) {
    if (Mathf.Abs(position.x) > mapSize_x || Mathf.Abs(position.y) > mapSize_y) {
      return;
    }

    Build oldBuild;
    oldBuild.mapPosition = position;
    oldBuild.cellsSize = cellSize;
    oldBuild.name = name;
    oldBuild.build_ptr = theBuild_ptr;

    buildsList_.Remove(oldBuild);

    for (int y = 0; y < cellSize.y; y++) {
      for (int x = 0; x < cellSize.x; x++) {
        currentMap[((oldBuild.mapPosition.y + y) * mapSize_x) + (oldBuild.mapPosition.x + x)].currentState = CellType.grass;
        pathCalculationGrid.RemoveObstacle(oldBuild.mapPosition.x + x, oldBuild.mapPosition.y + y);
      }
    }

  }


  public void AddPath(Vector2Int position) {
    currentMap[(position.y * mapSize_x) + position.x].currentState = CellType.path;
    pathCalculationGrid.SetStreet(new Vector2Int(position.x, position.y));
  }

  public void LessPath(Vector2Int position) {
    currentMap[(position.y * mapSize_x) + position.x].currentState = CellType.grass;
    pathCalculationGrid.RemoveStreet(new Vector3(position.x, 0.0f, position.y));
  }

  public List<Build> GetBuildsListByName(string name) {

    List<Build> currentList = new List<Build>();

    foreach(Build currentBuild in buildsList_) {

      if(currentBuild.name == name) {
        currentList.Add(currentBuild);
      }

    }

    return currentList;
  }

  public GameGrid GetGameGrid() {
    return pathCalculationGrid;
  }

  public int Width() {
    return mapSize_x;
  }

  public int Height() {
    return mapSize_y;
  }

  public Build GetBuildByPosition(int x, int y) {
    Build toReturn = new Build {
      name = "error"
    };

    foreach (Build currentBuild in buildsList_) {

      for(int bx=0; bx< currentBuild.cellsSize.x; bx++) {
        for(int by=0; by < currentBuild.cellsSize.y; by++) {

          if( (currentBuild.mapPosition.x + bx) == x && (currentBuild.mapPosition.y + by) == y) {
            toReturn = currentBuild;
            return toReturn;
          }

        }

      }

    }

    return toReturn;
  }

}
