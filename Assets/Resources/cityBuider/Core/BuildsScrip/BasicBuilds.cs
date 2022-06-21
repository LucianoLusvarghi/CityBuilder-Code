using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicBuidsData : Interface_Data {
    
  public NPC_Data npcData;  
  public int npc_ID;

  public bool npc_researchPath;
  public bool npc_researchingPath;

  public bool npc_sended;
}

public class BasicBuilds : MonoBehaviour, Interface_Workers {

  //NPC control
  [System.Serializable]
  class NPC_Info {
    public NPC_controler the_npc_controler;
    public BasicBuidsData data;
  };


  ThreadPath npc_path_calculator_ = null;
  float timeToRetryHavePath = 0.0f;
  int npcID_in_path_;
  
  Dictionary<int, NPC_Info> npcs_ = null;

  public ManagerGame Game_Manager = null;

  public string BuildName;
  public Vector2Int size_;  
  public int workesNeeded = 0;
  public int currentworkers = 0;
  
  List<MapController.Build> storagesToSendNpc = null;
  //-------------------------
  public GameObject smoke;

  //----------------------------
  

  // Use this for initialization
  public void Init () {
    if (null == Game_Manager) {
      Game_Manager = GameObject.Find("GM").GetComponent<ManagerGame>();
    }
    if (null == npcs_) {
      npcs_ = new Dictionary<int, NPC_Info>();
    }
    npcID_in_path_ = -1;

    if (null == storagesToSendNpc) {
      storagesToSendNpc = new List<MapController.Build>();
    }
    Game_Manager.populationSys.RegisterBuild(this);

    Transform container = Game_Manager.getPlaceableContainer(BuildName);
    if(null != container) {
      transform.SetParent(container);
    }

  }

  public void Destroy() {
    Game_Manager.populationSys.RemoveBuild(this);
    if(null != npc_path_calculator_) {
      Game_Manager.threadSys.ReturnThread(npc_path_calculator_);
      npc_path_calculator_ = null;
    }
  }

  
  public void Run_NPC_Update (float deltaTime) {

    
    timeToRetryHavePath -= deltaTime;
    if(timeToRetryHavePath <= 0.0f) {
      timeToRetryHavePath = 0.0f;
    } else {
      return;
    }

    if (npcID_in_path_ < 0 || null == npc_path_calculator_) {
      return;
    }

    foreach (KeyValuePair<int, NPC_Info> currentNPCPair in npcs_) {

      NPC_Info current_npc = currentNPCPair.Value;

      if (current_npc.data.npc_ID == npcID_in_path_) {

        if (npc_path_calculator_.IsFinished()) {

          List<PathNode> result = npc_path_calculator_.GetFinishList();

          if (result != null && result.Count > 0) {

            List<Vector3> route = new List<Vector3>();
            for (int j = 0; j < result.Count; j++) {
              route.Add(new Vector3(result[j].x, 1.0f, result[j].y));
            }

            current_npc.the_npc_controler.SetRoute(route);
            current_npc.data.npc_researchPath = true;

          } else {
            timeToRetryHavePath = 0.250f;
            current_npc.data.npc_researchPath = false;
          }

          current_npc.data.npc_researchingPath = false;

          npcID_in_path_ = -1;

          Game_Manager.threadSys.ReturnThread(npc_path_calculator_);
          npc_path_calculator_ = null;
        }

      }

    }

  }

  public void AddNpc(int ID, GameObject npcPrefab, bool autoReturn = true) {

    if (npcs_.ContainsKey(ID)) {
      Debug.Log("ERROR: NPC ID ALREADY EXIST");
      return;
    }

    NPC_Info new_npc = new NPC_Info {
      the_npc_controler = gameObject.AddComponent<NPC_controler>()
    };
    new_npc.the_npc_controler.npc_prefab_ = npcPrefab;
    new_npc.the_npc_controler.Init();
    new_npc.the_npc_controler.speed_ = 2.0f;
    new_npc.the_npc_controler.autoReturn_ = autoReturn;

    new_npc.data = new BasicBuidsData {
      npcData = new_npc.the_npc_controler.GetData(),
      npc_ID = ID,
      npc_researchPath = false,
      npc_researchingPath = false,
      npc_sended = false
    };

    npcs_.Add(ID, new_npc);
  }
    
  public NPC_controler GetNPCControler(int ID) {

    if (npcs_.ContainsKey(ID)) {
      return npcs_[ID].the_npc_controler;
    }
    return null;
  }

