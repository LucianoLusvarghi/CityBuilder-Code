using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWell : BasicBuilds, Interface_Save {

  //Save data
  SaveDataStructure saveData_;

  // Use this for initialization
  void Start () {
    base.Init();

    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "waterhole", gameObject);
    base.Game_Manager.saveSys.AddDataToSave(this);
    
    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/waterhole",
        position = new float[3],
        rotation = new float[4]
    };

      saveData_.position[0] = transform.position.x;
      saveData_.position[1] = transform.position.y;
      saveData_.position[2] = transform.position.z;
            
      saveData_.rotation[0] = transform.rotation.x;
      saveData_.rotation[1] = transform.rotation.y;
      saveData_.rotation[2] = transform.rotation.z;
      saveData_.rotation[3] = transform.rotation.w;

    }

  }

  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "waterhole", gameObject);

    base.Destroy();
  }

  public new SaveDataStructure GetSaveData() {
    return saveData_;
  }

  public void LoadData(SaveDataStructure loadedData) {}

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }

}
