using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Animation : MonoBehaviour {

  public GameObject Camera;

  public GameObject rootPoints;

  public float inverse_speed;

  public bool autoPlay = false;

  List<Transform> points;

  int index = 0;
  float lerp = 0.0f;

  bool playAnimation = false;


	// Use this for initialization
	void Start () {
    points = new List<Transform>();

    for(int i=0; i< rootPoints.transform.childCount; i++) {
      points.Add(rootPoints.transform.GetChild(i));
    }

  }
	
	// Update is called once per frame
	void Update () {
		
    if((Input.GetKeyDown(KeyCode.R) || autoPlay) && !playAnimation) {
      playAnimation = true;
      index = 0;
      lerp = 0.0f;

      Camera.transform.position = points[0].position;
      Camera.transform.rotation = points[0].rotation;
    }

    if (!playAnimation) {
      return;
    }

    lerp += Time.deltaTime * (1.0f / inverse_speed);

    if(lerp > 1.0f) {
      lerp = 0.0f;
      index++;
    }

    if(index >= points.Count - 1) {
      playAnimation = false;
      return;
    }

    Camera.transform.position = Vector3.Lerp(points[index].position, points[index + 1].position, lerp);
    Camera.transform.rotation = Quaternion.Lerp(points[index].rotation, points[index+1].rotation, lerp);





  }

}