  public BasicBuidsData GetNPCdata(int ID) {

    if (npcs_.ContainsKey(ID)) {
      return npcs_[ID].data;
    }
    return null;
  }

  public bool FindPathToPoints(int ID, List<MapController.Build> points, bool canWalkInGrass = false, bool startIntheBuild = true) {

    if (npcID_in_path_ > -1 || timeToRetryHavePath > 0.0f) {
      return false;
    }

    if (!npcs_.ContainsKey(ID)) { return false; }

    if (null == npc_path_calculator_) {
      npc_path_calculator_ = Game_Manager.threadSys.GetThread();
      if (null == npc_path_calculator_) {        
        return false;
      }
    }


    NPC_Info current_npc = npcs_[ID];

    if (current_npc.data.npc_ID == ID && (current_npc.the_npc_controler.finish_ || !current_npc.the_npc_controler.walk_)) {
      npcID_in_path_ = ID;

      npc_path_calculator_.SetGrid(Game_Manager.mapSys.GetGameGrid(), canWalkInGrass);

      if (startIntheBuild) {
        npc_path_calculator_.SetStartPoint(new Vector2Int((int)transform.position.x, (int)transform.position.z), size_);
      } else {

        Vector2Int currentNpcPosition = new Vector2Int();
        Vector3 lastPoint = current_npc.the_npc_controler.init_transform_;
        currentNpcPosition.Set((int)lastPoint.x, (int)lastPoint.z);
        npc_path_calculator_.SetStartPoint(currentNpcPosition, new Vector2Int(1,1));
      }

      npc_path_calculator_.SetEndsPoints(points);

      current_npc.the_npc_controler.ResetStatus();

      bool check = npc_path_calculator_.LaunchThread();
      if (check) {
        current_npc.data.npc_researchPath = false;
        current_npc.data.npc_researchingPath = true;
        return true;
      }
    }

    if (null != npc_path_calculator_) {
      Game_Manager.threadSys.ReturnThread(npc_path_calculator_);
      npc_path_calculator_ = null;
      npcID_in_path_ = -1;
    }

    return false;

  }


  public void SendNPC(int ID) {

    if (npcs_.ContainsKey(ID)) {
      NPC_Info current_npc = npcs_[ID];
      if (current_npc.data.npc_researchPath) {
        current_npc.the_npc_controler.StartRoute();        
      }
    }

  }

  
  public void ConfigNPCToSend(int ID, string producName, int ammount, bool pickUp, bool DestinyIsMarket) {
    if (!npcs_.ContainsKey(ID)) {return;}

    NPC_Info current_npc = npcs_[ID];

    if (current_npc.data.npc_sended) {return;}


    string buildTargetName = DestinyIsMarket ? "market" : "storage";
    List <MapController.Build> allStorageToSendNpc = Game_Manager.mapSys.GetBuildsListByName(buildTargetName);

    storagesToSendNpc.Clear();

    for (int s = 0; s < allStorageToSendNpc.Count; s++) {

      if (DestinyIsMarket) {
        Market currentStorage = allStorageToSendNpc[s].build_ptr.GetComponent<Market>();
        if (currentStorage.RecervStorage(producName, ammount, false)) {
          storagesToSendNpc.Add(allStorageToSendNpc[s]);
        }
      } else {
        Storage currentStorage = allStorageToSendNpc[s].build_ptr.GetComponent<Storage>();
        if (currentStorage.RecervStorage(producName, ammount, pickUp, false)) {
          storagesToSendNpc.Add(allStorageToSendNpc[s]);
        }
      }

    }

    if (storagesToSendNpc.Count > 0) {
      bool check = FindPathToPoints(ID, storagesToSendNpc);
      if (check) {
        current_npc.data.npc_sended = true;
      }
    }


  }

