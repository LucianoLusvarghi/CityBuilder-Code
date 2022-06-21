using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilitiesFactoryUI : MonoBehaviour {
  public Utilities_Data myOwnerData = null;

  
  public Text woodPlankStoredText;
  public Text UtilitiesStoredText;
  public Text woodProcessingProgressText;

  // Use this for initialization
  void Start() {
    UpdateText();
  }

  // Update is called once per frame
  void FixedUpdate() {
    UpdateText();
  }

  void UpdateText() {
    UtilitiesStoredText.text = "Utilities Stored: " + myOwnerData.utilitiesStored + " / 100";
    woodPlankStoredText.text = "Wood Planks Stored: " + myOwnerData.woodStored + " / 200";
    woodProcessingProgressText.text = "Wood Processing Progress: " + (int)(myOwnerData.currentUtilitieProduction * 100.0f) + "%";
  }

}
