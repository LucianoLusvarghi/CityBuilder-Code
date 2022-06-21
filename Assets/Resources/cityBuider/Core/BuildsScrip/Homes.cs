using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Homes_Data : Interface_Data {

  public float water_stored = 0.0f;

  public float firewood_stored = 0.0f;

  public float food_meat_stored = 0.0f;
  public float food_crops_stored = 0.0f;
  public float food_fish_stored = 0.0f;

  public float cloths_stored = 0.0f;

  public float utilities_stored = 0.0f;

  public float hygiene_value = 0.0f;
  public float nature_value = 0.0f;
  public float religion_value = 0.0f;

  public float weapons_stored = 0.0f;

  public int currentPopulation = 0;

  public bool fireResponse = false;
  public bool npcHaveWater = false;
  public float npcFireworking = 0.0f;
  public bool npcReturnToHome = false;
}

public class Homes : BasicBuilds, Interface_Save, Interface_UI {

 
  //ManagerGame Game_Manager;
  public GameObject[] levels_;

  public Homes_Data data_ = null;

  public float water_demandPerSecond;
  public float firewood_demandPerSecond;
  public float food_demandPerSecond;
  public float cloths_demandPerSecond;
  public float utilities_demandPerSecond;

  public float hygiene_decreasePerSecond;
  public float religion_decreasePerSecond;

  public bool isloaded = false;

  //NPC
  public GameObject npcPrefab_;

  public GameObject NPCincomming;
  public GameObject NPCleaving;
  public GameObject NPC_Prefab_fire_;
  //Save data
  SaveDataStructure saveData_;

  //UI
  public GameObject UI_prefab;
  public float timeDown = 0.0f;
  public ParticleSystem onDownLevel_;

  public int avaiblePopulation = 1;
  

  // Use this for initialization
  void Start () {
    base.Init();
    base.Game_Manager.saveSys.AddDataToSave(this);
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "home", gameObject);
    base.Game_Manager.buildsSys.RegisterBuild(this);

    //Events Register
    base.Game_Manager.eventSys.RegisterHome(this);
    base.Game_Manager.eventSys.RegisterBuildingCanBurn(this);

    for (int i = 0; i < 5; i++) {
      GameObject currentLevel = levels_[i];
      currentLevel.SetActive(false);
    }

