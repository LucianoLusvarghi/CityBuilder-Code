using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class CameraMovement_Data {

  public float speed_;
  public float sensitivity_;
  public float changeAltitude_;
  public float MinAltitude_;
  public float MaxAltitude_;

  public float lastX_;
  public float lastY_;

  public float rotationX_;
  public float rotationY_;

  public float currentAltitude_;

  public float[] postion;
  public float[] rotation;

}

public class CameraMovement : MonoBehaviour {

  public CameraMovement_Data data_;

  public float speed_ = 10.0f;
  public float boostSpeed = 50.0f;

  public float sensitivity_ = 2.0f;
  public float changeAltitude_ = 5.0f;

  public float Limit_minimun = 0.0f;
  public float Limit_maxim = 100.0f;

  public float MinAltitude_ = 15.0f;
  public float MaxAltitude_ = 50.0f;
  public float StartingAltitude = 25.0f;

  // Use this for initialization
  void Start () {

    if (!LoadData()) {
      data_.speed_ = speed_;
      data_.sensitivity_ = sensitivity_;
      data_.changeAltitude_ = changeAltitude_;
      data_.MinAltitude_ = MinAltitude_;
      data_.MaxAltitude_ = MaxAltitude_;

      data_.lastX_ = Input.mousePosition.x;
      data_.lastY_ = Input.mousePosition.y;
      data_.rotationX_ = 0.0f;
      data_.rotationY_ = 0.0f;
      transform.rotation = Quaternion.Euler(-data_.rotationY_, data_.rotationX_, 0.0f);

      data_.currentAltitude_ = StartingAltitude;

      Vector3 changeAltitude = transform.position;
      changeAltitude.y = data_.currentAltitude_;

      transform.position = changeAltitude;
    }
  }
	
	// Update is called once per frame
	void Update () {

    if (Input.GetMouseButton(2)) {

      data_.rotationX_ = data_.rotationX_ + ((Input.mousePosition.x - data_.lastX_) * data_.sensitivity_ * Time.deltaTime);
      data_.rotationY_ = data_.rotationY_ + ((Input.mousePosition.y - data_.lastY_) * data_.sensitivity_ * Time.deltaTime);

      if (data_.rotationY_ > 0.0f) data_.rotationY_ = 0.0f;
      if (data_.rotationY_ < -90.0f) data_.rotationY_ = -90.0f;

      transform.rotation = Quaternion.Euler(-data_.rotationY_, data_.rotationX_, 0.0f);
            
    }

   

   

    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
      Vector3 currentFordward = Quaternion.AngleAxis(data_.rotationX_, Vector3.up) * Vector3.forward;

      Vector3 newPosition = transform.position + (currentFordward * data_.speed_ * Time.deltaTime);
      if(newPosition.x > Limit_minimun && newPosition.x < Limit_maxim && newPosition.z > Limit_minimun && newPosition.z < Limit_maxim) {
        transform.position = newPosition;
      }

    }
    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
      Vector3 currentFordward = Quaternion.AngleAxis(data_.rotationX_, Vector3.up) * Vector3.back;

      Vector3 newPosition = transform.position + (currentFordward * data_.speed_ * Time.deltaTime);
      if (newPosition.x > Limit_minimun && newPosition.x < Limit_maxim && newPosition.z > Limit_minimun && newPosition.z < Limit_maxim) {
        transform.position = newPosition;
      }
    }
    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
      Vector3 currentFordward = Quaternion.AngleAxis(data_.rotationX_, Vector3.up) * Vector3.left;

      Vector3 newPosition = transform.position + (currentFordward * data_.speed_ * Time.deltaTime);
      if (newPosition.x > Limit_minimun && newPosition.x < Limit_maxim && newPosition.z > Limit_minimun && newPosition.z < Limit_maxim) {
        transform.position = newPosition;
      }
    }
    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
      Vector3 currentFordward = Quaternion.AngleAxis(data_.rotationX_, Vector3.up) * Vector3.right;

      Vector3 newPosition = transform.position + (currentFordward * data_.speed_ * Time.deltaTime);
      if (newPosition.x > Limit_minimun && newPosition.x < Limit_maxim && newPosition.z > Limit_minimun && newPosition.z < Limit_maxim) {
        transform.position = newPosition;
      }
    }
    if (Input.GetKey(KeyCode.LeftShift)) {
      data_.speed_ = boostSpeed;
    } else {
      data_.speed_ = speed_;
    }

    if(Input.mouseScrollDelta.y != 0.0f) {

      if(Input.mouseScrollDelta.y < 0.0f) {

        data_.currentAltitude_ += data_.changeAltitude_;
        if (data_.currentAltitude_ > data_.MaxAltitude_) data_.currentAltitude_ = data_.MaxAltitude_;
      } else {

        data_.currentAltitude_ -= data_.changeAltitude_;
        if (data_.currentAltitude_ < data_.MinAltitude_) data_.currentAltitude_ = data_.MinAltitude_;
      }

      Vector3 changeAltitude = transform.position;
      changeAltitude.y = data_.currentAltitude_;

      transform.position = changeAltitude;

    }

    data_.lastX_ = Input.mousePosition.x;
    data_.lastY_ = Input.mousePosition.y;

  }

  void OnDestroy() {
    SaveData();
  }

  void SaveData() {
    BinaryFormatter formatter = new BinaryFormatter();

    string path = Application.persistentDataPath + "/camera.esat";

    FileStream stream = new FileStream(path, FileMode.Create);

    data_.postion = new float[3];
    data_.postion[0] = transform.position.x;
    data_.postion[1] = transform.position.y;
    data_.postion[2] = transform.position.z;

    data_.rotation = new float[4];
    data_.rotation[0] = transform.rotation.x;
    data_.rotation[1] = transform.rotation.y;
    data_.rotation[2] = transform.rotation.z;
    data_.rotation[3] = transform.rotation.w;

    formatter.Serialize(stream, data_);
    stream.Close();
  }

  bool LoadData() {

    string path = Application.persistentDataPath + "/camera.esat";
    if (File.Exists(path)) {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream stream = new FileStream(path, FileMode.Open);

      data_ = formatter.Deserialize(stream) as CameraMovement_Data;

      if(data_.postion[0] < 0.0f ||  data_.postion[2] < 0.0f) {
        data_.postion[0] = 50.0f;
        data_.postion[2] = 50.0f;
      }

      transform.position = new Vector3(data_.postion[0], data_.postion[1], data_.postion[2]);
      transform.rotation = new Quaternion(data_.rotation[0], data_.rotation[1], data_.rotation[2], data_.rotation[3]);
      stream.Close();                  
      return true;

    }
    return false;
  }

}
