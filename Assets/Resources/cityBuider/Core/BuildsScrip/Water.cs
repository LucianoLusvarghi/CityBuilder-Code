using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : BasicBuilds {

	// Use this for initialization
	void Start () {
    base.Init();    
    base.Game_Manager.mapSys.RegiterBuild(new Vector2Int((int)transform.position.x, (int)transform.position.z), base.size_, "water", gameObject);
  }
	
}
