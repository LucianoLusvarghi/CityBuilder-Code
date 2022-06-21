using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LumberjackCabin_Data : Interface_Data {
  
  public float[] harvestTime;

  public int woodLogStored = 0;
  public int woodPlankStored = 0;

  public float woodProcessingProgress = 0.0f;

}

public class LumberjackCabin : BasicBuilds, Interface_Save, Interface_UI {
    
  //NPC
  public GameObject npc_lumberjack_prefab;
  public GameObject npc_carry_prefab;

  //Save data
  SaveDataStructure saveData_;
  LumberjackCabin_Data data_ = null;
  public bool isloaded = false;

  //UI
  public GameObject UI_prefab;

  //inetrnal data
  List<MapController.Build> treesToSendNpc;  

  public float woodProcessingTimeInSecond;
  public float harvetTimeInSecond;

  int expectedWoodLog = 0;

  // Use this for initialization
  void Start () {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "lumberjackCabin", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);
    //Events Register  
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);

    treesToSendNpc = new List<MapController.Build>();    

    //Save data config
    {
      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/LumberjackCabin",
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
      data_ = new LumberjackCabin_Data {
        harvestTime = new float[3]
      };

      for (int i = 0; i < 3; i++) {        
        data_.harvestTime[i] = 0.0f;
      }

    }

    
    if (!isloaded) {

      base.AddNpc( 0, npc_lumberjack_prefab, false);      

      base.AddNpc( 4, npc_carry_prefab, false);
      base.AddNpc( 5, npc_carry_prefab, false);

    }

    

  }


  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "lumberjackCabin", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);

    //Remove Envets    
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);

    for (int i = 0; i < 1; i++) {
      NPC_controler npcInfo = base.GetNPCControler(i);

      if ((npcInfo.walk_ && !npcInfo.reachDestiny) || (npcInfo.reachDestiny && !npcInfo.return_)) {
        Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
        MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);
        if (checkBuild.build_ptr) {
          checkBuild.build_ptr.GetComponent<Trees>().ResetNPCSend();
        }
      }

    }

    for (int i = 4; i <= 5; i++) {
      base.CanselNPCSend(i, "wood", 100, false, false);
    }

    base.Destroy();
  }


  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);

    if (data_.woodLogStored >= 25 && data_.woodPlankStored <= 150) {

      data_.woodProcessingProgress += (1.0f / woodProcessingTimeInSecond) * deltaTime;

      if(data_.woodProcessingProgress >= 1.0f) {
        data_.woodPlankStored += 50;
        data_.woodLogStored -= 25;
        data_.woodProcessingProgress = 0.0f;

        Game_Manager.resourceSys.AddResourseProduced("wood", 50.0f);
      }

    }

    //Send all lumberjack
    {
      for (int i = 0; i < 1; i++) {



        if (base.GetNPCdata(i) != null) {
          NPC_controler npcInfo = base.GetNPCControler(i);
          BasicBuidsData npcData = base.GetNPCdata(i);

          if (!npcData.npc_sended && (data_.woodLogStored + expectedWoodLog) <= 75 && currentworkers >= (3 + i) ) {

            treesToSendNpc.Clear();

            List<MapController.Build> currentTreeList = base.Game_Manager.mapSys.GetBuildsListByName("tree");

            for (int t = 0; t < currentTreeList.Count; t++) {
              if (currentTreeList[t].build_ptr.GetComponent<Trees>().CanHarvest()) {
                treesToSendNpc.Add(currentTreeList[t]);
              }
            }

            if (treesToSendNpc.Count > 0) {
              bool check = base.FindPathToPoints(i, treesToSendNpc, true);
              if (check) {
                npcData.npc_sended = true;
                expectedWoodLog += 25;
              }
            }

          } else if(npcData.npc_sended) {

            if (npcData.npc_researchPath) {

              if (!npcInfo.walk_) {
                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

                if (checkBuild.name == "tree") {
                  if (checkBuild.build_ptr.GetComponent<Trees>().CanHarvest()) {
                    checkBuild.build_ptr.GetComponent<Trees>().SendNPC();
                    base.SendNPC(i);
                  } else {
                    npcData.npc_sended = false;
                    expectedWoodLog -= 25;
                  }
                } else {
                  npcData.npc_sended = false;
                  expectedWoodLog -= 25;
                }
              } else if (!npcInfo.reachDestiny) {

                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

                if (checkBuild.name == "error") {
                  npcInfo.return_ = true;
                }

              }

              if (npcInfo.reachDestiny) {

                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);
                if (checkBuild.name != "error") {

                  if (data_.harvestTime[i] >= 1.0f && !npcInfo.return_) {

                    Trees treeDestiny = checkBuild.build_ptr.GetComponent<Trees>();
                    treeDestiny.Harvest();
                    npcInfo.return_ = true;
                  } else {
                    data_.harvestTime[i] += (1.0f / harvetTimeInSecond) * deltaTime;
                  }

                } else {
                  npcInfo.return_ = true;
                }

              }

              if (npcInfo.finish_ && data_.harvestTime[i] >= 1.0f) {
                data_.woodLogStored += 25;
                data_.harvestTime[i] = 0.0f;
                npcData.npc_sended = false;
                expectedWoodLog -= 25;
              }

            } else if (!npcData.npc_researchingPath) {
              npcData.npc_sended = false;
              expectedWoodLog -= 25;
            }

          }
        }

      }
    }



    //check if some of the NPC is not working and send it
    {
      for (int i = 4; i <= 5; i++) {

        if (data_.woodPlankStored >= 100) {
          base.ConfigNPCToSend(i, "wood", 100, false, false);
        }
        int canBeSended = 0;
        base.UpdateNPCToSend(i, "wood", 100, false, false, ref canBeSended);

        if (canBeSended > 0) {
          data_.woodPlankStored -= 100;
        } else if (canBeSended < 0) {
          data_.woodPlankStored += 100;
        }

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

    data_ = loadedData.allDataToSave_[0] as LumberjackCabin_Data;

    for (int i = 1; i < loadedData.allDataToSave_.Count; i++) {
      base.LoadData(loadedData.allDataToSave_[i]);
    }

    for(int i=0; i<1; i++) {
      BasicBuidsData npcData = base.GetNPCdata(i);
      if (npcData.npc_sended) {
        expectedWoodLog += 25;
      }
    }

    isloaded = true;

  }

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }


  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    LumberjackCabinUI myUI = currentCustomUI.GetComponent<LumberjackCabinUI>();

    if (null != myUI) {
      myUI.myOwnerData = data_;
    }

  }



}
