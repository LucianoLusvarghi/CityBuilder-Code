using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineAverangeControler : MonoBehaviour {

  public string myResourse_;

  public Text consumed_;
  public Text produced_;
  public Text diference_;

  public Text Stored_;
  public Text Avaible_;

  public LineRenderer line_produced;
  public LineRenderer line_consumed;

  public RectTransform panel_produced;
  public RectTransform panel_consumed;

  ManagerGame manager;
    
	// Use this for initialization
	public void Init() {
    

    manager = GameObject.Find("GM").GetComponent<ManagerGame>();

  }
	
  public void OnEnable() {

    if (!manager) {
      return;
    }

    for(int i=0; i< manager.resourceSys.average_.Count; i++) {

      if(manager.resourceSys.average_[i].name == myResourse_) {
        UpdateLine(manager.resourceSys.average_[i].average);
      }

    }

  }

  void UpdateLine(List<ResourcesStats.ResourseMonitor> newList) {

    if(newList.Count < 2) {
      return;
    }

    float max_value = 0;

    float consumed = 0.0f;
    float produced = 0.0f;

    for(int i=0; i< newList.Count; i++) {

      if (newList[i].consumed > max_value) {
        max_value = newList[i].consumed;
      }

      if (newList[i].produced > max_value) {
        max_value = newList[i].produced;
      }

      consumed += newList[i].consumed;
      produced += newList[i].produced;
    }

    consumed /= newList.Count;
    produced /= newList.Count;

    consumed_.text = "Consumed * min: " + (int)(consumed * 60.0f);
    produced_.text = "Produced * min: " + (int)(produced * 60.0f);
    diference_.text = "Diff: " + ((int)(produced * 60.0f) - (int)(consumed * 60.0f));

    float multiplier = 10.0f;

    while((max_value * multiplier) > panel_produced.rect.height) {
      multiplier -= 0.1f;
    }


    float x_delta = panel_produced.rect.width / (newList.Count - 1);

    line_produced.positionCount = newList.Count;
    line_consumed.positionCount = newList.Count;

    for (int i = 0; i < newList.Count; i++) {

      Vector3 position = new Vector3();

      position.Set(i * x_delta, newList[i].produced * multiplier, 0.0f);

      line_produced.SetPosition(i, position);

      position.y = newList[i].consumed * multiplier;
      line_consumed.SetPosition(i, position);

    }




  }

  public void UpdateStored(ResourcesStats.ResourseMonitor resourseInfo) {

    Stored_.text = "Stored: " + resourseInfo.stored;
    Avaible_.text = "Avaible: " + resourseInfo.avaible;
  }

}
