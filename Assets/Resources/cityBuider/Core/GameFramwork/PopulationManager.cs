using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interface_Workers {

  void SetWorkers(int count);
  int GetWorkersNeeded();
}

public class PopulationManager{

  List<Interface_Workers> buildsWithWorkers;

  public int workersNeeded = 0;
  public float currentWorkers = 0;

  readonly float populationToWorkersRatio = 0.6f;

  int population;

  public void Init(ManagerGame CurrentManager) {
    buildsWithWorkers = new List<Interface_Workers>();
  }

  public void RegisterBuild(Interface_Workers build) {

    if (!buildsWithWorkers.Contains(build)) {
      buildsWithWorkers.Add(build);
      workersNeeded += build.GetWorkersNeeded();
    }

  }

  public void RemoveBuild(Interface_Workers build) {
    if (buildsWithWorkers.Contains(build)) {
      buildsWithWorkers.Remove(build);
      workersNeeded -= build.GetWorkersNeeded();
    }
  }

  public void ChangeWorkersAvaible(int change) {
    population += change;
    currentWorkers = population * populationToWorkersRatio;
  }

  public void UpdateWorkers() {

    

    int workersNeeded_tmp = workersNeeded;
    float currentWorkers_tmp = currentWorkers;

    for(int i=0; i< buildsWithWorkers.Count; i++) {
      Interface_Workers currentBuild = buildsWithWorkers[i];

      float currentPorsentage = currentWorkers_tmp / workersNeeded_tmp;
      if (currentPorsentage > 1.0f) {
        currentPorsentage = 1.0f;
      }

      int currentWorkers = (int)(currentBuild.GetWorkersNeeded() * currentPorsentage);

      currentWorkers_tmp -= currentWorkers;
      workersNeeded_tmp -= currentBuild.GetWorkersNeeded();

      currentBuild.SetWorkers(currentWorkers);

    }

  }

}
