using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmUI : MonoBehaviour {

  public Farms myFarm;

  public ManagerGame manager = null;

  public Text foodStoredText;

  // Use this for initialization
  void Start () {
    manager = GameObject.Find("GM").GetComponent<ManagerGame>();
    foodStoredText.text = "FoodStored: " + myFarm.data_.foodStored + " / 200";
  }

  // Update is called once per frame
  void FixedUpdate () {
    foodStoredText.text = "FoodStored: " + myFarm.data_.foodStored + " / 200";
  }

  public void SelectCrop(int type) {

    manager.placeInWorldSys.ChangeCurrentObject(myFarm.cropsPrefabs_[type], myFarm.cropsShadowsPrefab_[type], new Vector2Int(5,5), "crops");


  }

  

}
