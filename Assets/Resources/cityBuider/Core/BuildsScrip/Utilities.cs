using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Utilities_Data : Interface_Data {
  public int woodStored = 0;
  public int utilitiesStored = 0;
  public float currentUtilitieProduction = 0.0f;
}

public class Utilities : BasicBuilds, Interface_Save, Interface_UI {
  
  //UI
  public GameObject UI_prefab;

  //Save data
  SaveDataStructure saveData_;
  Utilities_Data data_ = null;
  public bool isloaded = false;

  //NPC  
  public GameObject npc_carry_prefab;

  //
  public float UtilitiesProductionTimeInSecond;

  // Use this for initialization
  void Start () {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "utilities", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);
    //Events Register  
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);
    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/UtilitiesFactory",
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


    if (data_ == null) {
      data_ = new Utilities_Data();       
    }

    
    
    if (!isloaded) {      
      base.AddNpc(0, npc_carry_prefab, false);
      base.AddNpc(1, npc_carry_prefab, false);
    }


  }

  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "utilities", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);
    //Remove Envets    
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);

    base.CanselNPCSend(0, "wood", 50, true, false);
    base.CanselNPCSend(1, "utilities", 25, false, false);
    base.Destroy();
  }

  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);

    if (data_.woodStored >= 25 && data_.utilitiesStored < 100 && currentworkers >= 3) {
      data_.currentUtilitieProduction += (1.0f / UtilitiesProductionTimeInSecond) * deltaTime;

      if(data_.currentUtilitieProduction >= 1.0f) {
        data_.currentUtilitieProduction = 0.0f;
        data_.woodStored -= 25;
        data_.utilitiesStored += 25;
        Game_Manager.resourceSys.AddResourseProduced("utilities", 25.0f);
        Game_Manager.resourceSys.AddResourseConsumed("wood", 25.0f);
      }

    }

    //Send utilities
    {
      int canBeSended = 0;
      int currentNPC_ID = 1;

      if (data_.utilitiesStored >= 100) {
        base.ConfigNPCToSend(currentNPC_ID, "utilities", 100, false, false);
      }

      base.UpdateNPCToSend(currentNPC_ID, "utilities", 100, false, false, ref canBeSended);

      if (canBeSended > 0) {
        data_.utilitiesStored -= 100;
      } else if (canBeSended < 0) {
        data_.utilitiesStored += 100;
      }
    }

    if (currentworkers < 7) {
      float rand = Random.Range(0.0f, 1.0f);

      float margen = currentworkers / 7.0f;

      if (rand > margen) {
        return;
      }

    }

    //Find supply wood
    {
      int canBeSended = 0;
      int currentNPC_ID = 0;

      if (data_.woodStored <= 50) {
        base.ConfigNPCToSend(currentNPC_ID, "wood", 50, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "wood", 50, true, false, ref canBeSended)) {
        data_.woodStored += 50;        
      }
    }

    

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

    data_ = loadedData.allDataToSave_[0] as Utilities_Data;

    for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
      base.LoadData(loadedData.allDataToSave_[i]);
    }

    isloaded = true;

  }

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }

  
  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    UtilitiesFactoryUI myUI = currentCustomUI.GetComponent<UtilitiesFactoryUI>();

    if (null != myUI) {
      myUI.myOwnerData = data_;
    }

  }
  


}
