using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Market_Data : Interface_Data {

  public int firewood_stored = 0;
  public int firewood_ExpectedStored = 0;  

  public int food_meat_stored = 0;
  public int food_meat_ExpectedStored = 0; 

  public int food_vegetales_stored = 0;
  public int food_vegetales_ExpectedStored = 0; 

  public int food_fish_stored = 0;  
  public int food_fish_ExpectedStored = 0; 

  public int cloths_stored = 0;
  public int cloths_ExpectedStored = 0; 

  public int utilities_stored = 0;
  public int utilities_ExpectedStored = 0;  

  public int weapons_stored = 0;
  public int weapons_ExpectedStored = 0; 

}

public class Market : BasicBuilds, Interface_Save, Interface_UI {



  //UI
  public GameObject UI_prefab;

  //Save data
  SaveDataStructure saveData_;
  public Market_Data data_ = null;
  public bool isloaded = false;

  //NPC
  public GameObject npcPrefab_;

  // Use this for initialization
  void Start () {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "market", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);
    //Events Register  
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);
    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/Market",
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
      data_ = new Market_Data();      
    }
        
    if (!isloaded) {
      
      
      

      base.AddNpc( 0, npcPrefab_, false);
      base.AddNpc( 1, npcPrefab_, false);
      base.AddNpc( 2, npcPrefab_, false);
      base.AddNpc( 3, npcPrefab_, false);
      base.AddNpc( 4, npcPrefab_, false);
      

    }
  }

  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "market", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);
    //Remove Envets    
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);

    base.CanselNPCSend(0, "wood", 100, true, false);
    base.CanselNPCSend(1, "meat", 100, true, false);
    base.CanselNPCSend(2, "vegetables", 100, true, false);
    base.CanselNPCSend(3, "fish", 100, true, false);
    base.CanselNPCSend(4, "utilities", 100, true, false);

    base.Destroy();
  }

  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);

    if (currentworkers < 5) {
      float rand = Random.Range(0.0f, 1.0f);

      float margen = currentworkers / 5.0f;

      if(rand > margen) {
        return;
      }

    }

    //Find supply firewood 0
    {
      int canBeSended = 0;
      int currentNPC_ID = 0;

      if (data_.firewood_stored <= 100.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "wood", 100, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "wood", 100, true, false, ref canBeSended)) {
        data_.firewood_stored += 100;
        data_.firewood_ExpectedStored += 100;
      }
    }

    //Find supply food_meat 1    
    {
      int canBeSended = 0;
      int currentNPC_ID = 1;

      if (data_.food_meat_stored <= 100.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "meat", 100, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "meat", 100, true, false, ref canBeSended)) {
        data_.food_meat_stored += 100;
        data_.food_meat_ExpectedStored += 100;
      }
    }

    //Find supply food_vegetales 2
    {
      int canBeSended = 0;
      int currentNPC_ID = 2;

      if (data_.food_vegetales_stored <= 100.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "vegetables", 100, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "vegetables", 100, true, false, ref canBeSended)) {
        data_.food_vegetales_stored += 100;
        data_.food_vegetales_ExpectedStored += 100;
      }
    }

    //Find supply food_fish 3
    {
      int canBeSended = 0;
      int currentNPC_ID = 3;

      if (data_.food_fish_stored <= 100.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "fish", 100, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "fish", 100, true, false, ref canBeSended)) {
        data_.food_fish_stored += 100;
        data_.food_fish_ExpectedStored += 100;
      }
    }

    //Find supply utilities 4
    {
      int canBeSended = 0;
      int currentNPC_ID = 4;

      if (data_.utilities_stored <= 100.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "utilities", 100, true, false);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "utilities", 100, true, false, ref canBeSended)) {
        data_.utilities_stored += 100;
        data_.utilities_ExpectedStored += 100;
      }
    }

  }
  
  public bool RecervStorage(string productName, int productCount, bool reserv) {
        
    bool toReturn = false;

    switch (productName) {

      case "firewood": {        
        if (productCount <= data_.firewood_ExpectedStored) {

          if (reserv) {
            data_.firewood_ExpectedStored -= productCount;           
          }
          toReturn = true;
        }
        break;
      }

      case "meat": {
        if (productCount <= data_.food_meat_ExpectedStored) {

          if (reserv) {
            data_.food_meat_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

      case "vegetables": {
        if (productCount <= data_.food_vegetales_ExpectedStored) {

          if (reserv) {
            data_.food_vegetales_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

      case "fish": {
        if (productCount <= data_.food_fish_ExpectedStored) {

          if (reserv) {
            data_.food_fish_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

      case "cloths": {
        if (productCount <= data_.cloths_ExpectedStored) {

          if (reserv) {
            data_.cloths_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

      case "utilities": {
        if (productCount <= data_.utilities_ExpectedStored) {

          if (reserv) {
            data_.utilities_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

      case "weapons": {
        if (productCount <= data_.weapons_ExpectedStored) {

          if (reserv) {
            data_.weapons_ExpectedStored -= productCount;            
          }
          toReturn = true;
        }
        break;
      }

    }

    return toReturn;
  }

  public void CancelReservation(string productName, int productCount) {

    switch (productName) {

      case "firewood": {
        data_.firewood_ExpectedStored += productCount;        
        break;
      }

      case "meat": {
        data_.food_meat_ExpectedStored += productCount;        
        break;
      }

      case "vegetables": {
        data_.food_vegetales_ExpectedStored += productCount;        
        break;
      }

      case "fish": {
        data_.food_fish_ExpectedStored += productCount;        
        break;
      }

      case "cloths": {
        data_.cloths_ExpectedStored += productCount;        
        break;
      }

      case "utilities": {
        data_.utilities_ExpectedStored += productCount;        
        break;
      }

      case "weapons": {
        data_.weapons_ExpectedStored += productCount;        
        break;
      }

    }

  }


  public bool TakeStorage(string productName, int productCount) {

    bool ToReturn = false;

    switch (productName) {

      case "firewood": {
        if (productCount <= data_.firewood_stored) {
          data_.firewood_stored -= productCount;          
          ToReturn = true;
        }
        break;
      }

      case "meat": {
        if (productCount <= data_.food_meat_stored) {
          data_.food_meat_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

      case "vegetables": {
        if (productCount <= data_.food_vegetales_stored) {
          data_.food_vegetales_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

      case "fish": {
        if (productCount <= data_.food_fish_stored) {
          data_.food_fish_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

      case "cloths": {
        if (productCount <= data_.cloths_stored) {
          data_.cloths_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

      case "utilities": {
        if (productCount <= data_.utilities_stored) {
          data_.utilities_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

      case "weapons": {
        if (productCount <= data_.weapons_stored) {
          data_.weapons_stored -= productCount;
          ToReturn = true;
        }
        break;
      }

    }

    return ToReturn;
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

    data_ = loadedData.allDataToSave_[0] as Market_Data;

    if (null != data_) {

      for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
        base.LoadData(loadedData.allDataToSave_[i]);
      }

      isloaded = true;

    }
  }

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }


  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    MarketUI customUI = currentCustomUI.GetComponent<MarketUI>();

    if (null != customUI) {
      customUI.myOwnerData = data_;
    }

  }


  


}
