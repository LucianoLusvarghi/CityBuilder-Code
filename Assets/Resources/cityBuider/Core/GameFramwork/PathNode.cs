using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathNode{

  public GameGrid grid;

  public int x;
  public int y;

  public int numDestinies;
  public int[] gCost;
  public int[] hCost;
  public int[] fCost;

  public PathNode[] cameFromNode;

  public bool isWalkable;
  public bool grass;
  public bool isObsrtucted;

  public void Set_numDestinies(int newValue) {
    numDestinies = newValue;

    gCost = new int[newValue];
    hCost = new int[newValue];
    fCost = new int[newValue];

    cameFromNode = new PathNode[newValue];

    for (int i=0; i<newValue; i++) {
      gCost[i] = int.MaxValue;
      CalculateFCost(i);
      cameFromNode[i] = null;
    }
  }

  public void CalculateFCost(int i) {
    fCost[i] = gCost[i] + hCost[i];
  }

  public int GetLowestFCost() {
    int lower = fCost[0];
    for(int i = 1; i < numDestinies; i++) {
      if(fCost[i] < lower) {
        lower = fCost[i];
      }
    }
    return lower;
  }

}
