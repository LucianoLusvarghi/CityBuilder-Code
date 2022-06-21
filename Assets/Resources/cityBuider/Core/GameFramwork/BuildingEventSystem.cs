using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildsEvents_Data : Interface_Data {

  public float[] burningBuilding;
  public float[] time_burningBuilding;  

}

public class BuildingEventSystem : MonoBehaviour, Interface_Save {

  public List<BasicBuilds> burningBuilding;
  public List<float> time_burningBuilding;
  public List<GameObject> smokeEffect_list;

  List<Vector3> loadedBurningBuilding = null;
  List<float> loadedBurningTime = null;

  List<BasicBuilds> BuildingCanBurnRegisted;
  List<Homes> homes_list;
  
  public float TimeBetweenEventLaunch;
  public float FireProbabiliti;
  public float TimeBurning;

  public float currentTimePassed;

  public ManagerGame Game_Manager = null;

  // Use this for initialization
  void Start () {
    burningBuilding = new List<BasicBuilds>();
    BuildingCanBurnRegisted = new List<BasicBuilds>();
    smokeEffect_list = new List<GameObject>();
    if (loadedBurningBuilding == null) {
      loadedBurningBuilding = new List<Vector3>();
    }

    homes_list = new List<Homes>();

    Game_Manager = GameObject.Find("GM").GetComponent<ManagerGame>();

    Game_Manager.eventSys = this;

    Game_Manager.saveSys.AddDataToSave(this);

  }

  public void OnDestroy() {

    Game_Manager.saveSys.LessDataToSave(this);

  }

  // Update is called once per frame
  void Update () {

    if (Input.GetKeyDown(KeyCode.F)) {
      MakeFire();
    }

    if(currentTimePassed > TimeBetweenEventLaunch) {
      currentTimePassed = 0.0f;

      float random = Random.Range(0.0f, 100.0f);

      if(random < FireProbabiliti) {
        MakeFire();
       
      }

    }

    for(int i=0; i < burningBuilding.Count; i++) {

      time_burningBuilding[i] += Time.deltaTime;

      if(time_burningBuilding[i] > TimeBurning) {

        Destroy(burningBuilding[i].gameObject);        

        burningBuilding.RemoveAt(i);
        time_burningBuilding.RemoveAt(i);        
      }

    }



    currentTimePassed += Time.deltaTime;
  }


  public void RegisterBuildingCanBurn(BasicBuilds theBuild) {

    if (!BuildingCanBurnRegisted.Contains(theBuild)) {
      BuildingCanBurnRegisted.Add(theBuild);
    }

    for(int i=0; i < loadedBurningBuilding.Count; i++) {

      if(theBuild.transform.position == loadedBurningBuilding[i]) {


        burningBuilding.Add(theBuild);
        time_burningBuilding.Add(loadedBurningTime[i]);

        theBuild.changeActiveSmoke(true);

        loadedBurningBuilding.RemoveAt(i);
        loadedBurningTime.RemoveAt(i);
      }


    }


  }

  public void RemoveBuildingCanBurn(BasicBuilds theBuild) {

    if (BuildingCanBurnRegisted.Contains(theBuild)) {
      BuildingCanBurnRegisted.Remove(theBuild);
    }

    if (burningBuilding.Contains(theBuild)) {

      int index = burningBuilding.IndexOf(theBuild);

      burningBuilding.Remove(theBuild);
      time_burningBuilding.RemoveAt(index);

      theBuild.changeActiveSmoke(false);
    }

  }

  public void RegisterHome(Homes theHome) {
    if (!homes_list.Contains(theHome)) {
      homes_list.Add(theHome);
    }
  }

  public void RemoveHome(Homes theHome) {
    if (homes_list.Contains(theHome)) {
      homes_list.Remove(theHome);
    }
  }

  public List<BasicBuilds> GetBurningBuilding() {
    return burningBuilding;
  }

  public void ClearBurningBuild(BasicBuilds clearBuild) {
    if (burningBuilding.Contains(clearBuild)) {

      int index = burningBuilding.IndexOf(clearBuild);

      burningBuilding.Remove(clearBuild);
      time_burningBuilding.RemoveAt(index);

      clearBuild.changeActiveSmoke(false);
    }
  }

