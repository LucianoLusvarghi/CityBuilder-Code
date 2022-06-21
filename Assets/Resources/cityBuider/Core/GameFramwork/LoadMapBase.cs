using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMapBase : MonoBehaviour {

  public Texture2D[] maps;

  public GameObject streetPrefab;
  public GameObject treePrefab;
  public GameObject waterPrefab;
  public GameObject FishZonePrefab;
  public GameObject StartNodePrefab;

  //public Transform mapContainer;

  public Color streetColor;
  public Color treeColor;
  public Color waterColor;
  public Color FishZoneColor;
  public Color StartColor;

  public void LoadMap(int index) {

    Texture2D currentMap = maps[index];



    for(int y=0; y< currentMap.height; y++) {
      for(int x=0; x < currentMap.width; x++) {

        Color color = currentMap.GetPixel(x, y);

        GameObject currentInstance = null;

        if(CompareColors(color, streetColor)) {
          currentInstance = Instantiate(streetPrefab, new Vector3(x, 0.05f, y), new Quaternion());
        }else if (CompareColors(color, treeColor)) {
          Instantiate(treePrefab, new Vector3(x, 0.05f, y), new Quaternion());
        }else if (CompareColors(color, waterColor)) {
          currentInstance = Instantiate(waterPrefab, new Vector3(x, 0.05f, y), new Quaternion());
        }else if(CompareColors(color, FishZoneColor)) {
          currentInstance = Instantiate(FishZonePrefab, new Vector3(x, 0.05f, y), new Quaternion());
        } else if (CompareColors(color, StartColor)) {
          currentInstance = Instantiate(StartNodePrefab, new Vector3(x, 0.05f, y), new Quaternion());
        }

        if (currentInstance) {
          currentInstance.layer = 0;
        }

      }
    }


  }

  bool CompareColors(Color a, Color b) {
    return (a.r == b.r) && (a.g == b.g) && (a.b == b.b);
  }


}
