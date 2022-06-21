using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowsChangeColor : MonoBehaviour {

  public MeshRenderer[] buildParts;

	// Use this for initialization
	void Start () {
    buildParts = gameObject.GetComponentsInChildren<MeshRenderer>();
	}
	
  public void ChangeColor(Color newColor) {

    foreach(MeshRenderer currentMaterial in buildParts) {
      currentMaterial.material.color = newColor;
    }

  }

}
