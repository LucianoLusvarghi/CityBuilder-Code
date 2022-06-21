using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
public class SaveDataStructure{

  public string prefab;
  public float[] position;
  public float[] rotation;

  public List<Interface_Data> allDataToSave_;

}


[System.Serializable]
public class SaveDataFile {

  public List<SaveDataStructure> allDataToSave_;

}

public class SaveSystem{

  SaveDataFile dataFile_;

  List<Interface_Save> save_data;

  ManagerGame manager;

  int loadSlot = 0;

  public SaveSystem() {
    dataFile_ = new SaveDataFile {
      allDataToSave_ = new List<SaveDataStructure>()
    };

    save_data = new List<Interface_Save>();

    manager = GameObject.Find("GM").GetComponent<ManagerGame>();

  }
  
  public void AddDataToSave(Interface_Save data) {
    save_data.Add(data);
  }

  public void LessDataToSave(Interface_Save data) {
    save_data.Remove(data);
  }

  public void SaveData(int slot) {

    dataFile_.allDataToSave_.Clear();

    foreach(Interface_Save GO in save_data) {
      dataFile_.allDataToSave_.Add(GO.GetSaveData());
    }

    BinaryFormatter formatter = new BinaryFormatter();

    string path = Application.persistentDataPath + "/city_build_save_file" + slot +".esat";

    FileStream stream = new FileStream(path, FileMode.Create);
       
    formatter.Serialize(stream, dataFile_);
    stream.Close();

  }


  public void LoadData(int slot) {
    loadSlot = slot;
    manager.StartCoroutine(CorrutineLoadData());    
  }

  IEnumerator CorrutineLoadData() {


    foreach (Interface_Save oldObjectsToSave in save_data) {
      GameObject.Destroy(oldObjectsToSave.GetGameObjectInScene());
    }

    yield return new WaitForSeconds(0.02f);

    string path = Application.persistentDataPath + "/city_build_save_file" + loadSlot + ".esat";

    if (File.Exists(path)) {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream stream = new FileStream(path, FileMode.Open);

      SaveDataFile data = formatter.Deserialize(stream) as SaveDataFile;

      for (int i = 0; i < data.allDataToSave_.Count; i++) {

        SaveDataStructure currentDataLoaded = data.allDataToSave_[i];

        GameObject prefab = (GameObject)Resources.Load(currentDataLoaded.prefab, typeof(GameObject));

        Vector3 position = new Vector3(currentDataLoaded.position[0], currentDataLoaded.position[1], currentDataLoaded.position[2]);
        Quaternion rotation = new Quaternion(currentDataLoaded.rotation[0], currentDataLoaded.rotation[1], currentDataLoaded.rotation[2], currentDataLoaded.rotation[3]);

        GameObject instancia = GameObject.Instantiate(prefab, position, rotation);

        Interface_Save loadedObject = instancia.GetComponent<Interface_Save>();

        if (loadedObject != null) {
          loadedObject.LoadData(currentDataLoaded);
        }

      }

      stream.Close();

    }

  }


}
