using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingHutUI : MonoBehaviour {
  public FishingHut_Data myOwnerData = null;

  public Text FishText;
  

  // Use this for initialization
  void Start() {
    updateText();
  }

  // Update is called once per frame
  void FixedUpdate() {
    updateText();
  }

  void updateText() {
    FishText.text = "Fish Stored: " + myOwnerData.fishStored + " / 200";    
  }

}
