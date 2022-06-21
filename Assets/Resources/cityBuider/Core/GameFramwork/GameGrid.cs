using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



public class GameGrid {

  int sizeX_;
  int sizeY_;
  PathNode[] grid;

  public GameGrid(GameGrid other){
    sizeX_ = other.sizeX_;
    sizeY_ = other.sizeY_;

    grid = new PathNode[sizeX_ * sizeY_];
    for (int y = 0; y < sizeY_; y++) {
      for (int x = 0; x < sizeX_; x++) {

        grid[(y * sizeX_) + x] = new PathNode {
          x = x,
          y = y,
          isWalkable = other.grid[(y * sizeX_) + x].isWalkable,
          grid = this
        };

      }
    }
  }

  public GameGrid() { }

  public void Init(int sizeX, int sizeY, bool initWalk) {
    sizeX_ = sizeX;
    sizeY_ = sizeY;

    grid = new PathNode[sizeY * sizeX];
    for(int y=0; y < sizeY_; y++) {
      for(int x=0; x < sizeX_; x++) {

        grid[(y * sizeX_) + x] = new PathNode {
          x = x,
          y = y,
          isWalkable = initWalk,
          grid = this
        };

      }
    }
  }
  public int Width() { return sizeX_; } 
  public int Height(){ return sizeY_; }

  public PathNode GetNode(int x, int y) {
    if(x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
      return grid[(y * sizeX_) + x];
    }
    return null;
  }

  public void SetObstaclesList(Transform[] obstacles) {

    for(int i=0; i< obstacles.Length; i++) {
      int x = Mathf.RoundToInt(obstacles[i].position.x);
      int y = Mathf.RoundToInt(obstacles[i].position.z);
      if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
        grid[(y * sizeX_) + x].isObsrtucted = true;
      }
    }

  }

  public void SetStreetList(Transform[] obstacles) {

    for (int i = 0; i < obstacles.Length; i++) {
      int x = Mathf.RoundToInt(obstacles[i].position.x);
      int y = Mathf.RoundToInt(obstacles[i].position.z);
      if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
        grid[(y * sizeX_) + x].isWalkable = true;
      }
    }

  }


  public void SetObstacle(int x, int y) {
    if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
      grid[(y * sizeX_) + x].isObsrtucted = true;
    }
  }

  public void SetStreet(Vector2Int position) {
    int x = position.x;
    int y = position.y;
    if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
      grid[(y * sizeX_) + x].isWalkable = true;
    }
  }

  public void RemoveObstacle(int x, int y) {
    if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
      grid[(y * sizeX_) + x].isObsrtucted = false;
    }
  }

  public void RemoveStreet(Vector3 position) {
    int x = (int)position.x;
    int y = (int)position.z;
    if (x >= 0 && x < sizeX_ && y >= 0 && y < sizeY_) {
      grid[(y * sizeX_) + x].isWalkable = false;
    }
  }


  public void CloneGrid(GameGrid originalGrid, bool canWalkInGrass) {

    for(int i=0; i< sizeX_ * sizeY_; i++) {

      grid[i].isWalkable = originalGrid.grid[i].isWalkable;
      grid[i].grass = canWalkInGrass;
      grid[i].isObsrtucted = originalGrid.grid[i].isObsrtucted;
    }

  }
  

}
