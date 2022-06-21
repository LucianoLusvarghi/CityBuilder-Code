using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Crops_Data : Interface_Data {

  public float currentGrowthStatus_;
  public SaveDataStructure saveData_;
  public int myType_;
  public bool npcIncomming_;
  public int npcIncommingID_;
}

public class Crops : BasicBuilds, Interface_UI {

  public Farms myFarm;
  public int myType_;

  

  public float timeToFullGrowth_;


  //Save data    
  Crops_Data data_ = null;
  public bool isloaded = false;
  public bool isPrefab = true;

  //UI
  public GameObject UI_prefab;

  // Use this for initialization
  void Start() {

    base.Init();
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "crops", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);

    myFarm.AddCrop(this);

    if (null == data_ && !isPrefab) {
      data_ = new Crops_Data {
        currentGrowthStatus_ = 0.0f,
        myType_ = myType_
      };

      //Save data config
      {

        data_.saveData_ = new SaveDataStructure {
          allDataToSave_ = new List<Interface_Data>(),
          prefab = "cityBuider/Placeables/Crops" + myType_,
          position = new float[3],
          rotation = new float[4]
        };

        data_.saveData_.position[0] = transform.position.x;
        data_.saveData_.position[1] = transform.position.y;
        data_.saveData_.position[2] = transform.position.z;
                
        data_.saveData_.rotation[0] = transform.rotation.x;
        data_.saveData_.rotation[1] = transform.rotation.y;
        data_.saveData_.rotation[2] = transform.rotation.z;
        data_.saveData_.rotation[3] = transform.rotation.w;

      }

    }


    

  }

  private void OnDestroy() {

    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "crops", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);

    myFarm.LessCrop(this);
    base.Destroy();

  }

  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);
    if (null != data_) {
      if (data_.currentGrowthStatus_ < 1.0f && !isPrefab) {
        data_.currentGrowthStatus_ += (1.0f / timeToFullGrowth_) * deltaTime;
        if(data_.currentGrowthStatus_ > 1.0f) {
          data_.currentGrowthStatus_ = 1.0f;
        }
      }
    }
  }

  public bool Harvest() {

    bool toReturn = false;

    if(data_.currentGrowthStatus_ > 0.999f) {
      data_.currentGrowthStatus_ = 0.0f;
      toReturn = true;
    }

    return toReturn;

  }

  public bool Canharvest() {
    return (data_.currentGrowthStatus_ > 0.999f);
  }


  public Crops_Data Data() {
    return data_;
  }

  public void LoadData(Crops_Data newData) {
    data_ = newData;
  }

  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    CropsUI customUI = currentCustomUI.GetComponent<CropsUI>();

    if (null != customUI) {
      customUI.myOwnerData = data_;
    }

  }


}
