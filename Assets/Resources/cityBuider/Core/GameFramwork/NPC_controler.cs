using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC_Data {

  public float[] route;
  public bool walk;
  public bool return_;
  public float speed;
  public float lerp;
  public int index;
  public float[] init_transform;

  public bool finish;
  public bool autoReturn_;
  public bool reachDestiny;
  public string prefabName;
  public bool isActive;

  public bool animationCarryBox;
}

public class NPC_controler : MonoBehaviour {

  public GameObject npc_prefab_;
  public GameObject NPC;

  public List<Vector3> route_;
  public bool walk_;
  public bool return_;
  public float speed_;
  float lerp_;
  public int index_;
  public Vector3 init_transform_;

  public bool finish_;

  public bool isLoaded_ = false;

  public bool autoReturn_;
  public bool reachDestiny;

  NPC_Data data_;

  public NPC_Carry NPC_Animation;

  // Use this for initialization
  public void Init () {
    if (!isLoaded_) {
      walk_ = false;
      return_ = false;
      index_ = 1;
      finish_ = true;
      NPC = Instantiate(npc_prefab_, transform, true );
      NPC_Animation = NPC.GetComponent<NPC_Carry>();

      NPC.SetActive(false);

      data_ = new NPC_Data();

    }
  }
	
	// Update is called once per frame
	void Update () {

    bool canMove = true;

    if (NPC_Animation) {
      canMove = NPC_Animation.canMove;
    }

    if(walk_ && NPC.activeSelf && canMove) {

      if (!return_) {

        NPC.transform.position = Vector3.Lerp(init_transform_, route_[index_], lerp_);

        lerp_ += speed_ * Time.deltaTime;

        if(lerp_ > 1.0f) {
          lerp_ = 0.0f;
          init_transform_ = route_[index_];
          index_++;

          if(index_ >= route_.Count) {
            if (autoReturn_) {
              return_ = true;
            }
            reachDestiny = true;
            index_--;

            SetRotation(index_, index_ - 1);

          } else {
            SetRotation(index_ - 1, index_);
          }

        }

      } else {

        NPC.transform.position = Vector3.Lerp(init_transform_, route_[index_], lerp_);

        lerp_ += speed_ * Time.deltaTime;

        if (lerp_ > 1.0f) {
          lerp_ = 0.0f;
          init_transform_ = route_[index_];
          index_--;

          if (index_ < 0) {
            finish_ = true;
            
            index_ = 0;
          } else {
            SetRotation(index_ + 1, index_);
          }

        }


      }


    }

    if (finish_ && NPC.activeSelf) {
      if (NPC_Animation) {
        if (NPC_Animation.carringBox) {
          NPC_Animation.DropBox();
        }

        if (NPC_Animation.canMove) {
          NPC.SetActive(false);
        }

      } else {
        NPC.SetActive(false);
      }
      
    }

    if (NPC_Animation) {
      Vector3 currentPosition = NPC.transform.position;
      currentPosition.y = 0.0f;
      NPC.transform.position = currentPosition;
    }

	}

  public void SetRoute(List<Vector3> route) {
    if (route.Count < 1) return;
    route_ = route;
  }

  public void StartRoute(bool carry = false) {
    
    walk_ = true;
    return_ = false;
    reachDestiny = false;    
    NPC.SetActive(true);
    NPC.transform.position = route_[0];

    index_ = 1;
    init_transform_ = route_[0];
    finish_ = false;

    if (NPC_Animation) {

      NPC_Animation.canMove = false;

      if (carry) {
        NPC_Animation.StartCoroutine(NPC_Animation.COWaitNextFrame(NPC_Animation.PickBox));
      } else {
        NPC_Animation.StartCoroutine(NPC_Animation.COWaitNextFrame(NPC_Animation.StartMoving));
      }

      
    }

    SetRotation(0, 1);

  }

  public void ResetStatus() {
    walk_ = false;
    return_ = false;
    reachDestiny = false;
    finish_ = false;
    NPC.SetActive(false);
  }
  
  public void OnDestroy() {
    Destroy(NPC);
  }

  public NPC_Data GetData() {
    
    if (null != route_) {
      data_.route = new float[route_.Count * 3];
      for (int i = 0; i < route_.Count; i++) {
        data_.route[(i * 3) + 0] = route_[i].x;
        data_.route[(i * 3) + 1] = route_[i].y;
        data_.route[(i * 3) + 2] = route_[i].z;
      }
    } else {
      data_.route = new float[1];
    }

    data_.walk = walk_;
    data_.return_ = return_;
    data_.speed = speed_;
    data_.lerp = lerp_;
    data_.index = index_;
    data_.init_transform = new float[3];
    data_.init_transform[0] = init_transform_.x;
    data_.init_transform[1] = init_transform_.y;
    data_.init_transform[2] = init_transform_.z;
    data_.finish = finish_;

    data_.autoReturn_ = autoReturn_;
    data_.reachDestiny = reachDestiny;

    data_.prefabName = npc_prefab_.name;

    data_.isActive = NPC.activeSelf;

    if (NPC_Animation) {
      data_.animationCarryBox = NPC_Animation.carringBox;
    }

    return data_;
  }

  public void SetData(NPC_Data data) {

    route_ = new List<Vector3>();

    for(int i=0; i<data.route.Length / 3; i++) {
      route_.Add(new Vector3( data.route[(i*3)+0], data.route[(i * 3) + 1], data.route[(i * 3) + 2]) );
    }
         
    walk_ = data.walk ;
    return_ = data.return_;
    speed_ = data.speed ;
    lerp_ = data.lerp;
    index_ = data.index;
    init_transform_ = new Vector3(data.init_transform[0], data.init_transform[1], data.init_transform[2]);
    finish_ = data.finish;

    npc_prefab_ = (GameObject)Resources.Load("cityBuider/Placeables/" + data.prefabName, typeof(GameObject));

    autoReturn_ = data.autoReturn_;
    reachDestiny = data.reachDestiny;

    NPC = Instantiate(npc_prefab_, transform, true);
    NPC_Animation = NPC.GetComponent<NPC_Carry>();

    if (NPC_Animation) {
      if (data.animationCarryBox) {
        NPC_Animation.PickBox();
      }
    }

    NPC.SetActive(data.isActive);

    isLoaded_ = true;
    data_ = new NPC_Data();

    if (return_) {
      SetRotation(index_ + 1, index_);
    } else {
      SetRotation(index_ - 1, index_);
    }

  }


  public void SetRotation(int currentIndex, int nextIndex) {
    if(currentIndex < 0 || nextIndex < 0 || currentIndex >= route_.Count || nextIndex >= route_.Count) {
      return;
    }


    Vector3 current = route_[currentIndex];
    Vector3 next = route_[nextIndex];

    Vector3 result = next - current;

    Quaternion rotation = Quaternion.LookRotation(result);

    NPC.transform.rotation = rotation;

  }


}