    //Save data config
    {

      saveData_ = new SaveDataStructure {
        allDataToSave_ = new List<Interface_Data>(),
        prefab = "cityBuider/Placeables/Home",
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

    if (!isloaded) {
      data_ = new Homes_Data();      
   

      base.AddNpc(0, npcPrefab_, true);
      base.AddNpc(1, npcPrefab_, false);
      base.AddNpc(2, npcPrefab_, false);
      base.AddNpc(3, npcPrefab_, false);
      base.AddNpc(4, npcPrefab_, false);

      
      base.AddNpc(100, NPCincomming, false);
      base.AddNpc(101, NPCleaving, false);
      base.AddNpc(102, NPC_Prefab_fire_, false);

    }

    

  }

  public void OnDestroy() {

    base.Game_Manager.saveSys.LessDataToSave(this);
    base.Game_Manager.mapSys.RemoveBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "home", gameObject);
    base.Game_Manager.buildsSys.RemoveBuild(this);

    //Remove Envets
    base.Game_Manager.eventSys.RemoveHome(this);
    base.Game_Manager.eventSys.RemoveBuildingCanBurn(this);

    base.CanselNPCSend(1, "vegetables", 25, true, true);
    base.CanselNPCSend(2, "firewood", 25, true, true);
    base.CanselNPCSend(3, "fish", 25, true, true);
    base.CanselNPCSend(4, "utilities", 25, true, true);

    Game_Manager.populationSys.ChangeWorkersAvaible(-data_.currentPopulation);



    base.Destroy();
  }


  
  public override void UpdateBuild(float deltaTime) {
    base.Run_NPC_Update(deltaTime);

    //population controler
    {
      //Get new Population
      int currentNPC_ID = 100;
      if (data_.currentPopulation < avaiblePopulation) {        

        if (base.GetNPCdata(currentNPC_ID) != null) {
          NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
          BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

          if (!npcData.npc_sended) {

            List<MapController.Build> allWaetrHoleToSendNpc = base.Game_Manager.mapSys.GetBuildsListByName("startNode");
            if (allWaetrHoleToSendNpc.Count > 0) {
              bool check = base.FindPathToPoints(currentNPC_ID, allWaetrHoleToSendNpc);
              if (check) {
                npcData.npc_sended = true;
              }
            }

          } else {

            if (npcData.npc_researchPath) {

              if (!npcInfo.walk_) {
               
                base.SendNPC(currentNPC_ID);

                npcInfo.index_ = npcInfo.route_.Count - 1;
                npcInfo.init_transform_ = npcInfo.route_[npcInfo.index_];
                npcInfo.return_ = true;

              }

              if (npcInfo.finish_) {
                int change = avaiblePopulation - data_.currentPopulation;
                data_.currentPopulation += change;
                npcData.npc_sended = false;
                Game_Manager.populationSys.ChangeWorkersAvaible(change);
              }


            } else if (!npcData.npc_researchingPath) {
              npcData.npc_sended = false;
            }


          }
        }

      }

      //Population get out
      currentNPC_ID = 101;
      if (data_.currentPopulation > avaiblePopulation || base.GetNPCdata(currentNPC_ID).npc_sended) {
        

        if (base.GetNPCdata(currentNPC_ID) != null) {
          NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
          BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

          if (!npcData.npc_sended) {

            List<MapController.Build> allWaetrHoleToSendNpc = base.Game_Manager.mapSys.GetBuildsListByName("startNode");
            if (allWaetrHoleToSendNpc.Count > 0) {
              bool check = base.FindPathToPoints(currentNPC_ID, allWaetrHoleToSendNpc);
              if (check) {
                npcData.npc_sended = true;
              }
            }

          } else {

            if (npcData.npc_researchPath) {

              if (!npcInfo.walk_) {                

                int change = avaiblePopulation - data_.currentPopulation;
                data_.currentPopulation += change;
                
                Game_Manager.populationSys.ChangeWorkersAvaible(change);

                base.SendNPC(currentNPC_ID);
              }

              if (npcInfo.reachDestiny) {                
                npcData.npc_sended = false;
                npcInfo.ResetStatus();
              }


            } else if (!npcData.npc_researchingPath) {
              npcData.npc_sended = false;
            }


          }
        }

      }

    }

    //decrease values
    {
      data_.water_stored -= water_demandPerSecond * deltaTime;
      data_.food_crops_stored -= food_demandPerSecond * deltaTime;
      data_.food_fish_stored -= food_demandPerSecond * deltaTime;
      data_.firewood_stored -= firewood_demandPerSecond * deltaTime;
      data_.utilities_stored -= utilities_demandPerSecond * deltaTime;
    }

    //Check the values not below from zero
    {
      if (data_.water_stored < 0.0f) {
        data_.water_stored = 0.0f;
      }

      if (data_.food_crops_stored < 0.0f) {
        data_.food_crops_stored = 0.0f;
      }

      if (data_.food_fish_stored < 0.0f) {
        data_.food_fish_stored = 0.0f;
      }

      if (data_.firewood_stored < 0.0f) {
        data_.firewood_stored = 0.0f;
      }

      if (data_.utilities_stored < 0.0f) {
        data_.utilities_stored = 0.0f;
      }

    }

    //Level visualizacion
    {

      //Disable all level for update
      for (int i = 0; i < 5; i++) {
        GameObject currentLevel = levels_[i];
        currentLevel.SetActive(false);
      }

      int currentPopulation = avaiblePopulation;
      avaiblePopulation = 1;

      //Level_1
      if (data_.water_stored > 0.0f) {

        if (data_.currentPopulation >= avaiblePopulation) {
          levels_[0].SetActive(true);
          avaiblePopulation++;
        }
        //Level_2
        if (data_.firewood_stored > 0.0f) {
          if (data_.currentPopulation >= avaiblePopulation) {
            levels_[0].SetActive(false);
            levels_[1].SetActive(true);
            avaiblePopulation++;
          }

          //Level_3
          if (data_.food_fish_stored > 0.0f) {
            if (data_.currentPopulation >= avaiblePopulation) {
              levels_[1].SetActive(false);
              levels_[2].SetActive(true);
              avaiblePopulation++;
            }

            //Level_4
            if (data_.utilities_stored > 0.0f) {
              if (data_.currentPopulation >= avaiblePopulation) {
                levels_[2].SetActive(false);
                levels_[3].SetActive(true);
                avaiblePopulation++;
              }

              //Level_5
              if (data_.food_crops_stored > 0.0f) {
                if (data_.currentPopulation >= avaiblePopulation) {
                  levels_[3].SetActive(false);
                  levels_[4].SetActive(true);
                  avaiblePopulation++;
                }

              }
            }
          }
        }

      }

      if(timeDown > 0.0f) {
        timeDown -= deltaTime;
      }

      if(currentPopulation > avaiblePopulation) {
        timeDown = 15.0f;
        onDownLevel_.Play();

       


      }

    }

    if(data_.currentPopulation == 0) {
      return;
    }


    //Send and check npc for water suply
    {
      int currentNPC_ID = 0;

      if (data_.water_stored < 50.0f) {

        if (base.GetNPCdata(currentNPC_ID) != null) {
          NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
          BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

          if (!npcData.npc_sended) {

            List<MapController.Build> allWaetrHoleToSendNpc = base.Game_Manager.mapSys.GetBuildsListByName("waterhole");
            if (allWaetrHoleToSendNpc.Count > 0) {
              bool check = base.FindPathToPoints(currentNPC_ID, allWaetrHoleToSendNpc);
              if (check) {
                npcData.npc_sended = true;
              }
            }

          } else {

            if (npcData.npc_researchPath) {

              if (!npcInfo.walk_) {
                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

                if (checkBuild.name != "error") {
                  base.SendNPC(currentNPC_ID);
                }
                
              } else if (!npcInfo.reachDestiny) {

                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

                if (checkBuild.name == "error") {
                  npcInfo.return_ = true;                 
                }

              }

              if (npcInfo.finish_) {
                if (npcInfo.reachDestiny) {
                  data_.water_stored = 100.0f;
                }
                npcData.npc_sended = false;

              }


            } else if (!npcData.npc_researchingPath) {
              npcData.npc_sended = false;
            }


          }
        }

      }
    }


    //Send and check npc for vegetables
    {
      int canBeSended = 0;
      int currentNPC_ID = 1;

      if (data_.food_crops_stored <= 50.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "vegetables", 25, true, true);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "vegetables", 25, true, true, ref canBeSended)) {
        Game_Manager.resourceSys.AddResourseConsumed("vegetables", 25.0f);
        data_.food_crops_stored = 100.0f;
      }
    }


    //Send and check for firewood
    {
      int canBeSended = 0;
      int currentNPC_ID = 2;

      if (data_.firewood_stored <= 50.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "firewood", 25, true, true);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "firewood", 25, true, true, ref canBeSended)) {
        Game_Manager.resourceSys.AddResourseConsumed("wood", 25.0f);
        data_.firewood_stored = 100.0f;
      }
    }
    

    //Send and check for fish
    {
      int canBeSended = 0;
      int currentNPC_ID = 3;

      if (data_.food_fish_stored <= 50.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "fish", 25, true, true);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "fish", 25, true, true, ref canBeSended)) {
        Game_Manager.resourceSys.AddResourseConsumed("fish", 25.0f);
        data_.food_fish_stored = 100.0f;
      }
    }
    

    //Send and check for utilities
    {
      int canBeSended = 0;
      int currentNPC_ID = 4;

      if (data_.utilities_stored <= 50.0f) {
        base.ConfigNPCToSend(currentNPC_ID, "utilities", 25, true, true);
      }
      if (base.UpdateNPCToSend(currentNPC_ID, "utilities", 25, true, true, ref canBeSended)) {
        Game_Manager.resourceSys.AddResourseConsumed("utilities", 25.0f);
        data_.utilities_stored = 100.0f;
      }
    }

    if (data_.fireResponse) {
      FireResponse(deltaTime);
    }

  }

 
 

  void FireResponse(float deltaTime) {
    List<BasicBuilds> burningBuilding = base.Game_Manager.eventSys.GetBurningBuilding();

    if( (burningBuilding.Count == 0 || burningBuilding.Contains(this)) && (!data_.npcReturnToHome && !data_.npcHaveWater)) {
      data_.npcHaveWater = false;
      data_.npcFireworking = 0.0f;      

      int currentNPC_ID = 102;
      if (base.GetNPCdata(currentNPC_ID) != null) {
        NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
        BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

        //npcInfo.return_ = true;

        if (npcData.npc_sended) {
          data_.npcReturnToHome = true;
        }

        if (npcInfo.reachDestiny || npcInfo.finish_) {
          npcData.npc_sended = false;
          data_.npcReturnToHome = false;
          npcInfo.finish_ = true;
          data_.fireResponse = false;
        }

      }
      return;
    } else {

      int currentNPC_ID = 102;
      if (base.GetNPCdata(currentNPC_ID) != null) {
        NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
        BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

        if (npcInfo.walk_ && data_.npcHaveWater) {

          Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
          MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

          if (checkBuild.name != "error") {

            BasicBuilds burningBuild = checkBuild.build_ptr.GetComponent<BasicBuilds>();

            if (!Game_Manager.eventSys.CheckBurningBuilding(burningBuild) && npcData.npc_sended) {
              npcInfo.walk_ = false;
              data_.npcHaveWater = false;
              npcData.npc_sended = false;
              data_.npcReturnToHome = true;
            }
          } else {
            npcInfo.walk_ = false;
            data_.npcHaveWater = false;
            npcData.npc_sended = false;
            data_.npcReturnToHome = true;
          }
        }

      }

    }

    //Send npc for water
    if (!data_.npcHaveWater && !data_.npcReturnToHome) {

      int currentNPC_ID = 102;

      if (base.GetNPCdata(currentNPC_ID) != null) {
        NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
        BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

        if (!npcData.npc_sended) {

          List<MapController.Build> allWaetrHoleToSendNpc = base.Game_Manager.mapSys.GetBuildsListByName("waterhole");
          if (allWaetrHoleToSendNpc.Count > 0) {
            bool check = base.FindPathToPoints(currentNPC_ID, allWaetrHoleToSendNpc);
            if (check) {
              npcData.npc_sended = true;
            }
          }

        } else {

          if (npcData.npc_researchPath) {

            if (!npcInfo.walk_) {
              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name != "error") {
                base.SendNPC(currentNPC_ID);
              }

            } else if (!npcInfo.reachDestiny) {

              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name == "error") {

                npcInfo.return_ = true;
              }

            }


            if (npcInfo.reachDestiny) {
              npcInfo.walk_ = false;
              data_.npcHaveWater = true;
              npcData.npc_sended = false;
            }



          } else if (!npcData.npc_researchingPath) {
            npcData.npc_sended = false;
          }


        }
      }


    }


    //Send npc to the burningBuilding
    if (data_.npcHaveWater) {

      int currentNPC_ID = 102;

      if (base.GetNPCdata(currentNPC_ID) != null) {
        NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
        BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

        if (!npcData.npc_sended) {

          List<MapController.Build> allBurningBuildingToSendNpc = new List<MapController.Build>();

          for(int i=0; i< burningBuilding.Count; i++) {
            Transform buildingTransform = burningBuilding[i].transform;
            Vector3 buildingPosition = buildingTransform.position;
            MapController.Build theBuild = base.Game_Manager.mapSys.GetBuildByPosition((int)buildingPosition.x, (int)buildingPosition.z);

            allBurningBuildingToSendNpc.Add(theBuild);
          }

          if (allBurningBuildingToSendNpc.Count > 0) {
            bool check = base.FindPathToPoints(currentNPC_ID, allBurningBuildingToSendNpc, false, false);
            if (check) {
              npcData.npc_sended = true;
            }
          }

        } else {

          if (npcData.npc_researchPath) {

            if (!npcInfo.walk_) {
              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name != "error") {
                base.SendNPC(currentNPC_ID);
              }

            } else if (!npcInfo.reachDestiny) {

              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name == "error") {

                npcInfo.return_ = true;
              }

            }


            if (npcInfo.reachDestiny) {
              data_.npcFireworking += deltaTime;

              if(data_.npcFireworking >= 2.0f) {

                data_.npcFireworking = 0.0f;

                Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
                MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

                BasicBuilds burningBuild = checkBuild.build_ptr.GetComponent<BasicBuilds>();

                if (burningBuild) {
                  Game_Manager.eventSys.ClearBurningBuild(burningBuild);

                  npcInfo.walk_ = false;
                  data_.npcHaveWater = false;
                  npcData.npc_sended = false;
                  data_.npcReturnToHome = true;
                }
              }

            }



          } else if (!npcData.npc_researchingPath) {
            npcData.npc_sended = false;
          }


        }
      }


    }



    //Send npc to home
    if (data_.npcReturnToHome) {

      int currentNPC_ID = 102;

      if (base.GetNPCdata(currentNPC_ID) != null) {
        NPC_controler npcInfo = base.GetNPCControler(currentNPC_ID);
        BasicBuidsData npcData = base.GetNPCdata(currentNPC_ID);

        if (!npcData.npc_sended) {

          List<MapController.Build> allBurningBuildingToSendNpc = new List<MapController.Build>();

          
            Transform buildingTransform = transform;
            Vector3 buildingPosition = buildingTransform.position;
            MapController.Build theBuild = base.Game_Manager.mapSys.GetBuildByPosition((int)buildingPosition.x, (int)buildingPosition.z);

            allBurningBuildingToSendNpc.Add(theBuild);
          

          if (allBurningBuildingToSendNpc.Count > 0) {
            bool check = base.FindPathToPoints(currentNPC_ID, allBurningBuildingToSendNpc, false, false);
            if (check) {
              npcData.npc_sended = true;
            }
          }

        } else {

          if (npcData.npc_researchPath) {

            if (!npcInfo.walk_) {
              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name != "error") {
                base.SendNPC(currentNPC_ID);
              }

            } else if (!npcInfo.reachDestiny) {

              Vector3 lastPoint = npcInfo.route_[npcInfo.route_.Count - 1];
              MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

              if (checkBuild.name == "error") {

                npcInfo.return_ = true;
              }

            }


            if (npcInfo.reachDestiny) {
              npcInfo.walk_ = false;
              npcInfo.finish_ = true;
              data_.npcHaveWater = false;
              npcData.npc_sended = false;
              data_.npcReturnToHome = false;
              data_.fireResponse = false;
            }



          } else if (!npcData.npc_researchingPath) {
            npcData.npc_sended = false;
          }


        }
      }


    }


  }

  public new SaveDataStructure GetSaveData() {

    saveData_.allDataToSave_.Clear();

    saveData_.allDataToSave_.Add(data_);

    List<Interface_Data> basicData = base.GetSaveData();

    foreach(Interface_Data basicDataToSave in basicData) {
      saveData_.allDataToSave_.Add(basicDataToSave);
    }

    return saveData_;

  }

  public void LoadData(SaveDataStructure loadedData) {

    data_ = loadedData.allDataToSave_[0] as Homes_Data;

    for (int i=1; i < loadedData.allDataToSave_.Count; i++) {
      base.LoadData(loadedData.allDataToSave_[i]);
    }

    isloaded = true;

    base.Init();

    //Level visualizacion
    {

      //Disable all level for update
      for (int i = 0; i < 5; i++) {
        GameObject currentLevel = levels_[i];
        currentLevel.SetActive(false);
      }

      avaiblePopulation = 1;

      //Level_1
      if (data_.water_stored > 0.0f) {

        if (data_.currentPopulation >= avaiblePopulation) {
          levels_[0].SetActive(true);
          avaiblePopulation++;
        }
        //Level_2
        if (data_.firewood_stored > 0.0f) {
          if (data_.currentPopulation >= avaiblePopulation) {
            levels_[1].SetActive(true);
            avaiblePopulation++;
          }

          //Level_3
          if (data_.food_fish_stored > 0.0f) {
            if (data_.currentPopulation >= avaiblePopulation) {
              levels_[2].SetActive(true);
              avaiblePopulation++;
            }

            //Level_4
            if (data_.utilities_stored > 0.0f) {
              if (data_.currentPopulation >= avaiblePopulation) {
                levels_[3].SetActive(true);
                avaiblePopulation++;
              }

              //Level_5
              if (data_.food_crops_stored > 0.0f) {
                if (data_.currentPopulation >= avaiblePopulation) {
                  levels_[4].SetActive(true);
                  avaiblePopulation++;
                }

              }
            }
          }
        }

      }

            

    }

    Game_Manager.populationSys.ChangeWorkersAvaible(data_.currentPopulation);

  }

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }

  public void InvoqueCustomUI(UI_Manager ui_manager) {

    GameObject currentCustomUI = ui_manager.ChangeToCustomUI(UI_prefab);

    HomeUI homeUI = currentCustomUI.GetComponent<HomeUI>();

    if(null != homeUI) {
      homeUI.myOwnerData = this;
    }

  }

}
