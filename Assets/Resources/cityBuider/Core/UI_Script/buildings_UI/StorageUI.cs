using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour {

  public Storage myOwnerData = null;

  public Text wood_current;
  public Text wood_max;

  public Text fish_current;
  public Text fish_max;

  public Text utilities_current;
  public Text utilities_max;

  public Text vegetables_current;
  public Text vegetables_max;

  

  // Use this for initialization
  void Start () {
    UpdateText();
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    UpdateText();
  }



  void UpdateText() {

    wood_current.text = myOwnerData.storeManagment_[0].current.ToString();
    wood_max.text = myOwnerData.storeManagment_[0].max.ToString();

    fish_current.text = myOwnerData.storeManagment_[1].current.ToString();
    fish_max.text = myOwnerData.storeManagment_[1].max.ToString();

    utilities_current.text = myOwnerData.storeManagment_[2].current.ToString();
    utilities_max.text = myOwnerData.storeManagment_[2].max.ToString();

    vegetables_current.text = myOwnerData.storeManagment_[3].current.ToString();
    vegetables_max.text = myOwnerData.storeManagment_[3].max.ToString();
  }

  public void AddMax(int index) {

    if(myOwnerData.storeManagment_[index].max < 800) {
      myOwnerData.storeManagment_[index].max += 100;
    }

  }

  public void LessMax(int index) {

    if (myOwnerData.storeManagment_[index].max > 0) {
      myOwnerData.storeManagment_[index].max -= 100;
    }

  }

}
