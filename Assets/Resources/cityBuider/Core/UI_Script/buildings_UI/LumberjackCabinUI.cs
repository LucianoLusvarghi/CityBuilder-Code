using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LumberjackCabinUI : MonoBehaviour {

  public LumberjackCabin_Data myOwnerData = null;

  public Text woodLogStoredText;
  public Text woodPlankStoredText;
  public Text woodProcessingProgressText;

  // Use this for initialization
  void Start () {    
    UpdateText();
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    UpdateText();    
  }

  void UpdateText() {
    woodLogStoredText.text =          "Wood Logs Stored: " + myOwnerData.woodLogStored + " / 100";
    woodPlankStoredText.text =        "Wood Planks Stored: " + myOwnerData.woodPlankStored + " / 200";
    woodProcessingProgressText.text = "Wood Processing Progress: " + (int)(myOwnerData.woodProcessingProgress * 100.0f) + "%";
  }

}
