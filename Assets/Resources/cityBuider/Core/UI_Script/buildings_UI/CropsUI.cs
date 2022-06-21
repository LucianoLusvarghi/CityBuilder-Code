using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropsUI : MonoBehaviour {

  public Crops_Data myOwnerData = null;

  public Text GrowthStatus;

  // Use this for initialization
  void Start () {
    updateText();
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    updateText();
  }

  void updateText() {
    GrowthStatus.text = "Growth Status: " + (int)(100.0f * myOwnerData.currentGrowthStatus_) + "%";
  }
}
