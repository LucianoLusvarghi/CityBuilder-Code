using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketUI : MonoBehaviour {
  public Market_Data myOwnerData = null;

  public Text slot_1;
  public Text slot_2;
  public Text slot_3;
  public Text slot_4;
  public Text slot_5;
  public Text slot_6;
  public Text slot_7;
  
  // Use this for initialization
  void Start() {
    UpdateText();
  }

  // Update is called once per frame
  void FixedUpdate() {
    UpdateText();
  }

  void UpdateText() {

    slot_1.text = "firewood: " +    myOwnerData.firewood_stored + "/200";
    slot_2.text = "meat: " +        myOwnerData.food_meat_stored + "/200";
    slot_3.text = "vegetables: " +  myOwnerData.food_vegetales_stored + "/200";
    slot_4.text = "fish: " +        myOwnerData.food_fish_stored + "/200";
    slot_5.text = "cloths: " +      myOwnerData.cloths_stored + "/200";
    slot_6.text = "utilities: " +   myOwnerData.utilities_stored + "/200";
    slot_7.text = "weapons: " +     myOwnerData.weapons_stored + "/200";
    
  }

}