  public bool UpdateNPCToSend(int ID, string producName, int ammount, bool pickUp, bool DestinyIsMarket, ref int CanBeSended) {
    if (!npcs_.ContainsKey(ID)) { return false; }

    NPC_Info current_npc = npcs_[ID];

    if (!current_npc.data.npc_sended) { return false; }
    if((!current_npc.data.npc_researchPath && !current_npc.data.npc_researchingPath)) { current_npc.data.npc_sended = false; return false; }
    if (!current_npc.data.npc_researchPath) { return false; }

    if (!current_npc.the_npc_controler.walk_) {

      Vector3 lastPoint = current_npc.the_npc_controler.route_[current_npc.the_npc_controler.route_.Count - 1];
      MapController.Build finalStorage = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

      if (finalStorage.name != "error") {

        bool check = false;
        if (DestinyIsMarket) {
          check = finalStorage.build_ptr.GetComponent<Market>().RecervStorage(producName, ammount, true);
        } else {
          check = finalStorage.build_ptr.GetComponent<Storage>().RecervStorage(producName, ammount, pickUp, true);
        }

        if (check) {
          CanBeSended = 1;
          current_npc.the_npc_controler.StartRoute(!pickUp);          
        } else {
          current_npc.data.npc_sended = false;
        }
      }

    } else if (!current_npc.the_npc_controler.reachDestiny) {
      Vector3 lastPoint = current_npc.the_npc_controler.route_[current_npc.the_npc_controler.route_.Count - 1];
      MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

      if (checkBuild.name == "error") {
        current_npc.the_npc_controler.return_ = true;        
      }

    } else if (!current_npc.the_npc_controler.return_) {

      Vector3 lastPoint = current_npc.the_npc_controler.route_[current_npc.the_npc_controler.route_.Count - 1];
      MapController.Build finalStorage = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

      if (finalStorage.name != "error") {
        bool check = false;

        if (DestinyIsMarket) {
          check = finalStorage.build_ptr.GetComponent<Market>().TakeStorage(producName, ammount);          
        } else {
          check = finalStorage.build_ptr.GetComponent<Storage>().TakeStorage(producName, ammount, pickUp);
        }

        if (check) {
          current_npc.the_npc_controler.return_ = true;
          if (current_npc.the_npc_controler.NPC_Animation) {

            if (!pickUp) {
              current_npc.the_npc_controler.NPC_Animation.DropBox();
            } else {
              current_npc.the_npc_controler.NPC_Animation.PickBox();
            }
          }
        }
      }

    }

    if (current_npc.the_npc_controler.finish_) {
      current_npc.data.npc_sended  = false;

      if (current_npc.the_npc_controler.reachDestiny) {
        return true;
      } else {
        CanBeSended = -1;
      }
      
    }
    
    return false;
  }


  public void CanselNPCSend(int ID, string producName, int ammount, bool pickUp, bool DestinyIsMarket) {

    if (!npcs_.ContainsKey(ID)) { return; }
    NPC_Info current_npc = npcs_[ID];

    if(current_npc.the_npc_controler.walk_ && !current_npc.the_npc_controler.reachDestiny) {

      Vector3 lastPoint = current_npc.the_npc_controler.route_[current_npc.the_npc_controler.route_.Count - 1];
      MapController.Build checkBuild = Game_Manager.mapSys.GetBuildByPosition((int)lastPoint.x, (int)lastPoint.z);

      if (checkBuild.build_ptr) {
        if (DestinyIsMarket) {
          checkBuild.build_ptr.GetComponent<Market>().CancelReservation(producName, ammount);
        } else {
          checkBuild.build_ptr.GetComponent<Storage>().CancelReservation(producName, ammount, pickUp);
        }
      }

    }
  }


  public List<Interface_Data> GetSaveData() {

    List<Interface_Data> saveDataList = new List<Interface_Data>();

    foreach (KeyValuePair<int, NPC_Info> currentNPCPair in npcs_) {
      NPC_Info current_npc = currentNPCPair.Value;

      current_npc.data.npcData = current_npc.the_npc_controler.GetData();
      saveDataList.Add(current_npc.data);

    }

    return saveDataList;
  }

  public void LoadData(Interface_Data data) {

    BasicBuidsData realData = data as BasicBuidsData;

    if(null == npcs_) {
      npcs_ = new Dictionary<int, NPC_Info>();
    }

    if (realData != null) {

      NPC_Info new_npc = new NPC_Info {
        the_npc_controler = gameObject.AddComponent<NPC_controler>()
      };
      new_npc.the_npc_controler.SetData(realData.npcData);

      new_npc.data = new BasicBuidsData {
        npcData = new_npc.the_npc_controler.GetData(),
        npc_ID = realData.npc_ID,

        npc_researchingPath = false,
        npc_researchPath = realData.npc_researchPath,
        npc_sended = realData.npc_sended
      };

      npcs_.Add(new_npc.data.npc_ID, new_npc);

    }

  }

  //----------------------------------

  public void SetWorkers(int count) {
    currentworkers = count;
  }

  public int GetWorkersNeeded() {
    return workesNeeded;
  }

  //--------------------------

  public virtual void UpdateBuild(float deltaTime) {



  }


  public void changeActiveSmoke(bool newState) {
    smoke.SetActive(newState);
  }

}
