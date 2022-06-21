using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trees_Data : Interface_Data{

  public float currentGrowthStatus_;   
  public bool npcIncomming_;
  
}

public class Trees : BasicBuilds, Interface_Save{

  public float GrowthTime_;

  //Save data
  SaveDataStructure saveData_ = null;
  Trees_Data data_ = null;
  public bool isloaded = false;

  // Use this for initialization
  void Start () {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "tree", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);

    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/Tree",
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

    if(null == data_){
      data_ = new Trees_Data {
        currentGrowthStatus_ = 1.0f,
        npcIncomming_ = false
      };
    }



  }


  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "tree", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);
    base.Destroy();
  }


  public override void UpdateBuild(float deltaTime) {
    if (data_.currentGrowthStatus_ < 1.0f) {
      data_.currentGrowthStatus_ += (1.0f / GrowthTime_) * deltaTime;
      transform.localScale = new Vector3(data_.currentGrowthStatus_, data_.currentGrowthStatus_, data_.currentGrowthStatus_);
    }
	}

  public bool CanHarvest() {
    return data_.currentGrowthStatus_ >= 1.0f && !data_.npcIncomming_;
  }

  public void Harvest() {
    data_.currentGrowthStatus_ = 0.0f;
    data_.npcIncomming_ = false;
  }

  public bool SendNPC() {

    bool toReturn = false;

    if (!data_.npcIncomming_) {
      data_.npcIncomming_ = true;
      toReturn = true;
    }

    return toReturn;
  }

  public void ResetNPCSend() {
    data_.npcIncomming_ = false;
  }

  public new SaveDataStructure GetSaveData() {
    
    saveData_.allDataToSave_.Clear();
    saveData_.allDataToSave_.Add(data_);

    List<Interface_Data> basicData = base.GetSaveData();

    foreach (Interface_Data basicDataToSave in basicData) {
      saveData_.allDataToSave_.Add(basicDataToSave);
    }

    return saveData_;
  }

  public void LoadData(SaveDataStructure loadedData) {
        
    data_ = loadedData.allDataToSave_[0] as Trees_Data;

    for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
      base.LoadData(loadedData.allDataToSave_[i]);
    }

    isloaded = true;

  }
    
  public GameObject GetGameObjectInScene() {
    return gameObject;
  }


}
