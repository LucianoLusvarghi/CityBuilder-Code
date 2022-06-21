using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC_Carry : MonoBehaviour {

  public Animator animator;
  public GameObject box;

  public bool carringBox = false;
  public bool canMove = false;
  // Use this for initialization
  void Start () {
    StartCoroutine(COShowItem("none", .0f));
    StartMoving();    
  }
	
  public void PickBox() {
    StopMoving();
    carringBox = true;
    animator.SetTrigger("CarryPickupTrigger");
    StartCoroutine(COShowItem("box", .5f));
    StartCoroutine(CORestartMoving(1.1f));
  }

  public void DropBox() {
    StopMoving();
    carringBox = false;
    animator.SetTrigger("CarryPutdownTrigger");
    StartCoroutine(COShowItem("none", .7f));
    StartCoroutine(CORestartMoving(1.3f));
  }

  public void StartMoving() {
    canMove = true;
    animator.SetBool("Moving", true);
  }

  public void StopMoving() {
    canMove = false;
    animator.SetBool("Moving", false);
  }

  public IEnumerator COShowItem(string item, float waittime) {
    yield return new WaitForSeconds(waittime);

    if (item == "none") {
      box.SetActive(false);
    } else if (item == "box") {
      box.SetActive(true);
    }

    yield return null;
  }

  public IEnumerator CORestartMoving(float waittime) {
    yield return new WaitForSeconds(waittime);

    StartMoving();

    yield return null;
  }

  public IEnumerator COWaitNextFrame( Action callback ) {
    yield return 0;

    callback.Invoke();

    yield return null;
  }

}
