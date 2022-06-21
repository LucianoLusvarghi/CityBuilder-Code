using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface Interface_Save {

  SaveDataStructure GetSaveData();
  void LoadData(SaveDataStructure loadedData);
  
  GameObject GetGameObjectInScene();

}
