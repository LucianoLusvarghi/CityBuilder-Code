using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour {

	public void ChangeToSceneControls() {
    SceneManager.LoadScene(2);
  }

  public void ReturnToMainMenu() {
    SceneManager.LoadScene(0);
  }


}
