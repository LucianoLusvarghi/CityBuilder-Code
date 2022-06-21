using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class ThreadPath {

  private GameGrid grid_;

  GameGrid referenceGrid_;
  bool canWalkInGrass_ = false;

  bool finished_ = true;
  bool launched_ = false;

  List<PathNode> openList;
  List<PathNode> closedList;
  List<PathNode> finishList;

  Vector2Int startPoint;
  Vector2Int buildSize;

  List<Vector2Int> endsPoints;
  

  ThreadStart start;

  float lastTime;

  System.Random randomGenerator;

  public void Init(int x, int y) {
    grid_ = new GameGrid();
    grid_.Init(x, y, false);

    openList = new List<PathNode>();
    closedList = new List<PathNode>();
    finishList = new List<PathNode>();

    endsPoints = new List<Vector2Int>();
        
    start = new ThreadStart(this.CalculatePath);

    lastTime = Time.time;

    randomGenerator = new System.Random();
    
  }

  public void SetGrid(GameGrid mainGrid, bool canWalkInGrass = false) {    
    referenceGrid_ = mainGrid;
    canWalkInGrass_ = canWalkInGrass;
  }

  public void SetStartPoint(Vector2Int position, Vector2Int size) {
    startPoint = position;
    buildSize = size;
  }

  public void CalculeStartPoint(Vector2Int position, Vector2Int size) {

    if (null == grid_) return;
    startPoint.x = -1;
    startPoint.y = -1;
    List<Vector2Int> posibleStarts = new List<Vector2Int>();

    for(int y=0; y < size.y; y++) {
      for (int x=-1; x <= size.x;x++) {

        if (x < 0 || x == size.x) {
          Vector2Int currentPosition = new Vector2Int {
            x = position.x + x,
            y = position.y + y
          };

          if (grid_.GetNode(currentPosition.x, currentPosition.y).isWalkable) {
            posibleStarts.Add(currentPosition);
          }

        }

      }
    }

    for (int y = -1; y <= size.y; y++) {
      for (int x = 0; x < size.x; x++) {

        if (y < 0 || y == size.y) {
          Vector2Int currentPosition = new Vector2Int {
            x = position.x + x,
            y = position.y + y
          };

          if (grid_.GetNode(currentPosition.x, currentPosition.y).isWalkable) {
            posibleStarts.Add(currentPosition);
          }

        }

      }
    }


    if (posibleStarts.Count > 0) {
      int randStartPoint = randomGenerator.Next(0, posibleStarts.Count - 1);//Random.Range(0, posibleStarts.Count);


      startPoint = posibleStarts[randStartPoint];
    }

  }

  public void SetEndsPoints(List<MapController.Build> ends) {
    
    if(endsPoints == null) {
      endsPoints = new List<Vector2Int>();
    }

    endsPoints.Clear();

    for(int i=0; i < ends.Count; i++) {

      for(int y=0; y< ends[i].cellsSize.y; y++) {
        for(int x=0; x< ends[i].cellsSize.x; x++) {
          endsPoints.Add(new Vector2Int(ends[i].mapPosition.x + x, ends[i].mapPosition.y + y));          
        }
      }
    }
        
  }


  public bool IsFinished(){
    return finished_;
  }
  public bool IsLaunched() {
    return launched_;
  }

  public List<PathNode> GetFinishList() {

    if (finished_) {
      launched_ = false;
      return finishList;
    }

    return null;
  }

  public List<PathNode> TestClesedList() {
    return null;
  }
  public List<PathNode> TestOpendList() {
    return null;
  }

  public bool LaunchThread() {

    float currentTime = Time.time;

    if (!launched_ && (currentTime - lastTime) > 0.1f) {
      finished_ = false;
      Thread pathThreads = new Thread(start);
      pathThreads.Start();
      launched_ = true;

      lastTime = currentTime;
      return true;
    }

    return false;
  }

  private void CalculatePath() {

    finishList.Clear(); //= new List<PathNode>();

    grid_.CloneGrid(referenceGrid_, canWalkInGrass_);

    CalculeStartPoint(startPoint, buildSize);

    if (startPoint.x < 0 || startPoint.y < 0) {
      finished_ = true;
      launched_ = false;
      return;
    }

    for (int y = 0; y < grid_.Height(); y++) {
      for (int x = 0; x < grid_.Width(); x++) {
        grid_.GetNode(x, y).Set_numDestinies(endsPoints.Count);
      }
    }

    for (int i = 0; i < endsPoints.Count; i++) {
      grid_.SetStreet(endsPoints[i]);
    }

    PathNode startNode = grid_.GetNode(startPoint.x, startPoint.y);

    openList.Clear(); //= new List<PathNode> { startNode };
    openList.Add(startNode);

    closedList.Clear(); //= new List<PathNode>();
    

    for (int i = 0; i < endsPoints.Count; i++) {
      startNode.gCost[i] = 0;
      startNode.hCost[i] = CalculateDistance(startPoint, endsPoints[i]);
      startNode.CalculateFCost(i);
    }

    while (openList.Count > 0 && !finished_) {
      PathNode currentNode = GetLowestFCostNode(openList);

      for (int i = 0; i < endsPoints.Count; i++) {
        if (currentNode.y == endsPoints[i].y && currentNode.x == endsPoints[i].x) {
          CalculatePath(currentNode, finishList);
          finished_ = true;          
          return;
        }
      }

      openList.Remove(currentNode);
      closedList.Add(currentNode);

      foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
        if (closedList.Contains(neighbourNode)) continue;
        if (!neighbourNode.isWalkable && !(neighbourNode.grass && !neighbourNode.isObsrtucted) ) {
          closedList.Add(neighbourNode);
          continue;
        }

        for (int i = 0; i < endsPoints.Count; i++) {
          int tentativeGCost = currentNode.gCost[i] + CalculateDistance(new Vector2Int(currentNode.x, currentNode.y), new Vector2Int(neighbourNode.x, neighbourNode.y));

          if (tentativeGCost < neighbourNode.gCost[i]) {
            neighbourNode.cameFromNode[i] = currentNode;
            neighbourNode.gCost[i] = tentativeGCost;
            neighbourNode.hCost[i] = CalculateDistance(new Vector2Int(neighbourNode.x, neighbourNode.y), endsPoints[i]);
            neighbourNode.CalculateFCost(i);

            if (!openList.Contains(neighbourNode)) {
              openList.Add(neighbourNode);
            }
          }

        }//For

      }//For each
      //Thread.Sleep(17);
    }//while



    finished_ = true;
    launched_ = false;
    
  }

  private static int CalculateDistance(Vector2Int a, Vector2Int b) {
    //return Mathf.RoundToInt(Vector2Int.SqrMagnitude(a - b));

    Vector2Int vectorReultant = a - b;
    return (vectorReultant.x * vectorReultant.x) + (vectorReultant.y * vectorReultant.y);
  }
   
  private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
    PathNode lowestFCostNode = pathNodeList[0];
    for (int i = 1; i < pathNodeList.Count; i++) {
      if (pathNodeList[i].GetLowestFCost() < lowestFCostNode.GetLowestFCost()) {
        lowestFCostNode = pathNodeList[i];
      }
    }
    return lowestFCostNode;
  }

  private static void CalculatePath(PathNode endNode, List<PathNode> path) {
    path.Add(endNode);
    int index = 0;
    for (int i = 0; i < endNode.numDestinies; i++) {
      if (endNode.hCost[i] == 0) { index = i; }
    }
    PathNode currentNode = endNode;
    while (currentNode.cameFromNode[index] != null) {
      path.Add(currentNode.cameFromNode[index]);
      currentNode = currentNode.cameFromNode[index];
    }

    path.Reverse();
  }

  private List<PathNode> GetNeighbourList(PathNode currentNode) {
    List<PathNode> newighbourList = new List<PathNode>();
    if (currentNode.x - 1 >= 0) {
      newighbourList.Add(grid_.GetNode(currentNode.x - 1, currentNode.y));
    }
    if (currentNode.x + 1 < grid_.Width()) {
      newighbourList.Add(grid_.GetNode(currentNode.x + 1, currentNode.y));
    }
    if (currentNode.y + 1 < grid_.Height()) {
      newighbourList.Add(grid_.GetNode(currentNode.x, currentNode.y + 1));
    }
    if (currentNode.y - 1 >= 0) {
      newighbourList.Add(grid_.GetNode(currentNode.x, currentNode.y - 1));
    }

    return newighbourList;
  }

}
