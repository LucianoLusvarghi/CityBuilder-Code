using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Farms_Data : Interface_Data {

  public Crops_Data[] crops_;
  public int foodStored = 0;
  public float[] npcTimeWork;
}


public class Farms : BasicBuilds, Interface_Save, Interface_UI {


  public GameObject[] cropsPrefabs_;
  public GameObject[] cropsShadowsPrefab_;

  public List<Crops> crops_;

  //NPC
  public GameObject npc_farmer_prefab;
  public GameObject npc_carry_prefab;

  //Save data
  SaveDataStructure saveData_;
  public Farms_Data data_ = null;
  public bool isloaded = false;

  //UI
  public GameObject UI_prefab;

  //
  List<MapController.Build> cropsToSendNpc;

  int espectedFood = 0;

  // Use this for initialization
  void Start() {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "farm", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);

    //Events Register  
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);

    cropsToSendNpc = new List<MapController.Build>();
   
    //Save data config
    {

      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/Farm",
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
      data_ = new Farms_Data();
    }

    data_.npcTimeWork = new float[3];

    for (int i = 0; i < 3; i++)
    {
      data_.npcTimeWork[i] = 0.0f;
    }

    if (!isloaded) {
      for (int i = 0; i < cropsPrefabs_.Length; i++) {

        GameObject newPrefab = Instantiate(cropsPrefabs_[i], new Vector3(-1000.0f, 0.0f, -1000.0f), new Quaternion());

        newPrefab.GetComponent<Crops>().myFarm = this;

        cropsPrefabs_[i] = newPrefab;

      }

      base.AddNpc(0, npc_farmer_prefab, false);
      base.AddNpc(1, npc_farmer_prefab, false);
      base.AddNpc(2, npc_farmer_prefab, false);

      base.AddNpc(3, npc_carry_prefab, false);
      base.AddNpc(4, npc_carry_prefab, false);

    }



  }

  public void OnDestroy() {

    for (int i = 0; i < cropsPrefabs_.Length; i++) {
      GameObject newPrefab = cropsPrefabs_[i];
      Destroy(newPrefab);
    }

    for (int i = 0; i < crops_.Count; i++) {
      Destroy(crops_[i].gameObject);
    }

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "farm", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);

    //Remove Envets    
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);

    for (int i = 3; i < 5; i++) {
      base.CanselNPCSend(i, "vegetables", 100, false, false);
    }

    base.Destroy();
  }

  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);

    //Check crops for harvest
    for (int i = 0; i < crops_.Count; i++) {

      //check if crops can harvest, dont have a NPC incomming and can store
      if (crops_[i].Canharvest() && !crops_[i].Data().npcIncomming_ && (data_.foodStored + espectedFood) <= 175) {

        cropsToSendNpc.Clear();

        MapController.Build currentCrop = new MapController.Build {
          cellsSize = crops_[i].size_,
          mapPosition = new Vector2Int((int)crops_[i].transform.position.x, (int)crops_[i].transform.position.z)
        };

        cropsToSendNpc.Add(currentCrop);

        bool canSendNPC = false;

        //check if some npc is free to go to harvest
        for (int n = 0; n < 3 && !canSendNPC; n++) {

          BasicBuidsData npcData = base.GetNPCdata(n);

          if (!npcData.npc_sended && currentworkers >= (3 + 2*n) ) {
            bool check = base.FindPathToPoints(n, cropsToSendNpc);
            if (check) {
              crops_[i].Data().npcIncomming_ = true;
              crops_[i].Data().npcIncommingID_ = n;
              canSendNPC = true;
              npcData.npc_sended = true;
              espectedFood += 25;
            }
          }

        }

        //check if the crops have a npc incomming and isnt havest
      } else if (crops_[i].Data().npcIncomming_ && crops_[i].Canharvest()) {

        int npcID = crops_[i].Data().npcIncommingID_;

        NPC_controler npcInfo = base.GetNPCControler(npcID);
        BasicBuidsData npcData = base.GetNPCdata(npcID);

        //check if the npc is harveting
        if (npcInfo.reachDestiny && !npcInfo.return_) {

          //ncp work time
          if (data_.npcTimeWork[npcID] > 10.0f) {
            crops_[i].Harvest();
            npcInfo.return_ = true;            
            crops_[i].Data().npcIncomming_ = false;
            crops_[i].Data().npcIncommingID_ = 0;
          } else {
            data_.npcTimeWork[npcID] += deltaTime;
          }
        }

        if(!npcInfo.walk_ && npcData.npc_researchPath) {
          base.SendNPC(npcID);
        }

        if(!npcData.npc_researchPath && !npcData.npc_researchingPath) {
          crops_[i].Data().npcIncomming_ = false;
          crops_[i].Data().npcIncommingID_ = 0;
          npcData.npc_sended = false;
          espectedFood -= 25;
        }

      }
    }

    //check if the npc has returned to the farm
    for (int i = 0; i < 3; i++) {

      NPC_controler npcInfo = base.GetNPCControler(i);
      BasicBuidsData npcData = base.GetNPCdata(i);

      if (npcInfo.finish_ && npcData.npc_sended) {
        
        if (data_.npcTimeWork[i] > 10.0f) {
          data_.foodStored += 25;
          Game_Manager.resourceSys.AddResourseProduced("vegetables", 25.0f);
        }
        data_.npcTimeWork[i] = 0.0f;
        npcData.npc_sended = false;
        espectedFood -= 25;
      }
    }





    //check if some of the farmers is working and send if not or stored mode than 100 units
    {
      for (int i = 3; i < 5; i++) {
        if (data_.foodStored >= 100) {
          base.ConfigNPCToSend(i, "vegetables", 100, false, false);
        }
        int canBeSended = 0;
        base.UpdateNPCToSend(i, "vegetables", 100, false, false, ref canBeSended);

        if(canBeSended > 0) {
          data_.foodStored -= 100;
        } else if(canBeSended < 0) {
          data_.foodStored += 100;
        }

      }
    }

  }

  

  public void AddCrop(Crops crop) {

    for(int i=0; i < 3; i++) {
      if (cropsPrefabs_[i] != null) {
        if (cropsPrefabs_[i].GetComponent<Crops>() == crop) {
          return;
        }
      }
    }
    crop.isPrefab = false;
    crops_.Add(crop);
  }

  public void LessCrop(Crops crop) {
    for (int i = 0; i < 3; i++) {
      if (cropsPrefabs_[i] != null) {
        if (cropsPrefabs_[i].GetComponent<Crops>() == crop) {
          return;
        }
      }
    }
    if (crop.Data().npcIncomming_) {

      NPC_controler npcInfo = base.GetNPCControler(crop.Data().npcIncommingID_);      

      npcInfo.return_ = true;      
    }
    crops_.Remove(crop);
  }


  public new SaveDataStructure GetSaveData() {


    saveData_.allDataToSave_.Clear();

    data_.crops_ = new Crops_Data[crops_.Count];

    for(int i=0; i< crops_.Count; i++) {
      data_.crops_[i] = crops_[i].Data();
    }

    saveData_.allDataToSave_.Add(data_);

    List<Interface_Data> basicData = base.GetSaveData();

    foreach (Interface_Data basicDataToSave in basicData) {
      saveData_.allDataToSave_.Add(basicDataToSave);
    }

    return saveData_;

    
  }

  public void LoadData(SaveDataStructure loadedData) {

    for (int i = 0; i < cropsPrefabs_.Length; i++) {
      GameObject newPrefab = Instantiate(cropsPrefabs_[i], new Vector3(-1000.0f, 0.0f, -1000.0f), new Quaternion());      
      newPrefab.GetComponent<Crops>().myFarm = this;      
      cropsPrefabs_[i] = newPrefab;
    }


    data_ = loadedData.allDataToSave_[0] as Farms_Data;

    if (null != data_) {
      for (int i = 0; i < data_.crops_.Length; i++) {

        int currentCropPrefab = data_.crops_[i].myType_ - 1;

        Vector3 cropPosition = new Vector3(data_.crops_[i].saveData_.position[0], data_.crops_[i].saveData_.position[1], data_.crops_[i].saveData_.position[2]);
        Quaternion cropRotation = new Quaternion(data_.crops_[i].saveData_.rotation[0], data_.crops_[i].saveData_.rotation[1], data_.crops_[i].saveData_.rotation[2], data_.crops_[i].saveData_.rotation[3]);

        GameObject instancia = GameObject.Instantiate(cropsPrefabs_[currentCropPrefab], cropPosition, cropRotation);
        instancia.GetComponent<Crops>().LoadData(data_.crops_[i]);

      }
    }

    for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
      base.LoadData(loadedData.allDataToSave_[i]);
    }

    isloaded = true;

  }

  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    FarmUI customUI = currentCustomUI.GetComponent<FarmUI>();

    if (null != customUI) {
      customUI.myFarm = this;
    }

  }
  

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }

}
