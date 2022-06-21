using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SceneGameDataSave {

  public List<Interface_Data> save_data;

}

public class ManagerGame : MonoBehaviour {

  [System.Serializable]
  public struct BuildCost {
    public string resourceName;
    public int ammount;
  }

  [System.Serializable]
  public struct BuildsInfo {
    public string name;
    public GameObject prefab;
    public GameObject shadow;
    public Vector2Int cellsSize;
    public List<BuildCost> cost;

    public Transform container;
  };

  public BuildsInfo[] Placeables;
  


  static ManagerGame inst = null;

  public SaveSystem saveSys;
  public MapController mapSys;
  public PlaceInWorld placeInWorldSys;
  public ThreadManager threadSys;
  public PopulationManager populationSys;
  public ResourcesStats resourceSys;
  public Builds_manager buildsSys;
  public BuildingEventSystem eventSys;

  LoadMapBase loadMap = null;

  UI_Manager ui_manager;
  public UI_stats_manager ui_Stats;

  // Use this for initialization
  void Awake () {

    if (null != inst) {
      Destroy(this);
    } else {
      inst = this;

      //eventSys = GameObject.Find("BuildingEventSystem").GetComponent<BuildingEventSystem>();

      saveSys = new SaveSystem();      
      mapSys = new MapController();
      mapSys.Init(100, 100, MapController.CellType.grass);

      buildsSys = new Builds_manager();
      buildsSys.Init();

      loadMap = GetComponent<LoadMapBase>();
      if (loadMap) {
        loadMap.LoadMap(0);
      }
      placeInWorldSys = GameObject.Find("Main Camera").GetComponent<PlaceInWorld>();

      ui_manager = GameObject.Find("UI").GetComponent<UI_Manager>();
      ui_Stats = GameObject.Find("UI").GetComponent<UI_stats_manager>();

      threadSys = new ThreadManager();
      threadSys.Init(10, this);

      populationSys = new PopulationManager();
      populationSys.Init(this);

      //resourceSys = new resourcesStats();
      resourceSys.Init(this);

      

    }
  }
	 
  
  
	// Update is called once per frame
	void Update () {
   
    {
      
      GameObject loadSys = GameObject.Find("MainMenu");

      if (loadSys) {

        ChangeScene scene = loadSys.GetComponent<ChangeScene>();

        if (scene.Mode == 1 && scene.time > 0.0f) {
          Load();
          loadSys.GetComponent<ChangeScene>().Mode = 0;
        } else {
          scene.time += Time.deltaTime;
        }
      }
    }

    if (Input.GetKeyDown(KeyCode.Escape)) {

      if(Time.timeScale > 0.0f) {
        Time.timeScale = 0.0f;
        ui_manager.MoveToPauseMode();
        ui_Stats.SetActiveMainMonitor(false);
      } else {
        Time.timeScale = 1.0f;
        ui_manager.ReturnFromPauseMode();
      }

    }

    if (Input.GetKeyDown(KeyCode.Tab) && Time.timeScale > 0.0f) {
      ui_manager.ChangeToStatsMode();
    }

    populationSys.UpdateWorkers();


    resourceSys.Update(Time.deltaTime);

  }


  private void FixedUpdate() {
    buildsSys.Update(Time.fixedDeltaTime);
  }

  public void OnDestroy() {
    Time.timeScale = 1.0f;
  }


  public void ChangePlaceableSelected(string name) {

    for(int i=0; i< Placeables.Length; i++) {

      if( Placeables[i].name == name) {
        
        placeInWorldSys.ChangeCurrentObject(Placeables[i].prefab, Placeables[i].shadow, Placeables[i].cellsSize, name);

      }

    }

  }


  public bool PayCost(string name) {
    for (int i = 0; i < Placeables.Length; i++) {

      if (Placeables[i].name == name) {

        for (int c = 0; c < Placeables[i].cost.Count; c++) {
          if (!CheckCost(Placeables[i].cost[c])) {
            ui_manager.SetErrorMesage("Insuficient: need " + Placeables[i].cost[c].resourceName + " x " + Placeables[i].cost[c].ammount);
            return false;
          }
        }

        for (int c = 0; c < Placeables[i].cost.Count; c++) {
          RemoveCost(Placeables[i].cost[c]);
        }

        return true;
      }

    }
    return true;
  }

  public void ChangeToDestroyMode() {
    placeInWorldSys.SetDestructionMode();
  }

  public bool CheckCost(BuildCost cost) {

    List<MapController.Build> allStorageToSendNpc = mapSys.GetBuildsListByName("storage");

    int totalRecuest = 0;

    for(int i=0; i< allStorageToSendNpc.Count && totalRecuest < cost.ammount; i++) {
      Storage currentStorage = allStorageToSendNpc[i].build_ptr.GetComponent<Storage>();

      totalRecuest += currentStorage.GetTotalStoredOf(cost.resourceName);

    }


    return totalRecuest >= cost.ammount;
  }


  public void RemoveCost(BuildCost cost) {

    List<MapController.Build> allStorageToSendNpc = mapSys.GetBuildsListByName("storage");

    int totalRecuest = cost.ammount;

    for (int i = 0; i < allStorageToSendNpc.Count && totalRecuest > 0; i++) {
      Storage currentStorage = allStorageToSendNpc[i].build_ptr.GetComponent<Storage>();

      int currentStored = currentStorage.GetTotalStoredOf(cost.resourceName);
      if(currentStored >= totalRecuest) {
        currentStorage.RecervStorage(cost.resourceName, totalRecuest, true, true);
        currentStorage.TakeStorage(cost.resourceName, totalRecuest, true);
        totalRecuest = 0;
      } else {
        currentStorage.RecervStorage(cost.resourceName, currentStored, true, true);
        currentStorage.TakeStorage(cost.resourceName, currentStored, true);
        totalRecuest -= currentStored;
      }
    }

  }


  public void ReturnMainMenu() {
    SceneManager.LoadScene(0);
  }

  public void Load() {
    saveSys.LoadData(0);
    Time.timeScale = 1.0f;
    ui_manager.ReturnFromPauseMode();
  }

  public void Save() {
    saveSys.SaveData(0);
    Time.timeScale = 1.0f;
    ui_manager.ReturnFromPauseMode();
  }


  public Transform getPlaceableContainer(string name) {

    for (int i = 0; i < Placeables.Length; i++) {

      if (Placeables[i].name == name) {

        return Placeables[i].container;

      }

    }


    return null;
  }




}
