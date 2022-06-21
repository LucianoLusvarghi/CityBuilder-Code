using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceInWorld : MonoBehaviour {
    
  public Transform shadowContainer;

  GameObject currentObject;

  //GameObject shadow;
  ShadowsChangeColor shadowMaterial;

  Vector2Int objectSize_;

  RaycastHit checkMousePosition;

  bool destructionMode_;

  ManagerGame manager = null;

  UI_Manager ui_manager;

  string buildName;

  //
  Vector2Int startPosition_;
  bool pressed = false;
  List<GameObject> shadows_;

  // Use this for initialization
  void Start() {
    checkMousePosition = new RaycastHit();
    manager = GameObject.Find("GM").GetComponent<ManagerGame>();

    ui_manager = GameObject.Find("UI").GetComponent<UI_Manager>();

    currentObject = null;
    destructionMode_ = false;
    startPosition_ = new Vector2Int();
    startPosition_.Set(-1, -1);

    shadows_ = new List<GameObject>();

  }

  // Update is called once per frame
  void Update() {

    
    ButtonLeftDown();    
    ButtonLeftKeepPressed();    
    ButtonLeftUp();


    if (currentObject) {

      if (Input.GetMouseButtonDown(1)) {
        DeselectBuild();
      }

    } else if(!destructionMode_){

      if (Input.GetMouseButtonDown(1)) {

        if (!EventSystem.current.IsPointerOverGameObject()) {

          bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out checkMousePosition, float.MaxValue, ~(LayerMask.GetMask("IgnoreRaycast")));
          if (hit) {

            if (checkMousePosition.collider.gameObject.layer == LayerMask.NameToLayer("PlaceableAndDestructible")) {

              Interface_UI customUI = checkMousePosition.collider.gameObject.GetComponent<Interface_UI>();

              if (null != customUI) {
                customUI.InvoqueCustomUI(ui_manager);
              } else {
                ui_manager.ReturnToCategory();
              }

            } else {
              ui_manager.ReturnToCategory();
            }


          }
        }

      }

      if (Input.GetMouseButtonDown(0)) {
        if (!EventSystem.current.IsPointerOverGameObject()) {
          ui_manager.ReturnToCategory();
        }
      }

    }


   if (destructionMode_) {

      if (Input.GetMouseButton(0)) {
        if (!EventSystem.current.IsPointerOverGameObject()) {

          bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out checkMousePosition, float.MaxValue, ~(LayerMask.GetMask("IgnoreRaycast")));
          if (hit) {

            if (checkMousePosition.collider.gameObject.layer == LayerMask.NameToLayer("PlaceableAndDestructible")) {

              Destroy(checkMousePosition.collider.gameObject);

            }


          }
        }
      } else if (Input.GetMouseButtonDown(1)) {
        destructionMode_ = false;
        ui_manager.ReturnToCategory();
      }

    }

  }

  public void ChangeCurrentObject(GameObject newObject, GameObject newShadow, Vector2Int objectSize, string name ) {
    currentObject = newObject;
    destructionMode_ = false;
    objectSize_ = objectSize;

    DestroyShadows(true);

    GameObject shadow = Instantiate(newShadow, shadowContainer, true);
    shadow.layer = LayerMask.NameToLayer("IgnoreRaycast");
    shadowMaterial = shadow.GetComponent<ShadowsChangeColor>();
    shadowMaterial.ChangeColor(Color.green);

    shadows_.Add(shadow);

    buildName = name;

  }

  public void SetDestructionMode() {
    currentObject = null;
    destructionMode_ = true;
    DestroyShadows();
    shadowMaterial = null;
  }

  void DestroyShadows(bool destroyMain = false) {
    if (shadows_.Count > 0) {
      GameObject shadow = shadows_[0];
      for (int i = 1; i < shadows_.Count; i++) {
        Destroy(shadows_[i]);
      }
      shadows_.Clear();
      shadows_.Add(shadow);

      if (destroyMain) {
        Destroy(shadow);
        shadows_.Clear();
      }

    }
  }

  public void DeselectBuild() {
    currentObject = null;
    destructionMode_ = false;
    DestroyShadows();
    if (shadows_.Count > 0) {
      GameObject shadow = shadows_[0];
      Destroy(shadow);
      shadows_.Clear();
    }
    ui_manager.DeselectBuild();

    shadowMaterial = null;
  }

  bool CheckNeighbour(Vector3 position) {

    Vector2Int currentPosition = new Vector2Int {
      x = (int)position.x,
      y = (int)position.z
    };

    for (int y = 0; y < objectSize_.y; y++) {
      for (int x = 0; x < objectSize_.x; x++) {

        bool checkGrass = manager.mapSys.GetCell((uint)(currentPosition.x + x), (uint)(currentPosition.y + y)).currentState == MapController.CellType.grass;

        if (!checkGrass) {
          return true;
        }

      }
    }

    return false;
  }

  void ButtonLeftDown() {
    //Comprueba si hay un edificio seleccionado
    if (!currentObject) {      
      return;
    }    
    //Check mouse input
    if (!Input.GetMouseButtonDown(0)) {
      return;
    }
    //comprueba si no esta en la UI
    if (EventSystem.current.IsPointerOverGameObject()) {
      return;
    }

    if (pressed) {
      pressed = false;
      return;
    }


    //Raycast que ignora la sombra del objeto
    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out checkMousePosition, float.MaxValue, LayerMask.GetMask("Terrain") );


    if (hit) {
      startPosition_.x = (int)checkMousePosition.point.x;
      startPosition_.y = (int)checkMousePosition.point.z;
      pressed = true;
    }
  }

  void ButtonLeftKeepPressed() {
    //Comprueba si hay un edificio seleccionado
    if (!currentObject) {
      return;
    }
    //comprueba si no esta en la UI
    if (EventSystem.current.IsPointerOverGameObject()) {
      return;
    }

    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out checkMousePosition, float.MaxValue, LayerMask.GetMask("Terrain") );

    if (!hit) {
      return;
    }
        
    int hit_x = (int)checkMousePosition.point.x;
    int hit_y = (int)checkMousePosition.point.z;

    Vector3 currentHitPosition = new Vector3(hit_x, 0.05f, hit_y);
    shadows_[0].transform.position = currentHitPosition;

    shadowMaterial = shadows_[0].GetComponent<ShadowsChangeColor>();
    if (!CheckNeighbour(currentHitPosition)) {
      shadowMaterial.ChangeColor(Color.green);
    } else {
      shadowMaterial.ChangeColor(Color.red);
    }

    if (!pressed) {
      return;
    }

    DestroyShadows();

    Vector2Int endPosition = new Vector2Int(hit_x, hit_y);

    int expancion_x = (Mathf.Abs(endPosition.x - startPosition_.x)) / objectSize_.x;
    int expancion_y = (Mathf.Abs(endPosition.y - startPosition_.y)) / objectSize_.y;

    int signed_x = (endPosition.x - startPosition_.x) > 0.0f ? 1 : -1;
    int signed_y = (endPosition.y - startPosition_.y) > 0.0f ? 1 : -1;


    for (int y = 0; y < expancion_y; y++) {
      GameObject shadow = Instantiate(shadows_[0], shadowContainer, true);

      currentHitPosition = new Vector3(startPosition_.x, 0.05f, startPosition_.y + (y * objectSize_.y * signed_y));

      shadow.transform.position = currentHitPosition;

      shadows_.Add(shadow);

      shadowMaterial = shadow.GetComponent<ShadowsChangeColor>();

      if (!CheckNeighbour(currentHitPosition)) {
        shadowMaterial.ChangeColor(Color.green);
      } else {
        shadowMaterial.ChangeColor(Color.red);
      }

    }


    for (int x = 0; x < expancion_x; x++) {
      GameObject shadow = Instantiate(shadows_[0], shadowContainer, true);

      currentHitPosition = new Vector3(startPosition_.x + (x * objectSize_.x * signed_x), 0.05f, startPosition_.y + (expancion_y * objectSize_.y * signed_y));

      shadow.transform.position = currentHitPosition;

      shadows_.Add(shadow);

      shadowMaterial = shadow.GetComponent<ShadowsChangeColor>();

      if (!CheckNeighbour(currentHitPosition)) {
        shadowMaterial.ChangeColor(Color.green);
      } else {
        shadowMaterial.ChangeColor(Color.red);
      }

    }

  }



  void ButtonLeftUp() {

    if (!currentObject || !pressed) {
      return;
    }

    if (Input.GetMouseButton(0)) {
      return;
    }

    pressed = false;
    
    for (int i = 0; i < shadows_.Count; i++) {

      if (!CheckNeighbour(shadows_[i].transform.position)) {

        if (manager.PayCost(buildName)) {
          Instantiate(currentObject, shadows_[i].transform.position, new Quaternion());
        }
      }
    }
       
    DestroyShadows();

  }










}
