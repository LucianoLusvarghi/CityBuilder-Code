using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {

  public int Mode;

  static ChangeScene inst = null;

  public float time = 0.0f;

  // Use this for initialization
  void Start () {
    if(null != inst) {
      Destroy(inst.gameObject);      
    }

    inst = this;

    DontDestroyOnLoad(gameObject);
  }
		  
  public void SelectGame(int mode) {

    Mode = mode;

    SceneManager.LoadScene(1);
  }

  public void ExitGame() {
    Application.Quit();
  }
   

}
