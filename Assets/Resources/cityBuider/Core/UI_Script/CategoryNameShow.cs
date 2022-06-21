using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CategoryNameShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

  public Text NameShow;
  public string Name;

  public void OnPointerEnter(PointerEventData eventData) {
    NameShow.text = Name;
  }

  public void OnPointerExit(PointerEventData eventData) {
    NameShow.text = "";
  }

}
