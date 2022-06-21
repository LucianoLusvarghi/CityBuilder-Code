using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour {

  public Homes myOwnerData = null;

  public Text text_water_stored;
  public Text text_firewood_stored;

  public Text text_food_meat_stored;
  public Text text_food_crops_stored;
  public Text text_food_fish_stored;

  public Text text_cloths_stored;

  public Text text_utilities_stored;

  public Text text_hygiene_value;
  public Text text_nature_value;
  public Text text_religion_value;

  public Text text_weapons_stored;

  // Use this for initialization
  void Start () {
    UpdateTextInfoView();
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    UpdateTextInfoView();
  }

  void UpdateTextInfoView() {

    if (null != myOwnerData) {
            
      int needed = 0;
      {
        needed = 0;
        if (myOwnerData.data_.currentPopulation >= 1)
          needed = 100;

        text_water_stored.text = "Water Stored: " + (int)myOwnerData.data_.water_stored + " / " + needed;

        if(needed > 0 && (int)myOwnerData.data_.water_stored < 50) {
          text_water_stored.color = new Color(1.0f, 0.5f, 0.0f);
        } else {
          text_water_stored.color = Color.black;
        }

        if (myOwnerData.data_.currentPopulation == 1 && myOwnerData.timeDown > 0.0f) {
          text_water_stored.color = Color.red;
        } 

      }

      {
        needed = 0;
        if (myOwnerData.data_.currentPopulation >= 2)
          needed = 100;

        text_firewood_stored.text = "Firewood Stored: " + (int)myOwnerData.data_.firewood_stored + " / " + needed;

        if (needed > 0 && (int)myOwnerData.data_.firewood_stored < 50) {
          text_firewood_stored.color = new Color(1.0f, 0.5f, 0.0f);
        } else {
          text_firewood_stored.color = Color.black;
        }

        if (myOwnerData.data_.currentPopulation == 2 && myOwnerData.timeDown > 0.0f) {
          text_firewood_stored.color = Color.red;
        }

      }

      {
        needed = 0;
        text_food_meat_stored.text = "Meat Stored: " + (int)myOwnerData.data_.food_meat_stored + " / " + needed;
      }

      {
        needed = 0;
        if (myOwnerData.data_.currentPopulation >= 5)
          needed = 100;

        text_food_crops_stored.text = "Crops Stored: " + (int)myOwnerData.data_.food_crops_stored + " / " + needed;

        if (needed > 0 && (int)myOwnerData.data_.food_crops_stored < 50) {
          text_food_crops_stored.color = new Color(1.0f, 0.5f, 0.0f);
        } else {
          text_food_crops_stored.color = Color.black;
        }

        if (myOwnerData.data_.currentPopulation == 5 && myOwnerData.timeDown > 0.0f) {
          text_food_crops_stored.color = Color.red;
        }

      }

      {
        needed = 0;
        if (myOwnerData.data_.currentPopulation >= 3)
          needed = 100;
        text_food_fish_stored.text = "Fish Stored: " + (int)myOwnerData.data_.food_fish_stored + " / " + needed;

        if (needed > 0 && (int)myOwnerData.data_.food_fish_stored < 50) {
          text_food_fish_stored.color = new Color(1.0f, 0.5f, 0.0f);
        } else {
          text_food_fish_stored.color = Color.black;
        }

        if (myOwnerData.data_.currentPopulation == 3 && myOwnerData.timeDown > 0.0f) {
          text_food_fish_stored.color = Color.red;
        }

      }

      {
        needed = 0;
        text_cloths_stored.text = "Cloths Stored: " + (int)myOwnerData.data_.cloths_stored + " / " + needed;
      }

      {
        needed = 0;
        if (myOwnerData.data_.currentPopulation >= 4)
          needed = 100;
        text_utilities_stored.text = "Utilities Stored: " + (int)myOwnerData.data_.utilities_stored + " / " + needed;

        if (needed > 0 && (int)myOwnerData.data_.utilities_stored < 50) {
          text_utilities_stored.color = new Color(1.0f, 0.5f, 0.0f);
        } else {
          text_utilities_stored.color = Color.black;
        }

        if (myOwnerData.data_.currentPopulation == 4 && myOwnerData.timeDown > 0.0f) {
          text_utilities_stored.color = Color.red;
        }

      }

      {
        needed = 0;
        text_hygiene_value.text = "Hygiene Value: " + (int)myOwnerData.data_.hygiene_value + " / " + needed;
      }

      {
        needed = 0;
        text_nature_value.text = "Nature Value: " + (int)myOwnerData.data_.nature_value + " / " + needed;
      }

      {
        needed = 0;
        text_religion_value.text = "Religion Value: " + (int)myOwnerData.data_.religion_value + " / " + needed;
      }

      {
        needed = 0;
        text_weapons_stored.text = "Weapons Stored: " + (int)myOwnerData.data_.weapons_stored + " / " + needed;
      }
    }
  }

}
