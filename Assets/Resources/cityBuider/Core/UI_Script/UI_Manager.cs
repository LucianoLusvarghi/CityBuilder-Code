using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Manager : MonoBehaviour {

  [System.Serializable]
  public struct CategoryContainer {
    public string categoryName;
    public GameObject buildsCanvas;
  };

  [System.Serializable]
  public struct FastKeyCategory {
    public string categoryName;
    public KeyCode key;
  };

  [System.Serializable]
  public struct FastKeyBuild {
    public string categoryName;
    public string buildName;
    public KeyCode key;
  };

  ManagerGame GameManager_;
  [Space(30)]
  public GameObject categoryCanvas;
  public CategoryContainer[] buildsCanvas;
  [Space(30)]
  public GameObject customUICanvas;
  GameObject customUI;
  [Space(30)]
  public Text WorkersHaveView;
  public Text WorkersNeedView;
  [Space(30)]
  public Text ErrorMensage;
  public float errorTime = 1.6f;
  [Space(30)]
  public GameObject GameUI;
  public MoveDropDownMenu dropDownScript;
  public GameObject PauseUI;
  [Space(30)]
  public Toggle DestroyButton;
  public Toggle StatsButton;
  public Toggle MainMenu;
  [Space(30)]
  public FastKeyCategory[] keyCategory;
  public FastKeyBuild[] keybuilds;


  // Use this for initialization
  void Start() {

    GameManager_ = GameObject.Find("GM").GetComponent<ManagerGame>();

    WorkersHaveView.text = "Have: " + (int)GameManager_.populationSys.currentWorkers;
    WorkersNeedView.text = "Need: " + GameManager_.populationSys.workersNeeded;

    ErrorMensage.gameObject.SetActive(false);

    ReturnFromPauseMode();

  }

  public void Update() {

    WorkersHaveView.text = "Have: " + (int)GameManager_.populationSys.currentWorkers;
    WorkersNeedView.text = "Need: " + GameManager_.populationSys.workersNeeded;

    if (ErrorMensage.gameObject.activeSelf) {
      errorTime += Time.fixedDeltaTime;
      if (errorTime > 1.5f) {
        ErrorMensage.gameObject.SetActive(false);
      }
    }

    for (int i = 0; i < keyCategory.Length; i++) {

      if (Input.GetKeyDown(keyCategory[i].key)) {

        ReturnToCategory();

        Toggle[] buttons = categoryCanvas.GetComponentsInChildren<Toggle>();
        if (buttons != null) {
          buttons[i].isOn = true;
        }
      }
    }

    bool fastKeyBuildPressed = false;
    for (int i = 0; i < keybuilds.Length && !fastKeyBuildPressed; i++) {

      if (Input.GetKeyDown(keybuilds[i].key)) {

        DeselectBuild();

        for (int j = 0; j < buildsCanvas.Length && !fastKeyBuildPressed; j++) {

          if (keybuilds[i].categoryName == buildsCanvas[j].categoryName) {
            if (buildsCanvas[j].buildsCanvas.activeSelf) {

              Toggle[] buttons = buildsCanvas[j].buildsCanvas.GetComponentsInChildren<Toggle>();
              if (buttons != null) {

                for (int b = 0; b < buttons.Length && !fastKeyBuildPressed; b++) {

                  if (buttons[b].gameObject.name == "Button_" + keybuilds[i].buildName) {
                    buttons[b].isOn = true;
                    fastKeyBuildPressed = true;
                  }

                }

              }

            }
          }

        }

      }

    }


    if (Input.GetKeyDown(KeyCode.X)) {
      DestroyButton.isOn = !DestroyButton.isOn;
    }




    ToggleGroup allButtons = categoryCanvas.GetComponent<ToggleGroup>();

    dropDownScript.CanDesploy = allButtons.AnyTogglesOn() || customUI;
    dropDownScript.ForceDeploy = customUI;

  }


  public GameObject ChangeToCustomUI(GameObject customUIPrefab) {

    //categoryCanvas.SetActive(false);
    for (int i = 0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        buildsCanvas[i].buildsCanvas.SetActive(false);
      }
    }

    if (null != customUI) {
      Destroy(customUI);
    }

    customUI = Instantiate(customUIPrefab, customUICanvas.transform);
    return customUI;
  }



  public void ReturnToCategory(bool destructive = false) {

    if (null != customUI) {
      Destroy(customUI);
      customUI = null;
    }

    if (!destructive) {
      
      if (DestroyButton.isOn) {
        GameManager_.ChangeToDestroyMode();
        DestroyButton.isOn = false;
      }

    } 
      ToggleGroup allButtons = categoryCanvas.GetComponent<ToggleGroup>();
      if (allButtons) {
        allButtons.SetAllTogglesOff();
      }
    

    if (StatsButton.isOn) {
      ChangeToStatsMode();
    }

    for (int i = 0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        if (buildsCanvas[i].buildsCanvas.activeSelf) {
          allButtons = buildsCanvas[i].buildsCanvas.GetComponent<ToggleGroup>();
          if (allButtons) {
            allButtons.SetAllTogglesOff();
          }
        }

        buildsCanvas[i].buildsCanvas.SetActive(false);
      }
    }


  }

  public void ChangeToCategory(string categoryName) {

    if (null != customUI) {
      Destroy(customUI);
      customUI = null;
    }

    {

      ToggleGroup allButtons = categoryCanvas.GetComponent<ToggleGroup>();
      if (allButtons) {

        if (allButtons.AnyTogglesOn()) {

          if (DestroyButton.isOn) {
            GameManager_.ChangeToDestroyMode();
            DestroyButton.isOn = false;
          }

          if (StatsButton.isOn) {
            ChangeToStatsMode();
          }
        }
      }
    }

    for (int i = 0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        if (buildsCanvas[i].buildsCanvas.activeSelf) {
          ToggleGroup allButtons = buildsCanvas[i].buildsCanvas.GetComponent<ToggleGroup>();
          if (allButtons) {
            allButtons.SetAllTogglesOff();
          }
        }

        buildsCanvas[i].buildsCanvas.SetActive(false);
      }
    }
    
    for (int i=0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        if (buildsCanvas[i].categoryName == categoryName) {

          buildsCanvas[i].buildsCanvas.SetActive(true);
          GameManager_.placeInWorldSys.DeselectBuild();
          return;
        }
      }
    }

    
  }

  public void BuildSelected(string Name) {

    if (StatsButton.isOn) {
      ChangeToStatsMode();
    }

    if (DestroyButton.isOn) {
      //GameManager_.ChangeToDestroyMode();
      DestroyButton.isOn = false;
    }

    //Change only on toggle on, not any change in toggle
    for (int i = 0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        if (buildsCanvas[i].buildsCanvas.activeSelf) {
          Toggle[] buttons = buildsCanvas[i].buildsCanvas.GetComponentsInChildren<Toggle>(true);

          for (int t = 0; t < buttons.Length; t++) {
            if (buttons[t].isOn) {
              GameManager_.ChangePlaceableSelected(Name);
              return;
            }
          }

        }
      }
    }    
    
  }

  public void DeselectBuild() {
    for (int i = 0; i < buildsCanvas.Length; i++) {
      if (buildsCanvas[i].buildsCanvas) {
        if (buildsCanvas[i].buildsCanvas.activeSelf) {
          ToggleGroup allButtons = buildsCanvas[i].buildsCanvas.GetComponent<ToggleGroup>();

          if (allButtons) {
            allButtons.SetAllTogglesOff();
          }

          GameObject InfoPanel = buildsCanvas[i].buildsCanvas.transform.Find("InfoPanel").gameObject;

          for (int c = 0; c < InfoPanel.transform.childCount; c++) {
            InfoPanel.transform.GetChild(c).gameObject.SetActive(false);
          }

        }
      }
    }
  }

  public void ChageToDestructionMode() {

    if (MainMenu.isOn) {
      DestroyButton.isOn = false;      
    }

    if (StatsButton.isOn) {
      StatsButton.isOn = false;
    }

    if (DestroyButton.isOn) {
      ReturnToCategory(true);
      GameManager_.ChangeToDestroyMode();

    } else {
      GameManager_.placeInWorldSys.DeselectBuild();
    }

    

  }


  public void SetErrorMesage(string mensaje) {
    ErrorMensage.gameObject.SetActive(true);
    ErrorMensage.text = mensaje;
    errorTime = 0.0f;

  }



  public void MoveToPauseMode(bool fromKey = true) {

    if(!fromKey && !MainMenu.isOn) {
      ReturnFromPauseMode();
      return;
    }

    if (StatsButton.isOn) {
      StatsButton.isOn = false;
    }

    if (DestroyButton.isOn) {
      DestroyButton.isOn = false;
    }

    if (fromKey && !MainMenu.isOn) {
      MainMenu.isOn = true;
    }

    GameUI.SetActive(false);
    PauseUI.SetActive(true);
    ErrorMensage.gameObject.SetActive(false);
  }

  public void ReturnFromPauseMode() {
    GameUI.SetActive(true);
    PauseUI.SetActive(false);

    ReturnToCategory(true);
    ReturnToCategory(false);

    MainMenu.isOn = false;

    if (errorTime < 1.5f) {
      ErrorMensage.gameObject.SetActive(true);
    }
  }


  public void ChangeToStatsMode(bool fromKey = true) {

    if (MainMenu.isOn) {
      StatsButton.isOn = false;
      fromKey = false;
    }

    if (fromKey) {
      GameManager_.ui_Stats.SetActiveMainMonitor(!GameManager_.ui_Stats.MainMonitor_.activeSelf);
      StatsButton.isOn = GameManager_.ui_Stats.MainMonitor_.activeSelf;
    } else {

      if (StatsButton.isOn) {
        GameManager_.ui_Stats.SetActiveMainMonitor(true);
      } else {
        GameManager_.ui_Stats.SetActiveMainMonitor(false);
      }

    }

  }

}
