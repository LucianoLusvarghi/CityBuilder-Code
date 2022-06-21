using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveDropDownMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

  public float speed;

  public bool IsMouseOverMy;
  public bool CanDesploy;
  public bool ForceDeploy;

  RectTransform DropDown;
  RectTransform ContentRect;
  // Use this for initialization
  void Start () {
    DropDown = GetComponent<RectTransform>();

    GameObject contentCategory = GameObject.Find("ContentCategory");
    ContentRect = contentCategory.GetComponent<RectTransform>();

    speed = ContentRect.rect.height * 3.0f;

  }
	
	// Update is called once per frame
	void Update () {

    if ((IsMouseOverMy && CanDesploy) || ForceDeploy) {

      if (-DropDown.offsetMax.y > 0.0f) {

        float y = -DropDown.offsetMax.y;
        y -= Time.deltaTime * speed;

        DropDown.offsetMax = new Vector2(DropDown.offsetMax.x, -y);
        DropDown.offsetMin = new Vector2(DropDown.offsetMin.x, -y);

      } else {

        float y = -DropDown.offsetMax.y;
        y = 0.0f;

        DropDown.offsetMax = new Vector2(DropDown.offsetMax.x, -y);
        DropDown.offsetMin = new Vector2(DropDown.offsetMin.x, -y);
      }

    } else {

      if (-DropDown.offsetMax.y < ContentRect.rect.height) {

        float y = -DropDown.offsetMax.y;
        y += Time.deltaTime * speed;

        DropDown.offsetMax = new Vector2(DropDown.offsetMax.x, -y);
        DropDown.offsetMin = new Vector2(DropDown.offsetMin.x, -y);

      } 
      
      if(-DropDown.offsetMax.y > ContentRect.rect.height) {

        float y = -DropDown.offsetMax.y;
        y = ContentRect.rect.height;

        DropDown.offsetMax = new Vector2(DropDown.offsetMax.x, -y);
        DropDown.offsetMin = new Vector2(DropDown.offsetMin.x, -y);
      }

    }

	}

  public void OnPointerEnter(PointerEventData eventData) {
    IsMouseOverMy = true;
  }

  public void OnPointerExit(PointerEventData eventData) {
    IsMouseOverMy = false;
  }

}
