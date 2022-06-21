using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_stats_manager : MonoBehaviour {

  [System.Serializable]
  public struct ResoursesScreen {
    public string name;
    public GameObject theScreen;
  };

  public ResoursesScreen[] resourses_;

  public GameObject MainMonitor_;

  private void Start() {

    for (int i = 0; i < resourses_.Length; i++) {
      resourses_[i].theScreen.GetComponent<LineAverangeControler>().Init();
    }

  }

  public void SetActiveMainMonitor(bool active) {
    MainMonitor_.SetActive(active);
    
  }

  public void ActiveResourse(string ResourseName) {

    UpdateResourseLine(ResourseName);
    
    for (int i=0; i< resourses_.Length; i++) {

      resourses_[i].theScreen.SetActive(resourses_[i].name == ResourseName);

    }


  }

  public void UpdateResourseLine(string ResourseName) {

    for (int i = 0; i < resourses_.Length; i++) {

      if(resourses_[i].name == ResourseName && MainMonitor_.activeSelf) {

        resourses_[i].theScreen.GetComponent<LineAverangeControler>().OnEnable();

      }

    }

  }

  public void UpdateResourseStore(string ResourseName, ResourcesStats.ResourseMonitor resourse) {

    if(null == MainMonitor_) {
      return;
    }

    for (int i = 0; i < resourses_.Length; i++) {

      if (resourses_[i].name == ResourseName && MainMonitor_.activeSelf) {

        resourses_[i].theScreen.GetComponent<LineAverangeControler>().UpdateStored(resourse);

      }

    }

  }

}