  public bool CheckBurningBuilding(BasicBuilds theBuilding) {
    return burningBuilding.Contains(theBuilding);
  }

  void ChoseHomes(int index) {
    GameObject theBurningBuild = BuildingCanBurnRegisted[index].gameObject;

    int numHomesChosed = 3;

    if(homes_list.Count <= numHomesChosed) {
      numHomesChosed = homes_list.Count - 2;
      if(numHomesChosed <= 0) {
        return;
      }
    }

    List<Homes> homes = new List<Homes>();
    for (int i = 0; homes.Count < numHomesChosed && i < homes_list.Count; i++) {

      if (theBurningBuild != homes_list[i].gameObject && !homes_list[i].data_.fireResponse) {
        homes.Add(homes_list[i]);
      }
    }

    if(homes.Count < numHomesChosed) {
      numHomesChosed = homes.Count;
    }


    for (int i = 0; i < numHomesChosed; i++) {

      for (int j = i; j < numHomesChosed; j++) {
        
        if(i != j) {

          float distanseA = Vector3.Distance(theBurningBuild.transform.position, homes[i].transform.position);
          float distanseB = Vector3.Distance(theBurningBuild.transform.position, homes[j].transform.position);

          if(distanseB < distanseA) {
            Homes tmp = homes[i];
            homes[i] = homes[j];
            homes[j] = tmp;
          }

        }

      }

    }


    for (int h=0; h< numHomesChosed; h++) {
      

      for(int i=0; i < homes_list.Count; i++) {

        Vector3 homeChosedPosition = homes[h].transform.position;

        if (!homes.Contains(homes_list[i])){

          float distanseA = Vector3.Distance(theBurningBuild.transform.position, homeChosedPosition);
          float distanseB = Vector3.Distance(theBurningBuild.transform.position, homes_list[i].transform.position);

          if (distanseB < distanseA && !homes_list[i].data_.fireResponse) {
            homes[h] = homes_list[i];
          }

        }

      }

    }


    for(int i=0; i < numHomesChosed; i++) {

      homes[i].data_.fireResponse = true;

    }



  }

  void MakeFire() {

    int randomIndex = Random.Range(0, BuildingCanBurnRegisted.Count);

    burningBuilding.Add(BuildingCanBurnRegisted[randomIndex]);
    time_burningBuilding.Add(0.0f);

    BuildingCanBurnRegisted[randomIndex].changeActiveSmoke(true);

    ChoseHomes(randomIndex);

  }


  public SaveDataStructure GetSaveData() {
    SaveDataStructure saveData_ = new SaveDataStructure {
      allDataToSave_ = new List<Interface_Data>(),
      prefab = "cityBuider/Placeables/BuildingEventSystem",
      position = new float[3],
      rotation = new float[4]
    };

    saveData_.allDataToSave_.Clear();

    BuildsEvents_Data data_ = new BuildsEvents_Data {
      burningBuilding = new float[burningBuilding.Count * 3],
      time_burningBuilding = new float[burningBuilding.Count]
    };

    for(int i=0; i< burningBuilding.Count; i++) {

      data_.burningBuilding[(i * 3)] = burningBuilding[i].transform.position.x;
      data_.burningBuilding[(i * 3) + 1] = burningBuilding[i].transform.position.y;
      data_.burningBuilding[(i * 3) + 2] = burningBuilding[i].transform.position.z;


      data_.time_burningBuilding[i] = time_burningBuilding[i];
    }


    saveData_.allDataToSave_.Add(data_);

    return saveData_;

  }

  public void LoadData(SaveDataStructure loadedData) {

    BuildsEvents_Data data_;

    data_ = loadedData.allDataToSave_[0] as BuildsEvents_Data;

    loadedBurningBuilding = new List<Vector3>();
    loadedBurningTime = new List<float>();

    for (int i = 0; i < data_.time_burningBuilding.Length; i++) {

      loadedBurningBuilding.Add(new Vector3(data_.burningBuilding[(i*3)], data_.burningBuilding[(i * 3) +1], data_.burningBuilding[(i * 3) + 2]) );
      loadedBurningTime.Add(data_.time_burningBuilding[i]);
    }

  }

  public GameObject GetGameObjectInScene() {
    return gameObject;
  }
}
