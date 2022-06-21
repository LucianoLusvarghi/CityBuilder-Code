using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Storage_Data : Interface_Data {

  public Storage.ObjectsStorage[] listOfObjects;
  public Storage.ObjectsStorage[] listOfObjectsRecerved;
  public Storage.ObjectsStorage[] listOfObjectsRecuest;
  public Storage.ObjectsCanStore[] storeManagment;
}


public class Storage : BasicBuilds, Interface_Save, Interface_UI {


  [System.Serializable]
  public struct ObjectsStorage {
    public string name;
    public int count;
    public bool storeAnything;
  };

  [System.Serializable]
  public struct ObjectsCanStore {
    public string name;
    public int max;
    public int current;
    public int spected;
  }

  public GameObject[] SlotVisualation;

  public ObjectsCanStore[] storeManagment_;

  ObjectsStorage[] previsulizationOfListOfObjectsExpected;


  //UI
  public GameObject UI_prefab;

  //Save data
  SaveDataStructure saveData_;  
  public Storage_Data data_ = null;
  public bool isloaded = false;

  // Use this for initialization
  void Start () {
    

    if (!isloaded) {
      data_ = null;
      base.Init();
    }

    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "storage", gameObject);
    //Events Register  
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);

    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/Storage",
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

    previsulizationOfListOfObjectsExpected = new ObjectsStorage[8];
    for(int i=0; i < 8; i++) {
      previsulizationOfListOfObjectsExpected[i].count = 0;
      previsulizationOfListOfObjectsExpected[i].name = "Anything";
    }

    if (data_ == null) {
      data_ = new Storage_Data {
        listOfObjects = new ObjectsStorage[8],
        listOfObjectsRecerved = new ObjectsStorage[8],
        listOfObjectsRecuest = new ObjectsStorage[8],
        storeManagment = new ObjectsCanStore[storeManagment_.Length]
      };

      for (int i=0; i < 8; i++) {
        data_.listOfObjects[i].count = 0;
        data_.listOfObjects[i].name = "Anything";
        data_.listOfObjects[i].storeAnything = true;

        data_.listOfObjectsRecerved[i].count = 0;
        data_.listOfObjectsRecerved[i].name = "Anything";
        data_.listOfObjectsRecerved[i].storeAnything = true;

        data_.listOfObjectsRecuest[i].count = 0;
        data_.listOfObjectsRecuest[i].name = "Anything";
        data_.listOfObjectsRecuest[i].storeAnything = true;

        SlotVisualation[i].SetActive(false);
      }

      for(int i=0; i < storeManagment_.Length; i++) {
        data_.storeManagment[i] = storeManagment_[i];
      }


    }

  }

  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "storage", gameObject);
    //Remove Envets    
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);
    for (int i = 0; i < 8; i++) {

      Game_Manager.resourceSys.ChangeResourseStored(data_.listOfObjects[i].name, -data_.listOfObjects[i].count);
      Game_Manager.resourceSys.ChangeResourseAvaible(data_.listOfObjectsRecuest[i].name, -data_.listOfObjectsRecuest[i].count);

    }

    base.Destroy();
  }



  public bool RecervStorage(string productName, int productCount, bool productRecuest, bool reserv) {

    int originalProductCount = productCount;

    if (!productRecuest) {
      bool canStore = false;
      for (int i = 0; i < storeManagment_.Length; i++) {

        if (productName == storeManagment_[i].name) {

          if (storeManagment_[i].spected + productCount <= storeManagment_[i].max) {
            canStore = true;
          }

        }

      }

      if (!canStore) {
        return false;
      }
    }

    for (int i = 0; i < 8; i++) {

      if (productRecuest) {
        previsulizationOfListOfObjectsExpected[i].count = data_.listOfObjectsRecuest[i].count;
        previsulizationOfListOfObjectsExpected[i].name = data_.listOfObjectsRecuest[i].name;
      } else {
        previsulizationOfListOfObjectsExpected[i].count = data_.listOfObjectsRecerved[i].count;
        previsulizationOfListOfObjectsExpected[i].name = data_.listOfObjectsRecerved[i].name;
      }
    }

    if (productRecuest) {

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (previsulizationOfListOfObjectsExpected[i].name == productName) {

          if (previsulizationOfListOfObjectsExpected[i].count >= productCount) {

            previsulizationOfListOfObjectsExpected[i].count -= productCount;
            productCount = 0;

          } else {
            if (previsulizationOfListOfObjectsExpected[i].count > 0) {
              int realStore = previsulizationOfListOfObjectsExpected[i].count;

              productCount -= realStore;
              previsulizationOfListOfObjectsExpected[i].count -= realStore;

            }
          }
        }
      }



    } else {

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (previsulizationOfListOfObjectsExpected[i].name == productName) {

          int freeSpace = 100 - previsulizationOfListOfObjectsExpected[i].count;

          if (freeSpace >= productCount) {
            previsulizationOfListOfObjectsExpected[i].count += productCount;

            productCount = 0;
          } else {
            previsulizationOfListOfObjectsExpected[i].count = 100;
            productCount -= freeSpace;
          }
        }
      }

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (previsulizationOfListOfObjectsExpected[i].name == "Anything") {
          previsulizationOfListOfObjectsExpected[i].count = productCount;
          previsulizationOfListOfObjectsExpected[i].name = productName;
          productCount = 0;
        }
      }
    }

    if (productCount == 0 && reserv) {

      for (int i = 0; i < 8; i++) {
        if (productRecuest) {

          int diferense = data_.listOfObjectsRecuest[i].count - previsulizationOfListOfObjectsExpected[i].count;

          data_.listOfObjectsRecuest[i].count = previsulizationOfListOfObjectsExpected[i].count;
          data_.listOfObjectsRecuest[i].name = previsulizationOfListOfObjectsExpected[i].name;

          Game_Manager.resourceSys.ChangeResourseAvaible(data_.listOfObjectsRecuest[i].name, -diferense);
        } else {
          data_.listOfObjectsRecerved[i].count = previsulizationOfListOfObjectsExpected[i].count;
          data_.listOfObjectsRecerved[i].name = previsulizationOfListOfObjectsExpected[i].name;

          
        }
      }

      if (!productRecuest) {
        for (int i = 0; i < storeManagment_.Length; i++) {

          if (productName == storeManagment_[i].name) {

            storeManagment_[i].spected += originalProductCount;

          }

        }
      }

    }

    return (productCount == 0);
  }

  public bool CancelReservation(string productName, int productCount, bool productRecuest) {

    int originalProductCount = productCount;

    for (int i = 0; i < 8; i++) {

      if (productRecuest) {
        previsulizationOfListOfObjectsExpected[i].count = data_.listOfObjectsRecuest[i].count;
        previsulizationOfListOfObjectsExpected[i].name = data_.listOfObjectsRecuest[i].name;
      } else {
        previsulizationOfListOfObjectsExpected[i].count = data_.listOfObjectsRecerved[i].count;
        previsulizationOfListOfObjectsExpected[i].name = data_.listOfObjectsRecerved[i].name;
      }
    }
    

    if (productRecuest) {

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (previsulizationOfListOfObjectsExpected[i].name == productName) {

          int resever = 100 - previsulizationOfListOfObjectsExpected[i].count;
          
          if(productCount - resever >= 0) {
            previsulizationOfListOfObjectsExpected[i].count += resever;
            productCount -= resever;
          } else {
            previsulizationOfListOfObjectsExpected[i].count += productCount;
            productCount = 0;

          }


        }
      }



    } else {

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (previsulizationOfListOfObjectsExpected[i].name == productName) {

          int resever = previsulizationOfListOfObjectsExpected[i].count;

          if (productCount - resever >= 0) {
            previsulizationOfListOfObjectsExpected[i].count -= resever;
            productCount -= resever;
          } else {
            previsulizationOfListOfObjectsExpected[i].count -= productCount;
            productCount = 0;

          }

          if(previsulizationOfListOfObjectsExpected[i].count == 0) {
            previsulizationOfListOfObjectsExpected[i].name = "Anything";
          }

        }
      }


      for (int i = 0; i < storeManagment_.Length; i++) {

        if (productName == storeManagment_[i].name) {

          storeManagment_[i].spected -= originalProductCount;

        }

      }


    }


    for (int i = 0; i < 8; i++) {
      if (productRecuest) {
        int diferense = previsulizationOfListOfObjectsExpected[i].count - data_.listOfObjectsRecuest[i].count;

        data_.listOfObjectsRecuest[i].count = previsulizationOfListOfObjectsExpected[i].count;
        data_.listOfObjectsRecuest[i].name = previsulizationOfListOfObjectsExpected[i].name;

        Game_Manager.resourceSys.ChangeResourseAvaible(data_.listOfObjectsRecuest[i].name, diferense);

      } else {
        data_.listOfObjectsRecerved[i].count = previsulizationOfListOfObjectsExpected[i].count;
        data_.listOfObjectsRecerved[i].name = previsulizationOfListOfObjectsExpected[i].name;        
      }
    }


    return (productCount == 0);
  }


  public bool TakeStorage(string productName, int productCount, bool productRecuest) {


    for (int i = 0; i < storeManagment_.Length; i++) {

      if (productName == storeManagment_[i].name) {

        if (productRecuest) {
          storeManagment_[i].spected -= productCount;
          storeManagment_[i].current -= productCount;
        } else {
          storeManagment_[i].current += productCount;
        }


      }

    }






    if (productRecuest) {

      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (data_.listOfObjects[i].name == productName) {

          int reserverInTheSlot = data_.listOfObjects[i].count - data_.listOfObjectsRecuest[i].count;

          if (reserverInTheSlot > productCount) {
            reserverInTheSlot = productCount;
          }

          data_.listOfObjects[i].count -= reserverInTheSlot;
          data_.listOfObjectsRecerved[i].count -= reserverInTheSlot;
          productCount -= reserverInTheSlot;
          SlotVisualation[i].transform.localScale = new Vector3(1.0f, 0.01f * data_.listOfObjects[i].count, 1.0f);

          Game_Manager.resourceSys.ChangeResourseStored(productName, -reserverInTheSlot);


          if (data_.listOfObjects[i].count == 0 ) {
            
              data_.listOfObjects[i].name = "Anything";
            
            SlotVisualation[i].SetActive(false);
          }
          if (data_.listOfObjectsRecerved[i].count == 0) {
            data_.listOfObjectsRecerved[i].name = "Anything";
          }


        }
      }



    } else {


      


      for (int i = 0; i < 8 && productCount > 0; i++) {

        if (data_.listOfObjectsRecerved[i].name == productName) {

          if (data_.listOfObjects[i].count < 100) {

            data_.listOfObjects[i].name = data_.listOfObjectsRecerved[i].name;
            data_.listOfObjectsRecuest[i].name = data_.listOfObjectsRecerved[i].name;

            

            int freeSpace = data_.listOfObjectsRecerved[i].count - data_.listOfObjects[i].count;

            if (freeSpace > productCount) {
              freeSpace = productCount;
            }

            if(freeSpace > 0) {
              SlotVisualation[i].SetActive(true);
            }

            data_.listOfObjects[i].count += freeSpace;

            data_.listOfObjectsRecuest[i].count += freeSpace;

            Game_Manager.resourceSys.ChangeResourseStored(productName, freeSpace);
            Game_Manager.resourceSys.ChangeResourseAvaible(productName, freeSpace);

            SlotVisualation[i].transform.localScale = new Vector3(1.0f, 0.01f * data_.listOfObjects[i].count, 1.0f);
            productCount -= freeSpace;


          }

        }
      }

    }


    return productCount == 0;
  }

  public int GetTotalStoredOf(string productName) {

    int ToReturn = 0;

    for(int i=0; i < 8; i++) {

      if(data_.listOfObjectsRecuest[i].name == productName) {
        ToReturn += data_.listOfObjectsRecuest[i].count;
      }

    }

    return ToReturn;
  }



  public new SaveDataStructure GetSaveData() {

    saveData_.allDataToSave_.Clear();

    for (int i = 0; i < storeManagment_.Length; i++) {
      data_.storeManagment[i] = storeManagment_[i];
    }

    saveData_.allDataToSave_.Add(data_);

    List<Interface_Data> basicData = base.GetSaveData();

    foreach (Interface_Data basicDataToSave in basicData) {
      saveData_.allDataToSave_.Add(basicDataToSave);
    }

    return saveData_;


  }

  public void LoadData(SaveDataStructure loadedData) {

    data_ = loadedData.allDataToSave_[0] as Storage_Data;
    base.Init();

    if (null != data_) {

      for (int i = 0; i < 8; i++) {
        if(data_.listOfObjects[i].count != 0) {
          SlotVisualation[i].transform.localScale = new Vector3(1.0f, 0.01f * data_.listOfObjects[i].count, 1.0f);
          SlotVisualation[i].SetActive(true);

          Game_Manager.resourceSys.ChangeResourseStored(data_.listOfObjects[i].name, data_.listOfObjects[i].count);
          Game_Manager.resourceSys.ChangeResourseAvaible(data_.listOfObjectsRecuest[i].name, data_.listOfObjectsRecuest[i].count);

        } else {
          SlotVisualation[i].SetActive(false);
        }
      }

      for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
        base.LoadData(loadedData.allDataToSave_[i]);
      }

      for (int i = 0; i < storeManagment_.Length; i++) {
        storeManagment_[i] = data_.storeManagment[i];
      }


      isloaded = true;

    }
  }


  public GameObject GetGameObjectInScene() {
    return gameObject;
  }


  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    StorageUI customUI = currentCustomUI.GetComponent<StorageUI>();

    if (null != customUI) {
      customUI.myOwnerData = this;
    }

  }
  



}
